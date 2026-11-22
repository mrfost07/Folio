# Milestone 3 Automation Script: Managed Code, Namespaces, UI & Identifiers
$ErrorActionPreference = "Stop"

$root = "C:\Users\fosta\OneDrive\Desktop\STARTUP\Folio\Folio"
Set-Location $root

Write-Host "=== Phase 1 & 2: Updating C# files ==="
$csFiles = Get-ChildItem -Recurse -Include *.cs -Path "$root\src", "$root\tests"

$typeReplacements = @(
    @("ActiveFolioDragRegistryTests", "ActiveFolioDragRegistryTests"),
    @("ActiveFolioDragRegistry", "ActiveFolioDragRegistry"),
    @("FolioAttachmentHealthServiceTests", "FolioAttachmentHealthServiceTests"),
    @("FolioAttachmentHealthService", "FolioAttachmentHealthService"),
    @("FolioAttachmentHealthReport", "FolioAttachmentHealthReport"),
    @("FolioClipboardWriteScope", "FolioClipboardWriteScope"),
    @("FolioDataBackupServiceTests", "FolioDataBackupServiceTests"),
    @("FolioDataBackupService", "FolioDataBackupService"),
    @("FolioBackupManifest", "FolioBackupManifest"),
    @("FolioBackupFileManifest", "FolioBackupFileManifest"),
    @("FolioBackupSnapshotInfo", "FolioBackupSnapshotInfo"),
    @("FolioRestorePreparation", "FolioRestorePreparation"),
    @("FolioTodoWidgetRemap", "FolioTodoWidgetRemap"),
    @("FolioDomainItemCount", "FolioDomainItemCount"),
    @("FolioRestoreApplyKind", "FolioRestoreApplyKind"),
    @("FolioRestoreApplyResult", "FolioRestoreApplyResult"),
    @("FolioDataPathIsolationTests", "FolioDataPathIsolationTests"),
    @("FolioDataPathService", "FolioDataPathService"),
    @("FolioDiagnosticsBundleServiceTests", "FolioDiagnosticsBundleServiceTests"),
    @("FolioDiagnosticsBundleService", "FolioDiagnosticsBundleService"),
    @("FolioDiagnosticRect", "FolioDiagnosticRect"),
    @("FolioDisplayDiagnostic", "FolioDisplayDiagnostic"),
    @("FolioWidgetHostDiagnostic", "FolioWidgetHostDiagnostic"),
    @("FolioTrayQueueDiagnostic", "FolioTrayQueueDiagnostic"),
    @("FolioFileHostDiagnostic", "FolioFileHostDiagnostic"),
    @("FolioWidgetManagerDiagnostic", "FolioWidgetManagerDiagnostic"),
    @("FolioSettingsDiagnostic", "FolioSettingsDiagnostic"),
    @("FolioStartupLedgerDiagnostic", "FolioStartupLedgerDiagnostic"),
    @("FolioShortcutNativeDiagnostic", "FolioShortcutNativeDiagnostic"),
    @("FolioHotkeyDiagnostic", "FolioHotkeyDiagnostic"),
    @("FolioCallbackRadarDiagnostic", "FolioCallbackRadarDiagnostic"),
    @("FolioRecoveryOfferDiagnostic", "FolioRecoveryOfferDiagnostic"),
    @("FolioDiagnosticSnapshot", "FolioDiagnosticSnapshot"),
    @("FolioDragDataTests", "FolioDragDataTests"),
    @("FolioDragData", "FolioDragData"),
    @("FolioFeedbackKind", "FolioFeedbackKind"),
    @("FolioFeedbackSubmissionResult", "FolioFeedbackSubmissionResult"),
    @("FolioFeedbackDiagnosticsStatus", "FolioFeedbackDiagnosticsStatus"),
    @("FolioFeedbackItem", "FolioFeedbackItem"),
    @("FolioFeedbackMineResult", "FolioFeedbackMineResult"),
    @("FolioFeedbackSubmitPayload", "FolioFeedbackSubmitPayload"),
    @("FolioFeedbackMinePayload", "FolioFeedbackMinePayload"),
    @("FolioFeedbackSubmitResponse", "FolioFeedbackSubmitResponse"),
    @("FolioFeedbackMineResponse", "FolioFeedbackMineResponse"),
    @("FolioSearchDocument", "FolioSearchDocument"),
    @("FolioUpdaterTests", "FolioUpdaterTests"),
    
    # Method and property names
    @("AddFolioServices", "AddFolioServices"),
    @("IsFolioWindow", "IsFolioWindow"),
    @("IsFolioForeground", "IsFolioForeground"),
    @("IsPointerOverFolio", "IsPointerOverFolio"),
    @("IsFolioFileDrag", "IsFolioFileDrag"),
    @("ApplyFolioFileDragFeedback", "ApplyFolioFileDragFeedback"),
    @("hasFolioSourcePaths", "hasFolioSourcePaths"),
    @("HasFolioSourcePaths", "HasFolioSourcePaths"),
    @("SetFolioContentSearchEnabled", "SetFolioContentSearchEnabled"),
    @("SearchFolioContentAsync", "SearchFolioContentAsync"),
    @("SearchFolioContentSnapshot", "SearchFolioContentSnapshot"),
    @("EnsureFolioContentSnapshotAsync", "EnsureFolioContentSnapshotAsync"),
    @("RefreshFolioContentSnapshotAsync", "RefreshFolioContentSnapshotAsync"),
    @("BuildFolioContentSnapshotAsync", "BuildFolioContentSnapshotAsync"),
    @("_deskBoxContentRefreshLock", "_folioContentRefreshLock"),
    @("_deskBoxContentLifetimeCts", "_folioContentLifetimeCts"),
    @("_deskBoxContentSnapshot", "_folioContentSnapshot"),
    @("_deskBoxContentRefreshTask", "_folioContentRefreshTask"),
    @("_deskBoxContentLastRefreshMs", "_folioContentLastRefreshMs"),
    @("_deskBoxContentInitialized", "_folioContentInitialized"),
    @("FolioContentRefreshIntervalMilliseconds", "FolioContentRefreshIntervalMilliseconds"),
    @("FilterFolioItem", "FilterFolioItem"),
    @("SearchFolioContentToggle", "SearchFolioContentToggle"),
    @("SupportFolioDialog", "SupportFolioDialog"),
    @("SearchIncludeFolioContent", "SearchIncludeFolioContent"),
    @("IncludeFolioContent", "IncludeFolioContent"),

    # Environment variables
    @('"FOLIO_DEV_DATA_ROOT"', '"FOLIO_DEV_DATA_ROOT"'),
    @('"FOLIO_AOT_PREVIEW_DATA_ROOT"', '"FOLIO_AOT_PREVIEW_DATA_ROOT"'),
    @('"FOLIO_VERBOSE_LOG"', '"FOLIO_VERBOSE_LOG"'),
    @('"FOLIO_DEV_SHUTDOWN_PROBE"', '"FOLIO_DEV_SHUTDOWN_PROBE"'),

    # Compiler defines
    @('FOLIO_NATIVE_AOT', 'FOLIO_NATIVE_AOT'),
    @('FOLIO_AOT_SMOKE_HARNESS', 'FOLIO_AOT_SMOKE_HARNESS'),
    @('FOLIO_UPDATER', 'FOLIO_UPDATER'),
    @('FOLIO_STORE', 'FOLIO_STORE'),

    # IPC & Kernel objects
    @('Folio_SingleInstance_Mutex_', 'Folio_SingleInstance_Mutex_'),
    @('Folio_Activate_Event_', 'Folio_Activate_Event_'),
    @('Folio_Settings_Ipc_', 'Folio_Settings_Ipc_'),
    @('"folio-settings-host.log"', '"folio-settings-host.log"'),

    # AUMID & URI
    @('"Folio.Folio"', '"Folio.Folio"'),
    @('"Folio.NotificationActivationProbe"', '"Folio.NotificationActivationProbe"'),
    @('"Folio-NotificationActivationProbe"', '"Folio-NotificationActivationProbe"'),
    @('"folio://drag-drop/virtual"', '"folio://drag-drop/virtual"'),

    # Drag Formats
    @('"Folio.Internal.Text.v1"', '"Folio.Internal.Text.v1"'),
    @('"Folio.Internal.Source.v1"', '"Folio.Internal.Source.v1"'),
    @('"FolioSourceWidgetId"', '"FolioSourceWidgetId"'),
    @('"FolioSourcePaths"', '"FolioSourcePaths"'),
    @('"FolioInternalDragToken"', '"FolioInternalDragToken"'),
    @('"Folio.WidgetItemDrag.v2"', '"Folio.WidgetItemDrag.v2"'),
    @('"FolioDragSessionId"', '"FolioDragSessionId"'),
    @('"FolioStackReorderKey"', '"FolioStackReorderKey"'),
    @('"FolioSourceStackKey"', '"FolioSourceStackKey"'),

    # Registry
    @('@"Software\Folio\DirectInstall"', '@"Software\Folio\DirectInstall"'),
    @('@"Software\Folio\InstallLanguage"', '@"Software\Folio\InstallLanguage"'),
    @('"Software\\Folio\\DirectInstall"', '"Software\\Folio\\DirectInstall"'),
    @('"Software\\Folio\\InstallLanguage"', '"Software\\Folio\\InstallLanguage"'),
    @('string valueName = "Folio"', 'string valueName = "Folio"'),
    @('private const string AppName = "Folio"', 'private const string AppName = "Folio"'),

    # Task Scheduler
    @('internal const string LegacyTaskName = "Folio User Startup"', 'internal const string LegacyTaskName = "Folio User Startup"'),
    @('private const string StartupTaskId = "FolioStartupTask"', 'private const string StartupTaskId = "FolioStartupTask"'),
    @('"Folio User Startup"', '"Folio User Startup"'),

    # Palette
    @('"FolioNeutralFillSecondaryBrush"', '"FolioNeutralFillSecondaryBrush"'),
    @('"FolioNeutralFillTertiaryBrush"', '"FolioNeutralFillTertiaryBrush"'),
    @('"FolioNeutralLineBrush"', '"FolioNeutralLineBrush"'),
    @('"FolioNeutralTextPrimaryBrush"', '"FolioNeutralTextPrimaryBrush"'),

    # Paths & Files
    @('"Folio-Recovery"', '"Folio-Recovery"'),
    @('"Folio.log"', '"Folio.log"'),
    @('"Folio.Settings.log"', '"Folio.Settings.log"'),
    @('"Folio.Updater.log"', '"Folio.Updater.log"'),
    @('"Folio.exe"', '"Folio.exe"'),
    @('"Folio.lnk"', '"Folio.lnk"'),
    @('"Folio.Updater.exe"', '"Folio.Updater.exe"'),
    @('"Folio.Updater.*"', '"Folio.Updater.*"'),
    @('"Folio_Setup_', '"Folio_Setup_'),
    @('"Folio/backups"', '"Folio/backups"'),

    # Backup & Diagnostics
    @('"Folio.BackupSchemaVersion"', '"Folio.BackupSchemaVersion"'),
    @('"Folio.BackupManifestInvalid"', '"Folio.BackupManifestInvalid"'),
    @('"Folio-Auto-', '"Folio-Auto-'),
    @('"Folio-Backup-', '"Folio-Backup-'),
    @('"Folio-CloudBackup-', '"Folio-CloudBackup-'),
    @('"Folio-PreRestore-', '"Folio-PreRestore-'),
    @('"Folio-Diagnostics-', '"Folio-Diagnostics-'),
    @('"Folio-sanitized.log"', '"Folio-sanitized.log"'),

    # String keys
    @('"Settings.QuickCapture.ClipboardReason.FolioWrite"', '"Settings.QuickCapture.ClipboardReason.FolioWrite"'),
    @('"Weather.IconStyle.Folio"', '"Weather.IconStyle.Folio"'),
    @('"Search.Tab.Folio"', '"Search.Tab.Folio"'),
    @('"Search.Filter.Folio"', '"Search.Filter.Folio"'),
    @('"Settings.Search.Scope.Folio"', '"Settings.Search.Scope.Folio"'),
    @('"Settings.Search.Scope.Folio.Description"', '"Settings.Search.Scope.Folio.Description"'),
    @('"ignored:folio-write"', '"ignored:folio-write"'),
    @('"folio-content"', '"folio-content"'),
    @('"folio-foreground"', '"folio-foreground"'),
    @('"pointer-over-folio"', '"pointer-over-folio"'),
    @('"Folio shutdown hard stop"', '"Folio shutdown hard stop"'),
    
    # Doc comments
    @('<see cref="Folio.', '<see cref="Folio.'),
    @('<see cref="Folio"', '<see cref="Folio"')
)

