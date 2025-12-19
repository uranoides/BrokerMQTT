using System.Text.Json;

namespace MQTT.Sharing.Helpers
{
    public static class JsonHelper
    {
        public static string ToJson<T>(T data, bool indent = false)
        {
            if (data == null) return "{}";

            var options = new JsonSerializerOptions
            {
                WriteIndented = indent,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(data, options);
        }
    }
}
