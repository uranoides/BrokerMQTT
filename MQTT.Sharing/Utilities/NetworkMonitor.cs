using MQTT.Sharing.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MQTT.Sharing.Utilities
{
    public class NetworkMonitor
    {
        public static async Task<string> GetNetstatOutputAsync(string port)
        {
            string command = $"/c netstat -ano | findstr :{port}";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = command,
                    RedirectStandardOutput = true, // Essenziale per catturare la risposta
                    UseShellExecute = false,       // Essenziale per reindirizzare l'output
                    CreateNoWindow = true          // Non mostra la finestra del prompt
                }
            };

            try
            {
                process.Start();

                string result = await process.StandardOutput.ReadToEndAsync();

                await Task.Run(() => process.WaitForExit());

                return result;
            }
            catch (Exception ex)
            {
                return $"Errore nell'esecuzione del comando: {ex.Message}";
            }
        }
        public static List<NetstatEntry> ParseNetstatOutput(string output)
        {
            var entries = new List<NetstatEntry>();

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("Proto")) continue;

                string cleanLine = Regex.Replace(line.Trim(), @"\s+", " ");

                var parts = cleanLine.Split(' ').ToList();

                if (parts.Count >= 5)
                {
                    var entry = new NetstatEntry
                    {
                        Protocol = parts[0],
                        LocalAddress = parts[1],
                        ForeignAddress = parts[2],
                        State = parts.Count > 4 ? parts[3] : string.Empty,
                        ProcessId = int.Parse(parts.Last()) // Prende l'ultimo elemento come PID
                    };

                    if (parts.Count == 5)
                    {
                        if (parts[3].All(char.IsDigit))
                        {
                            entry.State = string.Empty;
                            entry.ProcessId = int.Parse(parts[3]);
                        }
                        else
                        {
                            entry.State = parts[3];
                            entry.ProcessId = int.Parse(parts[4]);
                        }
                    }
                    entries.Add(entry);
                }
            }

            return entries;
        }
    }
}
