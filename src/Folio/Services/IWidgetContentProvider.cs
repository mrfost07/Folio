using Folio.Contracts;
using Folio.Models;

namespace Folio.Services;

internal sealed record WidgetContentProviderContext(
    LocalizationService LocalizationService,
    SettingsService? SettingsService,
    Func<WidgetConfig, TodoWidgetStore>? TodoStoreFactory,
    Func<WidgetKind, WidgetContentDescriptor> GetDescriptor);

internal interface IWidgetContentProvider
{
    WidgetKind WidgetKind { get; }

    bool CanCreateDetachedContent { get; }

    IWidgetContent CreateDetachedContent(WidgetConfig config, WidgetContentProviderContext context);
}
