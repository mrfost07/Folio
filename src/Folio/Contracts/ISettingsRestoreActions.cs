namespace Folio.Contracts;

using Folio.Services;

/// <summary>
/// Settings-side surface for local backup restore transactions, the
/// snapshot inventory, and the attachment health scan, which live in the
/// main process once the settings UI runs in its own process.
/// </summary>
public interface ISettingsRestoreActions
{
    Task<FolioRestorePreparation> PrepareRestoreAsync(string archivePath);

    /// <summary>
    /// Persists the confirmation of a pending full restore; false means the
    /// pending marker could not be confirmed and the restore is discarded.
    /// </summary>
    Task<bool> ConfirmPendingFullRestoreAsync();

    Task CancelPendingRestoreAsync();

    Task<IReadOnlyList<FolioBackupSnapshotInfo>> GetSnapshotInventoryAsync();

    Task DeleteSnapshotAsync(string snapshotPath);

    Task<FolioAttachmentHealthReport> ScanAttachmentHealthAsync();
}
