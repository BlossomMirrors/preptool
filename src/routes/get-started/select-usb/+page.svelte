<script lang="ts">
  import { Button } from "$lib/components/ui/button";
  import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
  } from "$lib/components/ui/card";
  import { goto } from "$app/navigation";
  import { page } from "$app/stores";
  import { PowerShellScripts } from "$lib/scripts";

  let usbDrives = $state<Array<{ name: string; size: string; letter: string }>>([]);
  let isLoading = $state(false);
  let error = $state<string>("");
  let selectedDrive = $state<string>("");

  async function loadUsbDrives() {
    isLoading = true;
    error = "";

    try {
      const response = await PowerShellScripts.listUsbDrives();

      if (response.success && response.data) {
        usbDrives = response.data;
        if (usbDrives.length === 0) {
          error = "No USB drives found. Please connect a USB drive and refresh.";
        }
      } else {
        error = response.error || "Failed to list USB drives";
      }
    } catch (err) {
      error = String(err);
    } finally {
      isLoading = false;
    }
  }

  function selectDrive(letter: string) {
    selectedDrive = letter;
  }

  function proceed() {
    if (selectedDrive) {
      sessionStorage.setItem("selectedUsbDrive", selectedDrive);
      const next = $page.url.searchParams.get("next") || "simple";
      goto(`/get-started/${next}`);
    }
  }

  function goHome() {
    goto("/");
  }

  $effect.pre(() => {
    loadUsbDrives();
  });
</script>

<div class="min-h-screen p-4 space-y-4 bg-zinc-950">
  <div class="max-w-2xl mx-auto">
    <Button onclick={goHome} variant="outline" class="mb-4">← Back</Button>

    <Card class="bg-zinc-900 border-zinc-800">
      <CardHeader>
        <CardTitle class="text-white">Select USB Drive</CardTitle>
        <CardDescription>
          Choose the USB drive you want to use for BlossomOS
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        {#if error}
          <div class="text-sm text-red-400 bg-red-950 p-3 rounded">
            {error}
          </div>
          <Button onclick={loadUsbDrives} disabled={isLoading} class="w-full">
            {isLoading ? "Refreshing..." : "Refresh"}
          </Button>
        {:else if isLoading}
          <div class="text-center py-8">
            <div class="w-6 h-6 rounded-full border-2 border-blue-400 border-t-transparent animate-spin mx-auto"></div>
            <p class="text-zinc-400 mt-3">Scanning for USB drives...</p>
          </div>
        {:else if usbDrives.length > 0}
          <div class="space-y-2">
            {#each usbDrives as drive}
              <button
                onclick={() => selectDrive(drive.letter)}
                class="w-full text-left border rounded-lg p-4 transition {selectedDrive ===
                drive.letter
                  ? 'bg-blue-900 border-blue-700'
                  : 'bg-zinc-800 border-zinc-700 hover:border-zinc-600'}"
              >
                <div class="flex items-center justify-between">
                  <div>
                    <p class="font-semibold text-white">{drive.name}</p>
                    <p class="text-sm text-zinc-400">Drive: {drive.letter}:</p>
                    <p class="text-sm text-zinc-400">Size: {drive.size}</p>
                  </div>
                  {#if selectedDrive === drive.letter}
                    <span class="text-blue-400">✓</span>
                  {/if}
                </div>
              </button>
            {/each}
          </div>

          <div class="border-t border-zinc-700 pt-4 space-y-2">
            <p class="text-sm text-zinc-400">
              ⚠️ The selected USB drive will be formatted. All data will be lost.
            </p>
            <Button
              onclick={proceed}
              disabled={!selectedDrive}
              class="w-full"
            >
              Continue with {selectedDrive ? selectedDrive + ":" : "Selected Drive"}
            </Button>
          </div>
        {:else}
          <div class="text-center py-8">
            <p class="text-zinc-400 mb-4">No USB drives detected</p>
            <Button onclick={loadUsbDrives} disabled={isLoading} class="w-full">
              {isLoading ? "Refreshing..." : "Refresh"}
            </Button>
          </div>
        {/if}
      </CardContent>
    </Card>
  </div>
</div>
