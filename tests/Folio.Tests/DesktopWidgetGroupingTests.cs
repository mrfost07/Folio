using Folio.Core.Persistence;
using Folio.Platform;
using Folio.Services;

namespace Folio.Tests;

public sealed class DesktopWidgetGroupingTests
{
    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(3, -5, false)]
    [InlineData(4, 0, true)]
    [InlineData(0, -6, true)]
    public void GroupingStartsOnlyAfterTheWindowsDragThreshold(int dx, int dy, bool moved)
    {
        var start = new Win32Helper.POINT { X = -120, Y = 200 };
        var end = new Win32Helper.POINT { X = start.X + dx, Y = start.Y + dy };
        Assert.Equal(moved, DesktopGroupingGestureService.MeetsDragThreshold(start, end, 4, 6));
    }

    [Fact]
    public void ShellSelectionKeepsPathsWithSpacesAndRejectsOtherLaunchArguments()
    {
        var request = Assert.IsType<DesktopGroupingRequest>(DesktopGroupingActivation.Parse(
            ["--group-desktop-by-type", @"X:\My desktop\My file.txt", @"X:\My desktop\A folder", "--open-settings"]));

        Assert.True(request.SplitByType);
        Assert.Equal(new[] { @"X:\My desktop\My file.txt", @"X:\My desktop\A folder" }, request.Paths);
        Assert.Null(DesktopGroupingActivation.Parse(["--open-settings", @"X:\My desktop\My file.txt"]));
        Assert.Null(DesktopGroupingActivation.Parse(["--group-desktop-one"]));
    }

    [Fact]
    public void ACoalescedSignalCanDrainMultipleIndependentSelectionsOnce()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"folio-grouping-{Guid.NewGuid():N}");
        var queue = new DesktopGroupingActivation(directory);
        try
        {
            queue.Enqueue(new(false, [@"X:\Desktop\a.txt", @"X:\Desktop\b.txt"]));
            queue.Enqueue(new(true, [@"X:\Desktop\c.txt"]));

            Assert.Collection(queue.Drain(),
                first =>
                {
                    Assert.False(first.SplitByType);
                    Assert.Equal(2, first.Paths.Length);
                },
                second =>
                {
                    Assert.True(second.SplitByType);
                    Assert.Single(second.Paths);
                });
            Assert.Empty(queue.Drain());
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Theory]
    [InlineData("tool.EXE", "DesktopIcons.Apps")]
    [InlineData("notes.txt", "DesktopIcons.Files")]
    public void GroupingRecognizesFileTypes(string name, string category)
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), name);
        Assert.Equal(category, WidgetManager.DesktopIconCategory(path));
    }

    [Fact]
    public void GroupingRecognizesFolders()
    {
        Assert.Equal("DesktopIcons.Folders", WidgetManager.DesktopIconCategory(Path.GetTempPath()));
    }

    [Theory]
    [InlineData(@"X:\User\Desktop\notes.txt", true)]
    [InlineData(@"X:\Public\Desktop\tool.exe", true)]
    [InlineData(@"X:\User\Desktop\project\notes.txt", false)]
    [InlineData(@"X:\User\Documents\notes.txt", false)]
    public void GroupingOnlyAcceptsTopLevelItemsFromEitherDesktop(string path, bool accepted)
    {
        Assert.Equal(accepted, WidgetManager.IsDesktopWidgetSource(
            path, @"X:\User\Desktop", @"X:\Public\Desktop"));
    }
}
