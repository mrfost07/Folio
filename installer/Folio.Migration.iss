[Code]
const
  FolioAppCompatLayersKey = 'Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers';

function PrepareFolioDependencies(var NeedsRestart: Boolean): string; forward;

#if FolioBundledRuntime
function QuoteFolioCleanupArgument(Value: string): string;
begin
  Result := '"' + Value + '"';
end;

procedure CleanupFolioInstall;
var
  PowerShellPath: string;
  CleanupScriptPath: string;
  CurrentManifestPath: string;
  LegacyManifestPath: string;
  PreviousManifestPath: string;
  Parameters: string;
  ResultCode: Integer;
begin
  if not DirectInstallUpgrade then
  begin
    Log('Folio first-install path detected; stale install cleanup skipped.');
    Exit;
  end;

  ExtractTemporaryFile('cleanup-folio-install.ps1');
  ExtractTemporaryFile('Folio.LegacyBundledRuntimeFiles.txt');

  PowerShellPath := ExpandConstant('{sys}\WindowsPowerShell\v1.0\powershell.exe');
  CleanupScriptPath := ExpandConstant('{tmp}\cleanup-folio-install.ps1');
  CurrentManifestPath := ExpandConstant('{tmp}\Folio.InstallManifest.current.txt');
  LegacyManifestPath := ExpandConstant('{tmp}\Folio.LegacyBundledRuntimeFiles.txt');
  PreviousManifestPath := AddBackslash(WizardDirValue) + 'Folio.InstallManifest.txt';
  Parameters :=
    '-NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -File ' +
    QuoteFolioCleanupArgument(CleanupScriptPath) +
    ' -InstallRoot ' + QuoteFolioCleanupArgument(WizardDirValue) +
    ' -CurrentManifestPath ' + QuoteFolioCleanupArgument(CurrentManifestPath) +
    ' -LegacyManifestPath ' + QuoteFolioCleanupArgument(LegacyManifestPath) +
    ' -PreviousManifestPath ' + QuoteFolioCleanupArgument(PreviousManifestPath);

  if not Exec(PowerShellPath, Parameters, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    RaiseException('Folio could not start the install cleanup process.');

  if ResultCode <> 0 then
    RaiseException(Format('Folio install cleanup failed with exit code %d.', [ResultCode]));

  Log('Folio stale install-file cleanup completed successfully.');
end;
#endif

procedure DeleteAppCompatLayerValue(RootKey: Integer; ExePath: string);
var
  Value: string;
begin
  if ExePath = '' then
    Exit;

  if RegQueryStringValue(RootKey, FolioAppCompatLayersKey, ExePath, Value) and
     (Pos('RUNASADMIN', Uppercase(Value)) > 0) then
  begin
    if RegDeleteValue(RootKey, FolioAppCompatLayersKey, ExePath) then
      Log('Folio installer removed AppCompat RUNASADMIN value: ' + ExePath)
    else
      Log('Folio installer could not remove AppCompat value: ' + ExePath);
  end;
end;

procedure CleanupInstallAppCompatFlags(InstallPath: string);
var
  ExePath: string;
begin
  ExePath := AddBackslash(NormalizeDirPath(InstallPath)) + FolioLegacyExeName;
  DeleteAppCompatLayerValue(HKEY_CURRENT_USER, ExePath);

  if IsAdminInstallMode then
    DeleteAppCompatLayerValue(HKEY_LOCAL_MACHINE, ExePath);
end;

function InitializeSetup: Boolean;
begin
  // Program Files is now the supported all-users location. Older installations
  // found there are upgraded in place instead of being treated as disposable
  // legacy copies.
  Result := PrepareDirectInstallPlan;
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  Result := DirectInstallUpgrade and (PageID = wpSelectDir);
end;

function PrepareToInstall(var NeedsRestart: Boolean): string;
var
  DependencyError: string;
begin
  Result := '';

  // Close only the Folio process that belongs to the install being updated.
  // Restart Manager remains the final file-lock fallback for Setup itself.
  if not StopFolioProcessesAtPath(WizardDirValue) then
    Log('Folio path-scoped process shutdown failed; Setup will continue with Restart Manager handling file locks.');

  // Give the process time to fully exit before Restart Manager runs —
  // only meaningful when an existing install was just asked to close; a
  // first install has nothing to wait for.
  if DirectInstallUpgrade then
    Sleep(2000);
  Log('Folio process termination completed.');

  // Folio must continue to run at normal user integrity so Explorer drag and
  // drop remains available after an upgrade from older Program Files builds.
  CleanupInstallAppCompatFlags(WizardDirValue);

  DependencyError := PrepareFolioDependencies(NeedsRestart);
  if DependencyError <> '' then
    Result := DependencyError;
end;
