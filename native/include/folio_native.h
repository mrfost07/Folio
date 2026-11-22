#ifndef FOLIO_NATIVE_H
#define FOLIO_NATIVE_H

#include <stdint.h>

#define FOLIO_NATIVE_ABI_VERSION 2u
#define FOLIO_EXPLORER_SHELL_LAUNCH_STRUCT_VERSION_1 1u
#define FOLIO_QUICK_ACCESS_STRUCT_VERSION_1 1u
#define FOLIO_RECYCLE_BIN_STRUCT_VERSION_1 1u
#define FOLIO_MUSIC_VOLUME_STRUCT_VERSION_1 1u
#define FOLIO_NATIVE_STRUCT_VERSION_2 2u
#define FOLIO_NATIVE_DLL_NAME L"folio_native.dll"

#define FOLIO_NATIVE_CAPABILITY_SHORTCUT_READ_STORED_RAW_V2 (1ull << 0)
#define FOLIO_NATIVE_CAPABILITY_SHORTCUT_READ_EFFECTIVE_DIAGNOSTIC_V2 (1ull << 1)
#define FOLIO_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_NO_UI_V2 (1ull << 2)
#define FOLIO_NATIVE_CAPABILITY_SHORTCUT_WRITE_V2 (1ull << 3)
#define FOLIO_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_WITH_UI_V2 (1ull << 4)
#define FOLIO_NATIVE_CAPABILITY_MUSIC_VOLUME_V1 (1ull << 5)
#define FOLIO_NATIVE_CAPABILITY_EXPLORER_SHELL_LAUNCH_V1 (1ull << 6)
#define FOLIO_NATIVE_CAPABILITY_QUICK_ACCESS_V1 (1ull << 7)
#define FOLIO_NATIVE_CAPABILITY_RECYCLE_BIN_V1 (1ull << 8)
#define FOLIO_NATIVE_CAPABILITIES_STAGE_3C2 \
    (FOLIO_NATIVE_CAPABILITY_SHORTCUT_READ_STORED_RAW_V2 | \
     FOLIO_NATIVE_CAPABILITY_SHORTCUT_READ_EFFECTIVE_DIAGNOSTIC_V2 | \
     FOLIO_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_NO_UI_V2 | \
     FOLIO_NATIVE_CAPABILITY_SHORTCUT_WRITE_V2 | \
     FOLIO_NATIVE_CAPABILITY_SHORTCUT_RESOLVE_WITH_UI_V2)
#define FOLIO_NATIVE_CAPABILITIES_STAGE_4C \
    (FOLIO_NATIVE_CAPABILITIES_STAGE_3C2 | \
     FOLIO_NATIVE_CAPABILITY_MUSIC_VOLUME_V1)
#define FOLIO_NATIVE_CAPABILITIES_STAGE_4D4A \
    (FOLIO_NATIVE_CAPABILITIES_STAGE_4C | \
     FOLIO_NATIVE_CAPABILITY_EXPLORER_SHELL_LAUNCH_V1)
#define FOLIO_NATIVE_CAPABILITIES_STAGE_4D4B \
    (FOLIO_NATIVE_CAPABILITIES_STAGE_4D4A | \
     FOLIO_NATIVE_CAPABILITY_QUICK_ACCESS_V1)
#define FOLIO_NATIVE_CAPABILITIES_STAGE_5B4C1B1 \
    (FOLIO_NATIVE_CAPABILITIES_STAGE_4D4B | \
     FOLIO_NATIVE_CAPABILITY_RECYCLE_BIN_V1)
#define FOLIO_NATIVE_CAPABILITIES FOLIO_NATIVE_CAPABILITIES_STAGE_5B4C1B1

#define FOLIO_NATIVE_STATUS_OK 0u
#define FOLIO_NATIVE_STATUS_INVALID_ARGUMENT 1u
#define FOLIO_NATIVE_STATUS_INCOMPATIBLE_STRUCT 2u
#define FOLIO_NATIVE_STATUS_BUFFER_TOO_SMALL 3u
#define FOLIO_NATIVE_STATUS_COM_INITIALIZATION_FAILED 4u
#define FOLIO_NATIVE_STATUS_OBJECT_CREATION_FAILED 5u
#define FOLIO_NATIVE_STATUS_LOAD_FAILED 6u
#define FOLIO_NATIVE_STATUS_OPERATION_FAILED 7u
#define FOLIO_NATIVE_STATUS_INTERNAL_ERROR 8u
#define FOLIO_NATIVE_STATUS_NOT_IMPLEMENTED 9u

