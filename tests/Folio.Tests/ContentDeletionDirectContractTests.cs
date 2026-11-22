namespace Folio.Tests;

public sealed class ContentDeletionDirectContractTests
{
    [Fact]
    public void TodoDeletionEntryPoints_DoNotRouteThroughConfirmation()
    {
        string source = ReadRepositoryFiles(
            "src/Folio/Controls/WidgetContents/TodoWidgetContent.xaml.cs",
            "src/Folio/Controls/WidgetContents/TodoWidgetContent.EditingAndUndo.cs",
            "src/Folio/Controls/WidgetContents/TodoWidgetContent.ListInteraction.cs",
            "src/Folio/Controls/WidgetContents/TodoWidgetContent.Menus.cs",
            "src/Folio/Controls/WidgetContents/TodoWidgetContent.DragDrop.cs");

        Assert.Contains("DeleteItemAsync(", source, StringComparison.Ordinal);
        Assert.Contains("DeleteSelectedItemsAsync(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ShowDeleteItemConfirmation", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ShowDeleteSelectedConfirmation", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ShowTodoConfirmMenu", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ConfirmBeforeDelete", source, StringComparison.Ordinal);
    }

    [Fact]
    public void QuickCaptureDeletionEntryPoints_DoNotRouteThroughConfirmation()
    {
        string source = ReadRepositoryFiles(
            "src/Folio/Controls/WidgetContents/QuickCaptureSurfaceContent.xaml.cs");

        Assert.Contains("DeleteQuickCaptureItemAsync(", source, StringComparison.Ordinal);
        Assert.Contains("DeleteSelectedQuickCaptureItemsAsync(", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ConfirmDeleteItemAsync", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ShowQuickCaptureDeleteConfirmFlyout", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ShowQuickCaptureDeleteSelectedConfirmFlyout", source, StringComparison.Ordinal);
        Assert.DoesNotContain("QuickCapture.DeleteConfirm", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TodoDeleteConfirmationSetting_IsRemovedFromRuntimeAndSettingsUi()
    {
        string source = ReadRepositoryFiles(
            "src/Folio/Models/AppSettings.cs",
            "src/Folio/Services/SettingsService.cs",
            "src/Folio/ViewModels/SettingsViewModel.cs",
            // Batch 47 moved the Todo switch chain onto the section editor.
            "src/Folio/Features/Todo/TodoSettingsViewModel.cs",
            "src/Folio/ViewModels/SettingsViewModel.FeatureOptions.cs",
            "src/Folio/ViewModels/SettingsViewModel.SettingsSync.cs",
            "src/Folio/ViewModels/TodoWidgetViewModel.cs",
            "src/Folio/ViewModels/TodoWidgetViewModel.FilteringAndAppearance.cs",
            "src/Folio/Views/SettingsWindow.xaml");

        Assert.DoesNotContain("TodoConfirmBeforeDelete", source, StringComparison.Ordinal);
        Assert.DoesNotContain("ConfirmBeforeDelete", source, StringComparison.Ordinal);
    }

    private static string ReadRepositoryFiles(params string[] relativePaths)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null &&
               !Directory.Exists(Path.Combine(directory.FullName, "src", "Folio")))
        {
            directory = directory.Parent;
        }

        string repositoryRoot = directory?.FullName ??
            throw new DirectoryNotFoundException();
        return string.Join(
            Environment.NewLine,
            relativePaths.Select(path => File.ReadAllText(Path.Combine(
                repositoryRoot,
                path.Replace('/', Path.DirectorySeparatorChar)))));
    }
}
