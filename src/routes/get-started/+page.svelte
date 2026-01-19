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

  function selectDualBoot() {
    goto("/get-started/select-usb?next=dual-boot");
  }

  function selectSimple() {
    goto("/get-started/select-usb?next=simple");
  }

  function goHome() {
    goto("/");
  }
</script>

<div class="min-h-screen p-4 space-y-4 bg-zinc-950">
  <div class="max-w-2xl mx-auto">
    <Button onclick={goHome} variant="outline" class="mb-4">← Back</Button>

    <Card class="bg-zinc-900 border-zinc-800">
      <CardHeader>
        <CardTitle class="text-white">Get Started</CardTitle>
        <CardDescription>
          Prepare your system for BlossomOS installation
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-6">
        <div>
          <p class="text-zinc-400 mb-4">
            Would you like to set up a dual-boot configuration with Windows
            and BlossomOS, or just flash the USB drive?
          </p>
          <div class="grid grid-cols-2 gap-4">
            <Button
              onclick={selectDualBoot}
              class="h-auto flex-col py-6"
            >
              <div class="text-lg font-semibold">Yes, Dual-Boot</div>
              <p class="text-xs mt-2 opacity-90">
                Complete setup with system configuration
              </p>
            </Button>
            <Button
              onclick={selectSimple}
              variant="outline"
              class="h-auto flex-col py-6"
            >
              <div class="text-lg font-semibold">No, Just Flash USB</div>
              <p class="text-xs mt-2 opacity-90">Quick USB setup only</p>
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</div>