#define FOLIO_NATIVE_S_OK ((int32_t)0x00000000u)
#define FOLIO_NATIVE_S_FALSE ((int32_t)0x00000001u)
#define FOLIO_NATIVE_E_NOTIMPL ((int32_t)0x80004001u)
#define FOLIO_NATIVE_E_INVALIDARG ((int32_t)0x80070057u)
#define FOLIO_NATIVE_HRESULT_INSUFFICIENT_BUFFER ((int32_t)0x8007007Au)
#define FOLIO_NATIVE_HRESULT_NOT_ATTEMPTED ((int32_t)0x8000000Au)

#define FOLIO_SHORTCUT_READ_MODE_STORED_RAW 1u
#define FOLIO_SHORTCUT_READ_MODE_EFFECTIVE_DIAGNOSTIC 2u
#define FOLIO_SHORTCUT_WRITE_FLAG_SHELL_NAMESPACE_TARGET (1u << 0)

#define FOLIO_SHORTCUT_FIELD_TARGET_PATH (1u << 0)
#define FOLIO_SHORTCUT_FIELD_DESCRIPTION (1u << 1)
#define FOLIO_SHORTCUT_FIELD_ARGUMENTS (1u << 2)
#define FOLIO_SHORTCUT_FIELD_WORKING_DIRECTORY (1u << 3)
#define FOLIO_SHORTCUT_FIELD_ICON_PATH (1u << 4)

#define FOLIO_SHORTCUT_PHASE_COM_INITIALIZE (1u << 0)
#define FOLIO_SHORTCUT_PHASE_CREATE_OBJECT (1u << 1)
#define FOLIO_SHORTCUT_PHASE_LOAD (1u << 2)
#define FOLIO_SHORTCUT_PHASE_RESOLVE (1u << 3)
#define FOLIO_SHORTCUT_PHASE_SAVE (1u << 4)

#define FOLIO_MUSIC_VOLUME_OPERATION_GET_SNAPSHOT 1u
#define FOLIO_MUSIC_VOLUME_OPERATION_GET_SYSTEM 2u
#define FOLIO_MUSIC_VOLUME_OPERATION_SET_SYSTEM 3u
#define FOLIO_MUSIC_VOLUME_OPERATION_SET_SESSION 4u

#define FOLIO_MUSIC_VOLUME_PHASE_COM_INITIALIZE (1u << 0)
#define FOLIO_MUSIC_VOLUME_PHASE_CREATE_ENUMERATOR (1u << 1)
#define FOLIO_MUSIC_VOLUME_PHASE_GET_DEVICE (1u << 2)
#define FOLIO_MUSIC_VOLUME_PHASE_SYSTEM_VOLUME (1u << 3)
#define FOLIO_MUSIC_VOLUME_PHASE_ENUMERATE_SESSIONS (1u << 4)
#define FOLIO_MUSIC_VOLUME_PHASE_SESSION_VOLUME (1u << 5)

#define FOLIO_MUSIC_VOLUME_MATCH_NONE 0u
#define FOLIO_MUSIC_VOLUME_MATCH_IDENTIFIER_APP_ID 1u
#define FOLIO_MUSIC_VOLUME_MATCH_INSTANCE_APP_ID 2u
#define FOLIO_MUSIC_VOLUME_MATCH_DISPLAY_NAME 3u
#define FOLIO_MUSIC_VOLUME_MATCH_PROCESS_DISPLAY_NAME 4u
#define FOLIO_MUSIC_VOLUME_MATCH_APP_ID_PROCESS 5u
#define FOLIO_MUSIC_VOLUME_MATCH_IDENTIFIER_DISPLAY_NAME 6u
#define FOLIO_MUSIC_VOLUME_MATCH_SINGLE_FALLBACK 7u

