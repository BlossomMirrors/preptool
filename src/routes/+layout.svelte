<script lang="ts">
	import { page } from '$app/state';
	import { locales, localizeHref } from '$lib/paraglide/runtime';
	import { attachConsole, trace, debug, info, warn, error as logError } from '@tauri-apps/plugin-log';
	import "./layout.css";

	const { children } = $props();

	// Initialize logging on mount
	if (typeof window !== 'undefined') {
		// Attach console to forward all console output to logger
		attachConsole().catch(console.error);

		// Override console methods to also log them directly
		const originalLog = console.log;
		const originalDebug = console.debug;
		const originalInfo = console.info;
		const originalWarn = console.warn;
		const originalError = console.error;

		console.log = (...args) => {
			originalLog(...args);
			trace(args.map(arg => (typeof arg === 'string' ? arg : JSON.stringify(arg))).join(' ')).catch(() => {});
		};

		console.debug = (...args) => {
			originalDebug(...args);
			debug(args.map(arg => (typeof arg === 'string' ? arg : JSON.stringify(arg))).join(' ')).catch(() => {});
		};

		console.info = (...args) => {
			originalInfo(...args);
			info(args.map(arg => (typeof arg === 'string' ? arg : JSON.stringify(arg))).join(' ')).catch(() => {});
		};

		console.warn = (...args) => {
			originalWarn(...args);
			warn(args.map(arg => (typeof arg === 'string' ? arg : JSON.stringify(arg))).join(' ')).catch(() => {});
		};

		console.error = (...args) => {
			originalError(...args);
			logError(args.map(arg => (typeof arg === 'string' ? arg : JSON.stringify(arg))).join(' ')).catch(() => {});
		};
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
