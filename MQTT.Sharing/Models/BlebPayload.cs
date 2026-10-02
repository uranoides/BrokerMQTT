using System.Text.Json.Serialization;

namespace MQTT.Sharing.Models
{
    public class BlebPayload
    {
        [JsonPropertyName("gateway_ID")]
        public string Gateway_ID { get; set; }

        [JsonPropertyName("sensor_ID")]
        public string Sensor_ID { get; set; }

        [JsonPropertyName("sensor_type")]
        public string Sensor_Type { get; set; }

        [JsonPropertyName("sensor_communication")]
        public string Sensor_Communication { get; set; }

        [JsonPropertyName("sensor_area")]
        public string Sensor_Area { get; set; }

        [JsonPropertyName("sensor_location")]
        public string Sensor_Location { get; set; }

        [JsonPropertyName("sensor_status")]
        public string Sensor_Status { get; set; }

        [JsonPropertyName("sensor_threshold")]
        public long Sensor_Threshold { get; set; }

        [JsonPropertyName("sensor_value")]
        public long Sensor_Value { get; set; }

        [JsonPropertyName("presence")]
        public bool Presence { get; set; }

        [JsonPropertyName("rssi")]
        public int Rssi { get; set; }

        [JsonPropertyName("battery")]
        public decimal Battery { get; set; }

        [JsonPropertyName("LoRaretryON")]
        public bool LoRaretryON { get; set; }

        [JsonPropertyName("LoRaretries")]
        public int LoRaretries { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
}
