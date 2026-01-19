<script lang="ts">
  import { Button } from "$lib/components/ui/button";
  import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
  } from "$lib/components/ui/card";
  import IsoDownload from "$lib/components/IsoDownload.svelte";
  import { goto } from "$app/navigation";
  import { PowerShellScripts } from "$lib/scripts";
  import { onMount } from "svelte";

  type UsbDrive = { name: string; size: string; diskNumber: number };

  let isLoading = $state(false);
  let error = $state<string>("");
  let success = $state<string>("");
  let customIsoPath = $state<string>("");
  let selectedUsbDrive = $state<string>("");
  let usbDrives = $state<UsbDrive[]>([]);
  let loadingDrives = $state(false);
  let defaultIsoPath = $state<string>("");
  let hasIso = $state(false);

  async function loadUsbDrives() {
    loadingDrives = true;
    try {
      const result = await PowerShellScripts.listUsbDrives();
      if (result.success && result.data) {
        usbDrives = result.data as unknown as UsbDrive[];
      } else {
        error = "Failed to load USB drives";
      }
    } catch (err) {
      error = String(err);
    } finally {
      loadingDrives = false;
    }
  }

  async function runTool(toolId: string) {
    isLoading = true;
    error = "";
    success = "";

    try {
      const args: Record<string, string> = {};
      if (selectedUsbDrive && (toolId === "flash-usb" || toolId === "restore-usb")) {
        args.DiskNumber = selectedUsbDrive;
      }
      if (toolId === "flash-usb") {
        args.IsoPath = customIsoPath || defaultIsoPath;
      }

      const response = await PowerShellScripts.runScript(toolId, args);
      if (response.success) {
        success = `${toolId} completed successfully`;
      } else {
        error = response.error || `${toolId} failed`;
      }
    } catch (err) {
      error = String(err);
    } finally {
      isLoading = false;
    }
  }

  function handleFileSelect(e: Event) {
    const input = e.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      customIsoPath = input.files[0].name;
    }
  }

  function goBack() {
    goto("/");
  }

  function handleIsoDownloadComplete(e: CustomEvent<{ isoPath: string }>) {
    defaultIsoPath = e.detail.isoPath;
    hasIso = true;
  }

  onMount(async () => {
    await loadUsbDrives();
    
    try {
      const isoCheck = await PowerShellScripts.checkIsoExists();
      if (isoCheck.success && isoCheck.data?.exists) {
        defaultIsoPath = isoCheck.data.path;
        hasIso = true;
      }
    } catch (err) {
      console.error("Failed to check ISO", err);
    }
  });

  const tools = [
    {
      id: "flash-usb",
      title: "Flash USB Drive",
      description: "Write ISO to your USB drive",
      color: "bg-green-900 hover:bg-green-800",
      requiresUsb: true,
    },
    {
      id: "restore-usb",
      title: "Restore USB Drive",
      description: "Clear USB drive and create FAT32 partition",
      color: "bg-red-900 hover:bg-red-800",
      requiresUsb: true,
    },
    {
      id: "reboot-uefi",
      title: "Reboot to UEFI",
      description: "Restart and enter UEFI/BIOS setup",
      color: "bg-purple-900 hover:bg-purple-800",
    },
  ];
</script>

<div class="min-h-screen p-4 space-y-4 bg-zinc-950">
  <div class="max-w-2xl mx-auto">
    <Button onclick={goBack} variant="outline" class="mb-4">← Back</Button>

    <Card class="bg-zinc-900 border-zinc-800 mb-6">
      <CardHeader>
        <CardTitle class="text-white">Manual Setup Tools</CardTitle>
        <CardDescription>Run each tool independently at your own pace</CardDescription>
      </CardHeader>
      <CardContent class="space-y-6">
        <!-- ISO Download Component -->
        <IsoDownload on:download-complete={handleIsoDownloadComplete} />

        <!-- USB Drive Selection -->
        <div class="space-y-3">
          <div class="flex justify-between items-center">
            <label for="usb-drive-select" class="text-sm font-medium text-zinc-300">Select USB Drive</label>
            <Button
              onclick={loadUsbDrives}
              disabled={loadingDrives}
              variant="outline"
              class="text-xs"
            >
              {loadingDrives ? "Refreshing..." : "Refresh"}
            </Button>
          </div>

          {#if usbDrives.length === 0}
            <div class="text-sm text-zinc-400 p-3 bg-zinc-800 rounded">
              No USB drives found. Please insert a USB drive and click Refresh.
            </div>
          {:else}
            <select
              id="usb-drive-select"
              bind:value={selectedUsbDrive}
              class="w-full p-3 rounded border-2 bg-zinc-800 border-zinc-700 text-white"
            >
              <option value="">Choose a USB drive...</option>
              {#each usbDrives as drive}
                <option value={String(drive.diskNumber)}>
                  {drive.name || "USB Drive"} - Disk {drive.diskNumber} ({drive.size})
                </option>
              {/each}
            </select>
          {/if}
        </div>

        <!-- Custom ISO Selection -->
        <div class="space-y-2">
          <label for="custom-iso" class="text-sm font-medium text-zinc-300">Custom ISO (Optional)</label>
          <div class="flex gap-2">
            <input
              id="custom-iso"
              type="file"
              accept=".iso"
              onchange={handleFileSelect}
              class="flex-1"
            />
            {#if customIsoPath}
              <span class="text-sm text-green-400 truncate">{customIsoPath}</span>
            {/if}
          </div>
          <p class="text-xs text-zinc-400">Leave empty to use downloaded ISO or cache</p>
        </div>

        <!-- Messages -->
        {#if error}
          <div class="bg-red-950 border border-red-700 rounded p-3">
            <p class="text-sm text-red-200">{error}</p>
          </div>
        {/if}

        {#if success}
          <div class="bg-green-950 border border-green-700 rounded p-3">
            <p class="text-sm text-green-200">{success}</p>
          </div>
        {/if}

        <!-- Tools Grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
          {#each tools as tool}
            <Button
              onclick={() => runTool(tool.id)}
              disabled={isLoading || (tool.requiresUsb && !selectedUsbDrive) || (tool.id === "flash-usb" && !hasIso && !customIsoPath)}
              class={`${tool.color} text-white flex flex-col items-start p-4 h-auto justify-start`}
            >
              <span class="font-semibold">{tool.title}</span>
              <span class="text-xs text-zinc-300">{tool.description}</span>
            </Button>
          {/each}
        </div>

        {#if isLoading}
          <div class="flex items-center justify-center gap-2 text-zinc-400">
            <div class="w-4 h-4 rounded-full border-2 border-blue-400 border-t-transparent animate-spin"></div>
            <span class="text-sm">Running tool...</span>
          </div>
        {/if}
      </CardContent>
    </Card>
  </div>
</div>

<style>
</style>