#define FOLIO_EXPLORER_SHELL_LAUNCH_PHASE_COM_INITIALIZE (1u << 0)
#define FOLIO_EXPLORER_SHELL_LAUNCH_PHASE_CREATE_OBJECT (1u << 1)
#define FOLIO_EXPLORER_SHELL_LAUNCH_PHASE_WINDOWS (1u << 2)
#define FOLIO_EXPLORER_SHELL_LAUNCH_PHASE_DESKTOP (1u << 3)
#define FOLIO_EXPLORER_SHELL_LAUNCH_PHASE_DOCUMENT (1u << 4)
#define FOLIO_EXPLORER_SHELL_LAUNCH_PHASE_APPLICATION (1u << 5)
#define FOLIO_EXPLORER_SHELL_LAUNCH_PHASE_EXECUTE (1u << 6)

#define FOLIO_QUICK_ACCESS_OPERATION_QUERY_PIN_STATE 1u
#define FOLIO_QUICK_ACCESS_OPERATION_PIN 2u
#define FOLIO_QUICK_ACCESS_OPERATION_UNPIN 3u

#define FOLIO_QUICK_ACCESS_PIN_STATE_UNKNOWN 0u
#define FOLIO_QUICK_ACCESS_PIN_STATE_NOT_PINNED 1u
#define FOLIO_QUICK_ACCESS_PIN_STATE_PINNED 2u

#define FOLIO_QUICK_ACCESS_PHASE_COM_INITIALIZE (1u << 0)
#define FOLIO_QUICK_ACCESS_PHASE_CREATE_OBJECT (1u << 1)
#define FOLIO_QUICK_ACCESS_PHASE_QUICK_NAMESPACE (1u << 2)
#define FOLIO_QUICK_ACCESS_PHASE_ITEMS (1u << 3)
#define FOLIO_QUICK_ACCESS_PHASE_ENUMERATE (1u << 4)
#define FOLIO_QUICK_ACCESS_PHASE_ITEM_PATH (1u << 5)
#define FOLIO_QUICK_ACCESS_PHASE_PROPERTY (1u << 6)
#define FOLIO_QUICK_ACCESS_PHASE_PARENT_NAMESPACE (1u << 7)
#define FOLIO_QUICK_ACCESS_PHASE_PARSE_NAME (1u << 8)
#define FOLIO_QUICK_ACCESS_PHASE_INVOKE (1u << 9)

#define FOLIO_RECYCLE_BIN_OPERATION_QUERY 1u
#define FOLIO_RECYCLE_BIN_OPERATION_RESTORE 2u

#define FOLIO_RECYCLE_BIN_PHASE_COM_INITIALIZE (1u << 0)
#define FOLIO_RECYCLE_BIN_PHASE_CREATE_OBJECT (1u << 1)
#define FOLIO_RECYCLE_BIN_PHASE_NAMESPACE (1u << 2)
#define FOLIO_RECYCLE_BIN_PHASE_ITEMS (1u << 3)
#define FOLIO_RECYCLE_BIN_PHASE_ENUMERATE (1u << 4)
#define FOLIO_RECYCLE_BIN_PHASE_ITEM_NAME (1u << 5)
#define FOLIO_RECYCLE_BIN_PHASE_PROPERTY (1u << 6)
#define FOLIO_RECYCLE_BIN_PHASE_INVOKE (1u << 7)

#define FOLIO_SHORTCUT_DEFAULT_RESOLVE_TIMEOUT_MS 3000u
#define FOLIO_SHORTCUT_MAX_RESOLVE_TIMEOUT_MS 65535u
#define FOLIO_SHORTCUT_MAX_INPUT_PATH_CHARS 32767u
#define FOLIO_SHORTCUT_MAX_INPUT_VALUE_CHARS 32767u
#define FOLIO_EXPLORER_SHELL_LAUNCH_MAX_INPUT_CHARS 32767u
#define FOLIO_QUICK_ACCESS_MAX_INPUT_CHARS 32767u
#define FOLIO_RECYCLE_BIN_MAX_INPUT_CHARS 32767u
// Deep review R17 (G2 audit): mirrors FOLIO_MUSIC_VOLUME_MAX_INPUT_CHARS
// in lib.rs and MusicVolumeNativeBackend — the header was the one of the
// three mirror sites missing it.
#define FOLIO_MUSIC_VOLUME_MAX_INPUT_CHARS 32767u

