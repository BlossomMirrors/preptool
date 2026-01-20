<script lang="ts">
	import { page } from '$app/state';
	import { locales, localizeHref } from '$lib/paraglide/runtime';
	import { attachConsole } from '@tauri-apps/plugin-log';
	import "./layout.css";

	const { children } = $props();

	// Initialize logging on mount
	if (typeof window !== 'undefined') {
		attachConsole().catch(console.error);
	}
</script>

<div class="dark bg-background text-foreground">
	<main class="min-h-screen min-w-screen">
		<div class="mx-auto">
			{@render children()}
		</div>
	</main>
</div>

<div style="display:none">
	{#each locales as locale}
		<a
			href={localizeHref(page.url.pathname, { locale })}
		>
			{locale}
		</a>
	{/each}
</div>
