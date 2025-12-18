using MQTT.Sharing.Models;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MQTT.Sharing.Utilities
{
    public class JsonManager
    {
        private string JsonPathFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ConnectionSettings.json";
        private string JsonBlebPayloadPathFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\BlebPayload.json";
        public async Task<ConnectionSettings> GetConnectionByIdAsync(int connectionId)
        {
            List<ConnectionSettings> allEntries = await ReadJsonAsync();

            ConnectionSettings foundEntry = allEntries.FirstOrDefault(e => e.ConnectionId == connectionId);

            if (foundEntry == null)
            {
                Console.WriteLine($"Connessione con ID {connectionId} non trovata nel file JSON.");
            }

            return foundEntry;
        }
        public async Task<List<ConnectionSettings>> ReadJsonAsync()
        {
            if (!File.Exists(JsonPathFile))
            {
                return new List<ConnectionSettings>();
            }

            try
            {
                using (FileStream openStream = File.OpenRead(JsonPathFile))
                {
                    var entries = await JsonSerializer.DeserializeAsync<List<ConnectionSettings>>(openStream);
                    return entries ?? new List<ConnectionSettings>();
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Errore di parsing JSON: {ex.Message}");
                return new List<ConnectionSettings>();
            }
        }
        public async Task<string> LoadAndDeserializeFromResource()
        {
            try
            {
                using (FileStream fileStream = new FileStream(JsonBlebPayloadPathFile, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"ERRORE: File non trovato al percorso: {JsonBlebPayloadPathFile}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRORE durante la lettura o deserializzazione del file: {ex.Message}");
                return null;
            }
        }
        public async Task<SensorPayload> ReadJsonSensorPayloadAsync(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                Console.WriteLine("Errore: La stringa JSON è vuota o null.");
                return null;
            }

            try
            {
                var sensorData = JsonSerializer.Deserialize<SensorPayload>(jsonString);

                if (sensorData != null)
                {
                    Console.WriteLine("Deserializzazione riuscita.");
                    if (sensorData.Rssi.HasValue)
                    {
                        Console.WriteLine($"RSSI presente: {sensorData.Rssi.Value}");
                    }
                    else
                    {
                        Console.WriteLine("RSSI non presente nel JSON.");
                    }
                    return sensorData;
                }
                else
                {
                    Console.WriteLine("Errore: La deserializzazione ha prodotto un oggetto null.");
                    return null;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Errore di Deserializzazione JSON: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore generico durante la deserializzazione: {ex.Message}");
                return null;
            }
        }
        public async Task SaveJsonAsync(List<ConnectionSettings> entries)
        {
            using (FileStream createStream = File.Create(JsonPathFile))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                await JsonSerializer.SerializeAsync(createStream, entries, options);
            }
            Console.WriteLine($"File JSON salvato con successo in: {JsonPathFile}");
        }
        public async Task UpdateAndSaveEntryAsync(int processIdToUpdate, ConnectionSettings newValue)
        {
            List<ConnectionSettings> entries = await ReadJsonAsync();

            ConnectionSettings entryToUpdate = entries.FirstOrDefault(e => e.ConnectionId == processIdToUpdate);

            if (entryToUpdate != null)
            {
                entryToUpdate.Address = newValue.Address;
                entryToUpdate.Port = newValue.Port;
                entryToUpdate.Username = newValue.Username;
                entryToUpdate.Password = newValue.Password;
                entryToUpdate.TagVariablesFileName = newValue.TagVariablesFileName;
                entryToUpdate.ConnectionId = newValue.ConnectionId;

                await SaveJsonAsync(entries);
            }
            else
            {
                Console.WriteLine($"Elemento con PID {processIdToUpdate} non trovato.");
            }
        }
    }
}
