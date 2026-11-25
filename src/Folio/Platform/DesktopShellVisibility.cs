using System.Runtime.InteropServices;

namespace Folio.Platform;

internal static partial class DesktopShellVisibility
{
    private const uint ShowAllObjects = 1;

    internal static bool ShowsHiddenItems()
    {
        ShellState state = default;
        SHGetSetSettings(ref state, ShowAllObjects, 0);
        return (state.VisibilityBits & ShowAllObjects) != 0;
    }

    internal static bool TrySetShowsHiddenItems(bool show)
    {
        if (ShowsHiddenItems() == show)
        {
            return true;
        }

        ShellState state = new() { VisibilityBits = show ? ShowAllObjects : 0 };
        SHGetSetSettings(ref state, ShowAllObjects, 1);
        return ShowsHiddenItems() == show;
    }

    // SHELLSTATEW consists of eight DWORD-aligned fields. The first bit is
    // fShowAllObjects, requested independently from the rest of the state.
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    private struct ShellState
    {
        [FieldOffset(0)] public uint VisibilityBits;
    }

    [LibraryImport("shell32.dll")]
    private static partial void SHGetSetSettings(ref ShellState state, uint mask, int set);
}
