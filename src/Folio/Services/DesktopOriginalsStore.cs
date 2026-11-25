using System.Text.Json;
using System.Text.Json.Serialization;

namespace Folio.Services;

internal sealed record DesktopGroupingPreferences(bool ShowHidden, bool AutoArrange);

internal sealed record GroupedDesktopOriginal(
    DesktopOriginalCandidate Original, string WidgetId, string LinkPath,
    DesktopOriginalIdentity LinkIdentity, DateTime LinkWriteTime);

internal sealed record DesktopOriginalsState(
    bool Enabled, DesktopGroupingPreferences? PreviousPreferences, GroupedDesktopOriginal[] Items);

[JsonSerializable(typeof(DesktopOriginalsState))]
internal sealed partial class DesktopOriginalsJsonContext : JsonSerializerContext;

internal sealed class DesktopOriginalsStore(string path)
{
    internal Task<DesktopOriginalsState> LoadAsync() => ResilientJsonStore.LoadAsync(
        path,
        json => JsonSerializer.Deserialize(json, DesktopOriginalsJsonContext.Default.DesktopOriginalsState)
            ?? throw new JsonException("Missing desktop grouping state."),
        () => new(false, null, []),
        "DesktopOriginals");

    internal Task SaveAsync(DesktopOriginalsState state) => ResilientJsonStore.SaveAsync(
        path,
        JsonSerializer.SerializeToUtf8Bytes(state, DesktopOriginalsJsonContext.Default.DesktopOriginalsState));
}
