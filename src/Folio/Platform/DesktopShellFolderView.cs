using System.Runtime.InteropServices;

namespace Folio.Platform;

internal static class DesktopShellFolderView
{
    private static readonly Guid ShellWindowsClass = new("9BA05972-F6A8-11CF-A442-00A0C90A8F39");
    private static readonly Guid ShellWindowsInterface = new("85CB6900-4D95-11CF-960C-0080C7F4EE85");
    private static readonly Guid ServiceProviderInterface = new("6D5140C1-7436-11CE-8034-00AA006009FA");
    private static readonly Guid TopLevelBrowserService = new("4C96BE40-915C-11CF-99D3-00AA004AE837");
    private static readonly Guid ShellBrowserInterface = new("000214E2-0000-0000-C000-000000000046");
    private static readonly Guid FolderViewInterface = new("1AF3A467-214F-4298-908E-06B03E0B39F9");
    private static readonly Guid FolderViewOptionsInterface = new("3CC974D2-B302-4D36-AD3E-06D93F695D3F");
    private static readonly Guid ShellItemInterface = new("43826D1E-E718-42EE-BC55-A1E261C37BFE");

    // Verified against ExDisp.h and ShObjIdl_core.h.
    private const int FindWindowSlot = 15;
    private const int QueryServiceSlot = 3;
    private const int GetActiveViewSlot = 15;
    private const int GetChildSlot = 6;
    private const int CountItemsSlot = 7;
    private const int GetItemPositionSlot = 11;
    private const int GetItemSlot = 29;
    private const int GetSelectionSlot = 32;
    private const int GetDisplayNameSlot = 5;
    private const int SelectionCountSlot = 7;
    private const int SelectionItemSlot = 8;
    private const int GetAutoArrangeSlot = 14;
    private const int PositionItemsSlot = 16;
    private const int SetCurrentFolderFlagsSlot = 24;
    private const int SetFolderViewOptionsSlot = 3;
    private const uint AutoArrangeFlag = 1;
    private const uint CustomPositionFlag = 2;
    private const uint AllViewItems = 2;
    private const uint PositionWithoutSelection = 0x80000080;
    private const int ShellWindowDesktop = 8;
    private const int NeedDispatch = 1;
    private const uint FileSystemPath = 0x80058000;
    private const uint AbsoluteParsingName = 0x80028000;
    private const uint ClassContextAll = 23;
    private const int RpcChangedMode = unchecked((int)0x80010106);

    internal static string[] ReadPair(int firstIndex, int secondIndex) =>
        WithView(view =>
        {
            string? first = ReadName(view, firstIndex, FileSystemPath);
            string? second = ReadName(view, secondIndex, FileSystemPath);
            return Path.IsPathFullyQualified(first ?? "") && Path.IsPathFullyQualified(second ?? "") &&
                !string.Equals(first, second, StringComparison.OrdinalIgnoreCase)
                ? [first!, second!]
                : [];
        }, Array.Empty<string>());

    internal static (int SelectedCount, string[] Paths) ReadSelection() =>
        WithView<(int SelectedCount, string[] Paths)>(ReadSelection, (0, []));

    private static (int SelectedCount, string[] Paths) ReadSelection(IntPtr view)
    {
        int result = ComVtable.Slot<GetSelection>(view, GetSelectionSlot)(view, 0, out IntPtr selection);
        try
        {
            if (result < 0 || selection == IntPtr.Zero ||
                ComVtable.Slot<GetSelectionCount>(selection, SelectionCountSlot)(selection, out int count) < 0)
            {
                return (0, []);
            }

            var paths = new List<string>();
            var getItem = ComVtable.Slot<GetSelectionItem>(selection, SelectionItemSlot);
            for (int index = 0; index < count; index++)
            {
                int itemResult = getItem(selection, index, out IntPtr item);
                try
                {
                    if (itemResult >= 0 && item != IntPtr.Zero &&
                        ReadName(item, FileSystemPath) is { } path && Path.IsPathFullyQualified(path))
                    {
                        paths.Add(path);
                    }
                }
                finally
                {
                    Release(item);
                }
            }

            return (count, paths.ToArray());
        }
        finally
        {
            Release(selection);
        }
    }

    internal static bool? ReadAutoArrange() => WithView(ReadAutoArrange, (bool?)null);

