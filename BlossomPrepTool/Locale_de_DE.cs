using System;
using System.Collections.Generic;

namespace BlossomPrepTool
{
    internal static class Locale_de_DE
    {
        public static readonly IReadOnlyDictionary<string, string> Strings =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["WizardWelcome.Title"] = "BlossomOS Switch",
                ["WizardWelcome.Label"] = "WILLKOMMEN",
                ["WizardWelcome.Description"] = @"Bereite dein System für die Installation von BlossomOS mit diesem benutzerfreundlichen Tool vor.
Folge den Schritten für eine reibungslose Installation.",
                ["WizardWelcome.ManualSetupTitle"] = @"Installiere die BlossomOS-Wiederherstellungsumgebung
auf einem vorhandenen USB-Laufwerk",
                ["WizardWelcome.ManualSetupDesc"] = "Nur schnelle USB-Einrichtung",
                ["WizardWelcome.ManualSetupButton"] = "Manuelle Einrichtung",
                ["WizardWelcome.GetStartedTitle"] = @"Bereite dein System vor und installiere die Wiederherstellungsumgebung
auf dein USB-Laufwerk.",
                ["WizardWelcome.GetStartedDesc"] = "Komplette Einrichtung mit Systemkonfiguration",
                ["WizardWelcome.GetStartedButton"] = "Los geht's",

                ["WizardModeSelection.Title"] = "Einrichtungsmodus wählen",
                ["WizardModeSelection.SimpleMode"] = "Nur USB flashen",
                ["WizardModeSelection.DualBootMode"] = "Dual-Boot-Einrichtung",

                ["WizardIsoSource.Title"] = "Was möchtest du tun?",
                ["WizardIsoSource.DownloadTitle"] = "BlossomOS Abbild herunterladen",
                ["WizardIsoSource.DownloadDesc"] = @"Lade die neueste BlossomOS-
Wiederherstellungsumgebungsabbild von unseren
Servern herunter",
                ["WizardIsoSource.DownloadButton"] = "Abbild herunterladen",
                ["WizardIsoSource.UseOwnTitle"] = "Mein eigenes Abbild verwenden",
                ["WizardIsoSource.UseOwnDesc"] = @"Ich habe bereits eine BlossomOS Abbild-Datei
auf meinem Computer",
                ["WizardIsoSource.UseOwnButton"] = "Abbild-Datei auswählen",
                ["WizardIsoSource.RestoreTitle"] = "USB-Laufwerk wiederherstellen",
                ["WizardIsoSource.RestoreDesc"] = @"Konvertiere ein BlossomOS USB-Laufwerk zurück zu
einem normalen Windows USB-Laufwerk",
                ["WizardIsoSource.RestoreButton"] = "USB wiederherstellen",
                ["WizardIsoSource.BackButton"] = "← Zurück",

                ["WizardUsbSelection.Title"] = "USB-Laufwerk wählen",
                ["WizardUsbSelection.DriveLabel"] = "Wähle dein USB-Laufwerk:",
                ["WizardUsbSelection.RefreshButton"] = "⟳ Aktualisieren",
                ["WizardUsbSelection.NoUsbSelected"] = "Kein USB ausgewählt",
                ["WizardUsbSelection.ContinueButton"] = "Weiter →",
                ["WizardUsbSelection.BackButton"] = "← Zurück",
                ["WizardUsbSelection.DriveWithLabel"] = "{0} (Datenträger {1}, {2})",
                ["WizardUsbSelection.DriveWithoutLabel"] = "Datenträger {0} ({1})",

                ["WizardPartition.Title"] = "Speicherplatz zuweisen",
                ["WizardPartition.Description"] = "Wähle, wie viel Speicherplatz von deinem C:-Laufwerk für BlossomOS reserviert werden soll.",
                ["WizardPartition.AllocateLabel"] = "Speicherplatz für BlossomOS:",
                ["WizardPartition.DefaultSize"] = "50",
                ["WizardPartition.GBLabel"] = "GB",
                ["WizardPartition.Status"] = "Mindestens empfohlen: 50 GB",
                ["WizardPartition.NextButton"] = "Weiter →",
                ["WizardPartition.BackButton"] = "← Zurück",

