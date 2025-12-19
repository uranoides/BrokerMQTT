using System.Text.Json.Serialization;

namespace MQTT.Sharing.Models
{
    public class PublisherPayload
    {
        [JsonPropertyName("sensor_location")]
        public string SensorLocation { get; set; }

        [JsonPropertyName("sensor_status")]
        public string SensorStatus { get; set; }

        [JsonPropertyName("presence")]
        public bool Presence { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
}