    internal static bool TrySetAutoArrange(bool enabled) => WithView(view =>
    {
        if (ReadAutoArrange(view) == enabled)
        {
            return true;
        }

        Guid optionsId = FolderViewOptionsInterface;
        ComVtable.QueryInterface(view, ref optionsId, out IntPtr options);
        try
        {
            if (options != IntPtr.Zero)
            {
                _ = ComVtable.Slot<SetFlags>(options, SetFolderViewOptionsSlot)(
                    options, CustomPositionFlag, CustomPositionFlag);
            }

            return ComVtable.Slot<SetFlags>(view, SetCurrentFolderFlagsSlot)(
                view, AutoArrangeFlag, enabled ? AutoArrangeFlag : 0) >= 0 &&
                ReadAutoArrange(view) == enabled;
        }
        finally
        {
            Release(options);
        }
    }, false);

    internal static async Task<bool> TryPrepareForGroupingAsync()
    {
        if (!DesktopShellVisibility.ShowsHiddenItems() && ReadAutoArrange() == false)
        {
            return true;
        }

        var positions = new Dictionary<string, DesktopPoint>(StringComparer.OrdinalIgnoreCase);
        if (!WithView(view => VisitPositions(view, (name, _, point) => positions.TryAdd(name, point)), false) ||
            !DesktopShellVisibility.TrySetShowsHiddenItems(false) || !TrySetAutoArrange(false))
        {
            return false;
        }

        // Explorer refreshes its view after the preference changes. Restore
        // coordinates on the refreshed view, then verify across that refresh.
        var refreshInterval = TimeSpan.FromMilliseconds(500);
        for (int attempt = 0; attempt < 3; attempt++)
        {
            await Task.Delay(refreshInterval).ConfigureAwait(false);
            if (!WithView(view => RestorePositions(view, positions), false))
            {
                continue;
            }

            await Task.Delay(refreshInterval).ConfigureAwait(false);
            if (WithView(view => VisitPositions(view, (name, _, point) =>
                !positions.TryGetValue(name, out DesktopPoint original) || original == point), false))
            {
                return true;
            }
        }

        return false;
    }

    private static bool RestorePositions(IntPtr view, Dictionary<string, DesktopPoint> positions)
    {
        var positionItem = ComVtable.Slot<PositionItem>(view, PositionItemsSlot);
        return VisitPositions(view, (name, child, point) =>
            !positions.TryGetValue(name, out DesktopPoint original) || original == point ||
            positionItem(view, 1, in child, in original, PositionWithoutSelection) >= 0);
    }

