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

                            var displayName = GetDiskDisplayName(diskIndex);
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
                var logicalDisks = GetLogicalDisksForPhysicalDisk(diskIndex);
                var labels = new List<string>();

                foreach (var logicalDisk in logicalDisks)
                {
                    var volumeName = logicalDisk["VolumeName"]?.ToString();

                    if (string.IsNullOrWhiteSpace(volumeName))
                    {
                        var driveLetter = logicalDisk["Name"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(driveLetter))
                            volumeName = GetVolumeLabelByDriveLetter(driveLetter);
                    }

                    if (!string.IsNullOrWhiteSpace(volumeName))
                        labels.Add(volumeName);
                }

                if (labels.Count > 0)
                    return string.Join(" / ", labels.Distinct());
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
                var logicalDisks = GetLogicalDisksForPhysicalDisk(diskIndex);
                foreach (var logicalDisk in logicalDisks)
                {
                    var name = logicalDisk["Name"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(name))
                        volumes.Add(name);
                }
            }
            catch
            {
                // Return empty list
            }

            return volumes.Distinct().ToList();
        }

        private static List<ManagementObject> GetLogicalDisksForPhysicalDisk(int diskIndex)
        {
            var logicalDisks = new List<ManagementObject>();

            var scope = new ManagementScope(@"\\.\root\cimv2");
            scope.Connect();

            var partitionQuery = new ObjectQuery($"SELECT DeviceID FROM Win32_DiskPartition WHERE DiskIndex = {diskIndex}");
            using (var partitionSearcher = new ManagementObjectSearcher(scope, partitionQuery))
            {
                foreach (ManagementObject partition in partitionSearcher.Get())
                {
                    var partitionDeviceId = partition["DeviceID"]?.ToString();
                    if (string.IsNullOrWhiteSpace(partitionDeviceId))
                        continue;

                    var escapedPartitionDeviceId = EscapeWmiString(partitionDeviceId);
                    var logicalDiskQuery = new ObjectQuery($"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{escapedPartitionDeviceId}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");

                    using (var logicalDiskSearcher = new ManagementObjectSearcher(scope, logicalDiskQuery))
                    {
                        foreach (ManagementObject logicalDisk in logicalDiskSearcher.Get())
                        {
                            logicalDisks.Add(logicalDisk);
                        }
                    }
                }
            }

            return logicalDisks;
        }

        private static string GetVolumeLabelByDriveLetter(string driveLetter)
        {
            try
            {
                var scope = new ManagementScope(@"\\.\root\cimv2");
                scope.Connect();

                var escapedDriveLetter = EscapeWmiString(driveLetter);
                var query = new ObjectQuery($"SELECT Label FROM Win32_Volume WHERE DriveLetter = '{escapedDriveLetter}'");
                using (var searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (ManagementObject volume in searcher.Get())
                    {
                        var label = volume["Label"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(label))
                            return label;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private static string EscapeWmiString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace("\\", "\\\\").Replace("'", "''");
        }
    }
}
