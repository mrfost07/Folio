using Folio.Platform;
using Microsoft.Win32.SafeHandles;

namespace Folio.Services;

internal readonly record struct DesktopOriginalIdentity(ulong Volume, ulong High, ulong Low)
{
    internal static DesktopOriginalIdentity? From(FileService.FileTransferSourceIdentity? identity) =>
        identity is { FileId: { } id } value
            ? new(value.VolumeSerialNumber, id.High, id.Low)
            : null;
}

internal sealed record DesktopOriginalCandidate(string Path, DesktopOriginalIdentity Identity);

internal static class DesktopOriginalFile
{
    private const uint ReadAttributes = 0x80;
    private const uint WriteAttributes = 0x100;
    private const uint ShareAll = 7;
    private const uint Existing = 3;
    private const uint FileOrDirectory = 0x02000000;
    private const uint OpenReparsePoint = 0x00200000;

    internal static DesktopOriginalCandidate? Capture(string path)
    {
        using SafeFileHandle handle = Open(path, ReadAttributes);
        if (handle.IsInvalid ||
            !FileTransferNativeMethods.GetFileInformationByHandle(handle, out var information) ||
            information.NumberOfLinks > 1 ||
            (File.GetAttributes(handle) & (FileAttributes.Hidden | FileAttributes.System)) != 0 ||
            DesktopOriginalIdentity.From(FileService.IdentityFromHandle(handle)) is not { } identity)
        {
            return null;
        }

        return new(path, identity);
    }

    internal static bool TrySetHidden(DesktopOriginalCandidate original, bool hidden)
    {
        // Verify and modify the same open object. A replacement at the same
        // path must never inherit a previous file's grouping or restore.
        using SafeFileHandle handle = Open(original.Path, ReadAttributes | WriteAttributes);
        if (handle.IsInvalid)
        {
            int error = System.Runtime.InteropServices.Marshal.GetLastPInvokeError();
            if (error is 2 or 3)
            {
                return false;
            }

            throw new IOException($"The desktop item could not be opened for attribute changes ({error}).");
        }

        if (DesktopOriginalIdentity.From(FileService.IdentityFromHandle(handle)) != original.Identity)
        {
            return false;
        }

        FileAttributes current = File.GetAttributes(handle);
        if (current.HasFlag(FileAttributes.Hidden) == hidden)
        {
            return true;
        }

        FileAttributes next = hidden
            ? (current & ~FileAttributes.Normal) | FileAttributes.Hidden
            : current & ~FileAttributes.Hidden;
        File.SetAttributes(handle, next == 0 ? FileAttributes.Normal : next);
        return true;
    }

    private static SafeFileHandle Open(string path, uint access) =>
        FileTransferNativeMethods.CreateFileW(path, access, ShareAll, IntPtr.Zero, Existing,
            FileOrDirectory | OpenReparsePoint, IntPtr.Zero);
}
