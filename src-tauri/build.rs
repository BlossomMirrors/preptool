fn main() {
    // Allow cross-compilation: only check target OS, not host OS
    #[cfg(target_os = "windows")]
    {
        // Set Windows subsystem to windows for GUI application
        println!("cargo:rustc-link-arg=/SUBSYSTEM:WINDOWS");

        // Embed manifest for UAC elevation (administrator privileges)
        // Only works when using native Windows compilation or with winresource feature
        #[cfg(feature = "windows-resources")]
        {
            let mut res = winresource::WindowsResource::new();
            res.set_manifest_file("manifest.xml");
            if let Err(e) = res.compile() {
                eprintln!("Warning: Failed to compile Windows resources: {}", e);
            }
        }
    }

    tauri_build::build()
}
