using MQTTnet;
using MQTTnet.Protocol;
using System.Text;

namespace MQTT.Sharing.Utilities
{
    public class PublisherManager
    {
        private readonly IMqttClient mqttClient;
        private string Address;
        private int Port;

        public PublisherManager(string address, int port)
        {
            this.Address = address;
            this.Port = port;

            MqttClientFactory mqttFactory = new();
            mqttClient = mqttFactory.CreateMqttClient();
        }
        public async Task<bool> ConnectAsync()
        {
            string nuovoGuidString = Guid.NewGuid().ToString();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(Address, Port)
                .WithClientId(nuovoGuidString)
                .WithCleanSession(true)
                .Build();

            try
            {
                var result = await mqttClient.ConnectAsync(options, CancellationToken.None);

                if (result.ResultCode == MqttClientConnectResultCode.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public async Task PublishMessageAsync(string topic, string payload)
        {
            if (!mqttClient.IsConnected)
            {
                if (!await ConnectAsync()) return;
            }

            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payloadBytes)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag(true)
                .Build();

            var result = await mqttClient.PublishAsync(message, CancellationToken.None);

            if (result.ReasonCode == MqttClientPublishReasonCode.Success)
            {
                Console.WriteLine($"➡️ Pubblicato su '{topic}': {payload}");
            }
            else
            {
                Console.WriteLine($"❌ Errore nella pubblicazione: {result.ReasonCode}");
            }
        }
        public async Task DisconnectAsync()
        {
            if (mqttClient.IsConnected)
            {
                await mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
            }
        }
    }
}