#define FOLIO_NATIVE_UTF16_BUFFER_V1_SIZE_64 16u
#define FOLIO_NATIVE_UTF16_STRING_V1_SIZE_64 16u
#define FOLIO_SHORTCUT_READ_REQUEST_V2_SIZE_64 144u
#define FOLIO_SHORTCUT_READ_RESULT_V2_SIZE_64 136u
#define FOLIO_SHORTCUT_RESOLVE_REQUEST_V2_SIZE_64 192u
#define FOLIO_SHORTCUT_WRITE_REQUEST_V2_SIZE_64 144u
#define FOLIO_SHORTCUT_WRITE_RESULT_V2_SIZE_64 96u
#define FOLIO_SHORTCUT_UI_RESOLVE_REQUEST_V2_SIZE_64 64u
#define FOLIO_SHORTCUT_UI_RESOLVE_RESULT_V2_SIZE_64 64u
#define FOLIO_MUSIC_VOLUME_REQUEST_V1_SIZE_64 88u
#define FOLIO_MUSIC_VOLUME_RESULT_V1_SIZE_64 104u
#define FOLIO_EXPLORER_SHELL_LAUNCH_REQUEST_V1_SIZE_64 96u
#define FOLIO_EXPLORER_SHELL_LAUNCH_RESULT_V1_SIZE_64 88u
#define FOLIO_QUICK_ACCESS_REQUEST_V1_SIZE_64 96u
#define FOLIO_QUICK_ACCESS_RESULT_V1_SIZE_64 112u
#define FOLIO_RECYCLE_BIN_REQUEST_V1_SIZE_64 80u
#define FOLIO_RECYCLE_BIN_RESULT_V1_SIZE_64 104u

#if defined(_WIN32)
#define FOLIO_NATIVE_API __declspec(dllimport)
#define FOLIO_NATIVE_CALL __cdecl
#else
#define FOLIO_NATIVE_API
#define FOLIO_NATIVE_CALL
#endif

typedef struct FolioNativeUtf16BufferV1 {
    uint16_t* data;
    uint32_t capacity_chars;
    uint32_t reserved0;
} FolioNativeUtf16BufferV1;

typedef struct FolioNativeUtf16StringV1 {
    const uint16_t* data;
    uint32_t length_chars;
    uint32_t reserved0;
} FolioNativeUtf16StringV1;

typedef struct FolioShortcutReadRequestV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t mode;
    uint32_t flags;
    const uint16_t* shortcut_path;
    uint32_t shortcut_path_length_chars;
    uint32_t reserved0;
    FolioNativeUtf16BufferV1 target_path;
    FolioNativeUtf16BufferV1 description;
    FolioNativeUtf16BufferV1 arguments;
    FolioNativeUtf16BufferV1 working_directory;
    FolioNativeUtf16BufferV1 icon_path;
    uint64_t reserved[4];
} FolioShortcutReadRequestV2;

typedef struct FolioShortcutReadResultV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t load_hresult;
    int32_t resolve_hresult;
    uint32_t attempted_fields;
    uint32_t succeeded_fields;
    uint32_t present_fields;
    uint32_t caller_buffer_too_small_fields;
    uint32_t source_truncated_fields;
    int32_t target_hresult;
    int32_t description_hresult;
    int32_t arguments_hresult;
    int32_t working_directory_hresult;
    int32_t icon_hresult;
    int32_t icon_index;
    uint32_t target_required_chars;
    uint32_t description_required_chars;
    uint32_t arguments_required_chars;
    uint32_t working_directory_required_chars;
    uint32_t icon_required_chars;
    uint32_t reserved0;
    uint64_t reserved[4];
} FolioShortcutReadResultV2;

typedef struct FolioShortcutResolveRequestV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t timeout_ms;
    uint32_t flags;
    FolioShortcutReadRequestV2 read_request;
    uint64_t reserved[4];
} FolioShortcutResolveRequestV2;

typedef struct FolioShortcutWriteRequestV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t flags;
    int32_t icon_index;
    FolioNativeUtf16StringV1 shortcut_path;
    FolioNativeUtf16StringV1 target_path;
    FolioNativeUtf16StringV1 description;
    FolioNativeUtf16StringV1 arguments;
    FolioNativeUtf16StringV1 working_directory;
    FolioNativeUtf16StringV1 icon_path;
    uint64_t reserved[4];
} FolioShortcutWriteRequestV2;

typedef struct FolioShortcutWriteResultV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t save_hresult;
    uint32_t attempted_fields;
    uint32_t succeeded_fields;
    int32_t target_hresult;
    int32_t description_hresult;
    int32_t arguments_hresult;
    int32_t working_directory_hresult;
    int32_t icon_hresult;
    uint32_t reserved0;
    uint64_t reserved[4];
} FolioShortcutWriteResultV2;

