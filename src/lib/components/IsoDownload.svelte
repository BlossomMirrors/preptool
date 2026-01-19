<script lang="ts">
  import { invoke } from "@tauri-apps/api/core";
  import { listen } from "@tauri-apps/api/event";
  import { createEventDispatcher } from "svelte";
  import { Button } from "$lib/components/ui/button";
  import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
  } from "$lib/components/ui/card";
  import { downloadDir } from "@tauri-apps/api/path";
  import { exists, remove } from "@tauri-apps/plugin-fs";
  import { fetch } from "@tauri-apps/plugin-http";

  const dispatch = createEventDispatcher<{
    "download-complete": { isoPath: string };
  }>();

  let url = "";
  let downloadPath = "";
  let expectedHash = "";

  let isDownloading = false;
  let isVerifying = false;
  let wasCancelled = false;

  let downloadProgress = 0;
  let downloadSize = 0;
  let downloadedSize = 0;

  let error: string | null = null;
  let success: string | null = null;

  let unlistenProgress: (() => void) | null = null;

  downloadDir().then((dir) => {
    downloadPath = dir ? `${dir}\\BlossomOS.iso` : "";
  });

  async function fetchIsoMetadata() {
    try {
      const response = await fetch(
        "https://cdn.blossomos.org/iso/isodata.json",
      );
      if (!response.ok) {
        error = `Failed to fetch ISO metadata: HTTP ${response.status}`;
        return;
      }
      const data = await response.json();
      expectedHash = data.sha256;
      url = "https://cdn.blossomos.org/iso/" + data.name;
    } catch (err) {
      error = `Failed to fetch ISO metadata: ${String(err)}`;
    }
  }

  fetchIsoMetadata();

  async function startDownload() {
    if (!url || !downloadPath || !expectedHash) {
      error = "Missing download data";
      return;
    }

    error = null;
    success = null;
    wasCancelled = false;
    isDownloading = true;

    if (unlistenProgress) {
      unlistenProgress();
      unlistenProgress = null;
    }

    // Check if file already exists and validate it
    if (downloadPath && (await exists(downloadPath))) {
      try {
        isVerifying = true;
        isDownloading = false;
        const isValid = await invoke<boolean>("verify_sha256", {
          path: downloadPath,
          expectedSha256: expectedHash,
        });

        if (!isValid) {
          // File exists but SHA256 doesn't match, delete it
          await remove(downloadPath);
          isVerifying = false;
        } else {
          // File exists and is valid, skip download
          isVerifying = false;
          downloadProgress = 100;
          success = "File already downloaded and verified.";
          dispatch("download-complete", { isoPath: downloadPath });
          return;
        }
      } catch (err) {
        // If verification fails, delete the corrupted file
        await remove(downloadPath);
      }
    }

    unlistenProgress = await listen<[number, number]>(
      "download-progress",
      (event) => {
        if (!isDownloading) return;

        const [downloaded, total] = event.payload;
        downloadedSize = downloaded;
        downloadSize = total;

        if (total > 0) {
          downloadProgress = (downloaded / total) * 100;
        }
      },
    );

    try {
      await invoke("download_file", {
        url,
        path: downloadPath,
      });

      isDownloading = false;
      if (wasCancelled) {
        downloadProgress = 0;
        downloadedSize = 0;
        downloadSize = 0;
        return;
      }

      isVerifying = true;

      const ok = await invoke<boolean>("verify_sha256", {
        path: downloadPath,
        expectedSha256: expectedHash,
      });

      isVerifying = false;

      if (ok) {
        downloadProgress = 100;
        success = "Download completed and verified.";
        dispatch("download-complete", { isoPath: downloadPath });
      } else {
        error = "SHA256 mismatch.";
        downloadProgress = 0;
      }
    } catch (err) {
      isDownloading = false;
      isVerifying = false;
      error = String(err);
    } finally {
      if (unlistenProgress) {
        unlistenProgress();
        unlistenProgress = null;
      }
    }
  }

  async function cancelDownload() {
    isDownloading = false;
    isVerifying = false;
    wasCancelled = true;

    downloadProgress = 0;
    downloadedSize = 0;
    downloadSize = 0;

    error = null;
    success = null;

    await invoke("cancel_download");

    if (unlistenProgress) {
      unlistenProgress();
      unlistenProgress = null;
    }

    if (downloadPath && (await exists(downloadPath))) {
      await remove(downloadPath);
    }
  }

  function formatBytes(bytes: number): string {
    if (!bytes) return "0 Bytes";
    const k = 1024;
    const sizes = ["Bytes", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return `${(bytes / Math.pow(k, i)).toFixed(2)} ${sizes[i]}`;
  }
</script>

<Card class="bg-zinc-900 border-zinc-800 w-full">
  <CardHeader>
    <CardTitle class="text-white">ISO Download</CardTitle>
    <CardDescription>Download and verify BlossomOS ISO</CardDescription>
  </CardHeader>
  <CardContent class="space-y-4">
    {#if isDownloading || isVerifying}
      <div class="space-y-2">
        <div class="flex justify-between items-center">
          <span class="text-sm text-zinc-300">
            {isVerifying
              ? "Verifying SHA256..."
              : `Downloading: ${formatBytes(downloadedSize)} / ${formatBytes(downloadSize)}`}
          </span>
          <span class="text-sm font-semibold text-blue-400"
            >{downloadProgress.toFixed(1)}%</span
          >
        </div>
        <div class="w-full bg-zinc-800 rounded-full h-2 overflow-hidden">
          <div
            class="bg-blue-500 h-full transition-all duration-300"
            style="width: {downloadProgress}%"
          ></div>
        </div>
        <div class="flex gap-2">
          <Button onclick={cancelDownload} variant="destructive" class="flex-1" disabled={!isDownloading || isVerifying}>
            Cancel
          </Button>
        </div>
      </div>
    {/if}

    {#if error}
      <div
        class="p-3 bg-red-900/30 border border-red-700 rounded text-red-200 text-sm"
      >
        {error}
      </div>
    {/if}

    {#if success}
      <div
        class="p-3 bg-green-900/30 border border-green-700 rounded text-green-200 text-sm"
      >
        {success}
      </div>
    {/if}

    <div class="flex gap-2">
      <Button
        onclick={startDownload}
        disabled={isDownloading || isVerifying}
        class="flex-1"
      >
        {isVerifying
          ? "Verifying..."
          : isDownloading
            ? "Downloading..."
            : "Download ISO"}
      </Button>
    </div>
  </CardContent>
</Card>
