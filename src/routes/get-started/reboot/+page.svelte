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

  let showQRCode = $state(false);

  function toggleQRCode() {
    showQRCode = !showQRCode;
  }

  function goHome() {
    goto("/");
  }

  function goBack() {
    goto("/get-started");
  }
</script>

<div class="min-h-screen p-4 space-y-4 bg-zinc-950">
  <div class="max-w-2xl mx-auto">
    <Button onclick={goBack} variant="outline" class="mb-4">← Back</Button>

    <Card class="bg-zinc-900 border-zinc-800">
      <CardHeader>
        <CardTitle class="text-white">Reboot to UEFI</CardTitle>
        <CardDescription>
          Prepare to boot from USB and install BlossomOS
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-6">
        <div class="bg-blue-900 border border-blue-700 rounded-lg p-4">
          <h3 class="font-semibold text-blue-100 mb-3">
            Steps to Boot from USB
          </h3>
          <ol class="space-y-2 text-sm text-blue-100">
            <li class="flex gap-3">
              <span class="font-semibold min-w-fit">1.</span>
              <span>Insert your USB drive into the computer</span>
            </li>
            <li class="flex gap-3">
              <span class="font-semibold min-w-fit">2.</span>
              <span>Restart your computer</span>
            </li>
            <li class="flex gap-3">
              <span class="font-semibold min-w-fit">3.</span>
              <span>
                During startup, press the boot menu key (usually F2, F10, F11, or DEL)
              </span>
            </li>
            <li class="flex gap-3">
              <span class="font-semibold min-w-fit">4.</span>
              <span>Select your USB drive from the boot menu</span>
            </li>
            <li class="flex gap-3">
              <span class="font-semibold min-w-fit">5.</span>
              <span>Follow the BlossomOS installation prompts</span>
            </li>
          </ol>
        </div>

        <div class="border-t border-zinc-700 pt-4">
          <h3 class="font-semibold text-white mb-3">Mobile Tutorial</h3>
          <p class="text-sm text-zinc-400 mb-4">
            Scan the QR code below with your mobile device for a step-by-step
            video tutorial:
          </p>
          <div class="mt-4 p-4 bg-zinc-800 rounded-lg text-center border border-zinc-700">
            <img
              src="https://zxing.org/w/chart?cht=qr&chs=350x350&chld=L&choe=UTF-8&chl=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DdQw4w9WgXcQ"
              alt="QR Code"
              class="w-32 h-32 mx-auto"
            />
            <p class="text-sm text-zinc-400 mt-3">
              Scan with your mobile device
            </p>
          </div>
        </div>

        <Button onclick={goHome} class="w-full">
          Back to Home
        </Button>
      </CardContent>
    </Card>
  </div>
</div>
