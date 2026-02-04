using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public static class Win32DiskHelper
{
    private const uint IOCTL_DISK_SET_OFFLINE = 0x0007c0c;
    private const uint IOCTL_DISK_SET_ONLINE = 0x0007c08;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern SafeFileHandle CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(
        SafeFileHandle hDevice,
        uint dwIoControlCode,
        IntPtr lpInBuffer,
        int nInBufferSize,
        IntPtr lpOutBuffer,
        int nOutBufferSize,
        out int lpBytesReturned,
        IntPtr lpOverlapped);

    private const uint GENERIC_READ = 0x80000000;
    private const uint GENERIC_WRITE = 0x40000000;
    private const uint FILE_SHARE_READ = 0x00000001;
    private const uint FILE_SHARE_WRITE = 0x00000002;
    private const uint OPEN_EXISTING = 3;

    public static void SetOffline(int diskNumber)
    {
        var handle = CreateFile($"\\\\.\\PhysicalDrive{diskNumber}", GENERIC_READ | GENERIC_WRITE,
            FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

        if (handle.IsInvalid)
            throw new Exception("Failed to open disk handle");

        if (!DeviceIoControl(handle, IOCTL_DISK_SET_OFFLINE, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero))
            throw new Exception("Failed to set disk offline");
    }

    public static void SetOnline(int diskNumber)
    {
        var handle = CreateFile($"\\\\.\\PhysicalDrive{diskNumber}", GENERIC_READ | GENERIC_WRITE,
            FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

        if (handle.IsInvalid)
            throw new Exception("Failed to open disk handle");

        if (!DeviceIoControl(handle, IOCTL_DISK_SET_ONLINE, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero))
            throw new Exception("Failed to set disk online");
    }
}
