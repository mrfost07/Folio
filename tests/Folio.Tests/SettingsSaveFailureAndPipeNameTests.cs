using System.Text.RegularExpressions;
using Folio.Services;

namespace Folio.Tests;

/// <summary>
/// Batch R9: a failed settings commit must not latch the pending-save flag
/// (it suppressed every later reload push — divergence without
/// convergence), and the settings pipe name must discriminate sessions
/// (named pipes are machine-global while the mutex is session-local).
/// </summary>
public sealed class SettingsSaveFailureAndPipeNameTests : IDisposable
{
    private readonly string _tempRoot = Path.Combine(
        Path.GetTempPath(),
        "Folio.Tests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SaveAsync_FailureStillClearsThePendingFlag()
    {
        var settings = new SettingsService(_tempRoot, new FailingSink());
        await settings.LoadAsync();
        settings.SaveDebounced();
        Assert.True(settings.HasPendingSave);

        await settings.SaveAsync();

        Assert.False(settings.HasPendingSave,
            "a latched pending flag suppressed every later reload push after one bad commit");
    }

    [Fact]
    public async Task SaveCheckedAsync_FailureStillClearsThePendingFlag()
    {
        var settings = new SettingsService(_tempRoot, new FailingSink());
        await settings.LoadAsync();
        settings.SaveDebounced();
        Assert.True(settings.HasPendingSave);

        Assert.False(await settings.SaveCheckedAsync());
        Assert.False(settings.HasPendingSave);
    }

    [Fact]
    public void SettingsIpcPipeName_CarriesTheSessionDiscriminator()
    {
        Assert.Matches(
            new Regex(@"_S\d+$", RegexOptions.None, TimeSpan.FromSeconds(1)),
            FolioDataPathService.Current.SettingsIpcPipeName);
        Assert.Contains(
            FolioDataPathService.Current.InstanceScope,
            FolioDataPathService.Current.SettingsIpcPipeName,
            StringComparison.Ordinal);
    }

    private sealed class FailingSink : ISettingsPersistenceSink
    {
        public Task<bool> SaveAsync(SettingsService owner, SettingsPersistenceCommit commit) =>
            Task.FromResult(false);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempRoot))
            {
                Directory.Delete(_tempRoot, recursive: true);
            }
        }
        catch
        {
        }
    }
}
