# Changelog

## 1.5.9 - 2026-10-23

#### New features

- **A recovery offer when your configuration gets wiped.** If Folio starts with missing or corrupted settings and a valid automatic backup survived, a one-time prompt shows the backup's date and widget count and offers to restore it or start fresh; closing the prompt also counts as starting fresh, and the question never returns. Until you choose, scheduled automatic backups stay paused (manual "back up now" keeps working, and the pending backup cannot be pushed out of retention) with a reminder at most once per app run, and the settings page now discloses that snapshots in a custom backup folder must be restored manually after a wipe.
- **Windows 11-native right-click menus for files.** A single-column menu whose rows each carry an inline icon, laid out in Explorer's classic order and rendered directly on acrylic (the tray menu shares the material); an "Open with" submenu lists real associated apps — including UWP ones — with their icons and a "Choose another app" fallback; the blank-area menu gains a "New" submenu (folder plus ShellNew-registered types) and separate "View"/"Sort by" submenus; in list view only the icon-and-name block of a row right-clicks into the file's menu — the rest of the row opens the blank-area menu (and clears the selection) exactly like Explorer, so "View → Icons" is always reachable; a right-button drag on blank space runs Explorer's marquee selection, opening the context menu for whatever it swept — and the marquee starts from a list row's blank remainder with either button, never stealing the item drag from the icon-and-name block; the New list itself now mirrors Explorer exactly: every registration layout Windows and installers use (including the Office one), one row per file type sorted by extension, OS-localized labels, ready-made "New…" file names, valid empty ZIPs, and each row carrying its file type's application icon — plus a "Shortcut" row that creates the placeholder and opens its properties to finish it. Native ZIP support covers compress (with tar variants and multi-select) and extract-all. Namespace shortcuts (This PC, Recycle Bin, Control Panel) inherit their target's verbs — the Recycle Bin shortcut can be emptied right from its menu (greyed out when already empty), every item gains "Create shortcut", and options Windows never offered there (Open file location, Compress) are hidden.
- **The Recycle Bin shortcut in a box accepts drops as deletes.** Drag files onto it — from the desktop, Explorer, or another box — and they go to the Recycle Bin with the same confirmation Windows itself would show; the shortcut's empty/full icon updates right after you empty the bin or drop something in.
- **Drag Shell locations into boxes to create shortcuts.** Dropping Recycle Bin, This PC, and similar Explorer locations onto a box now creates a working shortcut, with toast feedback when the source has nothing to offer or is busy.
- **A redesigned first-run experience.** The onboarding opens with a three-piece logo assembly animation and a file-drop scene, and shows the real managed storage path with move/paste hints.
- **Expanded widgets can shrink twice as small** — the minimum size drops from 50 to 25 DIP.
- **Selected file names expand like the desktop.** In double-click-to-open mode, the trimmed name of the one selected file grows downward right where it is — no tile moves, and the expanded label is what stays selected. One more click on that expanded name starts an inline rename, just like a slow click on the Windows desktop.
- **Korean language support.** The interface, installer, and first-run experience now ship in Korean alongside the existing 12 languages; the system locale is honored automatically.
- **A calmer, more capable updater.** Update checks run once a day on a wall-clock anchor that survives sleep, and back off for two hours after a failure. The update toast's buttons now act: "Update now" goes straight into the one-click flow and "What's new" opens the release notes, which also open automatically after an update (inline release copy first, the GitHub tag as fallback). Updates can download in the background — metered connections are skipped — with a ready toast when the package is local, a status badge that follows each stage, version navigation across the full published history in the update log (identical on every machine, offline-readable once viewed), and an activity card in the corner of the settings window while anything is in flight; background download and update notifications each get their own switch (on by default).
- **A clear notice on fully elevated desktops.** When the whole desktop runs with administrator privileges — UAC disabled, the built-in Administrator account, or a company-managed machine — a one-time startup notice explains that dragging files out of widgets is a Windows-level limitation in that state (dragging in works) and shows the exact way back: the secpol.msc policy on Pro, the FilterAdministratorToken registry value on Home, or signing in with a standard account, with a copyable command and a toast fallback. The drag-drop diagnostics in settings attribute the same situation, and an acknowledged notice never returns.
- **The tray icon can be hidden.** A "Hidden" style joins the tray icon options: no icon is created at startup, a runtime switch takes effect immediately, and the icon stays gone through taskbar recreation.

#### Fixes

- Drags no longer freeze the app in callback-heavy moments — tray commands, virtual-file drops, shortcut launch drags; the right-click drag chooser no longer loses its "Open with" entry, and drag-launched shortcuts only accept command lines that point to files on disk.
- A short widget docked to the screen edge no longer jumps to a corner on every reveal: the "wildly offscreen" test now measures the work-area overlap, so edge-docked bars and strips are not mistaken for lost windows.
- Widget positions survive display-topology changes: a stale layout snapshot can no longer overwrite the user's latest placement, and fallback "shelter" geometry is never promoted to authoritative intent.
- Switching group tabs no longer leaves the capsule slow to expand on re-entry: the swapped-in member's expansion layout is warmed at urgent priority while the capsule settles, and the hover-expand wait has a real hard cap again.
- Hover-revealed buttons are removed from the Tab order — an invisible focus ring can no longer trigger them with the spacebar.
- Shortcut "Open file location" now opens the target's location; executables no longer offer an "Open with" the system would reject; the "Choose another app" dialog actually opens, and the app-enumeration crash behind it is fixed.
- Right-click menu positioning lands on the cursor across edge, lower-screen, and centered scenarios without drifting or covering the icon.
- Revealing a widget shortly after hiding it no longer stutters: the memory trim that follows a hide now waits 30 seconds instead of 3 and skips small gains, so a quick hide-and-show round trip stops paying the quarter-second page-in.
- Idle memory reclaim behaves again: the first trim of each idle stretch lands seconds after you stop interacting, and repeat trims inside the same stretch back off to a five-minute safety cadence — ending the twenty-second trim-and-refault churn that could sit on a noticeable slice of a CPU core with widgets on screen.
- Right-clicking a todo item and hovering into the color submenu no longer terminates the app on Windows 11 25H2: the unselected color dot is now drawn as pure geometry instead of a glyph pinned to a font whose file 25H2 removed while leaving its registration behind — the first measure of that menu failed fatally.
- Menus opened from a stack popover no longer render beneath it: while such a flyout is open, its bridge window is lifted into the topmost band alongside the popover, so the menu stays clickable.
- Per-widget backgrounds and title icons no longer keep showing the previous image after being changed: assets now load through a stream backed by a source-keyed cache, sidestepping the framework's URI-keyed image cache that has no eviction.
- Running the x64 setup over a native ARM64 install is now refused with an explanation instead of silently replacing it with the emulated build (which would strand every future update on the x64 channel); deliberately switching to the native ARM64 build in the other direction keeps working.

## 1.5.8 - 2026-10-14

#### Fixes

- More accurate administrator notices. The "running as administrator" notice now appears only when Folio runs above the shell, so fully elevated desktops — for example company-managed computers — no longer see it. When it appears, it closes the elevated instance in one click and provides the steps to restart with normal privileges, including how to clear a shortcut's "Run as administrator" checkbox. Autostart continues to restart itself with normal privileges silently. Available in all 12 languages.
- One-click updates no longer bring Folio back as administrator: the updater restarts the app at the shell's own level whenever the shell offers a standard-user context, so the app no longer restarts as administrator after an update on machines where it ran elevated.
- When Explorer itself runs elevated above Folio, the app stays usable: the reveal hotkey and settings keep working, an explanatory notice replaces the untranslated failure box, and the rare fatal-startup dialog is localized and lists the diagnosed environment causes (elevated Explorer, compatibility layers, shortcut run-as).
- Maintenance-wave drag and startup fixes: dragging Todo items no longer crashes; switching groups no longer loses stacked-member icons; cutting a subfolder and pasting it into the parent folder no longer hangs; auto-organization rules stay enabled across restarts instead of being silently switched off.
- When the desktop repeatedly crashes, Folio no longer restarts in a loop at sign-in: a startup ledger breaks the cycle, and diagnostics now include recent Folio and Explorer crash events for support.
- Settings: an update-download failure no longer raises an error when another dialog happens to be open at the same time.
- The drag-and-drop diagnostic detects a shortcut-level "Run as administrator" flag — an elevation source invisible to the compatibility-layer scan — and explains exactly how to clear it.
- An instance relaunched with normal privileges can be ended again by an unelevated Task Manager.

## 1.5.7 - 2026-10-13

#### Fixes

- Exiting from the tray now reliably ends the process. On machines where the system's application-exit handshake stalls — typically Windows with UAC fully disabled and an elevated Explorer — a background Folio process could stay resident after "exit": the tray vanished and a restart worked, but the leftover process slowly ate memory and kept the right-click menu proxy alive beside it. Every shutdown path now carries a delayed hard-exit backstop, and the elevation notice and search popup close along with shutdown.
- Windows 10: the "drag out" card under File widgets → drag-and-drop behavior shows its title and question-mark tip again, and the Windows 10 explanation renders in the description area under the title instead of stretching the card out of shape or spilling outside it on scaled, narrower windows.
- Long auxiliary texts in settings cards (the cloud-backup remote-path hint, runtime-health detail, and backup snapshot summaries) are width-capped so they can no longer push a card's title aside.
- Dialogs and preview cards — the desktop-organization abandon dialog, its preview cards, and the hotkey recorder — follow the app's theme override again instead of the raw system theme, and desktop-organization rule rows rebuild when the applied theme flips.
- Desktop-organization rule rows are clickable again; a leftover chevron (double arrows) and a click guard that swallowed every row click shipped together in 1.5.6.
- Store packaging: the display-name resource consistently resolves to the reserved store name ("Folio Widgets"), completing the 1.5.6 packaging follow-up.

## 1.5.6 - 2026-10-12

#### New features

- Widgets belong to their display, and every display combination remembers its own layout. Drag a widget to a screen and that screen becomes its home; unplug the display and, after a short grace period, its widgets reflow onto a remaining screen (or collapse into capsules, if you set it so); plug back in and every widget returns to its exact spot. Combinations seen for the first time seed sensibly instead of piling onto the primary.
- Layouts reflow elastically across scales. Topology profiles record sizes and anchors per display instead of raw pixels, so 1080p ↔ 4K switches, remote sessions, and DPI round-trips reflow the arrangement instead of stretching widgets or leaving whitespace gaps; existing profiles migrate once, in place.
- Backup restores adapt to the machine they land on. A full restore onto a different PC or account re-registers startup per the backup's state (deliberate Windows-level disables are never overridden) and rebases account-local paths — storage root, snapshot folder, default-storage widgets — into the current profile; the completion notice reports what was adapted.
- Windows-side display changes no longer shuffle widgets: primary-display switches, resolution and scale changes, monitor rearrangement, lock-screen re-enumeration, and full-screen games that change the resolution all leave placements alone, and late-arriving dock or DisplayLink screens no longer trigger the crowd-then-scatter dance.
- Elevated launches explain themselves. Running as administrator blocks Explorer drag and drop (a Windows security boundary), so an elevated launch now offers a one-click restart with normal privileges; autostart instances restart themselves and the compatibility flag that kept relaunching elevated is cleared.
- Pin widgets to a display. A new Displays page shows an arrangement preview with per-screen widget lists, flashes a number on each screen to identify it, pins everything to the current layout in one click, and sets the default display for new widgets; per-widget and per-group bindings from the context menu follow the display's hardware identity and survive renumbering.
- Record your own hotkeys. Press-to-record editors capture any Win / Ctrl / Alt / Shift combination or a lone F1–F24 key — including chords Windows reserves for itself, like Win+V or Alt+Space; conflicts with the other hotkey are rejected with a clear message and risky combinations warn before saving.
- Dress up your widgets: per-widget emoji or image icons, per-widget background images with fit and dim controls, border styles, three global background modes (follow material, unified image, or a panorama where each widget frames its slice), and a text-shadow toggle in Appearance.
- Safer storage migration. Moving files into managed storage is copy-verified with a preview, per-phase progress, and resumable retry; every file is hash-checked on both sides before the path switches, and originals are never deleted.
- Start silently: a new option launches Folio hidden alongside Windows, brought back by the first tray click, hotkey, or activation.
- Weather widgets gain an icon-style picker (Fluent, Folio line-art, Meteocons Flat and Line) rendered identically on Windows 10 and 11, independent of the system emoji font.
- More reveal animations: three new show/hide effects (Edge scale, Tilt, Wipe), a Spring easing, and staggered timing that slides a group's widgets in one after another.
- Desktop files wait a configurable dwell — real-time to 12 hours — before auto-organization ingests them; managed-path choices that would misbehave (network, removable, system drive, desktop-overlapping) warn or are rejected up front.

#### Platform

- Updated the Windows App Runtime to 2.5.1 and the .NET runtime to 10.0.12, and lowered the minimum supported Windows 10 build to 19041 (version 2004).

#### Fixes

- Changing the primary display keeps every placed widget exactly where it is; only widgets set to always follow the main display move.
- Round-tripping a resolution or scale change reproduces the exact previous layout; moves made in between are kept.
- Brief display dropouts (a DP monitor powering down, a KVM switch, input flipping back within seconds) no longer trigger any reflow.
- "Move all widgets to a display" is undoable: the info bar restores every widget's previous home, mode, and position, even if a display is unplugged before you undo.
- Restoring a settings file on a different PC re-homes widgets by fallback heuristics without errors or lost widgets.
- Store edition: backup restores no longer abort at the relaunch step — the restart goes through the packaged-app restart agent — and the update notice now also speaks Japanese, German, and Portuguese.
- Traditional Chinese: the auto-organize success toast no longer ends with a stray character.
- The settings search box keeps its rounded shape and icon inside the box when the window is widened.
- Dragging files out of a widget on Windows 10 no longer pops Explorer's Copy / Move / Cancel picker, and the drag-out settings dropdown shows its options again on Native AOT builds.
- The drag-out result hint knows whether modifiers could have flipped the outcome instead of always claiming "no modifiers".
- Tray double-clicks no longer replay the toggle queue — a second click inside the double-click window completes quietly (feedback 237) — and an early-logon tray flyout retries its assignment on the tray cadence instead of failing startup (feedback 343).
- The "new folder mapping" picker no longer comes up invisible after switching virtual desktops (issue 489); the "add file" picker no longer follows shortcuts — adding a desktop shortcut adds the .lnk itself (issue 458).
- Memory and CPU housekeeping: the organization suppression ledger is capped and loads lazily; quarantined backups rotate; todo and quick-capture stores stream saves; the todo reminder scan stops re-reading unchanged stores; glance images decode with less headroom and rotation sets stream from disk so the framework cache cannot pin every frame; collapsed-widget ambient animations moved to compositor key-frames; a never-instantiated window was removed.
- Hidden or docked widgets release the icons outside the rendering window, so they no longer pin bitmaps the cache already evicted (long-hide memory growth).
- A compact capsule no longer stays expanded after the widget color picker is dismissed another way (issue 437); it also no longer collapses while a native context menu is open, and no longer snaps to the panel's bottom edge when collapsed after resizing the widget in expanded state.
- Feedback submissions only count toward the cooldown on real success, and a stale server-side rate limit heals by rotating the anonymous client id (issue 449).
- Search explains an all-empty file list when Everything is not connected instead of showing a blank state (feedback 291).
- Quick Capture picks up clipboard images from WeChat, QQ, and browsers via raw PNG / CF_DIB fallback (feedback 335), and widgets following the global text size refresh the moment it changes.
- Renaming a managed widget folder retries transient locks (antivirus, indexer) and reports clearly when a long-lived lock persists (feedback 240).
- Cross-volume imports end with the truth: when the copy completed but a source file is still held open, the import completes with a warning instead of reading as total failure and inviting duplicate retries (feedback 236/316).
- A managed-storage root on the desktop (or a subfolder) is rejected up front — the organizer would re-ingest its own folders.
- Native AOT builds: the cloud-backup password box, weather error overlay, resize/move cursors, and the "transparency effects off" advisory all work again — each was a retail-only trim/manifest gap (Debug was unaffected); the weather binding surface is now guarded by a contract test.
- The music widget no longer throws on stale session reads when a player exits mid-refresh — the recurring timeline exception noise in diagnostics is gone.
- Diagnostics exports keep HRESULT hex codes readable instead of redacting them.
- Mouse clicks on file tiles no longer paint a thick keyboard-style focus border.
- Installers close a lingering thumbnail-proxy process during upgrades so an orphaned proxy cannot lock files.
- A pending full restore can no longer apply without your answer: if the app exits while the confirmation dialog is open, the next launch discards it.
- Deleting search results and network-drive widget folders always honors the Recycle Bin promise — volumes without one get the interactive Windows confirmation instead of a silent permanent delete.
- Removing a todo / Quick Capture attachment can only delete copies that provably live in the managed attachment store, and they go to the Recycle Bin.
- Closing a widget whose "move files back to the desktop" fails midway keeps the widget and explains what happened.
- Files dragged out of a widget and dropped back shortly afterwards are no longer silently ignored when a previous drag's completion event was lost.
- The restore launch skips its scheduled snapshot (the pre-restore archive already covers that state), and the completion notice only claims what actually happened.
- The storage-migration dialog closes on Esc and places its primary action consistently; the feedback-submitted notice renders its border correctly in both themes.

