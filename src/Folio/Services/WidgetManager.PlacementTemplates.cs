using System.Text.Json;
using Folio.Contracts;
using Folio.Helpers;
using Folio.Models;

namespace Folio.Services;

internal sealed record WidgetPlacementTemplate(
    CapsuleBehaviorSettings Behavior,
    CapsuleGeometrySettings Geometry,
    CapsuleArrangementSettings Arrangement,
    CapsuleAnimationSettings Animation,
    CapsuleTimingSettings Timing,
    string CompactMediaCornerMode,
    string[] BarOrder,
    Dictionary<string, WidgetCompactPlacement> FreePlacements,
    Dictionary<string, WidgetPlacementTemplateSurface> Surfaces,
    WidgetTopologyMonitorProfile[]? Monitors = null);

internal sealed record WidgetPlacementTemplateSurface(
    WidgetSurfaceLayoutProfile Placement,
    bool IsCollapsed,
    string? CollapseOverride,
    string? BoundScreenLabel);

internal static class WidgetPlacementTemplateStore
{
    private static string FilePath =>
        Path.Combine(FolioDataPathService.Current.DataDirectory, "placement-template.json");

    internal static Task SaveAsync(WidgetPlacementTemplate template) =>
        ResilientJsonStore.SaveAsync(FilePath,
            JsonSerializer.SerializeToUtf8Bytes(template, SettingsJsonContext.Default.WidgetPlacementTemplate));

    internal static Task<WidgetPlacementTemplate?> LoadAsync() =>
        ResilientJsonStore.LoadAsync<WidgetPlacementTemplate?>(FilePath, Parse, () => null, "PlacementTemplate");

    // The settings window can live in another process. Preview reads never
    // repair or rewrite the store while the main process may be saving it.
    internal static async Task<WidgetPlacementTemplate?> ReadPreviewAsync()
    {
        foreach (string path in new[] { FilePath, ResilientJsonStore.GetBackupPath(FilePath) })
        {
            try
            {
                return Parse(await File.ReadAllTextAsync(path));
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException)
            {
                // A missing or unreadable preview does not block the picker.
            }
        }

        return null;
    }

    private static WidgetPlacementTemplate Parse(string json)
    {
        var saved = JsonSerializer.Deserialize(json, SettingsJsonContext.Default.WidgetPlacementTemplate);
        if (saved is not { BarOrder: not null, FreePlacements: not null, Surfaces.Count: > 0 } ||
            saved.Surfaces.Values.Any(surface => surface?.Placement is not { Width: > 0, Height: > 0 }))
        {
            throw new JsonException("Invalid placement template.");
        }

        return saved;
    }
}

public sealed partial class WidgetManager
{
    internal async Task<bool> RunPlacementTemplateActionAsync(string action)
    {
        try
        {
            var settings = _settingsService.Settings;
            if (action == "save")
            {
                WidgetPlacementTemplate template = CapturePlacementTemplate(settings);
                if (template.Surfaces.Count == 0)
                {
                    return false;
                }

                await WidgetPlacementTemplateStore.SaveAsync(template);
                return true;
            }

            if (action == "personal")
            {
                var template = await WidgetPlacementTemplateStore.LoadAsync();
                if (template is null || !RestorePlacementTemplate(settings, template))
                {
                    return false;
                }
            }
            else
            {
                bool applied = false;
                _settingsService.MutateGraphUnderLock(() =>
                {
                    applied = ApplyBuiltInPlacementTemplate(settings.WidgetShell, action);
                    if (!applied)
                    {
                        return;
                    }

                    foreach (var (_, config, group) in EnumerateLayoutSurfaces(settings))
                    {
                        if (group is null)
                        {
                            config.Metadata.Remove(WidgetCollapseBehaviorNames.MetadataKey);
                        }
                        else
                        {
                            group.CollapseBehavior = CapsuleOptionKinds.CollapseSystem;
                        }
                    }
                });
                if (!applied)
                {
                    return false;
                }
                SetAllWidgetsCollapsed(true);
            }

            RepositionAllLoadedWindows();
            RefreshCapsuleBarLayout();
            NotifyScreenHomeChangedAction?.Invoke();
            return await _settingsService.SaveCheckedAsync();
        }
        catch (Exception error)
        {
            App.Log($"[PlacementTemplate] Could not complete '{action}': {error.Message}");
            return false;
        }
    }

