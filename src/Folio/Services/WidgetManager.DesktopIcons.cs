using Folio.Helpers;
using Folio.Models;

namespace Folio.Services;

public partial class WidgetManager
{
    internal void SetAllWidgetsCollapsed(bool collapsed)
    {
        if (collapsed)
        {
            new CapsuleSettingsCoordinator(_settingsService).SetWidgetCompactContentMode(
                SettingsService.WidgetCompactContentModeTitleOnly);
        }

        var surfaces = EnumerateLayoutSurfaces(_settingsService.Settings).ToArray();
        foreach (var (_, config, group) in surfaces)
        {
            if ((group?.IsCollapsed ?? config.IsCollapsed) != collapsed)
            {
                SetSurfaceCollapsed(config, group, collapsed, marker: null);
            }
        }
    }

    internal async Task<bool> CreateWidgetsFromDesktopPathsAsync(
        IEnumerable<string> paths, bool splitByType, int minimumItemCount = 1)
    {
        var (userDesktop, publicDesktop) = FileService.GetDesktopPaths();
        string[] desktopPaths = await Task.Run(() => paths
            .Where(Path.IsPathFullyQualified)
            .Select(Path.GetFullPath)
            .Where(path => IsDesktopWidgetSource(path, userDesktop, publicDesktop) &&
                FileService.ShouldDisplayEntry(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray());
        if (desktopPaths.Length < minimumItemCount)
        {
            return false;
        }

        if (!splitByType)
        {
            return await CreateDesktopIconWidgetAsync(null, desktopPaths);
        }

        var groups = await Task.Run(() => desktopPaths.GroupBy(DesktopIconCategory).ToArray());
        bool imported = false;
        foreach (var group in groups)
        {
            imported |= await CreateDesktopIconWidgetAsync(_localizationService.T(group.Key), group.ToArray());
        }

        return imported;
    }

    internal static bool IsDesktopWidgetSource(string path, string userDesktop, string publicDesktop)
    {
        string? parent = Path.GetDirectoryName(path);
        return !string.IsNullOrEmpty(parent) &&
            (string.Equals(parent, userDesktop, StringComparison.OrdinalIgnoreCase) ||
             string.Equals(parent, publicDesktop, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<bool> CreateDesktopIconWidgetAsync(string? name, IReadOnlyList<string> paths)
    {
        WidgetConfig config = await CreateManagedWidgetCoreAsync(name, placeForFirstRun: false);
        return _fileWidgets.TryGetValue(config.Id, out FileWidgetSession? session) &&
            await session.Content.ImportNativeDroppedFilesAsync(paths,
                containsTemporaryFiles: false, forcedIntent: FileDropIntent.Shortcut);
    }

    internal static string DesktopIconCategory(string path)
    {
        if (ShortcutHelper.IsShellLinkPath(path))
        {
            string? target = ShortcutHelper.ReadStoredMetadata(path)?.TargetPath;
            if (!string.IsNullOrWhiteSpace(target))
            {
                path = target;
            }
        }

        if (Directory.Exists(path))
        {
            return "DesktopIcons.Folders";
        }

        return Path.GetExtension(path).ToLowerInvariant() is ".exe" or ".com" or ".bat" or ".cmd" or ".appref-ms"
            ? "DesktopIcons.Apps"
            : "DesktopIcons.Files";
    }
}