                ["WizardFlash.Title"] = "USB-Laufwerk flashen",
                ["WizardFlash.Description"] = "Dies schreibt das Abbild auf dein USB-Laufwerk. Alle Daten werden gelöscht.",
                ["WizardFlash.Status"] = "Bereit",

                ["WizardSettings.Title"] = "Systemeinstellungen",
                ["WizardSettings.Description"] = @"Konfiguriere Systemeinstellungen für Dual-Boot-Kompatibilität:

• Stelle die Systemuhr auf UTC (erforderlich für Linux)
• Deaktiviere schnelles Herunterfahren (verhindert Partitionszugriffsprobleme)",
                ["WizardSettings.Status"] = "Bereit, Einstellungen zu konfigurieren",
                ["WizardSettings.ApplyButton"] = "Einstellungen anwenden",
                ["WizardSettings.BackButton"] = "← Zurück",

                ["WizardWinBTRFS.Title"] = "WinBtrfs installieren",
                ["WizardWinBTRFS.Description"] = @"WinBtrfs ist erforderlich, um von Windows aus auf BlossomOS-Partitionen zuzugreifen.

Dies installiert den Dateisystemtreiber, der Windows ermöglicht, Btrfs-Partitionen zu lesen und zu beschreiben.",
                ["WizardWinBTRFS.Status"] = "Bereit, WinBtrfs zu installieren",
                ["WizardWinBTRFS.InstallButton"] = "Installieren",
                ["WizardWinBTRFS.BackButton"] = "← Zurück",

                ["WizardDownload.Title"] = "Abbild herunterladen",
                ["WizardDownload.Status"] = "Bereit zum Herunterladen",

                ["WizardComplete.Title"] = "Einrichtung abgeschlossen!",

                ["Common.BackButton"] = "← Zurück",
                ["Common.NextButton"] = "Weiter →",

                ["MessageBox.ConfirmReboot"] = "Neustart bestätigen",
                ["MessageBox.NoUsbSelected"] = "Kein Laufwerk ausgewählt",
                ["MessageBox.IsoNotFound"] = "Abbild nicht gefunden",
                ["MessageBox.ConfirmUsbFlash"] = "USB-Flash bestätigen",
                ["MessageBox.InvalidSize"] = "Ungültige Größe",
                ["MessageBox.ConfirmPartitionResize"] = "Größenänderung der Partition bestätigen",
                ["MessageBox.ConfirmInstallation"] = "Installation bestätigen",

                ["Message.RebootToUefi"] = "Dies startet deinen Computer in die UEFI-Firmware-Einstellungen neu. Fortfahren?",
                ["Message.EraseUsbWarning"] = "Dies löscht alle Daten auf Datenträger {0}. Fortfahren?",
                ["Message.NoUsbSelected"] = "Bitte wähle zuerst ein USB-Laufwerk aus",
                ["Message.IsoNotFound"] = "Abbild-Datei nicht gefunden. Bitte lade ein Abbild herunter oder wähle eines aus.",
                ["Message.IsoNotFoundSimple"] = "Abbild-Datei nicht gefunden. Bitte lade sie zuerst herunter.",
                ["Message.InvalidPartitionSize"] = "Bitte gib eine gültige Größe ein (mindestens 20 GB empfohlen)",
                ["Message.PartitionResizeWarning"] = "Dies ändert die Größe deiner C:-Laufwerk-Partition. Stelle sicher, dass du eine Sicherung deiner wichtigen Daten hast. Fortfahren?",
                ["Message.InstallWinBtrfs"] = @"Dies installiert WinBtrfs über Chocolatey.
Fortfahren?",

                ["Button.Pause"] = "Pausieren",
                ["Button.Cancel"] = "Abbrechen",
                ["Button.StartFlash"] = "Flashen starten",
                ["Button.Finish"] = "Fertig",
                ["Button.RebootToUEFI"] = "In UEFI neu starten",
                ["Button.Refresh"] = "⟳ Aktualisieren",
                ["Button.Install"] = "Installieren",
                ["Button.ApplySettings"] = "Einstellungen anwenden",

