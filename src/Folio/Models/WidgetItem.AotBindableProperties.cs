#if FOLIO_NATIVE_AOT
namespace Folio.Models;

// The first non-empty File Widget AOT surface needs runtime DataTemplate
// bindings for the concrete item type. Keep this provider to the properties
// consumed by FileItemSurface.
[WinRT.GeneratedBindableCustomProperty([
    nameof(FallbackIconVisibility),
    nameof(FullPath),
    nameof(Icon),
    nameof(IconVisibility),
    nameof(Name),
    nameof(SecondaryInfo)
], [])]
public partial class WidgetItem
{
}
#endif
