using Folio.Helpers;
using Folio.Platform;
using Microsoft.UI.Dispatching;

namespace Folio.Services;

internal sealed class DesktopOriginalsService : IDisposable
{
    private readonly DesktopOriginalsStore _store;
    private readonly SettingsService _settings;
    private readonly DispatcherQueue _dispatcher;
    private readonly DispatcherQueueTimer _timer;
    private readonly DispatcherQueueTimer _auditTimer;
    private readonly SemaphoreSlim _gate = new(1);
    private readonly Dictionary<string, FileSystemWatcher> _linkWatchers = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _userDesktop;
    private readonly string _publicDesktop;
    private DesktopOriginalsState _state = new(false, null, []);
    private Dictionary<string, string> _reconciledRoots = [];
    private bool _stopping;

    internal DesktopOriginalsService(SettingsService settings, DispatcherQueue dispatcher, string storePath)
    {
        _settings = settings;
        _store = new(storePath);
        _dispatcher = dispatcher;
        (_userDesktop, _publicDesktop) = FileService.GetDesktopPaths();
        _timer = dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(3);
        _timer.IsRepeating = false;
        _timer.Tick += (_, _) => _ = ReconcileAsync();
        _auditTimer = dispatcher.CreateTimer();
        // Recover from missed notifications, a renamed root, or an unavailable watcher.
        _auditTimer.Interval = TimeSpan.FromMinutes(1);
        _auditTimer.Tick += (_, _) => ScheduleReconciliation();
        _settings.LayoutPersistCompleted += OnLayoutPersistCompleted;
    }

    internal bool Enabled => _state.Enabled;

    internal async Task LoadAsync()
    {
        await _gate.WaitAsync();
        try
        {
            _state = await _store.LoadAsync();
        }
        catch
        {
            _gate.Release();
            throw;
        }

        // Publish the stored toggle before processing desktop actions. Shell
        // and disk checks can finish in the background while startup proceeds.
        _ = ReconcileLockedAsync(startup: true);
    }

    internal async Task ToggleAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (_stopping)
            {
                return;
            }

            if (Enabled)
            {
                await CommitAsync(_state with { Enabled = false });
                await ReconcileCoreAsync(restoreAll: true, reapply: false);
                await RestorePreferencesAsync();
                return;
            }

            var previous = _state.PreviousPreferences ?? await Task.Run(() =>
                new DesktopGroupingPreferences(DesktopShellVisibility.ShowsHiddenItems(),
                    DesktopShellFolderView.ReadAutoArrange()
                    ?? throw new InvalidOperationException("Explorer's desktop layout is unavailable.")));
            // Persist the original preferences before touching Explorer so a
            // crash during activation can still be recovered by switching off.
            await CommitAsync(_state with { PreviousPreferences = previous });
            bool ready = await Task.Run(DesktopShellFolderView.TryPrepareForGroupingAsync);
            if (!ready)
            {
                await RestorePreferencesAsync();
                throw new InvalidOperationException("Explorer could not switch to manual icon placement and hidden items off.");
            }

