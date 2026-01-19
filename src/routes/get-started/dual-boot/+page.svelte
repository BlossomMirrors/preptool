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
  import { PowerShellScripts } from "$lib/scripts";
  import { onMount } from "svelte";

  let completedSteps = $state<string[]>([]);
  let isLoading = $state(false);
  let error = $state<string>("");
  let currentStepIndex = $state<number>(0);
  let showRestoreOption = $state(false);
  let showPartitionPrompt = $state(false);
  let partitionSizeGB = $state<number | null>(null);

  const steps = [
    {
      id: "disable-fast-startup",
      title: "Disable Fast Startup",
      description: "Disable Windows Fast Startup for dual-boot compatibility",
    },
    {
      id: "install-winbtrfs",
      title: "Install WinBtrfs",
      description: "Install WinBtrfs driver for Linux filesystem support",
    },
    {
      id: "set-utc-time",
      title: "Set UTC Time",
      description: "Configure system clock to UTC for dual-boot compatibility",
    },
    {
      id: "partition",
      title: "Partition Drive",
      description: "Create partition space for BlossomOS installation",
    },
    {
      id: "download-iso",
      title: "Download ISO",
      description: "Download the BlossomOS installation image",
    },
    {
      id: "flash-usb",
      title: "Flash USB Drive",
      description: "Write the ISO to your USB drive",
    },
  ];

  async function runStep(stepId: string) {
    isLoading = true;
    error = "";

    try {
      // Check if ISO is already cached and skip download
      if (stepId === "download-iso") {
        const cacheCheck = await PowerShellScripts.checkIsoExists();
        if (cacheCheck.success && cacheCheck.data?.exists) {
          completedSteps = [...completedSteps, stepId];
          // Auto-advance to next step
          if (currentStepIndex < steps.length - 1) {
            currentStepIndex++;
            await runStep(steps[currentStepIndex].id);
          }
          isLoading = false;
          return;
        }
      }

      // Prompt for partition size before running partition step
      if (stepId === "partition" && partitionSizeGB === null) {
        showPartitionPrompt = true;
        isLoading = false;
        return;
      }

      const args: Record<string, string> = {};
      
      // Only pass DiskNumber for USB-related scripts
      if (sessionStorage.getItem("selectedUsbDrive") && (stepId === "flash-usb" || stepId === "restore-usb")) {
        args.DiskNumber = sessionStorage.getItem("selectedUsbDrive")!;
      }
      
      // Only partition script needs FreeSpaceGB
      if (stepId === "partition" && partitionSizeGB !== null) {
        args.FreeSpaceGB = String(partitionSizeGB);
      }

      // Flash-usb needs the ISO path
      if (stepId === "flash-usb") {
        const isoCheck = await PowerShellScripts.checkIsoExists();
        if (isoCheck.success && isoCheck.data?.path) {
          args.ISOPath = isoCheck.data.path;
        } else {
          error = "ISO file not found. Please download the ISO first.";
          isLoading = false;
          return;
        }
      }

      const response = await PowerShellScripts.runScript(stepId, args);

      if (response.success) {
        completedSteps = [...completedSteps, stepId];
        // Auto-advance to next step if available
        if (currentStepIndex < steps.length - 1) {
          currentStepIndex++;
          await runStep(steps[currentStepIndex].id);
        }
      } else {
        error = response.error || "Step execution failed";
      }
    } catch (err) {
      error = String(err);
    } finally {
      isLoading = false;
    }
  }

  function confirmPartitionSize() {
    if (partitionSizeGB === null) {
      partitionSizeGB = 128;
    }
    showPartitionPrompt = false;
    runStep("partition");
  }

  function retryCurrentStep() {
    error = "";
    runStep(steps[currentStepIndex].id);
  }

  function proceedToReboot() {
    goto("/get-started/reboot");
  }

  function goBack() {
    goto("/get-started");
  }

  onMount(async () => {
    // Don't auto-start, wait for user to click "Start"
  });
</script>

