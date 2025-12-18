using System.Text.Json.Serialization;

namespace MQTT.Sharing.Models
{
    public class SensorPayload
    {
        [JsonPropertyName("gateway_ID")]
        public string GatewayId { get; set; } = string.Empty;

        [JsonPropertyName("sensor_ID")]
        public string SensorId { get; set; } = string.Empty;

        [JsonPropertyName("sensor_type")]
        public string SensorType { get; set; } = string.Empty;

        [JsonPropertyName("sensor_communication")]
        public string SensorCommunication { get; set; } = string.Empty;

        [JsonPropertyName("sensor_area")]
        public string SensorArea { get; set; } = string.Empty;

        [JsonPropertyName("sensor_location")]
        public string SensorLocation { get; set; } = string.Empty;

        [JsonPropertyName("sensor_status")]
        public string SensorStatus { get; set; } = string.Empty;

        [JsonPropertyName("sensor_value")]
        public int SensorValue { get; set; }

        public bool Presence { get; set; }

        public DateTime Timestamp { get; set; }

        [JsonPropertyName("sensor_threshold")]
        public int? SensorThreshold { get; set; }

        public int? Rssi { get; set; }

        public double? Battery { get; set; }

        [JsonPropertyName("LoRaretryON")]
        public bool? LoRaRetryOn { get; set; }

        [JsonPropertyName("LoRaretries")]
        public int? LoRaRetries { get; set; }
    }
}
