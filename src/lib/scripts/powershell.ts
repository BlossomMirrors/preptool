import { Command } from "@tauri-apps/plugin-shell";
import { resolveResource } from "@tauri-apps/api/path";
import { readDir } from "@tauri-apps/plugin-fs";

export interface PowerShellResult<T = unknown> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
  timestamp?: string;
}

export interface ExecuteScriptOptions {
  scriptName: string;
  args?: Record<string, string>;
}

export async function executePowerShellScript(
  options: ExecuteScriptOptions,
): Promise<string> {
  const { scriptName, args = {} } = options;

  try {
    const scriptPath = await resolveResource(`scripts/${scriptName}`);
    const psArgs = [
      "-ExecutionPolicy",
      "Bypass",
      "-NoProfile",
      "-NoLogo",
      "-File",
      scriptPath,
    ];

    for (const [key, value] of Object.entries(args)) {
      psArgs.push(`-${key}`);
      if (value !== "") psArgs.push(value);
    }

    const command = Command.create("powershell", psArgs);

    return new Promise((resolve, reject) => {
      let output = "";
      let hasError = false;

      command.stdout.on("data", (data) => {
        output += data;
      });

      command.stderr.on("data", (data) => {
        hasError = true;
        output += data;
      });

      command.on("close", (data) => {
        if (data.code === 0) {
          // Extract the last valid JSON or last non-empty line
          const rawOutput = output;

          // Look for the last '{' or '[' and try to parse from there
          let lastJsonStart = -1;
          for (let i = rawOutput.length - 1; i >= 0; i--) {
            if (rawOutput[i] === "}" || rawOutput[i] === "]") {
              lastJsonStart = i;
              break;
            }
          }

          if (lastJsonStart > -1) {
            // Search backwards from the last bracket to find the opening bracket
            let braceCount = 0;
            let bracketCount = 0;
            let startPos = lastJsonStart;
            const lastChar = rawOutput[lastJsonStart];

            if (lastChar === "}") {
              braceCount = 1;
              for (let i = lastJsonStart - 1; i >= 0; i--) {
                if (rawOutput[i] === "}") braceCount++;
                else if (rawOutput[i] === "{") {
                  braceCount--;
                  if (braceCount === 0) {
                    startPos = i;
                    break;
                  }
                }
              }
            } else if (lastChar === "]") {
              bracketCount = 1;
              for (let i = lastJsonStart - 1; i >= 0; i--) {
                if (rawOutput[i] === "]") bracketCount++;
                else if (rawOutput[i] === "[") {
                  bracketCount--;
                  if (bracketCount === 0) {
                    startPos = i;
                    break;
                  }
                }
              }
            }

            const jsonStr = rawOutput.substring(startPos, lastJsonStart + 1);
            try {
              JSON.parse(jsonStr);
              resolve(jsonStr);
              return;
            } catch {
              // Fall through to last line logic
            }
          }

          // Fallback: just get the last non-empty line
          const lines = rawOutput.split("\n");
          for (let i = lines.length - 1; i >= 0; i--) {
            const trimmed = lines[i].trim();
            if (trimmed) {
              resolve(trimmed);
              return;
            }
          }

          resolve("");
        } else {
          reject(
            new Error(
              output || `Script exited with code ${data.code}`
            )
          );
        }
      });

      command.on("error", (err) => {
        reject(err);
      });

      command.spawn().catch(reject);
    });
  } catch (error) {
    const errorMsg = String(error);
    if (
      errorMsg.includes("No such file or directory") ||
      errorMsg.includes("ENOENT")
    ) {
      throw new Error(
        "PowerShell is not available on this system. This feature requires Windows with PowerShell or PowerShell Core (pwsh) installed.",
      );
    }
    throw new Error(`PowerShell execution failed: ${error}`);
  }
}

