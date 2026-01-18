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
      "-File",
      scriptPath,
    ];

    for (const [key, value] of Object.entries(args)) {
      psArgs.push(`-${key}`);
      if (value !== "") psArgs.push(value);
    }

    const command = Command.create("powershell", psArgs);
    const output = await command.execute();

    if (output.code === 0) {
      return output.stdout.trim();
    } else {
      throw new Error(
        output.stderr || `Script exited with code ${output.code}`,
      );
    }
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
    return JSON.parse(output) as PowerShellResult<T>;
  } catch {
    return { success: true, data: output as T };
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
};
