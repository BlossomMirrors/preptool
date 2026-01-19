<script lang="ts">
  import { Button } from "$lib/components/ui/button";
  import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
  } from "$lib/components/ui/card";
  import { goto } from "$app/navigation";

  function handleGetStarted() {
    goto("/get-started");
  }

  function handleManualSetup() {
    goto("/manual-setup");
  }
</script>

<div class="flex justify-between items-center h-screen p-4 bg-zinc-950">
  <div class="rounded-lg border border-zinc-700 bg-zinc-900 p-8 lg:p-10 flex flex-col justify-between">
    <div class="space-y-4">
      <p class="text-xs uppercase tracking-[0.2em] text-zinc-400">
        Welcome
      </p>
      <h1 class="text-3xl font-semibold leading-tight font-serif text-white">
        BlossomOS Preperation Tool
      </h1>
      <p class="text-sm text-zinc-400 max-w-xl">
        Prepare your system for installing BlossomOS with this easy-to-use tool.
        Follow the steps to ensure a smooth installation process.
      </p>
    </div>
    <div
      class="rounded-lg border border-zinc-700 bg-zinc-900 p-4 mt-8 text-sm text-zinc-400"
    >
      <div class="flex items-center gap-3">
        <img src="/icon.svg" alt="Blossom" class="h-10 w-10" />
        <div>
          <p class="font-medium text-white font-serif">BlossomOS</p>
          <p class="text-xs text-zinc-400">
            The operating system for the rest of us.
          </p>
        </div>
      </div>
    </div>
  </div>
  <Card class="m-4 shadow-none border border-zinc-700 bg-zinc-900">
    <CardHeader>
      <CardTitle class="text-white">What would you like to do?</CardTitle>
    </CardHeader>
    <CardContent class="space-y-4">
      <div class="rounded-lg border border-zinc-700 bg-zinc-800 p-4">
        <p class="text-sm text-zinc-400">
          Install the BlossomOS recovery environment onto an existing USB drive
        </p>
        <div class="mt-3 flex gap-2">
          <Button variant="outline" onclick={handleManualSetup}
            >Manual setup</Button
          >
        </div>
      </div>
      <div class="rounded-lg border border-zinc-700 bg-zinc-800 p-4">
        <p class="text-sm text-zinc-400">
          Prepare your system and install the recovery environment onto your USB
          drive.
        </p>
        <div class="mt-3 flex gap-2">
          <Button onclick={handleGetStarted}>Get started</Button>
        </div>
      </div>
    </CardContent>
  </Card>
</div>