#### Interface

- The settings window got an information-architecture pass: low-frequency settings fold into accordions, pages put fundamentals first, sections slide in and out like Windows' own Settings, entry cards are clickable across the whole row, and notices render as native info bars.
- The file-stacking page is reorganized into two accordions — reveal mode first (with popover-only layout and visual-style options that freeze for inline reveal), then automatic stacking with its grouping rules; the master switch is gone and manual stacks are always available. The file-widgets overview shows the reveal mode inline, and the desktop storage-entry card moved to the bottom of the page.
- Destructive actions ask before they act: dissolving a group confirms through a small flyout, dangerous buttons wear a consistent red style, restore confirmations default to the safe button, and folder-delete confirmations show the parent path.
- Notifications deep-link to what they concern: update toasts open the About page, maintenance and folder-mapping notices route to their settings section, todo reminders carry reminder priority.
- The first-run walkthrough was redrawn with system-managed transitions that respect the "disable animations" setting; jump-list entries group under the system-localized Tasks section.
- Hiding widgets slides them fully out with a per-window fade at the screen edge; dragging and resizing read pointer positions from the input message, staying accurate across mixed-DPI setups.
- Dragging files out of a widget shows a modifier-key hint (Shift moves, Ctrl copies) and a post-drop receipt of what actually ran; both are dismissible on the spot or from Settings.
- Music widgets can hide the progress bar and times when the player provides no progress info, instead of showing a blank placeholder.
- File lists respond across the whole row (list) or tile (icon view); empty widgets explain themselves with causes and fixes.
- The remaining file pickers (attachments, widget images, backup restore, Everything browse) moved to the modern picker; all legacy dialogs are retired.
- "Move to display" maps positions proportionally; menus and settings use Windows' friendly device names instead of display indexes. The Displays page refreshes live on plug/unplug, and the identify overlay closes on topology changes.
- New widgets appear on the mouse's display by default or on a pinned one, falling back gracefully while it is offline.


## 1.5.5 - 2026-09-22

#### New features

- Cloud backup to your own server. Settings → Maintenance gains a cloud-backup page supporting WebDAV: pick the data domains to back up (todo, quick capture, widget style), set the interval and retention count, and store the password in Windows Credential Manager. "Test connection" validates the credentials you just typed, backups can run on demand, and remote snapshots are listed for browsing and restoring.
- Scoped restore. Pick which data domains come back and choose merge — newer entries win, nothing on this device is deleted — or a full snapshot-faithful restore. The confirmation dialog shows per-domain item counts; restores stage and validate first, then apply on the next launch with a pre-restore safety archive pinned.
- Hotkeys now self-heal. Windows can silently strip low-level hooks under memory pressure or timeouts, leaving the global hotkey, search hotkey, or desktop double-click activation dead until restart. A health watchdog now detects this and re-registers the hook automatically.
- New "Trim memory when idle" performance option returns memory after a few seconds of true quiet, not only when everything is hidden — guarded by working-set floors and ambient-animation awareness.
- New window-shadow toggle in Appearance settings, applied system-wide with write-back verification and searchable in settings.
- Storage-root migration is now interactive: a progress dialog you can cancel, with per-file retry / skip / apply-to-all when something fails, and skipped files can be migrated later.
- Compact widgets can override the expansion direction per widget (auto / down / up) from the collapse menu, and capsule bars can be reordered by dragging — including across monitors.

#### Drag and drop

- Drags out of widgets now carry the same native Shell data object Explorer uses. Copy-only drop targets that used to reject Folio drags — Chromium/Electron apps such as VS Code and browsers, WM_DROPFILES-era programs, and WinForms — now accept them. Same-volume drops default to move, cross-volume to copy, and Ctrl/Shift/Alt modifiers behave as expected on Windows 11; Windows 10 keeps the single-move behavior without prompting.
- Cross-volume transfers now go through the system file-operation engine with native progress, cancellation, and conflict dialogs, fixing the cross-volume import that completed with "0 items" on Native AOT builds. Same-volume moves keep the existing fast path.
- Aborted transfers clean up only the objects the operation itself created, so files that appeared in the destination mid-copy or pre-existing folders are never swept away; transfer errors now distinguish "file in use" from "access denied".

#### Interface

- Snap alignment feedback while resizing or dragging widgets is now a crisp accent edge band that settles once — the old looping breathing glow is gone, and the band dissolves cleanly around rounded corners.
- New widget groups now default to the tab navigation style.
- A one-time hint now explains when Mica/Acrylic materials fall back to a solid color because Windows transparency effects or battery saver turned them off — including the moment they are switched off mid-session.
- The settings window no longer freezes on the theme it had when it was closed: after being closed (hidden) and reopened it follows the system light/dark switch again.
- The in-app feedback dialog no longer gets stuck on "Submitting…" after a network timeout, keeps error text visible after a success, and disables the submit button properly while rate-limited; a "send another" button follows a successful submission.
- Widget folder-conflict messages now say exactly how two folders conflict (identical, nested inside, or containing) and name the other widget with its full path.

#### Performance and memory

- Idle z-order normalization now reorders widgets as a peer group instead of flattening them to the desktop bottom, reducing visible reordering flicker.
- Memory cleanup is better coordinated: immediate-hidden and idle trims no longer race each other.
- Entering a widget with a foreign drag skips a cross-process format query that could never succeed, so drag-over feels snappier.

#### Fixes

- Native AOT builds: the cloud-backup page no longer crashes with "Specified cast is not valid" when reading the password box (real WebDAV user report).
- Shortcut icons: shortcuts created by alt-dragging keep the target's own icon instead of Folio's, and arrow-overlaid shortcut icons render through a high-resolution compositing channel instead of pre-scaled 32/48 px bitmaps — blurry shortcut arrows at large sizes are fixed.
- Creating or renaming a widget is no longer blocked by a folder left behind by a closed same-named widget: the empty widget adopts the leftover folder and its contents.
- Migration rollback failures are no longer silent: a dedicated dialog lists the stranded folders with their widget names and offers retry, and partial cleanups keep their remaining list instead of claiming success.
- The migration dialog no longer loops forever when "apply to all" meets a locked file, and no longer waits forever for a decision after the dialog has been closed.
- Files without an association open through Folio's own picker, which reports honestly — the shell picker reported success even when it was cancelled — and a "recommended" handler is no longer mistaken for the default association.
- Cloud backups no longer include todo and quick-capture attachments (a single large attachment used to inflate the whole upload), and restores never delete local attachments.
- Merge restores no longer resurrect entries you deleted on this device: quick capture and todo deletions now leave tombstones that keep cloud snapshots from bringing them back.
- A backup the server accepted but never listed is no longer mistaken for a plain success or a failure — it is stamped "awaiting server confirmation" in the backup status and notified once, so a laggy WebDAV listing cannot hide a silently-dropped snapshot.
- Restored todo widgets adopt their orphaned on-disk store instead of starting empty, and manual widget ordering survives restore and rearrange flows more reliably.
- Restore packages are now validated against the bytes actually extracted — per-file and total caps enforced on the real stream — instead of trusting sizes declared inside the archive.

#### Reliability and upgrades

- The device-local widget layout moved into its own widget-layout.json file: it is adopted from settings.json on the first launch after updating, corrupt files are quarantined with .bak recovery instead of being treated as empty, and it stays out of cloud backups so restoring styles on another device never overwrites this device's layout.
- Layout files that parse but are not layout documents (an empty object or a null layout) are treated as corrupt instead of wiping the desktop.
- Layout and settings are now committed as a pair with rollback — a crash mid-save can no longer leave the two files describing different states. Widget-style restore became a journaled two-phase commit with crash recovery, and layout files written by a newer schema version fail honestly instead of pretending to save.
- Backup snapshots and settings saves share a write gate, so a backup can no longer capture a torn half-saved pair; pre-restore safety archives are pinned to their pending restore so retries don't stack or evict them, and pending-restore markers retry only a bounded number of times.
- Folder adoption resolves junctions and symbolic links before checking ownership, so aliased directories cannot bypass the takeover guards.

## 1.5.4 - 2026-09-18

#### Fixes

- Fix memory climbing and staying high after large batch operations in file widgets (importing, organizing, or cutting thousands of files could leave private memory near 1 GB with no recovery until restart). Settings persistence now streams to disk instead of materializing multi-megabyte buffers, and move-out batches no longer trigger thousands of redundant saves.
- Stop the visible freeze when cutting a large selection out of a widget: the cut-state sweep now runs once per batch instead of once per collection event.
- Bound the undo history retained inside settings.json. Batches beyond the receipt limits keep their history entry as a summary (real item count, no partial undo), so settings.json can no longer grow unboundedly after repeated large operations; existing bloated profiles shrink automatically on the first launch after updating.
- Speed up large widget operations by replacing per-file list scans with a scoped path index and binary insertion during imports and watcher reloads.

## 1.5.3 - 2026-09-16

#### New features

- Add standard startup and an optional scheduled-task method to direct installations. New users default to standard startup; existing installations keep their active method. Failed task registration can fall back to verified standard startup, failed switches preserve the previous entry, and Windows disable choices are respected.

#### Fixes

- Read scheduled-task definitions and rollback snapshots directly as Unicode so non-ASCII account names and installation paths pass registration verification.
- Register notifications before decoding activation arguments, preventing the uninitialized notification-deserializer path during notification launches. Queue early notification input until the main instance is ready, and keep ordinary startup available when notifications are unsupported or cannot register.
- Retry tray creation while the desktop shell starts and terminate fatal initialization failures instead of leaving a headless process holding the instance lock. Extend the watchdog to constructor initialization.
- Retry failed first-time autostart registration on a later launch without consuming the one-time default marker.
- Preserve boolean verification results in diagnostic exports while continuing to hide actual paths and account identifiers.

## 1.5.2 - 2026-09-15

#### Fixes

- File widgets no longer cap their contents at the first 30 items. Folders up to the render threshold show every item again, and larger folders extend the rendered window whenever the visible area is not yet filled (1.5.1 could leave tall widgets stuck at 30 items with no way to reach the rest — reported through the in-app feedback channel).

## 1.5.1 - 2026-09-15

#### New features

- Dropping files onto an application shortcut tile now opens them with that application, both for drops from the desktop and drags between widgets; non-executable or unverifiable targets keep the default system behavior.
- Hovering a file you are dragging over a widget-group tab for a short dwell now switches to that member, so multi-widget groups can be reached without dropping the file first.
- Added a sixth global hotkey preset for the hardware Copilot key (delivered as Win+Shift+F23 by most keyboards), including the show-desktop special case.
- In click-collapse mode, clicking a widget's title bar now collapses it directly; the identity area keeps its double-click meaning through a short delayed window.
- The search popup gains Images and Documents result tabs; switching tabs scrolls back to the top, and the category filter resets for each new query.
- Added an in-app feedback entry point in Settings that opens the feedback dialog with diagnostics context (delivery depends on the feedback service backend).
- Direct-install autostart now registers a least-privilege logon task as the single startup entry; the legacy Run key is treated only as a migration source, and a task disabled by the user is never bypassed.

#### Fixes

- Changing the default storage location no longer fails when a widget folder contains read-only files (a common trait of files received over chat apps, cloud drives, or attachment bundles): source cleanup now clears the read-only attribute before deleting and restores it if the delete is blocked.
- Source cleanup only deletes files that were verifiably copied and unchanged; a file that appears or changes inside the folder during the move is kept in both places instead of being deleted.
- When the old folder cannot be cleaned up after a complete copy, the migration finishes and offers to move the leftover folders to the recycle bin instead of failing and rolling back; retrying into a folder that still holds a previous attempt's copy offers to clear it first instead of creating "name (2)" duplicates. Dropping folders into widgets, organizing desktop files, and moving a deleted widget's contents back to the desktop share the same fixes.
- The recommended-apps grid in the search popup now responds to the mouse: single-click selects, double-click opens, and arrow keys plus Enter launch the highlighted card (mouse handling had silently never worked since the grid was introduced), and icons now resolve on the very first open instead of appearing only after reopening the popup.
- Opening a file with no shell association now raises the system Open With picker instead of appearing dead, success toasts for opens were removed, and the selection ghost after clicking an item is suppressed.
- Opening a shortcut whose stored target is gone now reports the lost target instead of claiming the request was handed to Windows.
- Dropping hidden or otherwise undisplayable shortcuts no longer swallows them silently — the drop is rejected as a whole with feedback, so files never land in the folder without a tile.
- Renaming a widget no longer leaves an empty tile when the on-disk rename lands inside the one-second save debounce window.
- Capsule transitions are fully repaired: collapsing a widget no longer leaves a residual 4-DIP upward offset on the title bar of the next expansion, and the first expansion after a fresh launch no longer leaves capsule icon and title ghosts floating over the expanded body (both were Composition properties left at their last animation frame or aborted mid-completion).
- Clicking an item in the native Windows context menu no longer stalls the whole system for one to two seconds: the low-level mouse hook moved off the UI thread onto a dedicated pump thread and unloads as soon as the menu closes.
- Associated icons whose artwork sits inside a larger transparent canvas no longer shrink inside file tiles; the significant-alpha boundary check re-issues the icon at its native frame size.
- Organize Desktop recovery is hardened: abandoning an unrecoverable restore has an explicit confirmation and diagnostics path, orphaned recovery journals are cleared, same-volume frees are exempt from the space check, failure messages are specific, and widget-mapping conflicts auto-resolve or reveal the conflicting tiles instead of failing silently.
- The recycle-bin third-level confirmation can no longer fail to appear in Smart mode because of a missing lease, and deleting a widget whose managed folder is empty now removes the empty folder outright.
- Search, notes, and other popup windows opened during a quick-reveal (tray or hotkey) session no longer sink below the raised widget layer while it stays on top.
- Switching the active member of a stack no longer flashes the previous member's tiles; the popover clears and refills its containers on switch.
- Search-row selection indicators align with the text center, and row heights are cached with a highlight retry after lazy metadata loads.
- Widget-local failure dialogs (delete and rename failures, the color picker) became in-widget feedback bars and flyouts instead of ContentDialogs that small widget windows clipped.
- Orphaned todo widget data is cleaned up, and cache directories are excluded from automatic backups.

