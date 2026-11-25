using System.Text.Json;
using Folio.Contracts;
using Folio.Models;
using Folio.Services;

namespace Folio.Tests;

public sealed class PlacementTemplateTests
{
    [Theory]
    [InlineData("left", "Left", "Auto", 200, 400)]
    [InlineData("right", "Right", "Auto", 360, 620)]
    [InlineData("top", "Top", "Down", 280, 500)]
    [InlineData("bottom", "Bottom", "Up", 620, 900)]
    [InlineData("floating", "Floating", "Auto", 100, 200)]
    public void PresetsSetPlacementAndHoverTiming(
        string name, string placement, string expansion, int expandDelay, int collapseDelay)
    {
        var settings = new WidgetShellSettingsSlice();
        Assert.True(WidgetManager.ApplyBuiltInPlacementTemplate(settings, name));
        Assert.Equal("Bar", settings.WidgetCapsuleArrangementMode);
        Assert.Equal("Smart", settings.WidgetCollapseBehavior);
        Assert.Equal("TitleOnly", settings.WidgetCompactContentMode);
        Assert.Equal(placement, settings.WidgetCapsuleBarPlacement);
        Assert.Equal(expansion, settings.WidgetCompactExpansionDirection);
        Assert.Equal(expandDelay, settings.WidgetCompactExpandDelayMs);
        Assert.Equal(collapseDelay, settings.WidgetCompactCollapseDelayMs);
    }

    [Fact]
    public void SavedTemplateRoundTripsPlacementAndHoverSettings()
    {
        var surfaces = new Dictionary<string, WidgetPlacementTemplateSurface>
        {
            ["weather"] = new(new() { X = 1427, Y = 48, Width = 375, Height = 195,
                CompactPlacement = new() { X = 1510, Y = 80 } }, false, "Smart", "Display")
        };
        var template = new WidgetPlacementTemplate(
            new("Smart", "TitleOnly", false), new("Aligned", "Down"),
            new("Free", "Floating", "Auto", 8), new("Smooth", 220),
            new(100, 200), "System", ["weather"], [], surfaces);
        var type = SettingsJsonContext.Default.WidgetPlacementTemplate;

        var restored = Assert.IsType<WidgetPlacementTemplate>(
            JsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(template, type), type));

        Assert.Equal(template.Timing, restored.Timing);
        Assert.Equal(template.Arrangement, restored.Arrangement);
        Assert.Equal(1427, restored.Surfaces["weather"].Placement.X);
        Assert.Equal(1510, restored.Surfaces["weather"].Placement.CompactPlacement!.X);
        Assert.Equal("Smart", restored.Surfaces["weather"].CollapseOverride);
        Assert.False(restored.Surfaces["weather"].IsCollapsed);
    }

    [Fact]
    public void NewWeatherAndMusicStylesSurviveNormalization()
    {
        Assert.Equal(WeatherOptionKinds.SkinForecast, WeatherOptionKinds.NormalizeSkin("Forecast"));
        Assert.Equal(MusicDisplayModes.Studio, SettingsService.NormalizeMusicDisplayMode("Studio"));
    }
}
