# Arclyra.PluginSignTool

`Arclyra.PluginSignTool` creates developer signing keys and signs `.arcplugin` packages for Arclyra Writing Studio.

> [!IMPORTANT]
> Developer keys will not work correctly until Arclyra validates your developer identity. To successfully sign packages, email **developers@arclyra.app** with your public key attached and basic information about your identity.

## What the signing tool does

Arclyra plugin packages are ZIP archives with a root `plugin/` folder. The signing tool adds a root-level `plugin.package.json` file that contains:

- a canonical SHA-256 hash of the package's `plugin/` folder contents;
- your developer name;
- your developer public key;
- an Arclyra-issued developer signature for that public key;
- a package signature created with your developer private key;
- signature format and algorithm metadata.

Arclyra uses this metadata to associate a package with a validated developer identity and detect package tampering.

## Interactive mode

Run the tool without arguments to start the prompt-driven workflow:

```powershell
dotnet run --project Arclyra.PluginSignTool/Arclyra.PluginSignTool.csproj
```

The interactive menu can:

1. generate plugin developer keys;
2. sign a `.arcplugin` package;
3. show command-line usage.

When selecting option `1`, the tool displays the developer identity notice before asking for key-generation details.

## Command-line usage

Show help:

```powershell
Arclyra.PluginSignTool.exe --help
```

Generate a developer key pair:

```powershell
Arclyra.PluginSignTool.exe --generate-developer-key --developer-name "Example Developer" --private-key developer-private.pem --public-key developer-public.pem
```

The default RSA key size is `4096`. You can override it with `--key-size`:

```powershell
Arclyra.PluginSignTool.exe --generate-developer-key --developer-name "Example Developer" --private-key developer-private.pem --public-key developer-public.pem --key-size 4096
```

## Developer validation workflow

After generating keys:

1. Keep `developer-private.pem` private. Do not email it, commit it, or package it with your plugin.
2. Email **developers@arclyra.app**.
3. Attach `developer-public.pem`.
4. Include basic information about your identity, such as your name, organization if applicable, plugin name, support/contact information, and website or project page if available.
5. Wait for Arclyra to validate your developer identity and provide a developer signature file, commonly named `developer.sig`.

You need the developer signature file before package signing will work correctly for public distribution.

## Sign a package

Build and package your plugin first. The `.arcplugin` archive must contain a root `plugin/` folder.

Sign in place:

```powershell
Arclyra.PluginSignTool.exe --sign-package --package MyPlugin.arcplugin --developer-name "Example Developer" --developer-private-key developer-private.pem --developer-public-key developer-public.pem --developer-signature developer.sig
```

Write a signed copy instead of overwriting the input package:

```powershell
Arclyra.PluginSignTool.exe --sign-package --package MyPlugin.arcplugin --developer-name "Example Developer" --developer-private-key developer-private.pem --developer-public-key developer-public.pem --developer-signature developer.sig --output MyPlugin.Signed.arcplugin
```

The tool extracts the package to a temporary folder, computes the canonical package hash, writes `plugin.package.json`, and recreates the `.arcplugin` archive.

## Embed an existing signature file

If you already have a `plugin.package.json` signature metadata file, embed it into a package:

```powershell
Arclyra.PluginSignTool.exe --embed-signature --package MyPlugin.arcplugin --signature plugin.package.json --output MyPlugin.Signed.arcplugin
```

Omit `--output` to overwrite the original package.

## Command reference

| Command | Purpose |
| --- | --- |
| `generate-developer-key` | Generates a developer private/public RSA key pair. |
| `sign-package` | Creates and embeds `plugin.package.json` using your developer keys and Arclyra-issued developer signature. |
| `embed-signature` | Copies an existing signature metadata file into a package. |

### `generate-developer-key` options

| Option | Required | Notes |
| --- | --- | --- |
| `--developer-name` | Yes | Developer or organization name associated with the key. |
| `--private-key` | Yes | Output path for the private key PEM. Keep this file secret. |
| `--public-key` | Yes | Output path for the public key PEM to send to Arclyra. |
| `--key-size` | No | RSA key size. Defaults to `4096`. |

### `sign-package` options

| Option | Required | Notes |
| --- | --- | --- |
| `--package` | Yes | Path to the `.arcplugin` package. |
| `--developer-name` | Yes | Developer name. Use the same identity submitted for validation. |
| `--developer-private-key` | Yes | Path to your private key PEM. |
| `--developer-public-key` | Yes | Path to your public key PEM. |
| `--developer-signature` | Yes | Path to the Arclyra-issued developer signature file. |
| `--output` | No | Signed package output path. If omitted, the input package is overwritten. |

### `embed-signature` options

| Option | Required | Notes |
| --- | --- | --- |
| `--package` | Yes | Path to the `.arcplugin` package. |
| `--signature` | Yes | Path to `plugin.package.json`. |
| `--output` | No | Signed package output path. If omitted, the input package is overwritten. |

## Troubleshooting

- **`Plugin package must contain a root plugin folder before it can be signed.`** Recreate the `.arcplugin` archive so its root contains `plugin/plugin.json`, not just the contents of the plugin folder.
- **Missing required option.** Check the command reference and provide every required `--option value` pair.
- **Validation or trust failures in Arclyra.** Confirm that Arclyra has validated your developer identity, that you are using the exact public key submitted to Arclyra, and that the package was not changed after signing.