$csCount = 0
foreach ($file in $csFiles) {
    $raw = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    $content = $raw
    
    # Namespaces & usings
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '\bnamespace\s+Folio\b', 'namespace Folio')
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '\busing\s+Folio\b', 'using Folio')
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '\busing\s+static\s+Folio\b', 'using static Folio')
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '\bglobal::Folio\b', 'global::Folio')
    $content = $content.Replace('[assembly: InternalsVisibleTo("Folio.Tests")]', '[assembly: InternalsVisibleTo("Folio.Tests")]')
    
    # Replacements map
    foreach ($pair in $typeReplacements) {
        $content = $content.Replace($pair[0], $pair[1])
    }

    if ($content -ne $raw) {
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        $csCount++
    }
}
Write-Host "Updated $csCount C# files."

Write-Host "=== Phase 3: Updating XAML files ==="
$xamlFiles = Get-ChildItem -Recurse -Include *.xaml -Path "$root\src"

$xamlReplacements = @(
    @('x:Class="Folio.', 'x:Class="Folio.'),
    @('using:Folio.', 'using:Folio.'),
    @('using:Folio"', 'using:Folio"'),
    
    @('FolioNeutralFillSecondaryBrush', 'FolioNeutralFillSecondaryBrush'),
    @('FolioNeutralFillTertiaryBrush', 'FolioNeutralFillTertiaryBrush'),
    @('FolioNeutralLineBrush', 'FolioNeutralLineBrush'),
    @('FolioNeutralTextPrimaryBrush', 'FolioNeutralTextPrimaryBrush'),
    @('FolioMenuFontFamily', 'FolioMenuFontFamily'),
    @('FolioMenuItemMinHeight', 'FolioMenuItemMinHeight'),
    @('FolioMenuItemFontSize', 'FolioMenuItemFontSize'),
    @('FolioMenuPresenterPadding', 'FolioMenuPresenterPadding'),
    @('FolioMenuItemPadding', 'FolioMenuItemPadding'),
    
    @('Title="Folio Content Widget"', 'Title="Folio Content Widget"'),
    @('Title="Folio Settings"', 'Title="Folio Settings"'),
    @('Title="Folio Onboarding"', 'Title="Folio Onboarding"'),
    @('Title="Folio"', 'Title="Folio"'),
    
    @('ms-appx:///Assets/folio.svg', 'ms-appx:///Assets/folio.svg'),
    @('ms-appx:///Assets/folio-mark-slab-0.svg', 'ms-appx:///Assets/folio-mark-slab-0.svg'),
    @('ms-appx:///Assets/folio-mark-slab-1.svg', 'ms-appx:///Assets/folio-mark-slab-1.svg'),
    @('ms-appx:///Assets/folio-mark-slab-2.svg', 'ms-appx:///Assets/folio-mark-slab-2.svg'),
    
    @('FilterFolioItem', 'FilterFolioItem'),
    @('Tag="Folio"', 'Tag="Folio"'),
    @('Tag="folio"', 'Tag="folio"'),
    @('SearchFolioContentToggle', 'SearchFolioContentToggle'),
    @('SupportFolioDialog', 'SupportFolioDialog'),
    
    @('Search.Tab.Folio', 'Search.Tab.Folio'),
    @('Settings.Search.Scope.Folio.Description', 'Settings.Search.Scope.Folio.Description'),
    @('Settings.Search.Scope.Folio', 'Settings.Search.Scope.Folio')
)

