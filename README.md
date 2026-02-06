<div align="center">
	<img src="app-icon.png" alt="BlossomOS Switch App Icon" width="120" />

# BlossomOS Switch

**Preperation tool to make Windows dual-boot work easily with BlossomOS**
</div>

BlossomOS Switch (internally named preptool) is a Windows utility that prepares a PC for installing BlossomOS. It guides users through creating a recovery USB and (optionally) configuring a Windows system for dual‑boot.

## Highlights

- Guided wizard with two setup modes: **Just Flash USB** and **Dual‑Boot Setup**
- Downloads the latest BlossomOS image from the CDN and verifies SHA‑256
- Pause/resume and cancelable downloads
- Flash USB drive using balena‑cli (auto‑installed via Chocolatey)
- Restore a BlossomOS USB back to a normal Windows USB (DiskPart)
- Resize the C: partition safely with validation and fallbacks
- Configure system settings for Linux compatibility (UTC clock, disable Fast Startup)
- Optional WinBtrfs installation for Windows access to Btrfs partitions
- Translated UI

## Setup Modes

### Just Flash USB
For creating a BlossomOS recovery USB only.

### Dual‑Boot Setup
Adds disk resizing, system settings, and WinBtrfs installation for a complete dual‑boot preparation flow.

## Requirements

- Windows 10/11
- .NET Framework 4.7.2
- Administrator privileges (required for disk/partition and system‑setting operations)
- Internet connection (for ISO download and Chocolatey packages)
- USB drive (the tool filters out very large/system drives for safety)

## Usage Overview

1. Launch the app **as Administrator**.
2. Choose **Just Flash USB** or **Dual‑Boot Setup**.
3. Select a USB drive.
4. Download the BlossomOS image or select a local ISO.
5. Flash the USB (or restore a USB back to normal format).
6. (Dual‑Boot) Resize the C: partition, apply system settings, and install WinBtrfs.
7. Finish and optionally reboot into UEFI firmware settings.

## ISO Download and Cache

Downloaded images are cached in the system temp directory:

```
%TEMP%\BlossomOS\BlossomOS.iso
```

The SHA‑256 checksum is validated using the metadata from:

```
https://cdn.blossomos.org/iso/isodata.json
```

## Building from Source

Open the solution file and build with Visual Studio:

```
preptool/BlossomPrepTool.slnx
```

Target framework: **.NET Framework 4.7.2**

## Notes & Safety

- Flashing or restoring a USB drive **erases all data on that drive**.
- Resizing partitions always carries risk. Ensure important data is backed up.
- Some steps (DiskPart, power settings, registry changes) require admin rights.

## License

See [LICENSE](LICENSE).
