# Ordinary plugins and Trusted Components

Ordinary plugins use the `.arcplugin` format, capability declarations and developer
signing described in [Packaging](packaging.md) and [Plugin signing tool](signtool.md).

The host reserves certain identities and signing profiles for first-party Trusted
Components. A `trustedComponent` manifest declaration does not grant authority:
the host requires an allowlisted identity/profile, a pinned release key and dedicated
integrity verification. An ordinary developer signature or user approval cannot
satisfy these checks. Ordinary plugins must omit this field and use their own IDs.

SDK 1.12 / host 50 adds optional `IPluginContext.FirstPartyUiService` for reserved
first-party placement. Ordinary plugins receive null and should use the standard
UI registrations for settings, panels, editor actions and owned browser toolbars;
see [UI extension API](ui-extension-api.md). No capability declaration grants access
to this service, and trusted status does not grant unrestricted host-service access.

All plugins remain in-process .NET code with the current user's privileges. Neither
capabilities nor signature classification provide an operating-system sandbox.
Signature verification establishes package identity and integrity, not correctness
of arbitrary plugin code.
