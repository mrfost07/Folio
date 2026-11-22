using Folio.Services;
using Folio.ViewModels;

namespace Folio.Tests;

public class FileNameLineCountContractTests
{
    [Fact]
    public void AppearanceDensitySettings_ExposesHiddenSingleAndDoubleSelection()
    {
        string root = FindRepositoryRoot();
        string settingsXaml = File.ReadAllText(Path.Combine(root, "src/Folio/Views/SettingsWindow.xaml"));
        string editor = File.ReadAllText(Path.Combine(root, "src/Folio/Features/Appearance/AppearanceSettingsViewModel.cs"));

        Assert.Contains("Settings.FileNameLines.Title", settingsXaml, StringComparison.Ordinal);
        Assert.Contains("AvailableFileNameLineCountOptions", settingsXaml, StringComparison.Ordinal);
        Assert.Contains("Settings.FileNameLines.Hidden", editor, StringComparison.Ordinal);
        Assert.Contains("Settings.FileNameLines.Single", editor, StringComparison.Ordinal);
        Assert.Contains("Settings.FileNameLines.Double", editor, StringComparison.Ordinal);
    }

    [Fact]
    public void IconView_FileAndStackNames_BindToConfiguredLineCount()
    {
        string root = FindRepositoryRoot();
        string itemSurface = File.ReadAllText(Path.Combine(root, "src/Folio/Controls/FileItemSurface.xaml"));
        string fileSurface = File.ReadAllText(Path.Combine(root, "src/Folio/Controls/WidgetContents/FileSurfaceContent.xaml"));

        Assert.Contains("LayoutContext.IconLabelMaxLines", itemSurface, StringComparison.Ordinal);
        Assert.Contains("DataContext.IconLabelMaxLines", fileSurface, StringComparison.Ordinal);
        Assert.Contains("LayoutContext.IconLabelVisibility", itemSurface, StringComparison.Ordinal);
        Assert.Contains("DataContext.IconLabelVisibility", fileSurface, StringComparison.Ordinal);
    }

    [Fact]
    public void IconTileHeight_ReservesConfiguredLinesAtMaximumSystemTextScale()
    {
        double oneLineHeight = WidgetViewModel.ResolveIconTileHeight(
            iconSize: SettingsService.DefaultIconSize,
            textSize: SettingsService.DefaultTextSize,
            fileNameLineCount: SettingsService.MinFileNameLineCount,
            verticalScale: SettingsService.DefaultVerticalSpacingScale,
            systemTextScaleFactor:
                WindowsCompatibilityService.MaxSystemTextScaleFactor);
        double twoLineHeight = WidgetViewModel.ResolveIconTileHeight(
            iconSize: SettingsService.DefaultIconSize,
            textSize: SettingsService.DefaultTextSize,
            fileNameLineCount: SettingsService.DefaultFileNameLineCount,
            verticalScale: SettingsService.DefaultVerticalSpacingScale,
            systemTextScaleFactor:
                WindowsCompatibilityService.MaxSystemTextScaleFactor);
        double defaultScaleTwoLineHeight = WidgetViewModel.ResolveIconTileHeight(
            iconSize: SettingsService.DefaultIconSize,
            textSize: SettingsService.DefaultTextSize,
            fileNameLineCount: SettingsService.DefaultFileNameLineCount,
            verticalScale: SettingsService.DefaultVerticalSpacingScale,
            systemTextScaleFactor:
                WindowsCompatibilityService.MinSystemTextScaleFactor);

        Assert.True(twoLineHeight > oneLineHeight);
        Assert.True(twoLineHeight > defaultScaleTwoLineHeight);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "src", "Folio")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root.");
    }
}
