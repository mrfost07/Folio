using Folio.Services;

namespace Folio.Tests;

public sealed class DesktopOriginalsTests : IDisposable
{
    private readonly string _root = Directory.CreateTempSubdirectory("folio-originals-").FullName;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void HidingAndRestoringKeepsTheSameObjectAndOtherAttributes(bool directory)
    {
        string path = Path.Combine(_root, "Original");
        if (directory)
        {
            Directory.CreateDirectory(path);
        }
        else
        {
            File.WriteAllText(path, "original contents");
        }
        FileAttributes before = File.GetAttributes(path);
        var original = Assert.IsType<DesktopOriginalCandidate>(DesktopOriginalFile.Capture(path));

        Assert.True(DesktopOriginalFile.TrySetHidden(original, hidden: true));
        Assert.True(File.GetAttributes(path).HasFlag(FileAttributes.Hidden));
        Assert.Null(DesktopOriginalFile.Capture(path));
        Assert.True(DesktopOriginalFile.TrySetHidden(original, hidden: false));
        Assert.Equal(before, File.GetAttributes(path));
        Assert.Equal(original, DesktopOriginalFile.Capture(path));
    }

    [Fact]
    public void AnOldReceiptCannotUnhideAReplacementAtTheSamePath()
    {
        string path = Path.Combine(_root, "Item.txt");
        File.WriteAllText(path, "original");
        var original = Assert.IsType<DesktopOriginalCandidate>(DesktopOriginalFile.Capture(path));
        File.Move(path, Path.Combine(_root, "Moved.txt"));
        File.WriteAllText(path, "replacement");
        File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);

        Assert.False(DesktopOriginalFile.TrySetHidden(original, hidden: false));
        Assert.True(File.GetAttributes(path).HasFlag(FileAttributes.Hidden));
        Assert.Equal("replacement", File.ReadAllText(path));
    }

    [Fact]
    public async Task OriginalReceiptAndWindowsPreferencesSurviveReload()
    {
        string path = Path.Combine(_root, "Item.txt");
        File.WriteAllText(path, "original");
        var original = Assert.IsType<DesktopOriginalCandidate>(DesktopOriginalFile.Capture(path));
        var entry = new GroupedDesktopOriginal(original, "widget",
            Path.Combine(_root, "Item.lnk"), new DesktopOriginalIdentity(1, 2, 3), DateTime.UtcNow);
        var store = new DesktopOriginalsStore(Path.Combine(_root, "desktop.json"));
        await store.SaveAsync(new DesktopOriginalsState(true, new DesktopGroupingPreferences(true, false), [entry]));

        DesktopOriginalsState loaded = await store.LoadAsync();
        Assert.True(loaded.Enabled);
        Assert.Equal(new DesktopGroupingPreferences(true, false), loaded.PreviousPreferences);
        Assert.Equal(entry, Assert.Single(loaded.Items));
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);
}
