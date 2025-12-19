using MQTT.Sharing.Enumerations;
using MQTT.Sharing.Models;
using MQTTnet;
using MQTTnet.Formatter;

namespace MQTT.Sharing.Helpers
{
    public static class MqttClientBuilderHelper
    {
        public static object ConfigureMqttOptions(ConnectionSettings connectionSettings)
        {
            var nuovoGuidString = Guid.NewGuid().ToString();
            MqttTransCodeProtocolVersion protocolVersion = (MqttTransCodeProtocolVersion)Convert.ToInt32(connectionSettings.MqttTransCodeProtocolVersion);
            object mqttClientOptions;

            if (connectionSettings.Username != null && connectionSettings.Password != null)
            {
                if (connectionSettings.MqttTransCodeProtocolVersion > 0)
                {
                    mqttClientOptions = new MqttClientOptionsBuilder()
                        .WithClientId(nuovoGuidString)
                        .WithTcpServer(connectionSettings.Address, connectionSettings.Port)
                        .WithCredentials(connectionSettings.Username, connectionSettings.Password)
                        .WithProtocolVersion((MqttProtocolVersion)protocolVersion)
                        .Build();
                }
                else
                {
                    mqttClientOptions = new MqttClientOptionsBuilder()
                        .WithClientId(nuovoGuidString)
                        .WithTcpServer(connectionSettings.Address, connectionSettings.Port)
                        .WithCredentials(connectionSettings.Username, connectionSettings.Password)
                        .Build();
                }
            }
            else
            {
                if (connectionSettings.MqttTransCodeProtocolVersion > 0)
                {
                    mqttClientOptions = new MqttClientOptionsBuilder()
                        .WithClientId(nuovoGuidString)
                        .WithTcpServer(connectionSettings.Address, connectionSettings.Port)
                        .WithProtocolVersion((MqttProtocolVersion)protocolVersion)
                        .Build();
                }
                else
                {
                    mqttClientOptions = new MqttClientOptionsBuilder()
                        .WithClientId(nuovoGuidString)
                        .WithTcpServer(connectionSettings.Address, connectionSettings.Port)
                        .Build();
                }
            }

            return mqttClientOptions;
        }
    }
}
