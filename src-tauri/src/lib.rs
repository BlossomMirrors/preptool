use futures_util::StreamExt;
use sha2::{Digest, Sha256};
use std::fs::File;
use std::io::{BufReader, Read, Write};
use std::sync::{
    atomic::{AtomicBool, Ordering},
    Arc,
};
use tauri::Emitter;
use tauri::Manager;
use tauri::Window;
use tauri::State;

#[tauri::command]
fn greet(name: &str) -> String {
    format!("Hello, {}! You've been greeted from Rust!", name)
}

#[tauri::command]
async fn download_file(
    window: Window,
    url: String,
    path: String,
    cancelled: State<'_, Arc<AtomicBool>>,
) -> Result<(), String> {
    // Reset on launch
    cancelled.store(false, Ordering::Relaxed);
    
    // Always delete existing file to prevent corruption from partial downloads
    let _ = std::fs::remove_file(&path);

    let client = reqwest::Client::new();
    let response = client.get(&url).send().await.map_err(|e| e.to_string())?;
    
    // Get the total file size
    let total_size = response.content_length().unwrap_or(0);

    // Create new file
    let mut file = File::create(&path).map_err(|e| e.to_string())?;

    let mut downloaded: u64 = 0;
    let mut stream = response.bytes_stream();

    while let Some(chunk) = stream.next().await {
        if cancelled.load(Ordering::Relaxed) {
            // Clean up incomplete file on cancel
            let _ = std::fs::remove_file(&path);
            return Err("Download cancelled".to_string());
        }
        let chunk = chunk.map_err(|e| e.to_string())?;
        file.write_all(&chunk).map_err(|e| e.to_string())?;
        downloaded += chunk.len() as u64;

        window
            .emit("download-progress", (downloaded, total_size))
            .ok();
    }

    // Ensure all data is flushed to disk
    file.flush().map_err(|e| e.to_string())?;
    file.sync_all().map_err(|e| e.to_string())?;
    drop(file); // Explicitly close the file

    // Verify the file was actually written
    let final_size = std::fs::metadata(&path)
        .map_err(|e| e.to_string())?
        .len();
    
    if final_size == 0 {
        return Err("Downloaded file is empty".to_string());
    }

    Ok(())
}

#[tauri::command]
fn cancel_download(cancelled: tauri::State<Arc<AtomicBool>>) {
    cancelled.store(true, Ordering::Relaxed);
}

#[tauri::command]
async fn verify_sha256(path: String, expected_sha256: String) -> Result<bool, String> {
    let file = File::open(&path).map_err(|e| e.to_string())?;
    let mut reader = BufReader::new(file);
    let mut hasher = Sha256::new();

    let mut buffer = [0u8; 8192];
    loop {
        let bytes_read = reader.read(&mut buffer).map_err(|e| e.to_string())?;
        if bytes_read == 0 {
            break;
        }
        hasher.update(&buffer[..bytes_read]);
    }

    let result = hasher.finalize();
    let actual = hex::encode(result);

    Ok(actual.eq_ignore_ascii_case(&expected_sha256))
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .manage(Arc::new(AtomicBool::new(false)))
        .plugin(tauri_plugin_http::init())
        .plugin(tauri_plugin_fs::init())
        .plugin(tauri_plugin_shell::init())
        .plugin(tauri_plugin_single_instance::init(|app, _args, _cwd| {
            let _ = app
                .get_webview_window("main")
                .expect("no main window")
                .set_focus();
        }))
        .plugin(tauri_plugin_opener::init())
        .invoke_handler(tauri::generate_handler![
            greet,
            download_file,
            verify_sha256,
            cancel_download
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
