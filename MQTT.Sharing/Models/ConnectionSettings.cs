namespace MQTT.Sharing.Models
{
    public class ConnectionSettings
    {
        public int ConnectionId { get; set; }
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public int MqttTransCodeProtocolVersion { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TagVariablesFileName { get; set; } = string.Empty;
        public int IntervalInMilliseconds { get; set; }
    }
}
