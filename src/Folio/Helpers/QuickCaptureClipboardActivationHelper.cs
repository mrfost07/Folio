using Folio.Models;
using Folio.Services;
using Microsoft.UI.Xaml;

namespace Folio.Helpers;

public static class QuickCaptureClipboardActivationHelper
{
    public static async Task<bool> EnableAsync(XamlRoot? xamlRoot, LocalizationService localizationService)
    {
        await App.Current.QuickCaptureSettings.EnableClipboardFromOpenWidgetAsync();
        App.Log("[QuickCaptureClipboard] Enabled");
        return true;
    }
}