    private static bool VisitPositions(IntPtr view, Func<string, IntPtr, DesktopPoint, bool> action)
    {
        if (ComVtable.Slot<CountItems>(view, CountItemsSlot)(view, AllViewItems, out int count) < 0)
        {
            return false;
        }

        var getChild = ComVtable.Slot<GetChild>(view, GetChildSlot);
        var getPosition = ComVtable.Slot<GetItemPosition>(view, GetItemPositionSlot);
        for (int index = 0; index < count; index++)
        {
            string? name = ReadName(view, index, AbsoluteParsingName);
            int result = getChild(view, index, out IntPtr child);
            try
            {
                if (string.IsNullOrEmpty(name) || result < 0 || child == IntPtr.Zero ||
                    getPosition(view, child, out DesktopPoint point) < 0 || !action(name, child, point))
                {
                    return false;
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(child);
            }
        }

        return true;
    }

    private static bool? ReadAutoArrange(IntPtr view) =>
        ComVtable.Slot<GetAutoArrange>(view, GetAutoArrangeSlot)(view) switch
        {
            0 => true,
            1 => false,
            _ => null
        };

    private static T WithView<T>(Func<IntPtr, T> action, T fallback)
    {
        int initialized = Ole32NativeMethods.CoInitializeEx(IntPtr.Zero, 0);
        if (initialized < 0 && initialized != RpcChangedMode)
        {
            return fallback;
        }

        IntPtr windows = IntPtr.Zero;
        IntPtr dispatch = IntPtr.Zero;
        IntPtr provider = IntPtr.Zero;
        IntPtr browser = IntPtr.Zero;
        IntPtr shellView = IntPtr.Zero;
        IntPtr folderView = IntPtr.Zero;
        try
        {
            Guid clsid = ShellWindowsClass;
            Guid windowsId = ShellWindowsInterface;
            Guid providerId = ServiceProviderInterface;
            Guid browserId = ShellBrowserInterface;
            Guid topLevel = TopLevelBrowserService;
            Guid folderId = FolderViewInterface;
            // CSIDL_DESKTOP is zero. The reserved root argument is VT_EMPTY.
            AutomationVariant desktop = new() { Type = 3 };
            AutomationVariant root = default;
            if (Ole32NativeMethods.CoCreateInstance(in clsid, IntPtr.Zero, ClassContextAll, in windowsId, out windows) < 0 ||
                windows == IntPtr.Zero ||
                ComVtable.Slot<FindWindow>(windows, FindWindowSlot)(
                    windows, ref desktop, ref root, ShellWindowDesktop, out _, NeedDispatch, out dispatch) < 0 ||
                dispatch == IntPtr.Zero ||
                ComVtable.QueryInterface(dispatch, ref providerId, out provider) < 0 ||
                provider == IntPtr.Zero ||
                ComVtable.Slot<QueryService>(provider, QueryServiceSlot)(
                    provider, ref topLevel, ref browserId, out browser) < 0 ||
                browser == IntPtr.Zero ||
                ComVtable.Slot<GetActiveView>(browser, GetActiveViewSlot)(browser, out shellView) < 0 ||
                shellView == IntPtr.Zero ||
                ComVtable.QueryInterface(shellView, ref folderId, out folderView) < 0 ||
                folderView == IntPtr.Zero)
            {
                return fallback;
            }

            return action(folderView);
        }
        finally
        {
            Release(folderView);
            Release(shellView);
            Release(browser);
            Release(provider);
            Release(dispatch);
            Release(windows);
            if (initialized >= 0)
            {
                Ole32NativeMethods.CoUninitialize();
            }
        }
    }

    private static string? ReadName(IntPtr view, int index, uint nameType)
    {
        Guid itemId = ShellItemInterface;
        IntPtr item = IntPtr.Zero;
        try
        {
            return ComVtable.Slot<GetItem>(view, GetItemSlot)(view, index, ref itemId, out item) >= 0 &&
                item != IntPtr.Zero ? ReadName(item, nameType) : null;
        }
        finally
        {
            Release(item);
        }
    }

    private static string? ReadName(IntPtr item, uint nameType)
    {
        IntPtr path = IntPtr.Zero;
        try
        {
            return ComVtable.Slot<GetDisplayName>(item, GetDisplayNameSlot)(item, nameType, out path) >= 0
                ? Marshal.PtrToStringUni(path)
                : null;
        }
        finally
        {
            Marshal.FreeCoTaskMem(path);
        }
    }

    private static void Release(IntPtr reference)
    {
        if (reference != IntPtr.Zero)
        {
            Marshal.Release(reference);
        }
    }

    // Folio targets 64-bit Windows. This VARIANT holds only VT_I4 or VT_EMPTY.
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    private struct AutomationVariant
    {
        [FieldOffset(0)] public ushort Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct DesktopPoint(int X, int Y);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int FindWindow(IntPtr self, ref AutomationVariant location, ref AutomationVariant root,
        int windowClass, out int hwnd, int options, out IntPtr dispatch);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int QueryService(IntPtr self, ref Guid service, ref Guid iid, out IntPtr result);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetActiveView(IntPtr self, out IntPtr view);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetItem(IntPtr self, int index, ref Guid iid, out IntPtr item);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetSelection(IntPtr self, int noneImpliesFolder, out IntPtr selection);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetSelectionCount(IntPtr self, out int count);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetSelectionItem(IntPtr self, int index, out IntPtr item);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetDisplayName(IntPtr self, uint nameType, out IntPtr name);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetAutoArrange(IntPtr self);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int CountItems(IntPtr self, uint scope, out int count);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetChild(IntPtr self, int index, out IntPtr child);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int GetItemPosition(IntPtr self, IntPtr child, out DesktopPoint position);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int PositionItem(IntPtr self, uint count, in IntPtr child, in DesktopPoint point, uint flags);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int SetFlags(IntPtr self, uint mask, uint flags);
}
