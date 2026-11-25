using System.Runtime.InteropServices;
using Folio.Platform;

namespace Folio.Services;

internal sealed class DesktopGroupingGestureService : IDisposable
{
    private const int LeftButtonUp = 0x0202;
    private const int ControlKey = 0x11;
    private const int LeftAltKey = 0xA4;
    private const int ShiftKey = 0x10;
    private const int EscapeKey = 0x1B;
    private const int DragWidth = 68;
    private const int DragHeight = 69;
    private readonly Action<string[]> _group;
    private readonly Thread _thread;
    private readonly Win32Helper.LowLevelMouseProc _hookProc;
    private readonly int _dragWidth = Math.Max(1, Win32Helper.GetSystemMetrics(DragWidth));
    private readonly int _dragHeight = Math.Max(1, Win32Helper.GetSystemMetrics(DragHeight));
    private volatile uint _threadId;
    private int _disposed;
    private int _resolving;
    private DragOrigin? _origin;

    internal DesktopGroupingGestureService(Action<string[]> group)
    {
        _group = group;
        _hookProc = MouseHook;
        _thread = new Thread(Run) { IsBackground = true, Name = "Folio.DesktopGroupingHook" };
        _thread.Start();
    }

    private void Run()
    {
        IntPtr hook = IntPtr.Zero;
        try
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            Win32Helper.PeekMessage(out _, IntPtr.Zero, 0, 0, Win32Helper.PM_NOREMOVE);
            _threadId = Win32Helper.GetCurrentThreadId();
            if (Volatile.Read(ref _disposed) != 0)
            {
                return;
            }

            hook = Win32Helper.SetWindowsMouseHookEx(
                Win32Helper.WH_MOUSE_LL, _hookProc, Win32Helper.GetModuleHandle(null), 0);
            if (hook == IntPtr.Zero)
            {
                App.Log($"[DesktopGrouping] Could not register drag gesture: {Marshal.GetLastWin32Error()}");
                return;
            }

            App.Log("[DesktopGrouping] Ctrl+Alt drag gesture registered.");
            while (Win32Helper.GetMessage(out var message, IntPtr.Zero, 0, 0) > 0)
            {
                Win32Helper.TranslateMessage(in message);
                Win32Helper.DispatchMessage(in message);
            }
        }
        catch (Exception ex)
        {
            App.Log($"[DesktopGrouping] Mouse hook stopped: {ex.Message}");
        }
        finally
        {
            if (hook != IntPtr.Zero)
            {
                Win32Helper.UnhookWindowsHookEx(hook);
            }

            _threadId = 0;
        }
    }

    private IntPtr MouseHook(int code, IntPtr message, IntPtr data)
    {
        if (code == 0 && message == (IntPtr)Win32Helper.WM_LBUTTONDOWN &&
            Win32Helper.IsKeyDown(ControlKey) && Win32Helper.IsKeyDown(LeftAltKey) &&
            !Win32Helper.IsKeyDown(ShiftKey))
        {
            var point = Marshal.PtrToStructure<Win32Helper.MSLLHOOKSTRUCT>(data).pt;
            IntPtr list = DesktopBlankHitTest.FindDesktopIconListAtPoint(point);
            if (list != IntPtr.Zero)
            {
                // Own both button events so Explorer cannot begin a move.
                _origin = new(point, list);
                return (IntPtr)1;
            }
        }

        if (code == 0 && message == (IntPtr)LeftButtonUp && _origin is { } origin)
        {
            _origin = null;
            var target = Marshal.PtrToStructure<Win32Helper.MSLLHOOKSTRUCT>(data).pt;
            if (!Win32Helper.IsKeyDown(EscapeKey) &&
                MeetsDragThreshold(origin.Point, target, _dragWidth, _dragHeight) &&
                Interlocked.CompareExchange(ref _resolving, 1, 0) == 0)
            {
                ThreadPool.QueueUserWorkItem(_ => ResolveDrop(origin, target));
            }

            return (IntPtr)1;
        }

        return Win32Helper.CallNextHookEx(IntPtr.Zero, code, message, data);
    }

    internal static bool MeetsDragThreshold(Win32Helper.POINT source, Win32Helper.POINT target,
        int width, int height) =>
        Math.Abs((long)target.X - source.X) >= width || Math.Abs((long)target.Y - source.Y) >= height;

    private void ResolveDrop(DragOrigin origin, Win32Helper.POINT target)
    {
        try
        {
            if (Volatile.Read(ref _disposed) != 0 ||
                DesktopBlankHitTest.FindDesktopIconListAtPoint(origin.Point) != origin.List ||
                DesktopBlankHitTest.FindDesktopIconListAtPoint(target) != origin.List ||
                DesktopBlankHitTest.GetListViewItemIndex(origin.List, origin.Point) is not (int first and >= 0) ||
                DesktopBlankHitTest.GetListViewItemIndex(origin.List, target) is not (int second and >= 0) ||
                first == second)
            {
                return;
            }

            string[] paths = DesktopShellFolderView.ReadPair(first, second);
            if (paths.Length == 2 && Volatile.Read(ref _disposed) == 0 &&
                DesktopBlankHitTest.GetListViewItemIndex(origin.List, origin.Point) == first &&
                DesktopBlankHitTest.GetListViewItemIndex(origin.List, target) == second)
            {
                _group(paths);
            }
        }
        catch (Exception ex)
        {
            App.Log($"[DesktopGrouping] Could not resolve desktop drag: {ex.Message}");
        }
        finally
        {
            Volatile.Write(ref _resolving, 0);
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        uint threadId = _threadId;
        if (threadId != 0)
        {
            Win32Helper.PostThreadMessage(threadId, Win32Helper.WM_QUIT, UIntPtr.Zero, IntPtr.Zero);
        }
    }

    private readonly record struct DragOrigin(Win32Helper.POINT Point, IntPtr List);
}