$xamlCount = 0
foreach ($file in $xamlFiles) {
    $raw = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    $content = $raw
    
    foreach ($pair in $xamlReplacements) {
        $content = $content.Replace($pair[0], $pair[1])
    }
    
    if ($content -ne $raw) {
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        $xamlCount++
    }
}
Write-Host "Updated $xamlCount XAML files."

Write-Host "=== Phase 4: Updating Localization JSON files ==="
$jsonFiles = Get-ChildItem -Include *.json -Path "$root\src\Folio\Strings"

$jsonKeyReplacements = @(
    @('Settings.QuickCapture.ClipboardReason.FolioWrite', 'Settings.QuickCapture.ClipboardReason.FolioWrite'),
    @('Weather.IconStyle.Folio', 'Weather.IconStyle.Folio'),
    @('Search.Tab.Folio', 'Search.Tab.Folio'),
    @('Search.Filter.Folio', 'Search.Filter.Folio'),
    @('Settings.Search.Scope.Folio.Description', 'Settings.Search.Scope.Folio.Description'),
    @('Settings.Search.Scope.Folio', 'Settings.Search.Scope.Folio')
)

$jsonCount = 0
foreach ($file in $jsonFiles) {
    $raw = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    $content = $raw
    
    # First replace exact keys
    foreach ($pair in $jsonKeyReplacements) {
        $content = $content.Replace("""$($pair[0])""", """$($pair[1])""")
    }
    
    # In JSON values: "Folio" -> "Folio", "folio.fun" -> "folio.fun", etc.
    $content = $content.Replace("Folio", "Folio")
    $content = $content.Replace("folio.fun", "folio.fun")
    $content = $content.Replace("folio-write", "folio-write")
    
    if ($content -ne $raw) {
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        $jsonCount++
    }
}
Write-Host "Updated $jsonCount localization JSON files."

Write-Host "Migration completed successfully!"