typedef struct FolioShortcutUiResolveRequestV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t flags;
    uint32_t reserved0;
    FolioNativeUtf16StringV1 shortcut_path;
    uint64_t owner_hwnd;
    uint64_t reserved[3];
} FolioShortcutUiResolveRequestV2;

typedef struct FolioShortcutUiResolveResultV2 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t load_hresult;
    int32_t resolve_hresult;
    uint32_t resolve_flags;
    uint64_t reserved[3];
} FolioShortcutUiResolveResultV2;

typedef struct FolioMusicVolumeRequestV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t operation;
    uint32_t flags;
    FolioNativeUtf16StringV1 source_app_user_model_id;
    FolioNativeUtf16StringV1 source_display_name;
    double volume;
    uint64_t reserved[4];
} FolioMusicVolumeRequestV1;

typedef struct FolioMusicVolumeResultV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    uint32_t match_kind;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t device_hresult;
    int32_t system_hresult;
    int32_t session_hresult;
    uint32_t has_session_volume;
    uint32_t operation_succeeded;
    uint32_t reserved0;
    double system_volume;
    double session_volume;
    uint64_t reserved[4];
} FolioMusicVolumeResultV1;

typedef struct FolioExplorerShellLaunchRequestV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t flags;
    uint32_t reserved0;
    FolioNativeUtf16StringV1 path;
    FolioNativeUtf16StringV1 working_directory;
    FolioNativeUtf16StringV1 verb;
    uint64_t reserved[4];
} FolioExplorerShellLaunchRequestV1;

typedef struct FolioExplorerShellLaunchResultV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t windows_hresult;
    int32_t desktop_hresult;
    int32_t document_hresult;
    int32_t application_hresult;
    int32_t execute_hresult;
    uint32_t operation_succeeded;
    uint32_t reserved0;
    uint64_t reserved[4];
} FolioExplorerShellLaunchResultV1;

typedef struct FolioQuickAccessRequestV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t operation;
    uint32_t flags;
    FolioNativeUtf16StringV1 folder_path;
    FolioNativeUtf16StringV1 parent_path;
    FolioNativeUtf16StringV1 folder_name;
    uint64_t reserved[4];
} FolioQuickAccessRequestV1;

typedef struct FolioQuickAccessResultV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t quick_namespace_hresult;
    int32_t items_hresult;
    int32_t enumerate_hresult;
    int32_t item_path_hresult;
    int32_t property_hresult;
    int32_t parent_namespace_hresult;
    int32_t parse_name_hresult;
    int32_t invoke_hresult;
    uint32_t pin_state;
    uint32_t operation_succeeded;
    uint32_t matched_item;
    uint32_t fallback_used;
    uint32_t reserved0;
    uint64_t reserved[4];
} FolioQuickAccessResultV1;

typedef struct FolioRecycleBinRequestV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t operation;
    uint32_t flags;
    FolioNativeUtf16StringV1 original_parent;
    FolioNativeUtf16StringV1 original_name;
    uint64_t reserved[4];
} FolioRecycleBinRequestV1;

typedef struct FolioRecycleBinResultV1 {
    uint32_t struct_size;
    uint32_t struct_version;
    uint32_t status;
    int32_t operation_hresult;
    uint32_t attempted_phases;
    int32_t com_hresult;
    int32_t create_hresult;
    int32_t namespace_hresult;
    int32_t items_hresult;
    int32_t enumerate_hresult;
    int32_t item_name_hresult;
    int32_t property_hresult;
    int32_t invoke_hresult;
    uint32_t matched_count;
    uint32_t restored_count;
    uint32_t operation_succeeded;
    uint32_t reserved0;
    uint32_t reserved1;
    uint64_t reserved[4];
} FolioRecycleBinResultV1;

#if UINTPTR_MAX == UINT64_MAX
#if defined(__cplusplus)
static_assert(sizeof(FolioNativeUtf16BufferV1) == FOLIO_NATIVE_UTF16_BUFFER_V1_SIZE_64,
              "FolioNativeUtf16BufferV1 ABI size changed");
static_assert(sizeof(FolioNativeUtf16StringV1) == FOLIO_NATIVE_UTF16_STRING_V1_SIZE_64,
              "FolioNativeUtf16StringV1 ABI size changed");