<div class="min-h-screen p-4 space-y-4 bg-zinc-950">
  <div class="max-w-2xl mx-auto">
    <Button onclick={goBack} variant="outline" class="mb-4">← Back</Button>

    <Card class="bg-zinc-900 border-zinc-800">
      <CardHeader>
        <CardTitle class="text-white">System Preparation</CardTitle>
        <CardDescription>
          Follow these steps to prepare for dual-boot installation
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        {#if showPartitionPrompt}
          <div class="border-2 border-blue-700 bg-blue-950 rounded-lg p-6 space-y-4">
            <h3 class="text-lg font-semibold text-white">Allocate Space for BlossomOS</h3>
            <p class="text-sm text-blue-200">
              How much free space would you like to allocate for the BlossomOS installation?
            </p>
            <p class="text-xs text-blue-300">
              ℹ️ This shrinks your Windows partition while keeping at least 20GB allocated to Windows.
            </p>
            
            <div class="space-y-3">
              <div class="flex items-center gap-3">
                <input
                  type="range"
                  min="50"
                  max="500"
                  step="5"
                  bind:value={partitionSizeGB}
                  class="flex-1"
                />
                <div class="text-right">
                  <div class="text-2xl font-bold text-white">{partitionSizeGB ?? 128}</div>
                  <div class="text-xs text-zinc-400">GB</div>
                </div>
              </div>
              
              <div class="flex gap-3">
                <input
                  type="number"
                  min="50"
                  max="500"
                  bind:value={partitionSizeGB}
                  class="flex-1 p-2 rounded border border-zinc-600 bg-zinc-800 text-white text-sm"
                />
              </div>
              
              <p class="text-xs text-zinc-400">
                Minimum: 50GB | Maximum: 500GB | Typical: 128GB - 256GB
              </p>
            </div>

            <div class="flex gap-3">
              <Button
                onclick={confirmPartitionSize}
                disabled={isLoading || (partitionSizeGB !== null && (partitionSizeGB < 50 || partitionSizeGB > 500))}
                class="flex-1"
              >
                Allocate {partitionSizeGB ?? 128}GB
              </Button>
              <Button
                onclick={() => {
                  showPartitionPrompt = false;
                  isLoading = false;
                }}
                variant="outline"
                disabled={isLoading}
                class="flex-1"
              >
                Cancel
              </Button>
            </div>
          </div>
        {/if}

        {#if completedSteps.length === 0 && currentStepIndex === 0}
          <div class="bg-blue-900 border border-blue-700 rounded-lg p-4 text-center space-y-4">
            <p class="text-blue-100">
              Click "Start" to begin the system preparation process
            </p>
            <Button onclick={() => runStep(steps[0].id)} class="w-full">
              Start
            </Button>
          </div>
        {/if}

        {#each steps as step, idx}
          <div
            class="border rounded-lg p-4 {completedSteps.includes(step.id)
              ? 'bg-green-900 border-green-700'
              : currentStepIndex === idx
                ? 'bg-blue-900 border-blue-700'
                : 'bg-zinc-800 border-zinc-700'}"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <h3 class="font-semibold text-white">{step.title}</h3>
                <p class="text-sm text-zinc-400 mt-1">
                  {step.description}
                </p>
              </div>
              <div class="ml-4 flex items-center gap-2">
                {#if completedSteps.includes(step.id)}
                  <span class="text-green-400">✓</span>
                {:else if currentStepIndex === idx}
                  <div class="w-4 h-4 rounded-full border-2 border-blue-400 border-t-transparent animate-spin"></div>
                {/if}
              </div>
            </div>

            {#if error && currentStepIndex === idx}
              <div class="mt-4 space-y-2">
                <div class="text-sm text-red-400 bg-red-950 p-3 rounded">
                  {error}
                </div>
                <Button
                  onclick={retryCurrentStep}
                  disabled={isLoading}
                  variant="outline"
                  class="w-full"
                >
                  Retry
                </Button>
              </div>
            {/if}
          </div>
        {/each}

        {#if completedSteps.length === steps.length && !isLoading}
          <div class="border-t border-zinc-700 pt-4 space-y-3">
            <div class="bg-green-900 border border-green-700 rounded-lg p-4">
              <h3 class="font-semibold text-green-100">
                ✓ Preparation Complete
              </h3>
              <p class="text-sm text-green-200 mt-2">
                Your system is ready for dual-boot installation.
              </p>
            </div>
            <Button onclick={proceedToReboot} class="w-full">
              Next: Reboot Instructions
            </Button>

            <div class="border-t border-zinc-700 pt-3">
              <p class="text-sm text-zinc-400 mb-2">
                Need to undo the changes?
              </p>
              <Button
                variant="outline"
                onclick={() => (showRestoreOption = !showRestoreOption)}
                class="w-full"
              >
                {showRestoreOption ? "Hide Restore Option" : "Restore USB Drive"}
              </Button>
              {#if showRestoreOption}
                <div class="mt-3 p-3 bg-red-950 border border-red-700 rounded">
                  <p class="text-sm text-red-200 mb-3">
                    This will restore your USB drive to a clean FAT32 partition. All data will be lost.
                  </p>
                  <Button
                    variant="outline"
                    onclick={async () => {
                      isLoading = true;
                      error = "";
                      try {
                        const response = await PowerShellScripts.runScript(
                          "restore-usb",
                          sessionStorage.getItem("selectedUsbDrive")
                            ? {
                                DiskNumber: sessionStorage.getItem(
                                  "selectedUsbDrive",
                                )!,
                              }
                            : undefined,
                        );
                        if (response.success) {
                          showRestoreOption = false;
                          error = "";
                        } else {
                          error =
                            response.error || "Failed to restore USB drive";
                        }
                      } catch (err) {
                        error = String(err);
                      } finally {
                        isLoading = false;
                      }
                    }}
                    disabled={isLoading}
                    class="w-full text-red-300"
                  >
                    {isLoading ? "Restoring..." : "Restore USB Drive"}
                  </Button>
                  {#if error}
                    <div class="mt-2 text-sm text-red-300">{error}</div>
                  {/if}
                </div>
              {/if}
            </div>
          </div>
        {/if}
      </CardContent>
    </Card>
  </div>
</div>
