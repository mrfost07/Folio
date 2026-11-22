using Folio.Services;

namespace Folio.ViewModels;

public sealed partial class SettingsViewModel
{
    public void RefreshManagedStorageState()
    {
        ManagedStorageRootPath = SettingsService.NormalizeManagedStorageRootPath(
            _settingsService.Settings.DefaultManagedStorageRootPath);
    }
}
