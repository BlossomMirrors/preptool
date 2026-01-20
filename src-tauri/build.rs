fn main() {
    // Only allow building for Windows
    #[cfg(not(target_os = "windows"))]
    {
        panic!("This application can only be built for Windows");
    }

    // Windows-specific build configuration
    #[cfg(target_os = "windows")]
    {
        // Set Windows subsystem to windows for GUI application
        println!("cargo:rustc-link-arg=/SUBSYSTEM:WINDOWS");

        // Embed manifest for UAC elevation (administrator privileges)
        let mut res = winresource::WindowsResource::new();
        res.set_manifest_file("manifest.xml");
        if let Err(e) = res.compile() {
            eprintln!("Warning: Failed to compile Windows resources: {}", e);
        }
    }

    tauri_build::build()
}
