using Folio.Platform;
#if FOLIO_NATIVE_AOT && FOLIO_AOT_SMOKE_HARNESS
using Folio.Services;
#endif

namespace Folio.Helpers;

/// <summary>
/// Invokes Shell-owned property UI for a file or folder. Explorer context menus
/// are intentionally handled by <see cref="ShellContextMenuProxy"/> so native
/// menu extensions never load into the DeskBox process.
/// </summary>
public static class ShellContextMenuHelper
{
    private const uint SHOP_FILEPATH = 0x2;

    /// <summary>
    /// Shows the native properties dialog for a file or folder.
    /// </summary>
    public static bool ShowProperties(IntPtr hwnd, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

#if FOLIO_NATIVE_AOT && FOLIO_AOT_SMOKE_HARNESS
        bool trackedInvocation =
            AotFilePropertiesFixture.TryBeginInvocation(hwnd, filePath);
#endif
        try
        {
            bool invoked = Shell32NativeMethods.SHObjectProperties(
                hwnd,
                SHOP_FILEPATH,
                filePath,
                null);
#if FOLIO_NATIVE_AOT && FOLIO_AOT_SMOKE_HARNESS
            if (trackedInvocation)
            {
                AotFilePropertiesFixture.RecordInvocationResult(
                    invoked,
                    error: null);
            }
#endif
            return invoked;
        }
        catch (Exception ex)
        {
            _ = ex;
#if FOLIO_NATIVE_AOT && FOLIO_AOT_SMOKE_HARNESS
            if (trackedInvocation)
            {
                AotFilePropertiesFixture.RecordInvocationResult(
                    invoked: false,
                    ex.ToString());
            }
#endif
            throw;
        }
    }
}
