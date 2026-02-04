using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace BlossomPrepTool
{
    /// <summary>
    /// Utility class for running PowerShell scripts and parsing JSON output
    /// </summary>
    public class PowerShellRunner
    {
        public class PowerShellOutput
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public Dictionary<string, object> Data { get; set; }
            public string RawOutput { get; set; }

            public bool IsSuccess => Status == "success";
            public bool IsError => Status == "error";
            public bool IsWarning => Status == "warning";
            public bool IsInfo => Status == "info";
        }

        public static PowerShellOutput ExecuteScript(string scriptContent, Dictionary<string, string> parameters = null)
        {
            var output = new StringBuilder();
            var error = new StringBuilder();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = BuildArguments(scriptContent, parameters),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    output.Append(process.StandardOutput.ReadToEnd());
                    error.Append(process.StandardError.ReadToEnd());
                    process.WaitForExit();

                    if (process.ExitCode != 0 && error.Length > 0)
                    {
                        return new PowerShellOutput
                        {
                            Status = "error",
                            Message = $"PowerShell exited with code {process.ExitCode}",
                            RawOutput = error.ToString(),
                            Data = new Dictionary<string, object>()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new PowerShellOutput
                {
                    Status = "error",
                    Message = $"Failed to execute PowerShell: {ex.Message}",
                    RawOutput = ex.ToString(),
                    Data = new Dictionary<string, object>()
                };
            }

            return ParseOutput(output.ToString());
        }

        public static PowerShellOutput ExecuteScriptFile(string scriptPath, Dictionary<string, string> parameters = null)
        {
            var scriptContent = System.IO.File.ReadAllText(scriptPath);
            return ExecuteScript(scriptContent, parameters);
        }

        private static string BuildArguments(string scriptContent, Dictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            sb.Append("-NoProfile -ExecutionPolicy Bypass -Command \"");

            // Escape the script content for PowerShell
            var escapedScript = scriptContent.Replace("\"", "\\\"");
            sb.Append(escapedScript);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var param in parameters)
                {
                    sb.Append($" -{param.Key} '{param.Value.Replace("'", "''")}'");
                }
            }

            sb.Append("\"");
            return sb.ToString();
        }

        private static PowerShellOutput ParseOutput(string output)
        {
            if (string.IsNullOrWhiteSpace(output))
            {
                return new PowerShellOutput
                {
                    Status = "error",
                    Message = "No output from PowerShell",
                    Data = new Dictionary<string, object>(),
                    RawOutput = output
                };
            }

            var result = new PowerShellOutput { RawOutput = output, Data = new Dictionary<string, object>() };

            // Try to parse JSON output
            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    var json = JObject.Parse(line);
                    result.Status = json["status"]?.ToString() ?? "unknown";
                    result.Message = json["message"]?.ToString() ?? "";

                    // Extract additional data
                    foreach (var prop in json.Properties())
                    {
                        if (prop.Name != "status" && prop.Name != "message")
                        {
                            result.Data[prop.Name] = prop.Value;
                        }
                    }

                    return result;
                }
                catch
                {
                    // Not JSON, continue
                }
            }

            // If no JSON found, return raw output as info
            result.Status = "info";
            result.Message = output;
            return result;
        }
    }
}
