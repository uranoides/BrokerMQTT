using MQTT.Sharing.Models;
using System.Net;
using System.Text.Json;

namespace MQTT.Sharing.Utilities
{
    public class CustomDataDeserializer
    {
        public static CustomData DeserializeFromXmlAttribute(string escapedJsonString)
        {
            if (string.IsNullOrEmpty(escapedJsonString))
            {
                return null;
            }

            try
            {
                string unescapedJson = WebUtility.HtmlDecode(escapedJsonString);
                CustomData config = JsonSerializer.Deserialize<CustomData>(unescapedJson);

                return config;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Errore di deserializzazione JSON: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore generico: {ex.Message}");
                return null;
            }
        }
    }
}
