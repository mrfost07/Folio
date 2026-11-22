using Folio.Models;
using Folio.ViewModels;

namespace Folio.Services;

internal static class QuickCaptureClipboardCopyPolicy
{
    public static bool ShouldCopyBitmap(QuickCaptureItemViewModel item) =>
        item.IsRecent && item.Type == QuickCaptureItemType.Image;
}