static_assert(sizeof(FolioShortcutReadRequestV2) == FOLIO_SHORTCUT_READ_REQUEST_V2_SIZE_64,
              "FolioShortcutReadRequestV2 ABI size changed");
static_assert(sizeof(FolioShortcutReadResultV2) == FOLIO_SHORTCUT_READ_RESULT_V2_SIZE_64,
              "FolioShortcutReadResultV2 ABI size changed");
static_assert(sizeof(FolioShortcutResolveRequestV2) == FOLIO_SHORTCUT_RESOLVE_REQUEST_V2_SIZE_64,
              "FolioShortcutResolveRequestV2 ABI size changed");
static_assert(sizeof(FolioShortcutWriteRequestV2) == FOLIO_SHORTCUT_WRITE_REQUEST_V2_SIZE_64,
              "FolioShortcutWriteRequestV2 ABI size changed");
static_assert(sizeof(FolioShortcutWriteResultV2) == FOLIO_SHORTCUT_WRITE_RESULT_V2_SIZE_64,
              "FolioShortcutWriteResultV2 ABI size changed");
static_assert(sizeof(FolioShortcutUiResolveRequestV2) == FOLIO_SHORTCUT_UI_RESOLVE_REQUEST_V2_SIZE_64,
              "FolioShortcutUiResolveRequestV2 ABI size changed");
static_assert(sizeof(FolioShortcutUiResolveResultV2) == FOLIO_SHORTCUT_UI_RESOLVE_RESULT_V2_SIZE_64,
              "FolioShortcutUiResolveResultV2 ABI size changed");
static_assert(sizeof(FolioMusicVolumeRequestV1) == FOLIO_MUSIC_VOLUME_REQUEST_V1_SIZE_64,
              "FolioMusicVolumeRequestV1 ABI size changed");
static_assert(sizeof(FolioMusicVolumeResultV1) == FOLIO_MUSIC_VOLUME_RESULT_V1_SIZE_64,
              "FolioMusicVolumeResultV1 ABI size changed");
static_assert(sizeof(FolioExplorerShellLaunchRequestV1) == FOLIO_EXPLORER_SHELL_LAUNCH_REQUEST_V1_SIZE_64,
              "FolioExplorerShellLaunchRequestV1 ABI size changed");
static_assert(sizeof(FolioExplorerShellLaunchResultV1) == FOLIO_EXPLORER_SHELL_LAUNCH_RESULT_V1_SIZE_64,
              "FolioExplorerShellLaunchResultV1 ABI size changed");
static_assert(sizeof(FolioQuickAccessRequestV1) == FOLIO_QUICK_ACCESS_REQUEST_V1_SIZE_64,
              "FolioQuickAccessRequestV1 ABI size changed");
static_assert(sizeof(FolioQuickAccessResultV1) == FOLIO_QUICK_ACCESS_RESULT_V1_SIZE_64,
              "FolioQuickAccessResultV1 ABI size changed");
static_assert(sizeof(FolioRecycleBinRequestV1) == FOLIO_RECYCLE_BIN_REQUEST_V1_SIZE_64,
              "FolioRecycleBinRequestV1 ABI size changed");
static_assert(sizeof(FolioRecycleBinResultV1) == FOLIO_RECYCLE_BIN_RESULT_V1_SIZE_64,
              "FolioRecycleBinResultV1 ABI size changed");
#elif defined(__STDC_VERSION__) && __STDC_VERSION__ >= 201112L
_Static_assert(sizeof(FolioNativeUtf16BufferV1) == FOLIO_NATIVE_UTF16_BUFFER_V1_SIZE_64,
               "FolioNativeUtf16BufferV1 ABI size changed");
_Static_assert(sizeof(FolioNativeUtf16StringV1) == FOLIO_NATIVE_UTF16_STRING_V1_SIZE_64,
               "FolioNativeUtf16StringV1 ABI size changed");
_Static_assert(sizeof(FolioShortcutReadRequestV2) == FOLIO_SHORTCUT_READ_REQUEST_V2_SIZE_64,
               "FolioShortcutReadRequestV2 ABI size changed");
_Static_assert(sizeof(FolioShortcutReadResultV2) == FOLIO_SHORTCUT_READ_RESULT_V2_SIZE_64,
               "FolioShortcutReadResultV2 ABI size changed");