                ["Status.Processing"] = "Wird verarbeitet...",
                ["Status.CheckingCache"] = "⠋ Überprüfe gecachtes Abbild...",
                ["Status.CacheVerified"] = "✓ Gecachtes Abbild erfolgreich verifiziert!",
                ["Status.UsingCached"] = "Verwende gecachtes Abbild",

                ["WizardComplete.QRMessage"] = @"Scanne mit deinem Mobilgerät
für Video-Tutorial",
                ["WizardComplete.KeepUSB"] = "💡 Wichtig: Bewahre dein USB-Laufwerk sicher auf - es kann zum Neuinstallieren oder Wiederherstellen von BlossomOS verwendet werden.",
                ["WizardComplete.Message"] = "Dein USB-Laufwerk ist startbereit!",

                ["Status.StartingDownload"] = "⠋ Starte Download...",
                ["Status.DownloadSuccess"] = "✓ Abbild erfolgreich heruntergeladen!",
                ["Status.DownloadCancelled"] = "⊘ Download abgebrochen",
                ["Status.ResumeDownload"] = "⠋ Setze Download fort...",
                ["Status.DownloadPaused"] = "⏸ Download unterbrochen",

                ["Status.RestoringUSB"] = "⠋ Stelle USB-Laufwerk wieder her...",
                ["Status.RestoreSuccess"] = "✓ USB-Laufwerk erfolgreich wiederhergestellt!",
                ["Status.RestoreFailed"] = "✗ Wiederherstellung des USB-Laufwerks fehlgeschlagen",
                ["Status.FlashingUSB"] = "⠋ Flashe USB-Laufwerk...",
                ["Status.FlashSuccess"] = "✓ USB-Laufwerk erfolgreich geflasht!",
                ["Status.FlashFailed"] = "✗ Flash-Vorgang fehlgeschlagen",
                ["Status.FlashCancelled"] = "⊘ Flashen abgebrochen",

                ["Message.RestoreUSB"] = "Dein USB-Laufwerk wird in das normale Windows-Format zurückversetzt. Alle Daten werden gelöscht.",
                ["WizardFlash.RestoreTitle"] = "USB-Laufwerk wiederherstellen",

                ["Status.ResizingPartition"] = "⠋ Ändere Partitionsgröße...",

                ["Button.Resume"] = "Fortsetzen",

                ["Main.DownloadISO"] = "Abbild herunterladen",
                ["Main.FlashUSB"] = "USB flashen",
                ["Main.ResizePartition"] = "Partition ändern",
                ["Main.InstallWinBTRFS"] = "WinBTRFS installieren",
                ["Main.Refresh"] = "Aktualisieren",
                ["Main.ClearLog"] = "Protokoll löschen",

                ["Main.USBDrives"] = "USB-Laufwerke:",
                ["Main.PartitionSize"] = "Größe (GB):",
                ["Main.Ready"] = "Bereit",
                ["Main.Title"] = "BlossomPrep Tool",

                ["Form.SelectLanguage"] = "Sprache auswählen",

                ["WizardPartition.DiskInfo"] = "C: Laufwerk: {0:F1} GB Gesamt, {1:F1} GB verwendet, {2:F1} GB frei",
                ["WizardUSBSelection.Selected"] = "Ausgewählt: Datenträger {0} ({1}GB)",
                ["WizardUSBSelection.NoUSB"] = "Kein USB ausgewählt",

                ["Status.PartitionResizeSuccess"] = "✓ Partitionsgröße erfolgreich geändert!",
                ["Status.PartitionResizeFailed"] = "✗ Partitionsgrößen-Änderung fehlgeschlagen",

                ["Status.ApplyingSettings"] = "⠋ Wende Systemeinstellungen an...",
                ["Status.SettingsSuccess"] = "✓ Systemeinstellungen erfolgreich konfiguriert!",
                ["Status.SettingsPartialFail"] = "⚠ Einige Einstellungen konnten nicht angewendet werden",

                ["Status.InstallingWinBTRFS"] = "⠋ Installiere WinBtrfs...",
                ["Status.WinBTRFSSuccess"] = "✓ WinBtrfs erfolgreich installiert!",
                ["Status.WinBTRFSFailed"] = "✗ WinBtrfs-Installation fehlgeschlagen"
            };
    }
}
