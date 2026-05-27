using System.Text.Json.Serialization;

namespace MQTT.Sharing.Models
{
    public class CustomData
    {
        public string Field { get; set; }
        public string Status { get; set; }
        public string Area { get; set; }
        public string AreaValue { get; set; }
        public string Location { get; set; }

        [JsonPropertyName("StatusValidValue")]
        public string ValidValue { get; set; }

        [JsonPropertyName("StatusErrorValue")]
        public string ErrorValue { get; set; }

        [JsonPropertyName("Sensor")]
        public string Sensor { get; set; }
    }
}
