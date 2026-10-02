using MQTT.Sharing.Enumerations;
using MQTT.Sharing.Helpers;
using MQTT.Sharing.Models;
using MQTT.Sharing.Utilities;
using MQTTnet.Exceptions;
using System.ComponentModel;
using System.Timers;
using System.Windows.Threading;
using TaskTimer = System.Timers.Timer;

namespace MQTT.Pubblisher.Imola.ViewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Text
        public delegate void TextLeftUpHandler(string Message);
        public event TextLeftUpHandler TextLeftUp;

        public delegate void TextLeftDownHandler(string Message);
        public event TextLeftDownHandler TextLeftDown;
        #endregion

        #region Properties
        private ConnectionSettings connectionSettings;
        public ConnectionSettings ConnectionSettings
        {
            get { return connectionSettings; }
            set
            {
                connectionSettings = value;
                OnPropertyChanged(nameof(ConnectionSettings));
            }
        }
        private bool isPublisherRunning = false;
        public bool IsPublisherRunning
        {
            get { return isPublisherRunning; }
            set
            {
                isPublisherRunning = value;
                OnPropertyChanged(nameof(IsPublisherRunning));
            }
        }
        private List<BlebSensor> blebSensorsPayloads;
        public List<BlebSensor> BlebSensorsPayloads
        {
            get { return blebSensorsPayloads; }
            set
            {
                blebSensorsPayloads = value;
                OnPropertyChanged(nameof(BlebSensorsPayloads));
            }
        }
        #endregion

        #region Builder
        public async Task LoadAsync()
        {
            ConnectionSettings = new ConnectionSettings()
            {
                ConnectionId = 1,
                Address = "192.168.0.XX",
                Port = 1883,
                MqttTransCodeProtocolVersion = 3,
                IntervalInMilliseconds = 30000,
                TagVariablesFileName = string.Empty,
                Username = string.Empty,
                Password = string.Empty,
            };
            InitializeTaskTimer();
            BlebSensorsPayloads = new List<BlebSensor>();
        }
        #endregion

        #region Methods
        public void ToggleTimerLogic()
        {
            if (!IsPublisherRunning)
            {
                taskTimer.Start();
                IsPublisherRunning = true;
                TextLeftUp?.Invoke($"Publisher Task Started..");
                Task.Run(async () => await ExecuteMainTaskAsync());
            }
            else
            {
                taskTimer.Stop();
                IsPublisherRunning = false;
                BlebSensorsPayloads = new List<BlebSensor>();
                //ResetAllCounters();
                TextLeftUp?.Invoke("Publisher Task Stopped..");
            }
        }
        #endregion

        #region Publisher
        private TaskTimer taskTimer;
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private void InitializeTaskTimer()
        {
            taskTimer = new TaskTimer(ConnectionSettings.IntervalInMilliseconds);
            taskTimer.Elapsed += TaskTimer_Elapsed;
            taskTimer.AutoReset = true;
        }
        private async void TaskTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await ExecuteMainTaskAsync();
        }
        private async Task ExecuteMainTaskAsync()
        {
            if (!IsPublisherRunning) return;

            try
            {
                if (connectionSettings == null)
                {
                    TextLeftDown?.Invoke("Errore: ConnectionSettings non ancora caricate.");
                    return;
                }

                var manager = new MqttPublisherManager(ConnectionSettings.Address, ConnectionSettings.Port);

                bool isConnected = await manager.ConnectAsync();

                if (isConnected)
                {
                    string randomMessage = GetRandomMessage("Radar");
                    await manager.PublishMessageAsync("Radar", randomMessage);
                    await manager.DisconnectAsync();
                }
                else
                {
                    TextLeftDown?.Invoke($"Connessione fallita per la pubblicazione.");
                }
            }
            catch (MqttCommunicationException ex)
            {
                dispatcher.Invoke(() =>
                {
                    TextLeftDown?.Invoke($"Errore comunicazione MQTT: {ex.Message}. Controlla il broker ({ConnectionSettings.Address}:{ConnectionSettings.Port})");
                });
            }
            catch (Exception ex)
            {
                dispatcher.Invoke(() =>
                {
                    TextLeftDown?.Invoke($"Errore grave nel task: {ex.Message}");
                });
            }
        }
        private string GetRandomMessage(string topic)
        {
            int numTotalTagsForTopic = 26;
            int randomIndex = EnumRandomizer.GetRandomInt(numTotalTagsForTopic);
            PublisherPayload publisherPayload = new PublisherPayload
            {
                Presence = true,
                SensorArea = "154",
                SensorLocation = randomIndex.ToString(),
                SensorStatus = BlebStatus.Valid.ToString(),
                Battery = (decimal)3.6,
                Timestamp = TimestampRoundTrip.GetTimeStamp()
            };
            return JsonHelper.ToJson(publisherPayload, true);
        }
        #endregion
    }
}