export async function executePowerShellScriptWithProgress(
  options: ExecuteScriptOptions,
  onProgress?: (message: string) => void,
): Promise<string> {
  const { scriptName, args = {} } = options;

  try {
    const scriptPath = await resolveResource(`scripts/${scriptName}`);
    const psArgs = [
      "-ExecutionPolicy",
      "Bypass",
      "-NoProfile",
      "-NoLogo",
      "-File",
      scriptPath,
    ];

    for (const [key, value] of Object.entries(args)) {
      psArgs.push(`-${key}`);
      if (value !== "") psArgs.push(value);
    }

    const command = Command.create("powershell", psArgs);

    return new Promise((resolve, reject) => {
      let output = "";

      command.stdout.on("data", (data) => {
        output += data;
        
        // Check for progress messages
        const lines = data.split("\n");
        for (const line of lines) {
          if (line.includes("[PROGRESS]")) {
            const message = line.replace("[PROGRESS]", "").trim();
            if (message && onProgress) {
              onProgress(message);
            }
          }
        }
      });

      command.stderr.on("data", (data) => {
        output += data;
      });

      command.on("close", (data) => {
        if (data.code === 0) {
          // Extract the last valid JSON or last non-empty line
          const rawOutput = output;

          // Look for the last '{' or '[' and try to parse from there
          let lastJsonStart = -1;
          for (let i = rawOutput.length - 1; i >= 0; i--) {
            if (rawOutput[i] === "}" || rawOutput[i] === "]") {
              lastJsonStart = i;
              break;
            }
          }

          if (lastJsonStart > -1) {
            // Search backwards from the last bracket to find the opening bracket
            let braceCount = 0;
            let bracketCount = 0;
            let startPos = lastJsonStart;
            const lastChar = rawOutput[lastJsonStart];

            if (lastChar === "}") {
              braceCount = 1;
              for (let i = lastJsonStart - 1; i >= 0; i--) {
                if (rawOutput[i] === "}") braceCount++;
                else if (rawOutput[i] === "{") {
                  braceCount--;
                  if (braceCount === 0) {
                    startPos = i;
                    break;
                  }
                }
              }
            } else if (lastChar === "]") {
              bracketCount = 1;
              for (let i = lastJsonStart - 1; i >= 0; i--) {
                if (rawOutput[i] === "]") bracketCount++;
                else if (rawOutput[i] === "[") {
                  bracketCount--;
                  if (bracketCount === 0) {
                    startPos = i;
                    break;
                  }
                }
              }
            }

            const jsonStr = rawOutput.substring(startPos, lastJsonStart + 1);
            try {
              JSON.parse(jsonStr);
              resolve(jsonStr);
              return;
            } catch {
              // Fall through to last line logic
            }
          }

          // Fallback: just get the last non-empty line
          const lines = rawOutput.split("\n");
          for (let i = lines.length - 1; i >= 0; i--) {
            const trimmed = lines[i].trim();
            if (trimmed) {
              resolve(trimmed);
              return;
            }
          }

          resolve("");
        } else {
          reject(
            new Error(
              output || `Script exited with code ${data.code}`
            )
          );
        }
      });

      command.on("error", (err) => {
        reject(err);
      });

      command.spawn().catch(reject);
    });
  } catch (error) {
    const errorMsg = String(error);
    if (
      errorMsg.includes("No such file or directory") ||
      errorMsg.includes("ENOENT")
    ) {
      throw new Error(
        "PowerShell is not available on this system. This feature requires Windows with PowerShell or PowerShell Core (pwsh) installed.",
      );
    }
    throw new Error(`PowerShell execution failed: ${error}`);
  }
}

export async function executePowerShellScriptJson<T = unknown>(
  options: ExecuteScriptOptions,
): Promise<PowerShellResult<T>> {
  const output = await executePowerShellScript(options);

  try {
    // Ensure we have valid JSON
    if (!output || (!output.startsWith('{') && !output.startsWith('['))) {
      return { success: false, error: `Invalid output: ${output}` };
    }
    return JSON.parse(output) as PowerShellResult<T>;
  } catch (error) {
    return { success: false, error: `Failed to parse JSON: ${error}` };
  }
}

