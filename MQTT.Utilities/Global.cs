using System.Text.Json.Serialization;

namespace MQTT.Utilities
{
    #region Global
    public static class Global
    {
        #region Topics
        public static List<string> TestTopics = new List<string>
        {
            "blebimola/palletizer1",
            "blebimola/palletizer2",
            "blebimola/unload_udc_01",
            "blebimola/unload_udc_02",
            "blebimola/unload_udc_03",
        };
        public static string GetRandomTestTopic()
        {
            int randomIndex = Random.Shared.Next(TestTopics.Count);

            return TestTopics[randomIndex];
        }
        #endregion

        #region Models
        public class ConnectionSettings
        {
            public int ConnectionId { get; set; }
            public string Address { get; set; } = string.Empty;
            public int Port { get; set; }
            public int MqttTransCodeProtocolVersion { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string TagVariablesFileName { get; set; } = string.Empty;
            public int IntervalInSeconds { get; set; }
        }
        public class VariableData
        {
            public int Id { get; set; }
            public string GroupName { get; set; } = string.Empty;
            public string VariableName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string CustomData { get; set; } = string.Empty;
        }
        public class CustomData
        {
            public string Field { get; set; }
            public string Status { get; set; }
            public string Location { get; set; }
            
            [JsonPropertyName("StatusValidValue")]
            public string ValidValue { get; set; }

            [JsonPropertyName("StatusErrorValue")]
            public string ErrorValue { get; set; }

            [JsonPropertyName("Sensor")]
            public string Sensor { get; set; }
        }
        public class NetstatEntry
        {
            public string Protocol { get; set; } = string.Empty;        
            public string LocalAddress { get; set; } = string.Empty;    
            public string ForeignAddress { get; set; } = string.Empty;  
            public string State { get; set; } = string.Empty;           
            public int ProcessId { get; set; }                          
        }
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
        #endregion
    }
    #endregion
}
