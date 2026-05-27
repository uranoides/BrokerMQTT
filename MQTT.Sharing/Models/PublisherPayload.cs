using System.Text.Json.Serialization;

namespace MQTT.Sharing.Models
{
    public class PublisherPayload
    {
        [JsonPropertyName("sensor_area")]
        public string SensorArea { get; set; }

        [JsonPropertyName("sensor_location")]
        public string SensorLocation { get; set; }

        [JsonPropertyName("sensor_status")]
        public string SensorStatus { get; set; }

        [JsonPropertyName("presence")]
        public bool Presence { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("battery")]
        public int Battery { get; set; }
    }
}