#### Performance and memory

- File widgets now render a windowed slice of very large folders and hydrate more rows as you scroll, so opening a folder with thousands of files no longer freezes the interface.
- First launch after a fresh install is faster: shell icon extraction runs as one batched request through the thumbnail proxy instead of per-item broker round trips.
- New installations now default to the Resource saver performance preset: visible-idle maintenance after 5 minutes, hidden cache release after 30 seconds with full recreatable-cache scope, and the small cache budget. The experimental immediate hidden working-set trim waits 3 seconds after hiding, and quick hide/show round trips trim at most once. Existing settings are never rewritten on upgrade.
- Marquee selection applies incrementally instead of clearing and refilling the list, display-topology changes (plug, unplug, resolution) are event-driven instead of a two-second poll, and the retired smart animation adapter stack plus throttled text-shadow reconciliation remove idle animation overhead.
- Capsule expansion animations only animate the properties the transition actually changes, and window bounds commits during animations are batched, removing start/stop cost from opening frames.

#### Interface

- Interaction chrome — the group drop-preview outline, capsule position-rail dots, and title wheel feedback — now draws the neutral interaction tone instead of the accent color and re-resolves correctly on light/dark flips.
- Corner radii across tiles, search rows, note cards, and todo rows converge to a tighter scale, and the immediate-trim setting documents its 3-second grace window in all 12 languages.

## 1.5.0 - 2026-09-06

#### New features

- Added automatic data snapshots with a configurable schedule: enable or disable backups (on by default), choose the interval from every 5 minutes to every 5 days (daily by default), keep between 3 and 30 snapshots (7 by default), and optionally store them in a custom directory. Invalid values hand-edited into the configuration file are normalized to the nearest preset, and a custom directory that becomes unavailable falls back to the default recovery folder with a single notification per degradation cycle.
- Redesigned Organize Desktop around a desktop preview card: tile selection is expressed through clear selected/unselected opacity, re-scanning keeps your previous selections (newly appearing files start unchecked), and the entry action is now "Preview organize". The source picker can optionally include the shared Public Desktop; moving its files may request administrator approval, a canceled approval keeps the completed personal-desktop work, and interrupted operations can be resumed or recovered at the next startup.
- Folder shortcuts (`.lnk`) that point inside the widget's own managed tree now open the folder inside the file widget instead of File Explorer; external, broken, or unverifiable shortcuts keep the default system behavior.
- Quick Capture and Todo each gained independent list and content text-size sliders in Settings; when left unset they keep following the global appearance text size.
- Added a Glance clock time-format setting: follow system (default, without AM/PM markers), 24-hour, or 12-hour.
- Added an optional setting to show the native Windows context menu when right-clicking a single file tile (off by default); stacks and multi-selection keep the built-in menu.
- File, folder, and `desktop.ini` custom icons are now fetched as 256-px high-resolution Shell icons with shortcut overlays preserved, and internet shortcuts (`.url`) resolve their icons through the Shell, so covers and per-user associations (for example Steam) display correctly.
- Added an experimental performance setting that trims the working set immediately after all widgets finish their hide animation (pairs with "Compress memory when idle").

#### Fixes

- Stabilized internal drag completion: a pointer release no longer rebuilds the file list before the Shell drop completes, and entering a drop revokes any provisional Move acceptance, so internal reorders can no longer trigger Shell shortcut cleanup.
- Inline rename fields now reliably receive keyboard focus across every entry point (group title, file item, stack popover, Quick Capture item), and a popup stealing focus within 300 ms is reclaimed instead of treated as a commit.
- Steam shortcuts are no longer treated as uninstalled when the Steam library sits on an unavailable removable or network drive; the probe reports "unknown" and keeps the item visible.
- Dropping files into the directory they already occupy — including junction- and symlink-resolved paths, the grid root, and stack popovers — is rejected up front with clear feedback instead of performing a duplicate transfer.
- Recursive folder copies now detect junction and symbolic-link cycles, and moving a folder into itself (including through a link) is refused.
- Shortcut health checks no longer mark network-share roots or environment-variable targets as broken; only a locally missing target is reported.
- Opening a file no longer leaves press or hover highlight on the item, and returning to the parent folder no longer reselects the folder you just exited.
- File icons refresh correctly after a file is moved out and back into a managed path, because watcher events now clear every cached icon variant, including failed lookups.
- File-widget mapping validation rejects paths that overlap the managed storage root or another widget's mapped directory.
- Settings and data files save more robustly: when atomic replacement fails, a verified in-place write is used, and a corrupt primary file can be recovered from the automatic backup.
- Group-tab drag reordering rolls back to the previous order when saving fails, and no longer writes when nothing changed.
- The in-app updater's official-download action opens the website instead of a mirror direct link, and update notes render as plain text.
- Fixed visual residue that could remain after a widget finished its compact-transition animation.
- Dragging files into a grid across volumes now posts Shell change notifications (per-item rename plus a forced directory refresh), so Explorer views — including OneDrive-backed desktops — no longer keep stale icons of moved files until a manual refresh.
- Quick Capture and Todo compact capsules show a static, ellipsized preview instead of a scrolling marquee, which previously displayed only a truncated fragment of long content.

#### Performance and memory

- Settings sections are now created on demand when first navigated to, so the Settings window opens faster and keeps a smaller resident tree; the settings search index is generated from localization keys, so search finds settings in sections that have not been created yet.
- Thumbnail and icon proxy payloads are read directly into their final buffer without growth-buffer copies.
- File items instantiate only the active layout (icon or list) instead of both, and widget title icons load their geometry lazily.
- High-resolution Shell icon requests are throttled through a bounded queue, keeping the icon proxy responsive while many icons load at once.

#### Interface

- Widget title bars are more compact (40-px minimum row with tightened padding).
- Group tabs use a native TabView appearance with a translucent selected background and content-sized labels.
- The Settings update section was rebuilt as a detail flow with clearer progress and action layout.

## 1.4.9 - 2026-09-01

- Fixed unbounded memory growth from repeatedly opening and closing the Settings window: each cycle used to leak a full native XAML tree (~10 MB) due to a known WinUI 3 window-close defect. The settings window is now reused — closing hides it and the same instance is reshown, with state refreshed on reopen.
- The idle deep memory collection no longer gives up when vetoed by its 120-second cooldown: it retries with backoff until the finalizer collection actually runs.
- Switching widget materials no longer destroys and recreates system backdrop controllers (their kind is mutable), eliminating per-switch native handle accumulation; DWM theme attributes are only re-issued when the theme actually changes.
- Restored the pre-1.4.5 idle memory compression behind a new setting, "Compress memory when idle" (on by default, in Performance settings): after the hidden-idle deep cleanup completes, the working set is paged out (e.g. 350 MB down to under 10 MB); while widgets are visible it only runs when the user is fully away and the footprint exceeds the original bloat thresholds.
- Hardened file-open interactions: opening is gated and traced, runs on a bounded STA runner, and file items get opening animations and text scaling.
- Renaming a file to a different extension now asks for confirmation using the Windows extension-change warning; the stack popover supports in-place rename.
- Stabilized the complete file drag protocol across Windows 10 and Windows 11: normal left-button drag-out no longer opens an operation chooser, internal sorting and stack membership never authorize Shell source deletion, and files can again move between the surface and stacks in both inline and popover modes. Session-scoped payload caching and guarded pointer-release recovery prevent stale targets and insertion lines that do not commit.
- File-widget empty states now follow the underlying source collection, so importing the first item hides the empty view immediately and moving out the final item restores it immediately.
- Full Native AOT Direct packages now include and architecture-audit the restore-locked Windows App Runtime Insights resource required by Windows 10 notification initialization.
- Startup registration was refactored into direct and store services with contract tests.
- Release validation: x64 test suite 3118/3118 passed; Debug and Release Native AOT builds verified (Release AOT steady state: ~115 MB private / ~171 MB working set).

## 1.4.8 - 2026-08-29

