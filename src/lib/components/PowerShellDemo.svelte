<script lang="ts">
    import { Button } from "$lib/components/ui/button";
    import {
        Card,
        CardContent,
        CardDescription,
        CardHeader,
        CardTitle,
    } from "$lib/components/ui/card";
    import { Input } from "$lib/components/ui/input";
    import { PowerShellScripts, listPowerShellScripts } from "$lib/scripts";

    let availableScripts = $state<string[]>([]);
    let isLoading = $state(false);
    let result = $state<string>("");
    let error = $state<string>("");

    let exampleName = $state("World");
    let exampleMessage = $state("Hello from PrepTool!");

    $effect(() => {
        loadScripts();
    });

    async function loadScripts() {
        try {
            availableScripts = await listPowerShellScripts();
        } catch (err) {
            error = `Failed to load scripts: ${err}`;
        }
    }

    async function runExampleScript() {
        isLoading = true;
        error = "";
        result = "";

        try {
            const response = await PowerShellScripts.example(
                exampleMessage,
                exampleName,
            );

            if (response.success) {
                result = JSON.stringify(response, null, 2);
            } else {
                error = response.error || "Script execution failed";
            }
        } catch (err) {
            error = String(err);
        } finally {
            isLoading = false;
        }
    }

    async function runSystemInfo(detailed: boolean = false) {
        isLoading = true;
        error = "";
        result = "";

        try {
            const response = await PowerShellScripts.getSystemInfo(detailed);

            if (response.success) {
                result = JSON.stringify(response.data, null, 2);
            } else {
                error = response.error || "Script execution failed";
            }
        } catch (err) {
            error = String(err);
        } finally {
            isLoading = false;
        }
    }
</script>

<div class="space-y-6">
    <Card>
        <CardHeader>
            <CardTitle>PowerShell Scripts</CardTitle>
            <CardDescription>
                Execute PowerShell scripts from your Tauri application
            </CardDescription>
        </CardHeader>
        <CardContent class="space-y-4">
            <div class="space-y-2">
                <h3 class="text-sm font-medium">Available Scripts:</h3>
                {#if availableScripts.length > 0}
                    <div class="flex flex-wrap gap-2">
                        {#each availableScripts as script}
                            <span
                                class="px-3 py-1 bg-muted rounded-md text-sm font-mono"
                            >
                                {script}
                            </span>
                        {/each}
                    </div>
                {:else}
                    <p class="text-sm text-muted-foreground">
                        No scripts found
                    </p>
                {/if}
            </div>

            <div class="border-t pt-4 space-y-4">
                <h3 class="text-sm font-medium">Example Script</h3>

                <div class="grid grid-cols-2 gap-3">
                    <div class="space-y-2">
                        <label for="name" class="text-sm">Name</label>
                        <Input
                            id="name"
                            bind:value={exampleName}
                            placeholder="Enter a name"
                            disabled={isLoading}
                        />
                    </div>
                    <div class="space-y-2">
                        <label for="message" class="text-sm">Message</label>
                        <Input
                            id="message"
                            bind:value={exampleMessage}
                            placeholder="Enter a message"
                            disabled={isLoading}
                        />
                    </div>
                </div>

                <Button
                    onclick={runExampleScript}
                    disabled={isLoading}
                    class="w-full"
                >
                    {isLoading ? "Running..." : "Run Example Script"}
                </Button>
            </div>

            <div class="border-t pt-4 space-y-3">
                <h3 class="text-sm font-medium">System Information</h3>
                <div class="grid grid-cols-2 gap-3">
                    <Button
                        onclick={() => runSystemInfo(false)}
                        disabled={isLoading}
                        variant="outline"
                    >
                        Get Basic Info
                    </Button>
                    <Button
                        onclick={() => runSystemInfo(true)}
                        disabled={isLoading}
                        variant="outline"
                    >
                        Get Detailed Info
                    </Button>
                </div>
            </div>

            {#if error}
                <div
                    class="mt-4 p-4 rounded-lg bg-destructive/10 border border-destructive/20"
                >
                    <p class="text-sm font-medium text-destructive">Error:</p>
                    <pre
                        class="mt-2 text-xs text-destructive/80 whitespace-pre-wrap break-all">{error}</pre>
                </div>
            {/if}

            {#if result}
                <div class="mt-4 p-4 rounded-lg bg-muted border border-border">
                    <p class="text-sm font-medium mb-2">Result:</p>
                    <pre
                        class="text-xs overflow-x-auto whitespace-pre-wrap break-all font-mono">{result}</pre>
                </div>
            {/if}
        </CardContent>
    </Card>
</div>
