using Folio.Core.Persistence;
using Folio.Services;

namespace Folio;

public partial class App
{
    private static DesktopGroupingActivation DesktopGroupingQueue =>
        new(Path.Combine(FolioDataPathService.Current.RootPath, "desktop-grouping-activations"));

    private readonly bool _isDesktopGroupingLaunch;
    private DesktopGroupingGestureService? _desktopGroupingGesture;
    internal DesktopOriginalsService? DesktopOriginals { get; private set; }

    private async Task LoadDesktopOriginalsAsync()
    {
        var service = new DesktopOriginalsService(SettingsService, UiDispatcherQueue,
            Path.Combine(FolioDataPathService.Current.DataDirectory, "grouped-desktop-originals.json"));
        DesktopOriginals = service;
        await service.LoadAsync();
        RefreshTrayMenuText();
    }

    internal async Task ToggleDesktopOriginalsAsync()
    {
        if (DesktopOriginals is not { } service)
        {
            return;
        }

        try
        {
            await service.ToggleAsync();
            Log($"[DesktopOriginals] Hide grouped originals enabled={service.Enabled}");
            if (service.Enabled)
            {
                await service.AdoptExistingLinksAsync();
            }
        }
        catch (Exception ex)
        {
            Log($"[DesktopOriginals] Could not switch desktop hiding: {ex.Message}");
            _nativeNotificationService?.TryShow(LocalizationService.T("DesktopIcons.HideOriginals"),
                LocalizationService.T("DesktopIcons.HidingUnavailable"));
        }
        finally
        {
            RefreshTrayMenuText();
        }
    }

    private async Task CloseDesktopOriginalsAsync()
    {
        if (DesktopOriginals is { } service)
        {
            DesktopOriginals = null;
            await service.CloseAsync();
        }
    }

    private void StartDesktopGroupingGesture()
    {
        _desktopGroupingGesture = new DesktopGroupingGestureService(paths =>
        {
            UiDispatcherQueue?.TryEnqueue(() =>
                SafeFireAndForget(() => GroupDraggedDesktopIconsAsync(paths), "desktop-grouping-gesture"));
        });
    }

    private async Task GroupDraggedDesktopIconsAsync(string[] paths)
    {
        if (!IsShuttingDown && WidgetManager is { } manager &&
            await manager.CreateWidgetsFromDesktopPathsAsync(paths, splitByType: false, minimumItemCount: 2))
        {
            Log("[DesktopGrouping] Created a widget from two desktop icons.");
        }
    }

    private static bool QueueDesktopGroupingActivation()
    {
        var request = DesktopGroupingActivation.Parse(Environment.GetCommandLineArgs()[1..]);
        if (request is null)
        {
            return false;
        }

        try
        {
            DesktopGroupingQueue.Enqueue(request);
            Log($"[DesktopGrouping] Queued Shell selection: items={request.Paths.Length}, split={request.SplitByType}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Log($"[DesktopGrouping] Could not queue Shell selection: {ex.Message}");
        }
        return true;
    }

    private async Task<bool> HandleDesktopGroupingActivationsAsync()
    {
        if (WidgetManager is not { } manager)
        {
            return false;
        }

        IReadOnlyList<DesktopGroupingRequest> requests;
        try
        {
            requests = DesktopGroupingQueue.Drain();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Log($"[DesktopGrouping] Could not read Shell selections: {ex.Message}");
            return false;
        }

        foreach (var request in requests)
        {
            try
            {
                if (await manager.CreateWidgetsFromDesktopPathsAsync(request.Paths, request.SplitByType))
                {
                    continue;
                }
            }
            catch (Exception ex)
            {
                Log($"[DesktopGrouping] Could not group Shell selection: {ex.Message}");
            }

            _nativeNotificationService?.TryShow(
                LocalizationService.T("DesktopIcons.GroupItems"),
                LocalizationService.T("DesktopIcons.SelectItems"));
        }

        return requests.Count > 0;
    }
}
