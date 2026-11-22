namespace Folio.Services;

using Folio.Contracts;

/// <summary>
/// In-process default of <see cref="ISettingsRestoreActions"/>: a thin
/// wrapper over the App-owned backup and attachment-health services.
/// </summary>
public sealed class InProcessSettingsRestoreActions : ISettingsRestoreActions
{
    private readonly FolioDataBackupService _dataBackupService;
    private readonly FolioAttachmentHealthService _attachmentHealthService;

    public InProcessSettingsRestoreActions(
        FolioDataBackupService dataBackupService,
        FolioAttachmentHealthService attachmentHealthService)
    {
        _dataBackupService = dataBackupService;
        _attachmentHealthService = attachmentHealthService;
    }

    public Task<FolioRestorePreparation> PrepareRestoreAsync(string archivePath) =>
        _dataBackupService.PrepareRestoreAsync(archivePath);

    public Task<bool> ConfirmPendingFullRestoreAsync() =>
        _dataBackupService.ConfirmPendingFullRestoreAsync();

    public Task CancelPendingRestoreAsync() =>
        _dataBackupService.CancelPendingRestoreAsync();

    public Task<IReadOnlyList<FolioBackupSnapshotInfo>> GetSnapshotInventoryAsync() =>
        _dataBackupService.GetSnapshotInventoryAsync();

    public Task DeleteSnapshotAsync(string snapshotPath) =>
        _dataBackupService.DeleteSnapshotAsync(snapshotPath);

    public Task<FolioAttachmentHealthReport> ScanAttachmentHealthAsync() =>
        _attachmentHealthService.ScanAsync();
}
