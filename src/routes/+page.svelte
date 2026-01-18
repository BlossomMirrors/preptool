<script lang="ts">
    import { invoke } from "@tauri-apps/api/core";
    import { Button } from "$lib/components/ui/button";
    import {
        Card,
        CardContent,
        CardDescription,
        CardHeader,
        CardTitle,
    } from "$lib/components/ui/card";
    import { Input } from "$lib/components/ui/input";
    import PowerShellDemo from "$lib/components/PowerShellDemo.svelte";

    let name = $state("");
    let greetMsg = $state("");
    let isLoading = $state(false);

    async function greet(event: Event) {
        event.preventDefault();
        if (!name.trim()) return;

        isLoading = true;
        try {
            greetMsg = await invoke("greet", { name });
        } catch (error) {
            greetMsg = "Error: " + String(error);
        } finally {
            isLoading = false;
        }
    }
</script>

<main class="min-h-screen">
    <div class="container mx-auto px-4 py-16">
        <div class="text-center mb-16 space-y-4">
            <h1 class="text-5xl font-bold tracking-tight">PrepTool</h1>
            <p class="text-xl text-muted-foreground max-w-2xl mx-auto">
                Built with Tauri, SvelteKit, and TypeScript
            </p>
        </div>

        <div class="flex justify-center items-center gap-8 mb-16">
            <a
                href="https://tauri.app"
                target="_blank"
                class="transition-transform hover:scale-110"
                aria-label="Tauri"
            >
                <img src="/tauri.svg" class="h-20 w-20" alt="Tauri Logo" />
            </a>
            <a
                href="https://svelte.dev"
                target="_blank"
                class="transition-transform hover:scale-110"
                aria-label="Svelte"
            >
                <img src="/svelte.svg" class="h-20 w-20" alt="Svelte Logo" />
            </a>
            <a
                href="https://vite.dev"
                target="_blank"
                class="transition-transform hover:scale-110"
                aria-label="Vite"
            >
                <img src="/vite.svg" class="h-20 w-20" alt="Vite Logo" />
            </a>
        </div>

        <div class="max-w-2xl mx-auto">
            <Card class="shadow-2xl">
                <CardHeader>
                    <CardTitle class="text-2xl">Welcome to PrepTool</CardTitle>
                    <CardDescription>
                        Enter your name below to receive a personalized greeting
                        from Rust
                    </CardDescription>
                </CardHeader>

                <CardContent>
                    <PowerShellDemo />
                </CardContent>
            </Card>
        </div>
    </div>
</main>
