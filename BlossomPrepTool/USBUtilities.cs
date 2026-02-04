using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace BlossomPrepTool
{
    public class USBInfo
    {
        public int DiskNumber { get; set; }
        public string DisplayName { get; set; }
        public double SizeGB { get; set; }
        public List<string> Volumes { get; set; } = new List<string>();
    }

    /// <summary>
    /// Utility class for detecting and managing USB drives
    /// </summary>
    public static class USBUtilities
    {
        private const long MaxUSBSizeBytes = 130L * 1024 * 1024 * 1024; // 130GB

        public static List<USBInfo> GetUSBDrives()
        {
            var usbDrives = new List<USBInfo>();

            try
            {
                // Query disk information
                var diskScope = new ManagementScope(@"\\.\root\cimv2");
                diskScope.Connect();

                var diskQuery = new ObjectQuery("SELECT * FROM Win32_DiskDrive");
                using (var diskSearcher = new ManagementObjectSearcher(diskScope, diskQuery))
                {
                    foreach (ManagementObject disk in diskSearcher.Get())
                    {
                        try
                        {
                            var size = Convert.ToInt64(disk["Size"]);
                            
                            // Skip if too large or system disk
                            if (size > MaxUSBSizeBytes)
                                continue;

                            var diskIndex = Convert.ToInt32(disk["Index"]);
                            
                            // Check if this is a system disk
                            if (IsSystemDisk(diskIndex))
                                continue;

                            var displayName = GetDiskDisplayName(diskIndex) ?? "USB Drive";
                            var sizeGB = Math.Round(size / (1024.0 * 1024.0 * 1024.0), 2);

                            usbDrives.Add(new USBInfo
                            {
                                DiskNumber = diskIndex,
                                DisplayName = displayName,
                                SizeGB = sizeGB,
                                Volumes = GetDiskVolumes(diskIndex)
                            });
                        }
                        catch
                        {
                            // Skip problematic disks
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting USB drives: {ex.Message}");
            }

            return usbDrives;
        }

        private static bool IsSystemDisk(int diskIndex)
        {
            try
            {
                var scope = new ManagementScope(@"\\.\root\cimv2");
                scope.Connect();

                // Get partitions for this disk
                var query = new ObjectQuery($"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='\\\\?\\GLOBALROOT\\Device\\Harddisk{diskIndex}\\DP(1)0'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
                using (var searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (ManagementObject partition in searcher.Get())
                    {
                        var partitionQuery = new ObjectQuery($"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");
                        using (var volSearcher = new ManagementObjectSearcher(scope, partitionQuery))
                        {
                            foreach (ManagementObject volume in volSearcher.Get())
                            {
                                var driveLetter = volume["Name"]?.ToString();
                                if (!string.IsNullOrEmpty(driveLetter))
                                {
                                    var systemPath = $@"{driveLetter}\Windows\System32";
                                    if (System.IO.Directory.Exists(systemPath))
                                        return true;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // If we can't determine, assume it's not a system disk
            }

            return false;
        }

        private static string GetDiskDisplayName(int diskIndex)
        {
            try
            {
                var scope = new ManagementScope(@"\\.\root\cimv2");
                scope.Connect();

                // Get all logical disks associated with this physical disk
                var query = new ObjectQuery($"SELECT Name, VolumeName FROM Win32_LogicalDisk WHERE DeviceID LIKE '%'");
                using (var searcher = new ManagementObjectSearcher(scope, query))
                {
                    var volumes = new List<string>();
                    foreach (ManagementObject disk in searcher.Get())
                    {
                        var volumeName = disk["VolumeName"]?.ToString();
                        if (!string.IsNullOrEmpty(volumeName))
                            volumes.Add(volumeName);
                    }

                    if (volumes.Count > 0)
                        return string.Join(" / ", volumes);
                }
            }
            catch
            {
                // Return null to use default
            }

            return null;
        }

        private static List<string> GetDiskVolumes(int diskIndex)
        {
            var volumes = new List<string>();

            try
            {
                var scope = new ManagementScope(@"\\.\root\cimv2");
                scope.Connect();

                // Similar logic to above but returns all found volumes
                var query = new ObjectQuery($"SELECT Name FROM Win32_LogicalDisk");
                using (var searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (ManagementObject disk in searcher.Get())
                    {
                        var name = disk["Name"]?.ToString();
                        if (!string.IsNullOrEmpty(name))
                            volumes.Add(name);
                    }
                }
            }
            catch
            {
                // Return empty list
            }

            return volumes;
        }
    }
}
