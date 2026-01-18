<script lang="ts">
	import { page } from '$app/state';
	import { locales, localizeHref } from '$lib/paraglide/runtime';
	import "./layout.css";

	const { children } = $props();
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
