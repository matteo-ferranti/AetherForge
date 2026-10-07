using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

namespace AetherForge.App.Services;

public static class FileSafetyService
{
    public static void EnsureDifferentFile(
        string inputPath,
        string outputPath)
    {
        if (AreSameFile(inputPath, outputPath))
        {
            throw new InvalidOperationException(
                LanguageService.Get("OutputMatchesOriginal"));
        }
    }

    public static bool AreSameFile(
        string firstPath,
        string secondPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(secondPath);

        firstPath = Path.GetFullPath(firstPath);
        secondPath = Path.GetFullPath(secondPath);

        if (string.Equals(
                firstPath,
                secondPath,
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!File.Exists(firstPath) ||
            !File.Exists(secondPath))
        {
            return false;
        }

        using SafeFileHandle firstHandle = File.OpenHandle(
            firstPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);

        using SafeFileHandle secondHandle = File.OpenHandle(
            secondPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);

        FileIdentity firstIdentity = GetIdentity(firstHandle);
        FileIdentity secondIdentity = GetIdentity(secondHandle);

        return
            firstIdentity.VolumeSerialNumber ==
                secondIdentity.VolumeSerialNumber &&
            firstIdentity.FileIdLow ==
                secondIdentity.FileIdLow &&
            firstIdentity.FileIdHigh ==
                secondIdentity.FileIdHigh;
    }

    private static FileIdentity GetIdentity(
        SafeFileHandle handle)
    {
        const int fileIdInfo = 18;

        bool success = GetFileInformationByHandleEx(
            handle,
            fileIdInfo,
            out FileIdentity identity,
            (uint)Marshal.SizeOf<FileIdentity>());

        if (!success)
        {
            throw new IOException(
                LanguageService.Get("CannotVerifyOutputSafety"));
        }

        return identity;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FileIdentity
    {
        public ulong VolumeSerialNumber;
        public ulong FileIdLow;
        public ulong FileIdHigh;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetFileInformationByHandleEx(
        SafeFileHandle handle,
        int informationClass,
        out FileIdentity information,
        uint bufferSize);
}