    private WidgetPlacementTemplate CapturePlacementTemplate(AppSettings settings)
    {
        var capsule = new CapsuleSettingsCoordinator(_settingsService);
        WidgetTopologyLayoutProfile? activeProfile = ActivePlacementProfile(settings);
        var surfaces = new Dictionary<string, WidgetPlacementTemplateSurface>(StringComparer.Ordinal);
        foreach (var (surfaceId, config, group) in EnumerateLayoutSurfaces(settings))
        {
            WidgetSurfaceLayoutProfile placement = group is null
                ? WidgetTopologyLayoutService.CaptureWidgetLayout(config, activeProfile?.Monitors ?? [])
                : WidgetTopologyLayoutService.CaptureGroupLayout(group, activeProfile?.Monitors ?? []);
            placement.PositionMonitorStableId = (group?.PositionMonitorStableId ?? config.PositionMonitorStableId)
                ?? placement.PositionMonitorStableId;
            surfaces[surfaceId] = new(placement,
                group?.IsCollapsed ?? config.IsCollapsed,
                group?.CollapseBehavior ?? config.Metadata.GetValueOrDefault(WidgetCollapseBehaviorNames.MetadataKey),
                group?.BoundScreenLabel ?? config.BoundScreenLabel);
        }

        return new(capsule.ReadBehavior(), capsule.ReadGeometry(), capsule.ReadArrangement(),
            capsule.ReadAnimation(), capsule.ReadTiming(), settings.WidgetShell.WidgetCompactMediaCornerMode,
            settings.WidgetShell.WidgetCapsuleBarOrder.ToArray(),
            new(settings.WidgetShell.WidgetCapsuleFreePlacements, StringComparer.Ordinal),
            surfaces, activeProfile?.Monitors.ToArray());
    }

    private bool RestorePlacementTemplate(AppSettings settings, WidgetPlacementTemplate template)
    {
        var matches = EnumerateLayoutSurfaces(settings)
            .Where(entry => template.Surfaces.ContainsKey(entry.SurfaceId))
            .ToArray();
        if (matches.Length == 0)
        {
            return false;
        }

        _settingsService.MutateGraphUnderLock(() =>
        {
            WidgetShellSettingsSlice shell = settings.WidgetShell;
            shell.WidgetCollapseBehavior = CapsuleOptionKinds.NormalizeCollapseBehavior(template.Behavior.CollapseBehavior);
            shell.WidgetCompactContentMode = CapsuleOptionKinds.NormalizeContentMode(template.Behavior.CompactContentMode);
            shell.WidgetCompactHideSensitiveContent = template.Behavior.HideSensitiveContent;
            shell.WidgetCompactWidthMode = CapsuleOptionKinds.NormalizeWidthMode(template.Geometry.CompactWidthMode);
            shell.WidgetCompactExpansionDirection = CapsuleOptionKinds.NormalizeExpansionDirection(template.Geometry.CompactExpansionDirection);
            shell.WidgetCapsuleArrangementMode = CapsuleOptionKinds.NormalizeArrangementMode(template.Arrangement.ArrangementMode);
            shell.WidgetCapsuleBarPlacement = CapsuleOptionKinds.NormalizeBarPlacement(template.Arrangement.BarPlacement);
            shell.WidgetCapsuleBarDirection = CapsuleOptionKinds.NormalizeBarDirection(template.Arrangement.BarDirection);
            shell.WidgetCapsuleBarSpacing = SettingsService.NormalizeWidgetCapsuleBarSpacing(template.Arrangement.BarSpacing);
            shell.WidgetCompactAnimationEffect = SettingsService.NormalizeWidgetCompactAnimationEffect(template.Animation.CompactAnimationEffect);
            shell.WidgetCompactAnimationDurationMs = SettingsService.NormalizeWidgetCompactAnimationDurationMs(template.Animation.CompactAnimationDurationMs);
            shell.WidgetCompactExpandDelayMs = SettingsService.NormalizeWidgetCompactExpandDelayMs(template.Timing.ExpandDelayMs);
            shell.WidgetCompactCollapseDelayMs = SettingsService.NormalizeWidgetCompactCollapseDelayMs(template.Timing.CollapseDelayMs);
            shell.WidgetCompactMediaCornerMode = SettingsService.NormalizeWidgetCompactMediaCornerMode(template.CompactMediaCornerMode);
            shell.WidgetCapsuleBarOrder = template.BarOrder.ToList();
            shell.WidgetCapsuleFreePlacements = template.FreePlacements;

            WidgetTopologyLayoutProfile? activeProfile = ActivePlacementProfile(settings);
            var authoredAt = DateTimeOffset.UtcNow;
            foreach (var (surfaceId, config, group) in matches)
            {
                WidgetPlacementTemplateSurface saved = template.Surfaces[surfaceId];
                WidgetSurfaceLayoutProfile placement = saved.Placement;
                placement.IsAuthoritative = true;
                placement.AuthoredAtUtc = authoredAt;
                if (activeProfile is not null)
                {
                    activeProfile.Surfaces[surfaceId] = CloneLayoutShallow(placement);
                }

                if (group is null)
                {
                    if (saved.CollapseOverride is { } collapse)
                        config.Metadata[WidgetCollapseBehaviorNames.MetadataKey] = collapse;
                    else
                        config.Metadata.Remove(WidgetCollapseBehaviorNames.MetadataKey);
                    RestoreMemberPlacement(config, saved, authoredAt);
                    continue;
                }

                WidgetTopologyLayoutService.ApplyToGroup(group, placement);
                group.ScreenBindingMode = placement.ScreenBindingMode;
                group.BoundScreenId = placement.BoundScreenId;
                group.BoundScreenLabel = saved.BoundScreenLabel;
                group.CollapseBehavior = saved.CollapseOverride ?? CapsuleOptionKinds.CollapseSystem;
                foreach (string memberId in group.MemberIds)
                {
                    if (FindConfig(memberId) is { } member)
                        RestoreMemberPlacement(member, saved, authoredAt);
                }
            }
        });

        foreach (var (surfaceId, config, group) in matches)
        {
            bool collapsed = template.Surfaces[surfaceId].IsCollapsed;
            if ((group?.IsCollapsed ?? config.IsCollapsed) != collapsed)
                SetSurfaceCollapsed(config, group, collapsed, marker: null);
        }
        return true;
    }

