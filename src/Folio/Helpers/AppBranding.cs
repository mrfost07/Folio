using System.Drawing;
using Microsoft.UI.Windowing;

namespace Folio.Helpers;

public static class AppBranding
{
    private const string LogoIconFileName = "folio.ico";
    private const string TrayIconLightFileName = "folio-tray-light.ico";
    private const string TrayIconDarkFileName = "folio-tray-dark.ico";

    public static string LogoIconPath => Path.Combine(AppContext.BaseDirectory, "Assets", LogoIconFileName);
    public static string TrayIconLightPath => Path.Combine(AppContext.BaseDirectory, "Assets", TrayIconLightFileName);
    public static string TrayIconDarkPath => Path.Combine(AppContext.BaseDirectory, "Assets", TrayIconDarkFileName);

    public static Icon? CreateTrayIcon(string style, bool isDarkTheme)
    {
        string trayIconPath = style switch
        {
            "Colorful" => LogoIconPath,
            "Black" => TrayIconLightPath,
            "White" => TrayIconDarkPath,
            _ => isDarkTheme ? TrayIconDarkPath : TrayIconLightPath
        };

        if (!File.Exists(trayIconPath))
        {
            return null;
        }

        try
        {
            return new Icon(trayIconPath);
        }
        catch (Exception ex)
        {
            global::Folio.App.Log($"[Branding] Failed to load tray icon: {ex.Message}");
            return null;
        }
    }

    public static void ApplyWindowIcon(AppWindow? appWindow)
    {
        if (appWindow is null || !File.Exists(LogoIconPath))
        {
            return;
        }

        try
        {
            appWindow.SetIcon(LogoIconPath);
        }
        catch (Exception ex)
        {
            global::Folio.App.Log($"[Branding] Failed to apply window icon: {ex.Message}");
        }
    }
}