            await CommitAsync(_state with { Enabled = true });
        }
        finally
        {
            _gate.Release();
            UpdateWatchers();
        }
    }

    internal DesktopOriginalCandidate? Capture(string path)
    {
        if (_stopping || !Enabled || !IsDesktopItem(path))
        {
            return null;
        }

        try
        {
            return DesktopOriginalFile.Capture(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            App.Log($"[DesktopOriginals] Could not inspect the original: {ex.Message}");
            return null;
        }
    }

    internal async Task AdoptExistingLinksAsync()
    {
        var managedWidgets = _settings.Settings.WidgetLayout.Widgets
            .Where(widget => widget.FollowsDefaultStoragePath && !widget.IsDisabled &&
                !string.IsNullOrWhiteSpace(widget.MappedFolderPath))
            .Select(widget => (widget.Id, Root: widget.MappedFolderPath!))
            .ToArray();
        foreach (var widget in managedWidgets)
        {
            var links = await Task.Run(() => DiscoverLinks(widget.Root));
            if (links.Count > 0)
            {
                await RecordAsync(widget.Id, links);
            }
        }
    }

    private List<(DesktopOriginalCandidate Original, string Link)> DiscoverLinks(string root)
    {
        var links = new List<(DesktopOriginalCandidate, string)>();
        try
        {
            if (!Directory.Exists(root))
            {
                return links;
            }

            foreach (string link in Directory.EnumerateFiles(root).Where(ShortcutHelper.IsShortcutPath))
            {
                string? target = ShortcutHelper.ReadStoredMetadata(link)?.TargetPath;
                DesktopOriginalCandidate? original = FindExactDesktopShortcutCopy(link);
                if (original is null && !string.IsNullOrWhiteSpace(target))
                {
                    original = Capture(target);
                }
                if (original is not null)
                {
                    links.Add((original, link));
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            App.Log($"[DesktopOriginals] Could not inspect an existing widget: {ex.Message}");
        }
        return links;
    }

    private DesktopOriginalCandidate? FindExactDesktopShortcutCopy(string link)
    {
        // A copied app shortcut no longer records the old shortcut's path.
        // Adopt it only when one desktop shortcut has the exact name and data.
        var file = new FileInfo(link);
        if (file.Length > 1024 * 1024)
        {
            return null;
        }

        byte[] data = File.ReadAllBytes(link);
        DesktopOriginalCandidate? match = null;
        foreach (string desktop in new[] { _userDesktop, _publicDesktop }
            .Where(directory => !string.IsNullOrWhiteSpace(directory))
            .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            string candidate = Path.Combine(desktop, file.Name);
            if (Capture(candidate) is { } original && new FileInfo(candidate).Length == data.Length &&
                File.ReadAllBytes(candidate).AsSpan().SequenceEqual(data))
            {
                if (match is not null)
                {
                    return null;
                }

                match = original;
            }
        }
        return match;
    }

    internal async Task RecordAsync(string widgetId,
        IReadOnlyList<(DesktopOriginalCandidate Original, string Link)> added)
    {
        await _gate.WaitAsync();
        try
        {
            if (_stopping || !Enabled || !ReadWidgetRoots().TryGetValue(widgetId, out string? widgetRoot))
            {
                return;
            }

            GroupedDesktopOriginal[] entries = await Task.Run(() =>
            {
                if (!ExplorerCanHideWithoutReflow())
                {
                    return [];
                }

                return added.Where(pair => IsUnder(pair.Link, widgetRoot) &&
                        !IsDesktopItem(pair.Link) && !IsUnder(pair.Link, pair.Original.Path))
                    .Select(pair =>
                    {
                        FileService.FileTransferSourceIdentity? link = FileService.TryCaptureSourceIdentity(pair.Link);
                        return DesktopOriginalIdentity.From(link) is { } id && link is { } value
                            ? new GroupedDesktopOriginal(pair.Original, widgetId, pair.Link, id, value.LastWriteTimeUtc)
                            : null;
                    })
                    .OfType<GroupedDesktopOriginal>()
                    .ToArray();
            });
            if (entries.Length == 0)
            {
                return;
            }

            // A durable receipt precedes each change to a user file.
            await CommitAsync(_state with
            {
                Items = _state.Items.Concat(entries).Distinct().ToArray()
            });
            await Task.Run(() =>
            {
                foreach (GroupedDesktopOriginal entry in entries)
                {
                    if (HasLiveLink(entry))
                    {
                        TryChange(entry.Original, hidden: true);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            App.Log($"[DesktopOriginals] Could not register grouped originals: {ex.Message}");
        }
        finally
        {
            _gate.Release();
            UpdateWatchers();
        }
    }

    private bool IsDesktopItem(string path) =>
        WidgetManager.IsDesktopWidgetSource(path, _userDesktop, _publicDesktop);

    private static bool IsUnder(string path, string directory)
    {
        string relative = Path.GetRelativePath(directory, path);
        return relative is not ("." or "..") && !Path.IsPathRooted(relative) &&
            !relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal);
    }

    private static bool ExplorerCanHideWithoutReflow() =>
        !DesktopShellVisibility.ShowsHiddenItems() && DesktopShellFolderView.ReadAutoArrange() == false;

    private Dictionary<string, string> ReadWidgetRoots() => _settings.Settings.WidgetLayout.Widgets
        .Where(widget => !widget.IsDisabled && !string.IsNullOrWhiteSpace(widget.MappedFolderPath) &&
            !_settings.Settings.WidgetLayout.DeletedWidgetIds.Contains(widget.Id))
        .DistinctBy(widget => widget.Id)
        .ToDictionary(widget => widget.Id, widget => widget.MappedFolderPath!);

    private static bool HasLiveLink(GroupedDesktopOriginal entry) =>
        FileService.TryCaptureSourceIdentity(entry.LinkPath) is { } link &&
        DesktopOriginalIdentity.From(link) == entry.LinkIdentity && link.LastWriteTimeUtc == entry.LinkWriteTime;

    private static bool TryChange(DesktopOriginalCandidate item, bool hidden)
    {
        try
        {
            if (DesktopOriginalFile.TrySetHidden(item, hidden))
            {
                Win32Helper.NotifyShellAttributesChanged(item.Path);
            }

            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            App.Log($"[DesktopOriginals] Could not update an original: {ex.Message}");
            return false;
        }
    }

    private async Task ReconcileAsync()
    {
        if (_stopping)
        {
            return;
        }

        if (!_gate.Wait(0))
        {
            ScheduleReconciliation();
            return;
        }

        await ReconcileLockedAsync(startup: false);
    }

    private async Task ReconcileLockedAsync(bool startup)
    {
        try
        {
            if (_stopping)
            {
                return;
            }

            using var timing = PerformanceLogger.Measure("DesktopOriginals.Reconcile");
            await ReconcileCoreAsync(restoreAll: !Enabled, reapply: startup && Enabled);
            if (startup && !Enabled)
            {
                await RestorePreferencesAsync();
            }
        }
        catch (Exception ex)
        {
            App.Log($"[DesktopOriginals] Restore check failed: {ex.Message}");
        }
        finally
        {
            _gate.Release();
            UpdateWatchers();
        }
    }

    private async Task ReconcileCoreAsync(bool restoreAll, bool reapply)
    {
        GroupedDesktopOriginal[] current = _state.Items;
        if (current.Length == 0)
        {
            return;
        }

        var roots = ReadWidgetRoots();
        _reconciledRoots = roots;
        GroupedDesktopOriginal[] retained = await Task.Run(() =>
        {
            bool hide = reapply && ExplorerCanHideWithoutReflow();
            var result = new List<GroupedDesktopOriginal>();
            foreach (var group in current.GroupBy(item => item.Original.Identity))
            {
                if (!Enabled)
                {
                    if (!TryChange(group.First().Original, hidden: false))
                    {
                        result.AddRange(group);
                    }
                    continue;
                }

                GroupedDesktopOriginal[] live = group.Where(item =>
                    roots.TryGetValue(item.WidgetId, out string? root) && IsUnder(item.LinkPath, root) &&
                    HasLiveLink(item)).ToArray();
                DesktopOriginalCandidate original = group.First().Original;
                if (live.Length > 0)
                {
                    if (hide || restoreAll)
                    {
                        TryChange(original, hidden: hide);
                    }
                    result.AddRange(live);
                }
                else if (!TryChange(original, hidden: false))
                {
                    result.AddRange(group);
                }
            }
            return result.ToArray();
        });

        if (!retained.SequenceEqual(current))
        {
            await CommitAsync(_state with { Items = retained });
        }
    }

    private async Task RestorePreferencesAsync()
    {
        if (_state.PreviousPreferences is not { } previous)
        {
            return;
        }

        bool restored = await Task.Run(() => DesktopShellVisibility.TrySetShowsHiddenItems(previous.ShowHidden) &&
            DesktopShellFolderView.TrySetAutoArrange(previous.AutoArrange));
        if (restored)
        {
            await CommitAsync(_state with { PreviousPreferences = null });
        }
    }

    private async Task CommitAsync(DesktopOriginalsState next)
    {
        await _store.SaveAsync(next);
        _state = next;
    }

    private void OnLayoutPersistCompleted() => _dispatcher.TryEnqueue(() =>
    {
        if (_stopping || _state.Items.Length == 0)
        {
            return;
        }

        Dictionary<string, string> roots = ReadWidgetRoots();
        foreach (GroupedDesktopOriginal item in _state.Items)
        {
            if (!string.Equals(roots.GetValueOrDefault(item.WidgetId),
                _reconciledRoots.GetValueOrDefault(item.WidgetId), StringComparison.OrdinalIgnoreCase))
            {
                ScheduleReconciliation();
                break;
            }
        }
    });

    private void OnLinkChanged(object sender, FileSystemEventArgs change)
    {
        if (ShortcutHelper.IsShortcutPath(change.FullPath) ||
            (change is RenamedEventArgs renamed && ShortcutHelper.IsShortcutPath(renamed.OldFullPath)))
        {
            _dispatcher.TryEnqueue(ScheduleReconciliation);
        }
    }

    private void OnWatcherError(object sender, ErrorEventArgs error) =>
        _dispatcher.TryEnqueue(ScheduleReconciliation);

    private void ScheduleReconciliation()
    {
        if (!_stopping && _state.Items.Length > 0 && !_timer.IsRunning)
        {
            _timer.Start();
        }
    }

    private void UpdateWatchers()
    {
        if (_stopping)
        {
            return;
        }

        var directories = Enabled
            ? _state.Items.Select(item => Path.GetDirectoryName(item.LinkPath))
                .OfType<string>().ToHashSet(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string directory in _linkWatchers.Keys.ToArray())
        {
            if (!directories.Contains(directory) || !_linkWatchers[directory].EnableRaisingEvents)
            {
                _linkWatchers.Remove(directory, out FileSystemWatcher? watcher);
                watcher?.Dispose();
            }
        }

        foreach (string directory in directories)
        {
            if (!_linkWatchers.ContainsKey(directory))
            {
                TryWatchLinks(directory);
            }
        }

        if (_state.Items.Length == 0)
        {
            _timer.Stop();
            _auditTimer.Stop();
        }
        else if (!_auditTimer.IsRunning)
        {
            _auditTimer.Start();
        }

        if (!Enabled)
        {
            // Retry originals whose attributes could not be restored, even
            // when there is no widget link left to watch.
            ScheduleReconciliation();
        }
    }

    private void TryWatchLinks(string directory)
    {
        FileSystemWatcher? watcher = null;
        try
        {
            watcher = new FileSystemWatcher(directory)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
            };
            watcher.Created += OnLinkChanged;
            watcher.Changed += OnLinkChanged;
            watcher.Deleted += OnLinkChanged;
            watcher.Renamed += OnLinkChanged;
            watcher.Error += OnWatcherError;
            watcher.EnableRaisingEvents = true;
            _linkWatchers.Add(directory, watcher);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            watcher?.Dispose();
            App.Log($"[DesktopOriginals] Could not watch widget links: {ex.Message}");
        }
    }

    internal async Task CloseAsync()
    {
        Dispose();
        await _gate.WaitAsync();
        try
        {
            await ReconcileCoreAsync(restoreAll: true, reapply: false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose()
    {
        _stopping = true;
        _timer.Stop();
        _auditTimer.Stop();
        _settings.LayoutPersistCompleted -= OnLayoutPersistCompleted;
        foreach (FileSystemWatcher watcher in _linkWatchers.Values)
        {
            watcher.Dispose();
        }
        _linkWatchers.Clear();
    }
}