    private static void RestoreMemberPlacement(
        WidgetConfig config, WidgetPlacementTemplateSurface saved, DateTimeOffset authoredAt)
    {
        WidgetTopologyLayoutService.ApplyToWidget(config, saved.Placement);
        config.ScreenBindingMode = saved.Placement.ScreenBindingMode;
        config.BoundScreenId = saved.Placement.BoundScreenId;
        config.BoundScreenLabel = saved.BoundScreenLabel;
        config.LastUserPlacementUtc = authoredAt;
    }

    private static WidgetTopologyLayoutProfile? ActivePlacementProfile(AppSettings settings) =>
        settings.WidgetLayout.ActiveWidgetTopologyKey is { } key &&
        settings.WidgetLayout.WidgetTopologyLayouts.TryGetValue(key, out var profile) ? profile : null;

    internal static bool ApplyBuiltInPlacementTemplate(WidgetShellSettingsSlice shell, string name)
    {
        var preset = name switch
        {
            "left" => (CapsuleOptionKinds.BarPlacementLeft, CapsuleOptionKinds.ExpansionDirectionAuto, 200, 400),
            "right" => (CapsuleOptionKinds.BarPlacementRight, CapsuleOptionKinds.ExpansionDirectionAuto, 360, 620),
            "top" => (CapsuleOptionKinds.BarPlacementTop, CapsuleOptionKinds.ExpansionDirectionDown, 280, 500),
            "bottom" => (CapsuleOptionKinds.BarPlacementBottom, CapsuleOptionKinds.ExpansionDirectionUp, 620, 900),
            "floating" => (CapsuleOptionKinds.BarPlacementFloating, CapsuleOptionKinds.ExpansionDirectionAuto, 100, 200),
            _ => default
        };
        if (preset.Item1 is null)
        {
            return false;
        }

        shell.WidgetCollapseBehavior = CapsuleOptionKinds.CollapseSmart;
        shell.WidgetCompactContentMode = CapsuleOptionKinds.ContentModeTitleOnly;
        shell.WidgetCapsuleArrangementMode = CapsuleOptionKinds.ArrangementBar;
        shell.WidgetCapsuleBarPlacement = preset.Item1;
        shell.WidgetCapsuleBarDirection = CapsuleOptionKinds.BarDirectionAuto;
        shell.WidgetCompactExpansionDirection = preset.Item2;
        shell.WidgetCompactExpandDelayMs = preset.Item3;
        shell.WidgetCompactCollapseDelayMs = preset.Item4;
        return true;
    }
}