export async function listPowerShellScripts(): Promise<string[]> {
  try {
    const scriptsPath = await resolveResource("scripts");
    const entries = await readDir(scriptsPath);

    return entries
      .filter((entry) => entry.name?.endsWith(".ps1"))
      .map((entry) => entry.name!)
      .sort();
  } catch (error) {
    console.error("Failed to list PowerShell scripts:", error);
    return [];
  }
}

export const PowerShellScripts = {
  async example(message?: string, name?: string): Promise<PowerShellResult> {
    const args: Record<string, string> = {};
    if (message) args.Message = message;
    if (name) args.Name = name;

    return executePowerShellScriptJson({ scriptName: "example.ps1", args });
  },

  async getSystemInfo(detailed = false): Promise<PowerShellResult> {
    const args: Record<string, string> = {};
    if (detailed) args.Detailed = "";

    return executePowerShellScriptJson({
      scriptName: "get-system-info.ps1",
      args,
    });
  },

  async custom(
    scriptName: string,
    args?: Record<string, string>,
  ): Promise<string> {
    return executePowerShellScript({ scriptName, args });
  },

  async runScript(
    scriptId: string,
    args?: Record<string, string>,
  ): Promise<PowerShellResult> {
    const scriptMap: Record<string, string> = {
      "disable-fast-startup": "disable-faststartup.ps1",
      "install-winbtrfs": "install-winbtrfs.ps1",
      "set-utc-time": "set-utc-time.ps1",
      partition: "partition.ps1",
      "download-iso": "download-iso.ps1",
      "flash-usb": "flash-usb.ps1",
      "restore-usb": "restore-usb.ps1",
      "reboot-uefi": "reboot-uefi.ps1",
      "install-chocolatey": "install-chocolatey.ps1",
      "get-system-info": "get-system-info.ps1",
      example: "example.ps1",
    };

    const scriptName = scriptMap[scriptId] || `${scriptId}.ps1`;

    try {
      const output = await executePowerShellScript({
        scriptName,
        args,
      });

      return {
        success: true,
        message: output,
        timestamp: new Date().toISOString(),
      };
    } catch (err) {
      return {
        success: false,
        error: String(err),
        timestamp: new Date().toISOString(),
      };
    }
  },

  async listUsbDrives(): Promise<
    PowerShellResult<Array<{ name: string; size: string; diskNumber: number }>>
  > {
    try {
      const output = await executePowerShellScript({
        scriptName: "get-usb-drives.ps1",
      });

      const drives = JSON.parse(output) as Array<{
        name: string;
        size: string;
        diskNumber: number;
      }>;
      return {
        success: true,
        data: drives,
        timestamp: new Date().toISOString(),
      };
    } catch (err) {
      return {
        success: false,
        error: String(err),
        timestamp: new Date().toISOString(),
      };
    }
  },

  async checkIsoExists(): Promise<
    PowerShellResult<{ exists: boolean; path: string }>
  > {
    try {
      const output = await executePowerShellScript({
        scriptName: "check-iso-cache.ps1",
      });

      const result = JSON.parse(output) as { exists: boolean; path: string };
      return {
        success: true,
        data: result,
        timestamp: new Date().toISOString(),
      };
    } catch (err) {
      return {
        success: false,
        error: String(err),
        timestamp: new Date().toISOString(),
      };
    }
  },

  async downloadIsoWithProgress(
    onProgress?: (message: string) => void,
  ): Promise<PowerShellResult<{ path: string }>> {
    try {
      const output = await executePowerShellScriptWithProgress(
        { scriptName: "download-iso.ps1" },
        onProgress,
      );

      const result = JSON.parse(output) as {
        success: boolean;
        message: string;
        path: string;
      };
      return {
        success: result.success,
        data: result.success ? { path: result.path } : undefined,
        message: result.message,
        timestamp: new Date().toISOString(),
      };
    } catch (err) {
      return {
        success: false,
        error: String(err),
        timestamp: new Date().toISOString(),
      };
    }
  }
};