_Static_assert(sizeof(FolioShortcutResolveRequestV2) == FOLIO_SHORTCUT_RESOLVE_REQUEST_V2_SIZE_64,
               "FolioShortcutResolveRequestV2 ABI size changed");
_Static_assert(sizeof(FolioShortcutWriteRequestV2) == FOLIO_SHORTCUT_WRITE_REQUEST_V2_SIZE_64,
               "FolioShortcutWriteRequestV2 ABI size changed");
_Static_assert(sizeof(FolioShortcutWriteResultV2) == FOLIO_SHORTCUT_WRITE_RESULT_V2_SIZE_64,
               "FolioShortcutWriteResultV2 ABI size changed");
_Static_assert(sizeof(FolioShortcutUiResolveRequestV2) == FOLIO_SHORTCUT_UI_RESOLVE_REQUEST_V2_SIZE_64,
               "FolioShortcutUiResolveRequestV2 ABI size changed");
_Static_assert(sizeof(FolioShortcutUiResolveResultV2) == FOLIO_SHORTCUT_UI_RESOLVE_RESULT_V2_SIZE_64,
               "FolioShortcutUiResolveResultV2 ABI size changed");
_Static_assert(sizeof(FolioMusicVolumeRequestV1) == FOLIO_MUSIC_VOLUME_REQUEST_V1_SIZE_64,
               "FolioMusicVolumeRequestV1 ABI size changed");
_Static_assert(sizeof(FolioMusicVolumeResultV1) == FOLIO_MUSIC_VOLUME_RESULT_V1_SIZE_64,
               "FolioMusicVolumeResultV1 ABI size changed");
_Static_assert(sizeof(FolioExplorerShellLaunchRequestV1) == FOLIO_EXPLORER_SHELL_LAUNCH_REQUEST_V1_SIZE_64,
               "FolioExplorerShellLaunchRequestV1 ABI size changed");
_Static_assert(sizeof(FolioExplorerShellLaunchResultV1) == FOLIO_EXPLORER_SHELL_LAUNCH_RESULT_V1_SIZE_64,
               "FolioExplorerShellLaunchResultV1 ABI size changed");
_Static_assert(sizeof(FolioQuickAccessRequestV1) == FOLIO_QUICK_ACCESS_REQUEST_V1_SIZE_64,
               "FolioQuickAccessRequestV1 ABI size changed");
_Static_assert(sizeof(FolioQuickAccessResultV1) == FOLIO_QUICK_ACCESS_RESULT_V1_SIZE_64,
               "FolioQuickAccessResultV1 ABI size changed");
_Static_assert(sizeof(FolioRecycleBinRequestV1) == FOLIO_RECYCLE_BIN_REQUEST_V1_SIZE_64,
               "FolioRecycleBinRequestV1 ABI size changed");
_Static_assert(sizeof(FolioRecycleBinResultV1) == FOLIO_RECYCLE_BIN_RESULT_V1_SIZE_64,
               "FolioRecycleBinResultV1 ABI size changed");
#endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_native_abi_version(void);
FOLIO_NATIVE_API uint64_t FOLIO_NATIVE_CALL folio_native_capabilities(void);
FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_shortcut_read_v2(
    const FolioShortcutReadRequestV2* request,
    FolioShortcutReadResultV2* result);
FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_shortcut_resolve_no_ui_v2(
    const FolioShortcutResolveRequestV2* request,
    FolioShortcutReadResultV2* result);
FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_shortcut_write_v2(
    const FolioShortcutWriteRequestV2* request,
    FolioShortcutWriteResultV2* result);
FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_shortcut_resolve_with_ui_v2(
    const FolioShortcutUiResolveRequestV2* request,
    FolioShortcutUiResolveResultV2* result);
FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_music_volume_v1(
    const FolioMusicVolumeRequestV1* request,
    FolioMusicVolumeResultV1* result);
FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_explorer_shell_launch_v1(
    const FolioExplorerShellLaunchRequestV1* request,
    FolioExplorerShellLaunchResultV1* result);
FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_quick_access_v1(
    const FolioQuickAccessRequestV1* request,
    FolioQuickAccessResultV1* result);
FOLIO_NATIVE_API uint32_t FOLIO_NATIVE_CALL folio_recycle_bin_v1(
    const FolioRecycleBinRequestV1* request,
    FolioRecycleBinResultV1* result);

#ifdef __cplusplus
}
#endif

#endif