- Unified Direct releases on standard-named Full Native AOT installers that bundle the matching Windows App Runtime. In-place upgrades now remove only files owned by an older Folio payload, preventing obsolete private runtime DLLs from surviving across installer variants (issue #137).
- Resolved mapped folder junctions and symbolic links to their physical traversal paths at runtime while preserving the configured logical path, avoiding Windows RedirectionGuard failures and following junction retargets during refresh and watcher reconnects.
- Interactive file deletions now delegate confirmation to the Windows Shell, following each user's Explorer settings instead of a Folio dialog (issue #86). Permanent deletes use the native prompt, and Native AOT moves surface the system name-conflict dialog instead of silently overwriting. Background cleanup and rollback remain silent.
- Folder watchers that hit a persistent access-denied subtree (broken ACLs on a nested folder) now back off on a long interval with a single warning in the log, instead of restarting and erroring every few seconds. Recovery after fixing permissions or reconnecting a drive still happens automatically.
- A failing icon-only desktop.ini watcher no longer marks the whole folder as degraded; the listing and the main watcher keep working.
- Widgets created while no usable display work area existed (for example with a cloud-gaming virtual adapter as the primary display) are re-placed automatically once a usable display appears, instead of permanently stacking at default coordinates.
- Folio shows a one-time informational toast when the primary monitor is backed by a virtual display adapter; certain virtual GPU drivers are known to break WinUI 3 rendering and layout.
- Stack popovers keep their requested column count at fractional DPI scales (125%, 150%, …): the viewport reserve now scales with the grid shape, because per-item physical-pixel rounding adds up to one DIP per column and a fixed one-DIP reserve still wrapped 3-column grids into 2+1 rows.
- Folio maintains an independent `Folio Files.lnk` entry for the managed storage folder. Existing users keep the default-on behavior, the path follows a storage migration, and uninstall offers to create a collision-safe shortcut when managed files remain.
- Windows 10 now forces square widget and capsule-media corners at render time while preserving the user's saved preference for a later Windows 11 upgrade. Windows 11 continues to apply the selected corner mode to the outer window and embedded media surfaces.
- New installations and restored defaults now use the Standard weather skin, while the Rich skin remains available as an explicit choice.
- Search keyboard navigation now keeps the selected row and highlight synchronized after arrow-key movement and Ctrl+Tab tab cycling. Search tabs are text-only, content-sized, and use a taller, better-spaced selection indicator.

## 1.4.7 - 2026-08-29

- Moved extended Windows Shell context menus into an isolated helper process, so a faulty third-party Shell extension can no longer terminate the Folio process.
- Fixed 3x3 stack popovers wrapping five items as 2+2+1 at fractional DPI scales; the layout now reserves a physical-pixel-safe viewport and pins the requested row/column geometry explicitly.
- Kept hidden desktop-layer widgets hidden during Explorer drag and activation transitions, while preserving the expected peer order for expanded capsules.
- Restored Native AOT binding metadata for Glance calendar day decorations.

## 1.4.6 - 2026-08-28

Hotfix for an issue on some systems where dragging .lnk shortcuts between widgets could inadvertently send them to the Recycle Bin.

Folio 1.4.6 is a major feature, performance, and runtime update. The notes below cover everything added or changed since 1.4.3.

#### Important before updating

- Folio now uses Windows App SDK and Windows App Runtime 2.4 instead of 2.2. A PC that only has the 2.2 runtime will download and install 2.4 once during a Direct-installer update; this is expected and does not remove the older shared runtime used by other applications.
- The installer downloads Windows App Runtime 2.4 only when it is missing. A restart may be requested after runtime installation. For a fully offline update, install the matching x64 or ARM64 Windows App Runtime 2.4 first.
- GitHub Direct builds now use Native AOT and no longer download or require a separate .NET 10 runtime.
- Normal in-place updates keep the existing Folio settings, widget layouts, Todo, Quick Capture, and managed files. The updater also pins the current-user or all-users install scope so a silent update cannot switch scope unexpectedly.
- The minimum supported system remains Windows 10 21H2 (build 19044), and the Direct installer now enforces that requirement.

#### Performance and resource use

- Added Balanced, Resource saver, and Custom performance modes under Settings > General > Performance and resources.
- Custom mode can control hidden-widget cache cleanup, visible-idle cleanup, transient-window release, icon/thumbnail/image cache budget, and individual continuous animations such as text marquee, vinyl rotation, Glance image rotation, and capsule effects.
- Hidden and inactive widgets now release recreatable UI surfaces, decoded images, icons, and thumbnails according to the selected policy. Search and other temporary windows can also release their visual trees after remaining hidden.
- Reused process-wide WinRT settings objects, shared brushes, cached window factories, batched background work, and targeted stack updates reduce repeated allocations, redundant settings fan-out, and full list rebuilds.
- File widgets, Music, and other surfaces avoid unnecessary refreshes when their data is still current, reducing idle CPU and background work without delaying real changes.
- Window animation pacing adapts to the refresh rate of the current display, with additional frame-pacing and backdrop safeguards for Windows 10.

#### Multi-display layouts, movement, and reveal

- Folio now stores a separate widget layout for each known monitor topology. Reconnecting a previous display arrangement restores the positions, sizes, group surfaces, and capsule placement saved for that arrangement.
- Display hot-plug, work-area, and DPI changes are stabilized before restore, and layout writes are paused during the transition so temporary coordinates do not overwrite a known layout.
- A replacement or differently scaled monitor receives a proportional in-bounds layout instead of leaving widgets off-screen.
- Hold Ctrl while dragging a widget title to move all eligible widgets on the current display as one bounded group.
- Widget snapping now works while moving as well as resizing, supports a configurable gap, and keeps screen-edge placement inside the usable work area.
- Added a Quick Reveal layer for temporarily showing widgets above other windows without permanently changing their desktop-layer behavior.

#### Hotkeys and desktop activation

- Global activation now provides ready-made choices for F7 (default), double Ctrl, Alt+Space, Win+Space, and a standalone Win-key tap, while retaining custom shortcut recording.
- Reserved Windows combinations show their system-side effects before they are enabled; modifier chords and incomplete taps are rejected so they do not trigger Folio accidentally.
- Added an optional double-click on a blank desktop area to show or hide all widgets. Icon clicks and distant or slow clicks are excluded.
- Quick Reveal preserves the first activating click and dismisses only for the matching desktop action, reducing lost clicks and unexpected hides.

#### File stacking 2.0 and capsule interaction

- File stacking now separates the master switch from automatic grouping. Manual stacks remain available when automatic grouping is off, and automatic stacking is off by default for new users.
- A stack can open inline or in a separate popover. Popovers support Adaptive, 3×3, and 5×5 layouts, vertical overflow scrolling, and either the widget material or a neutral acrylic style.
- Popover placement follows the source stack and current work area, adapts to item count and file-widget layout, and stays within screen edges.
- Stack popovers now share the file grid's icon size, density, filename, selection, and Ctrl+mouse-wheel sizing behavior.
- Interacting with a popover, context menu, drag operation, title editor, or close confirmation keeps a hover-expand capsule or widget group open until the interaction is finished and the pointer has left.
- Switching between stacks no longer reveals the previous stack on the first frame. Reused popover windows bind and compose the new content while hidden before being shown.
- Fixed Native AOT popovers collapsing into one clipped column, repeated opening/closing retaining excess memory, duplicate item activation, and stale first-frame content.
- Capsule settings now use the expansion behavior as the main control. New widgets expand downward by default, the Sensitive hover preset uses 100 ms expand / 200 ms collapse delays, and the Relaxed animation preset consistently uses 360 ms.
- Fixed collapsed-capsule width changes reverting after pointer release on Windows 10, and kept group close confirmations operable while a group is collapsed or hover-expanded.

#### File widgets and Windows integration

- File and stack-popover selection now follows the Windows desktop shape more closely: the highlight fills the available row or column width with a narrow horizontal gap and adapts vertically to the icon and filename.
- File names can now be hidden in icon view in addition to the existing one-line and two-line choices. Each file widget can also override the global icon size, and widgets can be resized down to 50×50.
- Dragging files can follow the Windows default copy/move decision, including cross-volume behavior and modifier-key shortcut creation. Native drop images and target descriptions are used for Explorer, folders, stacks, and file widgets.
- Shell copy and move operations show per-item progress badges while keeping source, destination, and receiving folders protected from conflicting mutations.
- Added Create shortcut, Permanently delete with confirmation and partial-result reporting, and Run as administrator for supported executable targets. Folio itself remains at normal user privilege.
- The More menu opens near the originating mouse pointer, with a stable button fallback for keyboard or touch. More system operations uses a Windows 10-compatible native Shell path and reports invocation failures instead of silently succeeding.
- Folder and case-only renames are committed atomically, shortcut icons resolve through Shell PIDLs, and the duplicate full-path tooltip line was removed.

#### Search powered by Everything

- File search now reads Everything's existing index over local IPC and merges file and folder results with Folio notes, todos, and settings in the same search window.
- Settings can detect or launch Everything, choose its executable, show connection and permission status, opt into advanced Everything syntax, and filter low-value system/cache paths.
- Folio includes the IPC integration component but does not bundle or install the Everything application. File search requires Everything to be installed, running, and explicitly allowed in Folio.
- The legacy Folio-maintained file index, USN tracking, Windows Index integration, and native search core were removed. Folio-owned leftover index data is cleaned automatically, eliminating a duplicate background index.

#### Appearance, media, and everyday details

- Widget foregrounds can follow the app theme or use light, dark, or custom text and monochrome-control colors, with an optional text edge treatment and per-widget overrides.
- Glance adds independent background-image transparency, while Music can switch between available media sessions or follow the system-selected source.
- Todo manual reordering, notification actions, Quick Capture attachments, Glance switching, and feature-widget settings were hardened for Native AOT builds.
- Desktop organization can optionally include retained folders, large files, and items beyond the quick batch, and now explains access-denied, in-use, changed, unavailable, or failed transfers separately.
- Onboarding can recommend a suitable internal non-system drive for managed files. If the selected storage drive is temporarily disconnected, widgets remain intact and recover after the drive returns.
- Weather startup uses fresh cached forecasts immediately, preserves manual locations, bounds automatic retries, and keeps refresh work off the interaction path.

#### Startup, persistence, and packaging reliability

- Startup no longer forces Explorer to create a desktop host while Windows is restoring desktop icon positions. Widget restoration proceeds immediately, while desktop-layer attachment waits for Explorer's existing icon host to stabilize.
- Auto-start now uses the per-user Run entry and appears in Windows Startup apps. Legacy task registrations are migrated when safe, and disabling Folio from Windows is reflected by the in-app switch.
- Fixed a Microsoft Store persistence failure that could restore settings and widget data from an older state after reopening Folio. Atomic replacement now retries and uses a verified backup/write-through fallback when Windows temporarily blocks destination removal.
- Native AOT compatibility fixes restore settings dropdowns, file stacks, Glance, Music, Todo, Quick Capture, image attachments, support QR images, and multiple-widget switching in Direct builds.
- Installer filenames remain `Folio_Setup_<version>_<arch>.exe`, preserving the update contract used by 1.4.3. Direct installers continue to be produced for x64 and ARM64.

## 1.4.3 - 2026-08-19

#### Windows 10 motion and materials

- Windows 10 startup now treats a rejected tray Efficiency Mode request as an optional optimization failure, so `WidgetManager` initialization and desktop organization can continue.
- Widget resize, capsule transitions, and full-widget expand or collapse keep moving the real widget windows while reducing redundant UI-thread work and repeated window-position updates.
- Resize guidance is updated more efficiently and restores the illuminated edge feedback without placing a placeholder over the widget.
- Windows 10 receives a dedicated material compatibility path. Unsupported Mica and system-corner choices are identified as Windows 11-only, while acrylic, solid, and fully transparent modes retain their Windows 10 behavior.
- Rapidly interrupted animations now hand off to the latest operation more cleanly, reducing delayed or sticky responses during repeated interaction.

#### Traditional Chinese

- Traditional Chinese (`zh-TW`) is available throughout the app and in both x64 and ARM64 installers.
- Language persistence, update notes, weather descriptions, city search, lunar calendar content, Todo, Quick Capture, and desktop organization now distinguish Simplified and Traditional Chinese.
- Traditional Chinese terminology and punctuation were reviewed for Taiwan usage, with complete resource-key and formatting-placeholder parity across all supported languages.

#### Release packages

- x64 and ARM64 are available as recommended framework-dependent installers and as larger `_Full` offline installers containing private .NET and Windows App Runtime components.

## 1.4.2 - 2026-08-16

#### Todo and Quick Capture

- Todo checkmarks stay with the intended task under custom sorting and filtering. Unchecking an item in Completed removes only that task without clearing the next row's checkmark.
- Single-pane Todo detail now uses a clear back arrow and removes the duplicate completion control. Header actions use consistent spacing and a compact frosted surface.
- Clipboard images in Quick Capture show thumbnails in both list and detail views. Copying an image record writes the image itself back to Windows instead of a local path.
- Todo and Quick Capture share equal-size attachment tiles. Images show thumbnails, other files show file icons, and the horizontal strip reveals a remove button on hover.
- Records and Pinned keep their empty states after repeated tab changes. Pinned has its own add card and creates pinned content directly.
- Meaningful Quick Capture drafts are saved before switching records, while an empty draft no longer blocks navigation or creates a blank entry.
- Newly created Todo and Quick Capture widgets include editable localized guides. Resetting either feature restores its guide so the main functions remain easy to discover.
- Markdown tables, code, links, task lists, emphasis, and pasted images render more consistently. Text and semantic accent colors remain readable in both light and dark themes.
- Quick Capture search now opens in the tab row instead of adding another line. The field's clear button removes the query, while Cancel or Esc closes search and restores the tabs.

#### File widgets, stacks, and previews

- Manual stacks remain available when automatic stacking is turned off. Changing the members or order of an automatic stack converts it to a manual stack so later rule updates do not overwrite the change.
- Files can be dragged into a stack from File Explorer, a browser, or another file widget. Members can be removed individually, a stack can be dissolved, and a stack with one remaining member dissolves automatically.
- Dragging an item out of a widget no longer changes its sorting mode. Reordering inside a stack stays local to that stack instead of rearranging the entire file widget.
- Stack expand and collapse transitions use dedicated motion, keyboard handling, focus recovery, and clearer ownership cues. Drop highlights and pulsing borders are cleared as soon as a drag ends.
- File-type and custom stacking rules now follow the selected mode independently, avoiding unexpected groups produced by two rule sets at once.
- Files and applications opened from a file widget now launch through the Explorer desktop shell. They receive the same user environment as desktop-launched items, improving compatibility with local development tools, services, and globally installed integrations.
- When QuickLook reaches the edge of the current file widget, arrow-key navigation can continue into an adjacent visible file widget without closing the preview.

#### Capsule mode, widget groups, and desktop layer

- Capsule expansion direction can be set to Auto, Down, or Up. Fixed directions keep the title edge anchored, including after widgets are hidden and restored with F7.
- Hover auto-expand reconnects after mode changes, hiding, and restoring. Hiding a widget also clears stale hover state and the temporary drag mask.
- Widgets assigned to the desktop layer stay there when their title or empty area is clicked. Dynamic-layer widgets keep their existing temporary foreground behavior.
- Expanding, collapsing, and resizing widgets release interrupted animation work more reliably, making repeated compact-mode operations feel smoother.
- Group wheel navigation no longer stalls during rapid input or jumps two members from one effective step. One switch produces one background highlight instead of repeated flashing.
- First-to-last circular navigation remains available, so continuous wheel movement can keep cycling through the group.
- Newer navigation requests replace unfinished older ones without rolling back an already visible member. Collapsed groups also show a compact position indicator.
- Widgets recover their desktop position more reliably after interaction, display changes, or Win+D, reducing unexpected overlap above other applications.

#### Music and interface polish

- Music transport controls use filled Windows-style icons. Play, pause, previous, and next share one responsive size, while repeat and volume remain compact auxiliary controls.
- Cover mode uses a taller frosted control strip with left-aligned song information and a tighter single-line layout. Record and control modes receive matching spacing and hover treatment.
- Todo, Quick Capture, onboarding, desktop organization, and release notes use clearer frosted action surfaces with more consistent margins and compact controls.
- The floating search window's drag handle uses an opaque light or dark color, avoiding the visible center overlap caused by translucent layers.
- Feature-widget settings use the same colorful icons as widget title bars, with card spacing aligned to the rest of Settings. Several unclear settings descriptions are shorter and more direct.
- The title-bar add button opens the same widget-creation menu from file and feature widgets. Todo and Quick Capture keep their content-specific new-item actions inside their workspaces.

#### Onboarding, search, and smaller details

- The opening logo animation is slower and fades smoothly into the first onboarding step without shrinking toward the corner. The product message stays visible long enough to read.
- Onboarding moves through the main actions more directly and keeps feature-widget choices synchronized with Settings.
- Background search refreshes retain unchanged rows, selection, and resolved icons, reducing visible rebuilding and flicker.
- Weekly high and low temperatures use the same visual scale as their day labels, and several empty, hover, and compact states now follow one consistent style.

## 1.4.1 - 2026-08-12

#### File widget polish

- File-widget scrollbars now appear during pointer, wheel, and keyboard activity, then hide after three seconds without interaction.
- Grid and list layouts share the same scrollbar activity tracking and release their timer when the surface is unloaded or disposed.

#### Visibility reliability

- Hide All now cancels pending automatic and topmost safety restoration before the hide animation begins.
- Desktop-layer restoration is rejected for hidden, actively hiding, or closing widgets, preventing stale callbacks from reopening windows after the user hides them.
- Application, package, x64 installer, and ARM64 installer versions are aligned on 1.4.1 / 1.4.1.0.

## 1.4.0 - 2026-08-11

#### Todo and Quick Capture workspaces

- Todo and Quick Capture now share a responsive master/detail foundation. Narrow widgets use a focused single-pane flow; wide widgets can keep the list and editor visible together with a persisted, adjustable splitter.
- Standalone and grouped Quick Capture hosts now use the same surface, layout policy, tab visibility, preview-line limit, material refresh, selection restore, and edit behavior.
- Todo notes and Quick Capture records gain native Markdown editing and safe preview, including formatting commands, task lists, tables, source-preserving edits, configurable Enter behavior, and compatibility with existing plain-text records.
- Quick Capture attachments can be removed directly while editing in the shared host. Pending files and persisted attachments follow the same empty-record safeguards as the standalone editor.

#### Settings and live application

- Todo and Quick Capture settings now use the same compact hierarchy: the feature switch comes first, wide-layout behavior follows it, related controls are grouped in native expanders, and related display choices use concise multi-selection menus.
- Quick Capture wide-open mode, tab visibility, list preview lines, Markdown defaults, and Enter behavior now update both active hosts instead of waiting for a reconstruction or affecting only the legacy window.
- Weather view selection is no longer overwritten by the segmented control's template default during initialization. Explicit Day or Week choices persist even when they match the global default.
- Quick Capture materials refresh correctly after recycled list containers, theme changes, view switches, and unsaved paper-style changes. Clipboard-history rows cannot inherit a saved note's paper color.

#### Capsule, groups, and desktop layer

- Smart hover expansion is rearmed after a Todo or Quick Capture member is switched into an existing group window. Real pointer movement can repair a missing routed entry event, and a deliberate dwell over title controls can expand without stealing an active click.
- Segmented tabs are realized against the final expanded width before the capsule animation starts, preventing Todo and Quick Capture tabs from appearing after the body.
- Rapid group-member wheel input is coalesced per surface, while completed live presenter swaps are protected from stale cancellation callbacks.
- Temporary title and interaction raises are tracked with generation-safe leases. The complete batch returns to the desktop layer after interaction, and stale delayed callbacks cannot release a newer raise.
- Desktop-layer ownership is reasserted during idle normalization so moving or interacting with a widget does not leave it above applications or break its Win+D desktop behavior.

#### Memory and reliability

- Hidden widgets now perform a soft cleanup after 30 seconds: dead localization targets, file metadata, icon and thumbnail caches, an unused search shell, eligible managed objects, and a high working set are released without waiting for the deep-cleanup timer.
- When widgets remain visible but Folio has had no foreground, pointer, settings, search, onboarding, or widget interaction for 30 seconds, visible-idle maintenance releases caches and performs allocation-aware managed collection. Any new activity restarts the idle window.
- The heavy-cleanup marker is always consumed, preventing a stale flag from permanently disabling visible-idle maintenance and compact-expansion warmup.
- Version metadata is aligned across the application, package manifest, x64 installer, and ARM64 installer at 1.4.0 / 1.4.0.0.

## 1.3.9 - 2026-08-09

#### Capsule mode and startup

- Smart auto-hide now synchronizes from the physical pointer when the mode is entered, retries collapses deferred by menus or interaction, and limits concurrent bounds animations during bulk state changes.
- The currently expanded capsule holds an explicit peer-layer lease so another Folio widget cannot cover it. Stale collapse callbacks cannot release the newer expanded widget.
- A new application session restores every enabled standalone widget and the active member of each widget group. Shutdown no longer persists process teardown as a user-requested hidden state.
- Group-title wheel navigation wraps and retains its pending member until preparation, persistence, and content switching finish.

#### Selection, drag, and keyboard parity

- Dragging from a multi-selection now preserves the full batch in file widgets, search results, standalone Quick Capture, and grouped Quick Capture.
- Internal drag payloads retain mixed file and folder paths instead of dropping folders when files are present.
- Quick Capture tab drops apply every dragged item, and Delete handles all selected Quick Capture or Todo rows.

#### Large transfers and cancellation

- Transfer UI appears during preparation, uses indeterminate progress while byte totals are unknown, and ignores stale callbacks from an earlier operation.
- Pressing Cancel immediately changes the card to an explicit canceling state with a progress indicator. The transfer pipeline reports canceling and canceled phases in order.
- Cross-volume moves use cancellable streamed copies. Same-volume moves keep their atomic path, read-only source files can be removed safely, progress callbacks are throttled for large batches, and canceled batches roll back completed work where possible.

#### First-run experience

- The onboarding flow is reduced to one introduction, two optional real exercises, and a final choice. Forced path confirmation and repeated explanation panels are removed.
- Managed file drops are explained as Move by default. File and visibility exercises complete only after their actual operation succeeds.
- The 2.5-second Folio logo sequence remains, while each step now pairs concise text with native icons, state diagrams, and icon-backed progress feedback.
- Application, package, x64 installer, and ARM64 installer versions are aligned on 1.3.9 / 1.3.9.0.

## 1.3.8 - 2026-08-08

#### Highlights

- Added Hindi, Spanish, French, Arabic, Bengali, and Russian language options. System-language detection, package resources, weather, update messages, onboarding, and core file-widget flows now recognize all six locales.
- Standardized product wording around file widgets, folder mapping, automatic stacks, widget groups, tray creation, and Move to Recycle Bin.
- The first file widget opens on the right, explains managed storage and folder mapping, and points users to the system tray when they need another widget.
- Fixed pasting from the desktop into a file widget, copying or cutting multiple selected files to Explorer, and stale cut-state visuals after a completed move.

#### Reliability and updates

- Startup activation, command-line fallback, and interactive launches now share one policy. Startup stays silent and tray-first, while a deliberate launch remains interactive.
- The initial file widget is created only for a genuinely new profile. Removing every file widget is respected, and settings recovery does not recreate one unexpectedly.
- Dynamic widgets remain on the desktop when using Win+D, while still raising above other applications when explicitly invoked.
- Uninstall now offers a safe choice between keeping application data for a later reinstall and permanently removing settings, widget layouts, notes, tasks, caches, logs, update files, and recovery snapshots. Files in the configured managed-storage location are always preserved.
- Localized installer, dependency, upgrade, and uninstall messages now substitute paths and item counts correctly; all six new installer languages also pass the matching locale to Folio on first launch.
- ARM64 updates now use their own installer URL, SHA-256, and size from the stable manifest.
- Application, package, x64 installer, and ARM64 installer versions are aligned on 1.3.8 / 1.3.8.0.

#### Localization scope

- The new language packs prioritize the main file-widget, onboarding, weather, and update experiences. Less-used detailed settings temporarily fall back to English rather than mixing in unrelated Chinese text.
- The x64 and ARM64 installers now offer the same eleven selectable interface languages as Folio, including localized dependency download and uninstall messages.

## 1.3.7 - 2026-08-05

#### Unified File Widgets and Groups

- **One shared file surface:** standalone file widgets and grouped file widgets now run through the same `FileSurfaceContent` and unified content host. The legacy standalone file interaction tree and XAML have been removed, eliminating a major source of behavior drift.
- **Group and standalone settings stay aligned:** list details, path display, compact list spacing, title styles, context menus, QuickLook, sorting, auto stacks, and cyclic Ctrl+Tab switching now follow the same implementation.
- **File operations are more complete:** newly created folders scroll into view and enter inline naming, new and existing folders can be renamed, files can be moved or copied into a folder item, and manual ordering is restored reliably after a cold start.
- **Native drag behavior is more predictable:** shortcut drag-out uses a Shell-compatible virtual-file path and reconciles a completed move, while reorder insertion indicators use a softer transition and no longer block dropping onto a folder.

#### Hotkeys, State, and Recovery

- **Repeated global toggles are serialized:** rapid tray or hotkey presses are processed through one state transaction, preventing widget windows from being stranded off-screen or the global hotkey from becoming unresponsive after several toggles.
- **Lifecycle recovery is broader:** display topology, DPI, sleep and resume, remote-session changes, and Explorer restarts share window-state recovery and diagnostics.
- **Settings survive shutdown more reliably:** pending file-order and configuration changes are flushed before exit, recoverable snapshots protect against damaged settings, and save failures are recorded instead of silently reverting to defaults.
- **Weather view preference persists:** the selected Day or Week forecast mode is restored after an application restart.

#### Search and Interface

- **Search returns useful work sooner:** providers publish staged incremental results and failures are isolated so one unavailable source does not block the remaining search pipeline.
- **Selection and tables are clearer:** Ctrl/Shift multi-selection, rubber-band selection with edge auto-scroll, result rows, sortable columns, and header hover states have been refined for predictable batch work.
- **Windows-native visual states:** file hover and selection use theme-adaptive neutral colors with tighter corners; destructive Close actions use one softer red treatment across widgets, and file context menus have a consistent order.
- **Settings and support polish:** About uses space more efficiently, the unnecessary repository button was removed, a feedback email card was added, and diagnostics can be exported as a privacy-filtered package.

#### Installation and Updates

- **Upgrades keep one Folio installation:** stable installer identity and existing-path detection reuse and lock the current install directory, preventing a normal upgrade from creating a second copy.
- **The update handoff is visible:** after Folio closes, the downloaded installer opens normally so the user can see progress without choosing the installation path again.
- **Download failures are actionable:** the updater offers retry and official-site fallback, while long release notes open in a dedicated view instead of being clipped above the progress bar.
- **Dual-architecture distribution:** framework-dependent x64 and ARM64 installers reuse compatible .NET 10 and Windows App Runtime 2.2 installations and download only a missing architecture-matched dependency.

## 1.3.6 - 2026-08-04

#### Widget Group Reliability and Interaction

- **Fast tab switching is recoverable:** Ctrl+Tab is now handled once per physical key gesture, with a short cooldown for rapid separate presses. Repeated key-down messages can no longer queue a switch storm or leave the keyboard path waiting for a manual tab click to recover.
- **Faster grouped file surfaces:** inactive group members retain a bounded, reusable content cache. Rapid tab changes no longer repeatedly dispose and recreate file grids, watchers, and icon work.
- **Correct group layering:** grouped widgets now keep the same temporary foreground state as standalone widgets at startup and across merge, detach, dissolve, and member-switch transitions.
- **QuickLook works inside groups:** Space reaches the selected file surface in a group and opens the same QuickLook preview behavior as a standalone file widget.

#### File Import and Visual Polish

- **Browser and WeChat drops are reliable:** virtual browser files preserve their resolved extension when copied into a standalone widget, and the native drop bridge lets WeChat and other non-WinUI sources import into grouped file widgets.
- **Initial icon hydration is consistent:** grouped file surfaces no longer cancel their first background refresh merely because their desktop window is shown without activation. Placeholder icons are temporary, square rounded cards with a softer appearance.
- **Capsule and content transitions are smoother:** compact expansion warm-up follows the actual incoming group content, while expensive surface work is deferred during geometry transitions.

#### Desktop Organization and Release

- **Organization targets are clearer:** the preview reports the actual existing widget destination rather than misleadingly showing a new target, and placement can keep working when the visible desktop is crowded or widgets are hidden.
- **Framework-dependent installers:** x64 and ARM64 installers remain small and check the matching .NET 10 Runtime and Windows App Runtime 2.2. Missing dependencies are downloaded only when needed.

## 1.3.5 - 2026-08-03

> Scope note: Desktop organization, automatic organization, and widget groups were not part of 1.3.4. They are new in 1.3.5; the 1.3.4 section below remains the historical release record.

#### New in 1.3.5

- **Desktop organization (new):** A responsive card preview shows the files to move, the selected destination, the items that stay on the Desktop, and the final storage location before anything runs.
- **Desktop organization controls (new):** Each card has its own organize checkbox and target selector, with readable icon previews, a destination explanation, a non-moving-items panel, and responsive window sizing.
- **Automatic organization (new):** Growing downloads, temporary/archive work, extraction, and same-path replacements wait for a stable terminal state; 100 MB is the large-file threshold and incomplete baselines are never committed.
- **Widget groups (new):** File widgets can be merged, scrolled with the title wheel, detached by dragging the title, dissolved, and normalized to the group’s standard or compact presentation with consistent z-order.
- **File dragging and sorting:** Cross-screen reorder uses a breathing insertion line and commits once on drop; standalone and grouped surfaces share the same virtualization-safe insertion calculation.
- **Folder widget synchronization:** Structured enumeration distinguishes empty, partial, unavailable, and access-denied folders; offline snapshots stay visible, refreshes preserve manual order, and watcher generations isolate stale mappings.
- **Search and USN reliability:** Root manifests, partial-scan protection, subtree cleanup, watcher recovery, and incremental USN create/delete/rename/hard-link updates prevent valid results from disappearing during outages or journal gaps.
- **Path and merge safety:** Junction, symlink, SUBST, UNC-alias, nested-mapping, merge, dissolve, and save-failure paths receive real-identity checks and rollback protection.
- **Windows compatibility and recovery:** Backdrop fallback, reduced-motion/high-contrast handling, sleep/unlock/RDP recovery, Explorer restart checks, display recovery, and startup z-order normalization improve Windows 10 and Windows 11 behavior.
- **Release notes, diagnostics, and localization:** Settings opens the latest bilingual Markdown notes in a separate window; watcher/index health is visible; five language resources and reliability tests are synchronized.

## 1.3.4 - 2026-07-29

#### Resource Lifecycle and Memory

- **Feature widgets now release their UI**: Disabling Todo, Quick Capture, Music, Weather, or Search closes the corresponding window and releases its content, view model, subscriptions, timers, and feature-owned services while preserving saved data.
- **Settings closes instead of hiding**: Closing Settings now destroys the window and releases its WinUI visual tree. Reopening Settings creates a fresh window.
- **Transient timer ownership fixed**: One-shot capsule timers now detach their handlers when they fire or are cancelled, and music timers, storyboards, and event subscriptions stop with their owning view instead of remaining rooted for the process lifetime.
- **Guarded idle maintenance**: Folio can compact managed/native heaps and trim resident pages after full background inactivity. A separate threshold-based maintenance pass can also run while widgets remain visible but Folio is not being used; foreground, pointer, resize, search, indexing, and other active work suppress collection.
- **Bounded caches**: File icons, decoded bitmaps, and metadata caches now have count and estimated-memory limits, with diagnostics for verifying their size.

#### Search Responsiveness and Index Residency

- **Search follows its feature switch**: Heavy search services are not initialized at startup when Search is disabled. Turning Search off releases its popup, hotkey registration, custom/USN indexes, history/action services, file metadata service, and icon cache.
- **Popup shell warm-up**: When Search is enabled, Folio prepares the empty popup shell during a low-priority idle slice so a desktop-widget click does not have to construct the full WinUI window first.
- **Open-only widget action**: Repeated search-widget clicks now open or refocus the popup and can no longer toggle it closed while queued pointer events are still arriving.
- **Window-first loading**: The native popup is shown and focused before recommendations, result icons, or an idle-unloaded index do work. Index restoration starts when the popup is invoked, not after the user types.
- **Idle index unload**: After five minutes without Search, the large resident custom index is saved and released while lightweight file-system watchers remain active. Changes are collected in a small delta map and reconciled when the popup restores the index.
- **Lower-cost indexing and ranking**: Persisted indexes are streamed instead of building a second full JSON copy; directory strings are pooled; stale scans are cancellable; fresh indexes can be reused; and searches retain only the highest-ranked candidates.
- **Visible result fallbacks**: File and folder results without a decoded Shell icon now display an appropriate fallback glyph instead of an empty icon column.

#### Capsule Mode and Window Layering

- **Idle expansion warm-up**: Collapsed widgets pre-measure their expanded layout during an idle UI slice, reducing the first hover expansion hitch without visibly opening every capsule.
- **First-hover recovery**: Smart hover expansion reads the native cursor position after startup, tray restore, and wake, so a capsule no longer needs an activating click before hover works.
- **No hover-through between capsules**: Smart expansion verifies the native pointer root window before opening. Moving inside an expanded widget can no longer trigger an overlapping collapsed capsule underneath it or let that capsule steal the foreground layer.
- **Foreground-safe hover expansion**: If a capsule is still physically above the desktop after a temporary tray/F7 raise has ended, hover expansion reorders it only among Folio windows and keeps the current external application in front.
- **Immediate title-bar collapse**: In click-to-toggle mode, clicking the expanded title bar collapses the widget on the next UI turn without the previous fixed 420 ms delay. Other expansion modes are unchanged.

#### Weather Redesign

- **Responsive information hierarchy**: Mini, compact, and expanded layouts were rebuilt around current conditions, compact metrics, hourly forecast, sunrise/sunset, and a scrollable Week view.
- **Standard and Rich skins**: Standard follows the selected app theme and capsule surface; Rich uses condition-aware gradients and matching compact presentation. Rich is now the default for new users and after reset.
- **Contrast across conditions**: Rich-skin text is selected from the weather backdrop rather than blindly following app light/dark mode, while interaction indicators and daylight details use palettes tuned separately for Standard and Rich surfaces.
- **Cleaner native selection**: Day/Week and current-hour selection use a small system-style bottom indicator without changing label weight or color, and labels no longer clip at compact heights.
- **Compact sunrise arc**: The previous tall divider was replaced by a lower sunrise/sunset arc with a daylight progress marker.
- **Reduced continuous effects**: Decorative perpetual weather animations were removed. Only short, state-driven feedback such as the active refresh rotation remains.
- **Forecast and symbol fixes**: Restored the multi-day forecast in the largest single-day layout and replaced the boxed fog emoji with a compatible cloud symbol.
- **Late-day hourly continuity**: MSN forecasts now continue into following days when the current day has no remaining hourly rows.
- **Reliable city selection**: City search ignores spacing, punctuation, and diacritics, ranks exact text matches before proximity, validates coordinates, and queues a fresh request when the previous city's refresh is still running.

#### Music Stability and Efficiency

- **Stable track handoff**: Bursts of media-property events are settled and generation-checked before display, keeping the previous complete track visible until the next complete title and artist are available.
- **Transient empty-state protection**: Brief empty metadata returned while changing tracks is retried instead of clearing the capsule and rebuilding its text several times.
- **Cover work is deduplicated**: Covers reload only when the session/track signature changes, decode at a bounded size, and discard stale asynchronous results.
- **Inactive animation work stops**: Title marquee and record rotation stop while the widget is hidden or collapsed and are restarted only when the expanded surface is visible. The marquee no longer leaves a flashing trailing clone while paused.
- **Lower-cost progress updates**: Expanded, collapsed, hidden, and non-seekable states use different refresh behavior instead of running the full timeline path continuously.
- **Control sizing polish**: Play is slightly larger in Controls, Record vertical, and Record horizontal layouts; previous/next, playback mode, and volume use a smaller consistent secondary size.
- **Empty-cover corners**: Fixed the soft upper corners around the empty album-art surface in Controls mode.

#### File Icons and Launching

- **Sharper large icons**: Executables and shortcuts request higher-resolution Windows Shell icon sources, fixing visibly blurred enlarged icons for apps such as packaged or custom launchers.
- **Cleaner small icons**: High-quality downsampling reduces the jagged look that appeared after improving large icon extraction.
- **Stack icon scaling**: Icons inside automatic stacks now follow the configured file-icon size.
- **Bounded thumbnail memory**: Decoded icon and thumbnail data is evicted by both item count and estimated bytes.
- **Shortcut launch compatibility**: `.lnk` files use the Windows Shell execution path, avoiding direct-resolution failures and reducing shortcut-specific crashes or long stalls.

#### Widgets and Context Menus

- **Blank-area menus**: Todo and Quick Capture blank content areas now expose the same widget menu as their title bars.
- **File-widget menu completion**: The file content menu includes Title style and Expansion mode, positioned below Auto stack and above Sort by.
- **Conditional Paste**: Paste is omitted when the clipboard does not contain content that the file widget can accept, instead of showing a disabled command.
- **Close from file content**: File and mapped-folder content menus now include the same Close widget command and second-step confirmation used by the title-bar menu, positioned above Refresh.

#### Hotkeys and Product Cleanup

- **Removed low-level keyboard hooks**: Global and Search hotkeys now use standard `RegisterHotKey`/`WM_HOTKEY` handling, avoiding intercepted D-key input, repeats, and latency.
- **RDP modifier recovery**: Modifier key-up recovery prevents Alt/Ctrl/Shift from remaining logically pressed after a Remote Desktop hotkey.
- **Correct Search hotkey lifecycle**: The Search hotkey switch refreshes registration correctly and cannot open Search while the Search widget feature is disabled.
- **Simplified hotkey settings**: Global and Search hotkeys share the top-level Shortcuts & Interaction page in compact expandable cards.
- **Opt-in first-run behavior**: New installations and reset defaults keep capsule mode and the global Search hotkey off until the user enables them.
- **Search widget title default**: Newly created Search widgets use floating title chrome by default, matching Weather and Music.
- **Image widget removed**: The legacy image-gallery widget and its associated services, settings, models, and views were removed.

#### Distribution

- **Version 1.3.4**: Application, file, assembly, MSIX, and installer versions are aligned to 1.3.4.
- **x64 and ARM64 installers**: Both packages are framework-dependent. Setup checks the matching .NET 10 Runtime and Windows App Runtime 2.2 architecture and downloads only a missing dependency.

## 1.3.3 - 2026-07-24

#### Drag & Drop Enhancements

- **WeChat file drag-drop**: Drag files and images directly from WeChat chat windows into grid items. A background polling thread reliably detects mouse release during OLE drags, bypassing WinUI 3's unreliable Drop event for non-Chromium drag sources.
- **Browser URL drag-drop**: Dragging images or file links from browsers (Chrome, Edge, Firefox) now downloads the URL to a temporary file and imports it automatically.
- **Folder drop support**: Files dragged onto a folder item within a grid are transferred into that folder.
- **Virtual file fix**: NativeDropTarget now tries `TYMED_ISTREAM` and `TYMED_HGLOBAL` separately for `FileContents`, fixing silent failures on browser virtual file drops.

#### Stack Group Management

- **Rename stacks**: Right-click a stack group header to rename it with an inline text box. Custom names persist across restarts.
- **Reorder stacks**: Move stack groups up or down via the right-click context menu.
- **Disable/restore stacks**: "Don't stack this group" dissolves a stack so its members display as loose items. Disabled groups can be restored from the same menu.

#### Weather Data Source

- **MSN Weather (default)**: Added MSN Weather API as the default data source — the same source used by the Windows weather widget. Provides current conditions, hourly and daily forecasts, sunrise/sunset, and UV index.
- **Open-Meteo fallback**: If the primary source fails, the app automatically falls back to Open-Meteo. Users can also manually select the data source in Settings → Weather.
- **Weather code mapping**: Added reverse mapping from MSN weather description text to WMO codes, so MSN-sourced data reuses the existing emoji/glyph/animation system.

#### F7 Z-Order Reliability Fix

- **Silent restore fix**: `ClearTopMostPreservingForeground` now unconditionally calls `BringWindowToFront(foreground)` instead of gating on `wasTopMost`. Previously, raised widgets used temporary TopMost (TOPMOST→NOTOPMOST), so the gate was always false — the restore state changed but the visual didn't, causing "flicker without collapse" on the next F7 press.
- **Cross-process click detection**: Replaced unreliable low-bit `GetAsyncKeyState` with a 50ms high-bit mouse sampler that detects up→down edges globally. The 200ms restore monitor now consumes an `_outsideMousePressObserved` flag, reliably detecting clicks on foreign windows — including clicks on an already-activated window.
- **Drag poll interference fix**: Reduced the drag-drop background polling timeout from 30s to 5s to minimize interference with `GetAsyncKeyState` low-bit consumption.

#### Search Widget Refinements

- **Clear history button**: Added a clear button in the search widget footer to wipe recent queries and result cards while preserving pinned favorites.
- **Live sync**: The search widget now auto-refreshes when search history changes in the popup.
- **Removed auto-generated recommendations**: The widget body shows only items the user actually opened. Auto-generated Start Menu app shortcuts are no longer shown in the widget body.
- **Simplified placeholder**: Search placeholder changed to "Search files..." for clarity.

#### UI / UX Polish

- **Tray icon label fix**: The "Black" and "White" tray icon labels were swapped — now correctly labeled.
- **Collapsed preview chevron**: Hidden the chevron icon in collapsed stack preview mode for a cleaner look.
- **Search popup**: Removed the "Recommended apps" section header; minor padding adjustment.

#### Localization

- Added new strings for weather data source, stack management, and search clear button across all five supported languages (zh-CN, en-US, ja-JP, de-DE, pt-BR).
- Fixed tray icon color labels in all languages.

## 1.3.2 - 2026-07-23

#### Search System (New)

- **Full-text desktop search**: Added a complete search infrastructure with a dedicated search popup window, global hotkey activation, and instant result display. Supports searching files, folders, applications, and settings from a single entry point.
- **USN Journal indexing**: Added `UsnJournalIndexService` that reads the NTFS USN change journal for fast, low-overhead file discovery without walking the directory tree.
- **Windows Index integration**: Added `WindowsIndexSearchService` that queries the Windows Indexing Service for content-searchable results including document metadata.
- **Search result ranking**: Added `SearchResultRanker` with a weighted scoring algorithm considering name match position, file type priority, recency, and path depth.
- **Search history**: Added `SearchHistoryService` with recent-search persistence, quick re-execution, and history clearing.
- **Recommended apps panel**: The search popup shows a recommended-apps panel with keyboard navigation (arrow keys + Enter to launch, Space to preview via QuickLook).
- **Type sort and filter**: Search results can be sorted and filtered by type (files, folders, apps, settings).
- **Search widget content**: Added `SearchWidgetContent` and `SearchWidgetContentAdapter` so search can be embedded as a widget content type.
- **Search settings section**: Added a dedicated Search settings page with hotkey configuration, index scope, result count, and recommendation toggles.

#### Multi-Language Expansion

- **Three new languages**: Added Japanese (ja-JP), German (de-DE), and Brazilian Portuguese (pt-BR) localization with 1500+ translated strings each, covering all widgets, settings, onboarding, search, tray menus, dialogs, and status messages.
- **Localization architecture rewrite**: Replaced the legacy `.resw` resource system with a JSON-based `LocalizationService` that loads embedded `Strings/{locale}.json` files at runtime, enabling easier community translation contributions and hot-reload during development.

#### Onboarding Redesign

- **Five-step focused flow**: Rebuilt the first-run guide from the previous multi-panel layout into a focused five-step flow: Welcome, Feature Overview, Storage Setup, Hotkey Configuration, and Completion.
- **Refined layout and animations**: Each step uses purpose-built entrance animations, consistent spacing, and localized content in all five supported languages.

#### Adaptive Tray Animation System

- **Hardware-adaptive controller**: Added `AdaptiveTrayAnimationController` and `HardwareAdaptiveAnimationService` that detect GPU capability and adjust animation complexity accordingly — full composition animations on discrete GPUs, simplified fallbacks on integrated graphics.
- **Batch animation driver**: Added `WidgetTrayBatchAnimationDriver` for synchronized multi-widget tray show/hide, eliminating per-widget timing drift.
- **Smart animation adapter**: Added `SmartAnimationAdapter` that bridges the adaptive controller with existing widget animation paths.
- **Synchronized scheduling**: Replaced fire-and-forget `DispatcherQueue.TryEnqueue` calls with synchronized scheduling, reducing animation tearing on rapid tray toggles.

#### Weather Improvements

- **City database expansion**: Expanded the offline `cities.json` database from ~500 to 5000+ entries covering significantly more small and medium cities worldwide.
- **Improved city search**: `CitySearchService` now uses prefix + fuzzy matching with CJK-aware tokenization for better results in Chinese, Japanese, and other non-Latin scripts.
- **Location service refinement**: Improved `WindowsLocationHelper` with better timeout handling, fallback to last-known position, and clearer error reporting.
- **Weather layout improvements**: Refined `WeatherWidgetContent` layouts for all four size modes with better spacing, icon alignment, and forecast list scrolling.

#### Installer and Updater

- **ARM64 installer**: Added `Folio.arm64.iss` and `Folio.Dependencies.arm64.iss` for native ARM64 Windows builds.
- **Reliable process kill**: Installer now uses a robust process-termination sequence before overwrite, avoiding file-lock failures.
- **Force-update enforcement**: `AppUpdateService` now enforces mandatory updates when the server flags a version as critical.
- **Helper cleanup**: Stale update-helper directories under `%LocalAppData%\Folio\update-helper` are cleaned up automatically.
- **Official download URLs**: Switched all download links to the official GitHub Releases channel.
- **Migration support**: Added `Folio.Migration.iss` for handling data migration during major version upgrades.
- **English installer language**: Added `Languages/English.isl` for proper English installer UI on non-Chinese systems.
- **Installer language selection**: The installer now shows a language-selection dialog (Chinese, English, Japanese, German, Brazilian Portuguese) pre-selected to the system locale. The chosen language is written to `HKCU\Software\Folio\InstallLanguage`, and Folio uses it as the default app language on first run (a manual in-app change still wins).
- **Search popup polish**: The result-list header now aligns with the data rows; the sort header carries a subtle background and shares the menu-bar margins.
- **Weather capsule fix**: Removed a duplicate title icon so capsule mode shows only the weather emoji.
- **Capsule hover mask**: Hidden the semi-transparent right-edge hover mask in capsule mode (interaction unchanged).

#### Architecture and Maintainability

- **App.Tray extraction**: Extracted `App.Tray.cs` (704 lines) from `App.xaml.cs`, consolidating all tray icon, menu, and lifecycle logic.
- **ServiceRegistry**: Added `ServiceRegistry` for centralized service location, reducing constructor parameter explosion.
- **SettingsMigrationService**: Added `SettingsMigrationService` for versioned settings schema migrations.
- **FileMetaService**: Added `FileMetaService` (332 lines) for unified file metadata extraction (icon, type name, size formatting).
- **AppDiagnosticsService**: Added `AppDiagnosticsService` for runtime diagnostics including handle counts, memory pressure, and UI thread responsiveness.
- **User guide**: Added a 10-chapter user guide under `docs/user-guide/` covering getting started, file widgets, todo, quick capture, capsule mode, stacks & QuickLook, appearance, backup, advanced workflows, and troubleshooting.

#### Fixes

- **QuickLook compatibility (critical)**: Fixed a critical issue where Folio could crash QuickLook's single-threaded named-pipe server by connecting without sending data (pipe probe in `CanPreview` and raw `CreateFile` fallback). Availability checks now use process enumeration only; the pipe is touched exclusively when sending a Toggle message.
- **Wallpaper loss**: Prevented desktop wallpaper loss caused by repeated `WorkerW` window spawns during widget layer operations in `WidgetLayerService`.
- **Capsule mode defaults**: Aligned `AppSettings` initial values with `ApplyDefaultPreferences` so new installs and global reset produce identical capsule behavior.
- **Tray menu height**: Fixed tray right-click menu height display issue.
- **Dead shortcut cleanup**: Added detection and removal of invalid `.lnk` and `.url` shortcuts (e.g. uninstalled Steam games) that previously showed broken icons in file widgets.
- **Weather code mapping**: Expanded `WeatherCodeMapper` with additional WMO weather codes for more accurate condition descriptions.

## 1.3.1 - 2026-07-20

- **High-DPI icon clarity**: Replaced 32×32 `SHGFI_LARGEICON` extraction with `SHGetImageList` Jumbo (256×256) and Extra Large (48×48) fallback, plus `SHDefExtractIcon` for indexed icons. File and shortcut icons now match desktop icon clarity on high-DPI displays.
- **Steam shortcut icons**: Resolved `.url` shortcut icons not displaying by parsing comma-separated icon indices (e.g. `steam.exe,0`), searching for Steam via registry, and falling back to common install locations and system PATH.
- **Animation performance**: Replaced CPU-driven opacity/scale animations with GPU Composition KeyFrame animations. Added `DWMWA_TRANSITIONS_FORCEDISABLED` to prevent DWM interference. Replaced `AppWindow.Move` with direct `SetWindowPos` P/Invoke for lower latency.
- **Black screen transition fix**: Removed backdrop suppression during tray show/hide animations so Mica/Acrylic material remains consistent throughout the animation cycle.
- **Music artwork corner radius**: Fixed asymmetric corner radius on music widget album art in both empty and populated states by dynamically computing `CornerRadius` from widget settings.
- **Duplicate startup entries**: Unified startup registration to a single registry `Run` key. Removed redundant startup shortcut creation from the installer and drag-drop permission service. Added automatic cleanup of legacy startup shortcuts.
- **Capsule mode defaults**: New installs and global reset now default to hover auto-expand, sensitive hover response, and relaxed expand/collapse animation for a smoother first-run experience.
- **Freeze diagnostics**: Added a UI thread watchdog that detects and logs unresponsive periods with handle counts and GC generation. Fixed a sync-over-async deadlock in `StoreStartupService.RequestEnableAsync`.
- **Manual sort improvement**: Any drag-and-drop action in a file widget now automatically switches to manual sort mode. Subsequent sort mode clicks revert to automatic sorting. Removed the explicit "Manual Sort" menu option.

## 1.3.0 - 2026-07-18

- **Capsule mode**: Widgets can now collapse into compact desktop surfaces and expand by click or title-area hover. Compact content supports smart highlights, summaries or icon-and-title only, with an optional privacy mode for Todo and Quick Capture text.
- **Stable compact geometry**: Added aligned and independent width modes, anchored expansion toward the available screen area, separate compact and expanded bounds, resize guides, and per-widget overrides. Dragging content over a compact widget temporarily expands it without permanently moving or resizing either state.
- **Combined capsule layouts**: Compact widgets can remain independently positioned or form a movable combined bar. The bar supports user-defined order, floating or edge placement, automatic direction, adjustable spacing, and reliable restoration of free-layout positions.
- **Automatic file stacks**: File widgets can group related items by type or date without moving the underlying files. Custom extension rules support names, priority ordering, live match previews, thresholds, internal sorting and a configurable unmatched-file policy.
- **Stack interaction and QuickLook compatibility**: Stacks use an in-widget spread/collapse interaction with selection and path-copy commands. When QuickLook is already running, pressing Space on a selected file forwards the preview request without adding settings, startup work or a hard dependency.
- **Todo and Quick Capture workflows**: Added multiple file attachments with link-or-copy storage, attachment-aware clipboard formatting, configurable tab visibility, drag-to-tab actions, new Todo views for Active, This week and This month, adjustable list preview lines, and configurable Enter/Ctrl+Enter save behavior.
- **Appearance and responsive content**: Added Mica Alt and Base acrylic, material intensity, neutral/accent/hidden border colors, compact/standard/relaxed/custom density presets, and forced Cover or Controls layouts for Music. Solid material now remains fully opaque.
- **Windows-style Settings redesign**: Reorganized crowded pages into focused detail pages, moved global search into the title area, improved search matching and result navigation, surfaced important choices directly on entry cards, and made per-page hierarchy and reset behavior more consistent.
- **Backup, restore and attachment health**: Added integrity-checked ZIP export/restore, automatic and pre-restore snapshots, staged restart-safe restore, resilient JSON recovery, and attachment health scans for missing linked files, missing managed files and orphaned managed attachments.
- **Window and animation reliability**: Refined show/hide transitions, detail-page transitions, title-bar collapse actions, hover hit regions, Z-order, multi-monitor bounds restoration, resize alignment and tray menu sizing. Rapid capsule and stack interactions now use guarded state transitions to reduce flicker and stuck intermediate states.
- **Installer upgrades**: The installer now closes a running Folio process reliably before replacing application files, avoiding the intermittent Retry / Ignore / Cancel prompt during overwrite installs.
- **Maintainability and tests**: Split the largest window, widget, settings, Todo, Quick Capture, Music and Weather classes into focused modules. Expanded regression coverage across attachments, backup safety, settings migration/search, compact bounds and privacy, stacks, animation and positioning.

## 1.2.9 - 2026-07-13

- **Todo experience rebuilt**: Replaced the dense single-page workflow with a card-based task list and a dedicated full-widget detail view. The detail title is a resizable multiline field, metadata actions use a compact horizontal toolbar, and task cards display up to ten lines.
- **Todo scheduling and organization**: Added eight color markers and filters, due date, reminder, recurrence and attachment workflows, recurring-task history handling, and context-menu shortcuts. Reminder and recurrence stay disabled until a due date exists.
- **Todo selection and batch actions**: Added box selection and batch copy, delete, color, reminder, due-date and recurrence commands. Removed native ListView selection residue after opening or returning from details, including the add-task card focus state.
- **Quick Capture redesign**: Reworked notes into a spacious card list and body-only full-widget editor. Added paper/material styles, image add and replace actions, thumbnails, image-specific copy controls, pin state feedback, and optional creation-time display.
- **Quick Capture sorting and batch actions**: Added animated drag sorting for regular and pinned notes while preserving drag-out copy behavior. Added batch copy, delete, pin and paper commands, and fixed stale image previews after returning from details.
- **Responsive widgets**: Reduced the minimum widget size from 200 x 200 to 150 x 150. Refined Weather layouts for small and medium sizes. Music now switches to an artwork-first layout below 180 px, keeps controls and the title readable, centers artwork cropping, reuses title marquee behavior, and applies a slower nonblank artwork transition.
- **Music simplification**: Removed all spectrum/rhythm visuals, settings, ViewModel state and related code. Unified solid playback icons, button sizing and rounded-rectangle hover surfaces across minimal and medium layouts.
- **File and context-menu improvements**: Simplified the native-style vertical menu order, kept Open and Extract text from image, renamed the Explorer command to Open file location, removed Show more options, and placed Delete last. Added file multi-selection actions and ensured commands close their flyouts immediately.
- **Widget title editing**: Rename commands now enter a focused editor immediately. Double-clicking any widget title starts editing, while clicking the remaining title area commits and exits.
- **Dynamic Z-order fixes**: Reworked tray and hotkey activation so all widgets raise together without a one-second flash, repeated activation hides them consistently, and subsequent foreground changes are managed naturally by Windows. Clicking a title raises all widgets; clicking content raises the current widget.
- **Settings and reset behavior**: Global reset now preserves language, startup and feature-widget switches, resets Quick Capture creation-time visibility and resize snapping correctly, clears per-widget title-style overrides, and uses confirmation dialogs for destructive feature-widget resets. Feature-widget menu wording now represents enable/disable rather than deletion.
- **Default settings updated**: New installs and global reset use Comfortable density, Mica material, Thin border, Round corners, segmented Todo and Quick Capture tabs, and hidden Todo footer counts.
- **Performance and memory**: Reduced unrelated Todo refreshes caused by organizer history writes, suppressed redundant settings broadcasts for file operations, tightened cancellation-token and event cleanup, removed music spectrum allocation paths, and improved repeated language/material/theme change behavior.
- **Reliability fixes**: Fixed Quick Capture clearing removing the add-note entry, image-plus-text copy behavior, created-time visibility, image persistence between list and detail views, completed Todo checkmark contrast, overlapping Todo cards, feature-widget reset completeness, and several tray/hotkey layer edge cases.
- **Tests**: Expanded coverage for Todo recurrence, persistence and batch changes; Quick Capture image, ordering and reset behavior; settings defaults; organizer notification suppression; responsive Music layout; and Weather layout thresholds.

## 1.2.8 - 2026-07-11

- **Weather widget**: Added a full-featured weather widget with four layout modes (Mini, Compact, Standard, Detailed), city search with offline `cities.json` database and Windows Location API, auto-location, hourly and weekly forecasts, sunrise/sunset times, UV index, precipitation probability, humidity, wind speed, atmospheric pressure, rich skin backgrounds, and configurable refresh interval. Standard view activates at 250×215 and above. Horizontal scrollbar and mouse wheel support for the hourly temperature list. Week forecast fills remaining space and is scrollable even on small widgets. Loading animation with rotating refresh icon and content hiding during refresh.
- **Resize snap guide**: Added `ResizeGuideOverlayService` that detects edge alignment (8px threshold) with other widgets and work area boundaries during widget resize, showing accent-colored highlight bars on matched edges. Integrated into `WidgetWindow`, `QuickCaptureWidgetWindow`, and `ContentWidgetWindow`.
- **Quick Capture fixes**: Widget reset now properly clears all internal data via `QuickCaptureService.ClearAsync()`. Fixed blank display on first tab switch — data now appears immediately instead of requiring multiple switches. Added `ScheduleTransitionSafetyFallback` to `PlayItemsViewTransition` for more reliable content transitions.
- **Settings defaults unified**: Default appearance changed to Mica material, Medium border, Round corners — consistent across new installs, reset-to-defaults, and `AppSettings` initial values. Global reset (`ApplyDefaultPreferences`) now restores `CustomAccentColor` to `#0078D4` and `FocusClickedWidgetOnRaise` to `false`. Todo widget single-widget reset now restores `TodoReminderEnabled` and `TodoDefaultReminderOffsetMinutes`. Weather widget single-widget reset now clears `WeatherLatitude` and `WeatherLongitude` to prevent inconsistent location behavior.
- **Widget title font size**: Content widget titles now dynamically compute `TitleTextSize` and `TitleIconSize` from user settings (`TextSize`/`IconSize`) instead of using fixed values. Added `RefreshMetrics()` method to update title metrics when settings change.
- **Memory leak fix**: `TodoWidgetViewModel` now implements `IDisposable`, and `TodoWidgetContentAdapter` properly disposes the ViewModel on widget disposal, preventing memory leaks.
- **Delete widget button**: Added delete widget option to the "more" menu for all widget types, not just file widgets. Fixed `ContentWidgetWindow` more flyout placement.
- **Settings UI expansion**: Added standalone material type selector (Mica / Acrylic / Solid) and border style selector (None / Thin / Medium) to the Appearance settings page.
- **Widget layer improvement**: Added `SetWindowToDesktopLevel` method in `WidgetLayerService` that pushes windows to desktop level without using `HWND_BOTTOM`, preventing widgets from being hidden by Win+D while staying at desktop level.
- **Localization simplification**: Shortened drag-and-drop diagnostics text across both Chinese and English for clearer, more concise messaging.
- **FolderWatcherService optimization**: Replaced `CancellationTokenSource`-based debounce with `DispatcherQueueTimer`, reducing thread-pool task creation for file system change notifications.
- **Code refactoring — WidgetWindowBase**: Extracted a new `WidgetWindowBase.cs` (1027 lines) consolidating window setup, backdrop management, layer/Z-order control, drag/resize logic, and display-change restoration. `ContentWidgetWindow` and `QuickCaptureWidgetWindow` now inherit from this shared base, eliminating duplicated window management code.
- **Code refactoring — WidgetWindow**: Split `WidgetWindow.xaml.cs` (~5000 lines) into 6 partial classes: `Rename` (335 lines), `Menus` (677), `ItemSurface` (364), `Clipboard` (249), `Selection` (397), `DragDrop` (806). Main file reduced to 2828 lines.
- **Code refactoring — WidgetManager**: Split `WidgetManager.cs` (3532 lines) into 4 partial classes: `WidgetManager.TrayAnimation.cs` (353 lines, tray show/hide animation), `WidgetManager.ZOrder.cs` (376 lines, Z-order/layer management/mouse hook), `WidgetManager.Storage.cs` (595 lines, managed storage/folder mapping/orphan cleanup), `WidgetManager.FeatureWidgets.cs` (889 lines, Music/Weather/Todo/QuickCapture feature widgets). Core file reduced to 1383 lines.
- **Log rotation**: Added `TryRotateLogFileIfNeeded` with a 5MB size threshold — automatically backs up to `.bak` and cleans up old logs.
- **Atomic settings writes**: `SettingsService.SaveToFileOnlyAsync` now writes to a `.tmp` file first, then moves it to the final path, preventing configuration corruption from interrupted writes.
- **Onboarding redesign**: Simplified onboarding animation API and refined the first-run experience.

## 1.2.7 - 2026-07-09

- Replaced the fixed 16ms `DispatcherQueueTimer` in widget tray animation with `CompositionTarget.Rendering`, which is driven by the system VSync. Animations now run at the native display refresh rate — 60fps on 60Hz screens, 144fps on 144Hz screens — eliminating visible stutter on high-refresh-rate displays.
- Replaced the triple `Task.Delay` backdrop refresh pattern with a single staged `DispatcherQueueTimer` (80ms → 240ms → 580ms), reducing thread-pool scheduling overhead and keeping all refresh work on the UI thread.
- Removed an empty `PointerMoved` handler from widget item surfaces and added state guards to `PointerEntered`/`PointerExited` so hover surface updates are skipped while the window is closing, animating, or hidden.
- Stopped the music widget's progress, visualizer, and transition timers when the widget is deactivated, and restarted them on activation, eliminating unnecessary CPU usage when the widget is not visible.
- Added a 200ms debounce to `UISettings.ColorValuesChanged` in `ThemeService` so system accent color and theme changes no longer trigger redundant `RefreshAppearance` calls.
- Reduced the icon cache limit from 500 to 200 entries and set `DecodePixelWidth` before `SetSourceAsync` for both icons (48px) and image thumbnails (80px), lowering memory usage from decoded bitmaps.

## 1.2.6 - 2026-07-08

- Improved Todo reminder reliability with per-task reminder offsets, snooze state persistence after app restart, and safer reminder grace handling for overdue tasks.
- Added native notification actions for Todo reminders, including completing a task directly from the notification and choosing snooze options from the reminder surface.
- Refined recurring Todo behavior so completed recurrence history can be folded under the active recurring task instead of filling the main list with repeated completed rows.
- Improved Todo and Quick Capture multi-select workflows, including rectangle selection from blank list space, formatted copy, drag-out text packages, and Escape-to-clear behavior.
- Improved Quick Capture tab switching responsiveness by delaying heavier list refresh work, reducing UI-thread churn, stabilizing empty-state layout, and using a softer content transition.
- Updated Quick Capture clipboard defaults so automatic clipboard capture starts disabled by default, and disabling Quick Capture also disables automatic clipboard and image capture.
- Expanded automated coverage for Todo reminders, recurring tasks, Quick Capture clipboard settings, multi-select behavior, and settings defaults.

## 1.2.5 - 2026-07-08

- Improved the Todo widget with due times that keep hour/minute/second precision, native Windows reminder notifications, overdue suffix labels, completed-item sorting, click-to-copy, multi-select copying, and formatted clipboard output.
- Added Todo and Quick Capture drag/drop conversion so text can be moved between the two feature widgets more naturally.
- Added configurable Todo and Quick Capture tab styles, allowing each widget to use either the indicator-style tab bar or the segmented-button style.
- Added configurable top-right widget hover actions under Appearance -> Window appearance details. Users can choose 1 to 3 actions from lock position, lock size, add, more, and delete.
- Refined widget action icons with Fluent icon shapes, better lock-state icons, compact-title sizing, and clearer rendering. Resetting title styles now clears per-widget overrides so widgets follow the global title style again.
- Improved Music widget details, including a horizontal system-volume flyout, light-theme styling fixes, click-away closing, wider slider behavior, and album-ambience corner alignment with the widget corner setting.
- Added an optional file path tooltip switch under File widgets -> File display.
- Improved Direct and Microsoft Store update-channel behavior and wording, including Store-aware manual checks and clearer Direct fallback guidance.
- Expanded tests for Todo reminders, clipboard formatting, settings defaults, update behavior, and title-style reset behavior.

## 1.2.4 - 2026-07-06

- Fixed the in-app update installation handoff after an update has been downloaded.
- Runs `Folio.Updater.exe` from a detached local update-helper directory before starting the installer, so the installer can safely overwrite the Folio install directory.
- Updated installer packaging so old versions can update without the running updater locking `Folio.Updater.*`.

## 1.2.3 - 2026-07-06

- Added a configurable desktop widget layer mode under Settings -> General. Users can keep the existing Dynamic behavior or switch to Desktop pinned mode.
- Improved Desktop pinned behavior so visible widgets are reattached to the desktop layer after Show Desktop / Win+D and display-topology refreshes.
- Updated Settings navigation icons to Fluent color icons and refined several Settings layouts for more consistent spacing and card hierarchy.
- Removed redundant ToggleSwitch on/off text while preserving the native WinUI switch visual style.
- Removed extra decorative icons from the widget title icon setting and update-status rows.

## 1.2.2 - 2026-07-06

- Upgraded the Direct/Inno build baseline to .NET 10 and Windows App SDK 2.2.
- Updated installer runtime dependency detection to .NET 10 Runtime x64 and Windows App Runtime 2.2 x64.

## 1.2.1 - 2026-07-05

- Improved monitor-aware widget positioning for display topology changes, DPI changes, external display plug/unplug, and 1080p-to-4K display swaps. Visible widgets now restore against the current monitor work area, while hidden widgets are rechecked before being shown.
- Added configurable desktop widget title icons with color, filled mono, line mono, hidden, and localized text-label modes. Color icons are now the default for both new installs and restored default preferences.
- Added title-style selection to the file widget blank-area context menu, matching the existing title-bar menu behavior.
- Refined Settings organization by moving the widget title icon preference under Appearance -> Window appearance details.
- Aligned new-user defaults, reset defaults, and invalid-value fallbacks for animation and title-icon preferences. The default widget animation is now consistently `SlideFade`.
- Moved Quick Capture default-view normalization into the Quick Capture settings normalization path.
- Expanded automated coverage for settings defaults, widget title icon defaults, Quick Capture default-view normalization, and monitor-aware widget positioning.

## 1.2.0 - 2026-07-02

- Changed the project license from MIT to GPL-3.0-only for future source code and releases. Previously published MIT-licensed Folio versions remain under the MIT License.
- Completed the first large widget architecture refactor after 1.1.10: widgets now share a `WidgetShell`, content host, content factory, registry, session manager, window factory, and diagnostic path instead of keeping each widget type as a separate window implementation.
- Introduced the feature-widget foundation used by Todo, Quick Capture, Music, and future content widgets, including content providers, persisted widget kinds, lifecycle handling, positioning, z-order/session behavior, and settings integration.
- Added the Todo widget as a first-class desktop widget with local storage, task completion, filtering, inline editing, full-screen editing, custom due times, and coverage for store/view-model/content-adapter behavior.
- Added the Music widget with Windows media session integration, playback controls, playback mode switching, system volume control, responsive waveform styles, compact-card layout, long-title marquee behavior, and optional album-color ambience.
- Reworked Quick Capture on top of the newer content/widget infrastructure, with more consistent input and editing surfaces, safer recent-content refresh behavior, cached thumbnails for recent image previews, and reduced duplicate preview loading.
- Unified widget chrome and editing details across file and feature widgets: title bar metrics, title styles, inline editors, full-screen editor surfaces, hover/pressed states, empty states, tooltips, action buttons, segmented controls, and light/dark icon behavior.
- Reorganized Settings around the new architecture: feature-widget controls, appearance groups, file-widget display options, interaction/global hotkey controls, music rhythm options, Quick Capture preferences, and clearer localized labels.
- Improved managed-storage and widget lifecycle maintenance, including safer cleanup/restore paths, default managed storage handling, session persistence, and diagnostics around widget windows.
- Expanded automated coverage for the refactor with tests for content factories, widget registry/session/positioning, content window factory, Todo storage/view models, chrome mode resolution, feature-widget settings, storage cleanup, and Quick Capture thumbnail behavior.
- Updated release metadata, installer versioning, documentation, and dependency notes for the 1.2.0 build. The installer continues to check for .NET 8 Runtime x64 and Windows App Runtime 2.1.3 x64.

## 1.1.10 - 2026-06-29

- Fixed Quick Capture recent clipboard monitoring after restart: clipboard event listening now initializes on the UI thread and `Refresh()` safely marshals back to the UI dispatcher when needed.
- Fixed Quick Capture list scrolling when recent content grows beyond the widget height by constraining the list area to the remaining widget space.
- Added the phase-1 widget architecture refactoring plan to document the next stable refactor path before starting the architecture work.

## 1.1.9 - 2026-06-29

- Fixed clipboard monitoring not persisting after restart: `QuickCaptureEnabled` now defaults to `true`.
- Fixed widget z-order: widgets now follow natural Windows z-order instead of being pushed to bottom. Added 2s safety auto-restore timer.
- Fixed QuickCapture z-order consistency with WidgetWindow: both now use `ClearTopMostOnly`, `BringAllVisibleWidgetsToFront`, and 300ms deactivation guard.
- Improved QuickCapture UI: Sticky Notes style input, search moved to tab bar, expand button for full-screen editing, input only visible on Records tab.
- Fixed toggle switch text localization: deferred to `SettingsRoot.Loaded` for proper control resolution.
- Fixed tray menu font: explicit `FontFamily` fallback when `DefaultMenuFlyoutItemStyle` not found.
- Fixed widget animation: removed scale effect from SlideFade and ScaleSlide effects for pure slide-in.
- Fixed global hotkey (F7) not working after packaged install.
- Fixed delete widget crash: added `_isClosing` guard to `ApplyBackdropPreference`.
- Fixed hotkey toggle: `ShouldHideWidgetsForTrayToggle` now hides when widgets are visible.

## 1.1.8 - 2026-06-29

- Fixed app not launching and tray menu losing WinUI styling on other computers: caused by `EnableMsixTooling` being disabled. Restored to `true` for proper WinUI resource resolution.
- Fixed global hotkey (F7) not working: `OnLaunched` was called multiple times, creating a new `GlobalHotkeyService` that overwrote the already-attached instance. Now reuses existing instance and adds late-attach fallback.
- Fixed toggle switch style consistency: all toggles now use `SettingToggleSwitchStyle`.
- Fixed settings window staying on top when opened from tray right-click.

## 1.1.6 - 2026-06-29

- Fixed settings window staying on top when opened from tray right-click menu. Removed always-on-top logic and let Windows handle z-order naturally.
- Improved widget z-order reliability: reduced safety auto-restore timer from 5s to 2s.

## 1.1.5 - 2026-06-29

- Fixed clipboard monitoring not persisting after restart: changed QuickCaptureEnabled default to true so clipboard monitoring starts automatically on first install.
- Fixed widget z-order issue where widgets could get stuck on top after interaction: added 5-second safety timer that auto-restores widgets to desktop layer if they remain topmost without user interaction.
- Improved toggle switch style consistency across all settings: removed visual padding and aligned all toggles to the right edge.
- Simplified corner radius options: removed "System default" option, keeping only Small radius, Round, and Square corners. Default remains Small radius.

## 1.1.4 - 2026-06-28

- Fixed widget z-order issue where widgets would stay above fullscreen browser after batch raise. Widgets now properly hide behind other windows when clicking outside.
- Optimized startup initialization: theme refresh and clipboard service now initialize in parallel, reducing launch time by 2-3 seconds.
- Added error handling to critical async event handlers (file drop, drag completion) to prevent unhandled exceptions from crashing the app.
- Added `SafeFireAndForget` helper method for safe async execution in event handlers.

## 1.1.3 - 2026-06-27

- Optimized widget animation performance: replaced per-frame Win32 P/Invoke opacity calls with GPU-accelerated Visual.Opacity, cached Composition Visual, and enabled Windows-native cubic bezier easing curves.
- Simplified animation settings: removed redundant direction-specific effects, added a single "Slide direction" dropdown and "Easing intensity" control with None/Light/Standard/Strong options. Direction dropdown is disabled for effects that have no slide component.
- Fixed animation effect inconsistency between file widgets and Quick Capture widgets: both now support the same set of effects with identical parameters.
- Added image thumbnail previews for image files in widgets instead of generic file type icons.
- Fixed single-click file open not working when "Double-click to open" is disabled.
- Fixed right-click triggering single-click open instead of showing the context menu.
- Fixed widget click events being consumed by box selection logic, preventing ItemClick from firing.
- Removed "Focus clicked widget only" setting due to unreliable z-order behavior across different widget types. All widgets now always show and hide together.
- Improved default settings: animation effect defaults to Fade, speed defaults to Standard.

## 1.1.2 - 2026-06-26

- Optimized Quick Capture tab switching performance: added equality guards to item view model updates, replaced O(n²) collection diffing with dictionary-based O(n) lookup, and cached tab/item action button brushes to eliminate per-switch allocations.
- Added a new setting "Focus clicked widget only" for batch widget raise behavior. When enabled, clicking one widget hides all others; when disabled (default), all widgets stay visible together.
- Fixed Z-order inconsistency during batch raise from tray: widgets no longer fall behind fullscreen applications when clicking one widget, and the previously-clicked widget no longer stays on top unexpectedly.
- Unified Z-order behavior between file widgets and Quick Capture widgets to prevent asymmetric deactivation handling.

## 1.1.1 - 2026-06-26

- Fixed internal dragging for shortcut files (`.lnk`) in managed widgets. Folio now keeps its own path-based drag metadata even when Windows cannot convert a shortcut into a `StorageItem`.

## 1.1.0 - 2026-06-26

- Added drag-and-drop diagnostics in Settings with one-click repair for Folio compatibility flags, startup entries, and shortcuts. If Windows 10/11 cannot drag files into widgets, run this repair first.
- Improved Explorer drag/drop compatibility for managed and mapped widgets, including native shell message allowance, legacy shell format fallback, and more useful drop diagnostics.
- Fixed widget sorting stability with natural name ordering, deterministic tie-breakers, and correct insertion when new files are added while a sort mode is active.
- Improved Quick Capture text editing: saved text now opens the inline editor on double-click, while the context menu can edit text in Notepad and sync changes back.
- Changed the default tray icon style to colorful for new installs and restored defaults.
- Improved first-run onboarding so it is marked complete after the first install launch and no longer reappears just because widgets are empty.
- Improved installer and uninstall behavior: current-user install remains the default, the install folder can be changed, startup can be selected during setup, and uninstall can optionally keep or remove local Folio app data.

## 1.0.9 - 2026-06-25

- Reworked Settings into a cleaner Windows-style structure with fewer top-level categories, native ComboBox/NumberBox controls, toggle on/off labels, drill-in rows, and a clearer Quick Capture settings entry.
- Added widget item sorting by name, size, item type, and date modified, with per-widget persistence and repeat-click ascending/descending behavior.
- Improved widget menus by separating title-bar widget management from content-area file actions, including view switching, sorting, paste, refresh, and mapped-folder actions.
- Improved Quick Capture tabs, title buttons, hover actions, copy feedback, and shared show/restore behavior with regular widgets.
- Improved drag/drop compatibility, empty-widget drop handling, managed-vs-mapped drag captions, z-order restoration, icon hydration retries, and Chinese IME support during file/folder rename.
- Changed the installer to current-user installation by default and added automatic migration from older Program Files administrator installs to reduce Explorer drag/drop permission conflicts.
- Added clearer guidance for Explorer drag/drop failures: Folio should not be run as administrator, because Windows can block file drops from non-elevated Explorer windows into elevated Folio windows.
- Refined first-run onboarding with shorter Windows-style copy and simpler setup choices.

## 1.0.8 - 2026-06-24

- Improved Windows 11 23H2 drag/drop compatibility by launching Folio after install as the original user instead of inheriting the installer elevation level.
- Improved Explorer drag/drop handling for file widgets by accepting link-style requested operations when the widget can safely resolve them into the configured managed action.
- Improved drag hover captions so managed storage widgets show "managed widget" and mapped-folder widgets show "mapped folder" as distinct targets.
- Improved Quick Capture copy feedback by replacing per-row copy bubbles with a stable bottom-centered toast.
- Improved Quick Capture clipboard writes with short automatic retries when Windows temporarily locks the clipboard, reducing first-click copy failures.
- Tightened Quick Capture title button sizing and hover action styling so More, Delete, and item actions match the regular widget controls more closely.
- Fixed Quick Capture click-to-copy feedback, copy failure messaging, and several post-1.0.7 polish issues around mapped widgets and drag prompts.

## 1.0.7 - 2026-06-23

- Improved tray and global-hotkey behavior so file widgets and Quick Capture are raised, hidden, and restored as one group.
- Added a light WidgetManager restore path that keeps Folio widgets together after menu interactions and restores the group only after the user moves back to another app.
- Improved full-screen app behavior: F7 can raise widgets again when they are visible but covered, and a keyboard-hook fallback prevents apps such as Axure from consuming the configured hotkey first.
- Improved widget show/hide animation with linear timing, shorter default duration, and group-aware off-screen slide distances so adjacent widgets move out consistently.
- Improved Quick Capture layout, hover actions, tab switching, copy/open behavior, image previews, and inline editing for long text.
- Added system-open behavior for Quick Capture items: single click copies with feedback, double click opens text, links, or images in the user's default app.
- Added orphan managed-storage folder management so removed widget folders can be restored, opened, moved back to Desktop, or deleted from Settings.
- Improved managed and mapped widget safety with duplicate-name guards, folder recovery handling, mapped-folder rename sync, icon refresh stability, and file-name display fixes.
- Improved performance and responsiveness around directory refresh, clipboard capture, tab switching, list rendering, and temporary topmost confirmation.

## 1.0.6 - 2026-06-21

- Added Quick Capture as an optional feature widget for local text, link, screenshot, and recent clipboard capture workflows.
- Added Quick Capture Records, Pinned, and Recent views with hover actions, compact search, drag-out support, image thumbnails, and save-to-file-widget actions.
- Added upload-friendly storage access: managed storage can be pinned to Quick Access, opened from the tray, and mirrored with folder shortcuts for file pickers.
- Improved drag/drop and clipboard behavior so file drags stay file-first, path copying is explicit, and Folio's own clipboard writes are ignored by Recent capture.
- Improved file widgets with custom Explorer icon refresh, filename extension display controls, shortcut-arrow settings placement, and clearer migration progress/result feedback.
- Improved tray/global-hotkey layering so widgets stay temporarily raised until the user clicks another app, and Settings can join the temporary topmost layer when opened during that state.
- Improved Quick Capture polish with scoped-search messaging, target-widget refresh/highlight after saving, compact edit dialogs, tighter tab/action layout, and theme-aligned styling.
- Improved first-run onboarding scaling for high-DPI setups and fixed several small layout, acrylic, and refresh edge cases.

## 1.0.5 - 2026-06-18

- Rebuilt first-run onboarding with a Folio logo intro, a five-step guide, looping right-side feature scenes, and Chinese, English, light-mode, and dark-mode support.
- Added an optional global hotkey that triggers the same show, hide, and temporary-raise flow as the tray left-click action.
- Improved Settings and tray access with managed-storage opening, Quick Access pinning, download-link actions, and maintenance controls.
- Improved storage and mapping workflows, including default storage migration, mapped shortcut sync, orphan managed-folder cleanup, and steadier drag/drop behavior.
- Removed remaining stale blur-toggle plumbing and release animation/window references more promptly after Settings or onboarding closes.

## 1.0.4 - 2026-06-16

- Improved tray left-click behavior so raised widgets stay on top while the pointer moves, then return to desktop level only after the user clicks another non-Folio window.
- Added follow-up topmost confirmation when raising multiple widgets from the tray so every visible widget is brought forward consistently.
- Improved tray right-click menu positioning by anchoring the WinUI menu from the actual tray icon rectangle and keeping it out of the tray icon hit area.
- Added automatic backdrop refresh retries after widget show, tray reveal, theme, and appearance changes to recover acrylic surfaces that occasionally render as flat gray.
- Reworked Settings into a left-side navigation layout with dedicated General, Appearance, Widget layout, Animation, Storage, Interaction, Maintenance, and About sections.

## 1.0.3 - 2026-06-16

- Added configurable widget show/hide animation effects and speed presets in Settings, with Chinese and English labels.
- Reworked tray animation execution to use smoother frame pacing, real window movement for slide effects, and composition-driven opacity/scale transitions.
- Reduced widget animation flicker by avoiding duplicate item transitions during mapped-folder reveal and by restoring the final visual state consistently.
- Improved tray left-click behavior so visible desktop-layer widgets hidden behind other apps are raised temporarily instead of being hidden immediately.
- Improved tray-launched Settings behavior so the Settings window opens temporarily on top and clears topmost state after focus leaves.
- Improved temporary foreground behavior for newly created widgets, widget dragging, and folder picker ownership.
- Added performance logging support and coverage for the performance logger.

## 1.0.2 - 2026-06-16

- Added Chinese and English localization across widgets, settings, tray menus, onboarding, dialogs, notes, empty states, and status messages.
- Added a language selector in Settings and refreshed localized text dynamically when the user changes languages.
- Reworked onboarding to expose important setup choices directly in the flow, including managed-drop behavior, the default storage path, folder mapping, and startup launch.
- Improved onboarding visuals, right-side step animations, and repeated scene playback so each step better matches the feature being introduced.
- Fixed startup-launch behavior so Folio starts silently to the tray after reboot instead of opening Settings.
- Improved tray behavior so right-click "Show all widgets" temporarily raises widgets just like left-clicking the tray icon.
- Improved widget show/hide animation with a unified right-to-left motion, removed per-widget cascade timing, and reduced mapped-widget flicker.
- Improved mapped-folder reveal behavior by suppressing duplicate item transitions during window animation.
- Improved light-mode styling in onboarding and settings, including text contrast, surface colors, and the active-step indicator shape.
- Improved widget selection behavior so selecting an item in one widget clears selections in other widgets.
- Improved drag-selection responsiveness and reduced repeated visual work during rectangle selection.
- Improved shortcut handling so broken `.lnk` files use the native Windows resolve/delete prompt when opened.
- Improved file operations around cut/copy, mapped folders, desktop drag-out refresh, and shell clipboard data.
- Updated README to Chinese by default, added an English README switch, and refreshed release documentation.

## 1.0.1 - 2026-06-12

- Added a Windows-native onboarding guide, with an entry in Settings for replaying it.
- Improved tray reveal behavior, widget show/hide animations, and temporary foreground behavior.
- Improved default settings, reset-to-defaults, live appearance preview, and display density controls.
- Improved widget file interactions including drag and drop, cut, rename, delete confirmation, and keyboard shortcuts.
- Fixed installer dependency detection for .NET 8 Runtime x64 and Windows App Runtime 2.1.3 x64.
- Improved installer shortcut icons and overwrite-install behavior while preserving user settings and managed files.

## 1.0.0 - 2026-06-11

- Initial public test release.

