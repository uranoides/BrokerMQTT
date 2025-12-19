using MQTT.Sharing;
using MQTT.Sharing.Models;
using MQTT.Sharing.Utilities;
using MQTTnet;
using MQTTnet.Exceptions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using TaskTimer = System.Timers.Timer;

namespace MQTT.Publisher.ViewModels
{
    public class PublisherVM : INotifyPropertyChanged
    {
        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Error
        public delegate void ErrorHandler(string ErrorMessage, bool Silent = false);
        public event ErrorHandler Error;
        #endregion

        #region Text
        public delegate void TextLeftUpHandler(string Message);
        public event TextLeftUpHandler TextLeftUp;

        public delegate void TextLeftDownHandler(string Message);
        public event TextLeftDownHandler TextLeftDown;

        public delegate void TextRightUpHandler(string Message);
        public event TextRightUpHandler TextRightUp;

        public delegate void TextRightDownHandler(string Message);
        public event TextRightDownHandler TextRightDown;
        #endregion

        #region Variables
        public List<VariableData> TagVariables;
        public List<BlebSensor> BlebSensorsAll;
        private readonly Random random = new Random();
        #endregion

        #region Properties
        private int publisherTimerIntervalMSec = 100;
        public int PublisherTimerIntervalMSec
        {
            get { return publisherTimerIntervalMSec; }
            set
            {
                publisherTimerIntervalMSec = value;
                OnPropertyChanged(nameof(PublisherTimerIntervalMSec));
            }
        }

        private List<BlebSensor> blebSensors;
        public List<BlebSensor> BlebSensors
        {
            get { return blebSensors; }
            set
            {
                blebSensors = value;
                OnPropertyChanged(nameof(BlebSensors));
            }
        }
        private BlebSensor selectedBlebSensor;
        public BlebSensor SelectedBlebSensor
        {
            get { return selectedBlebSensor; }
            set
            {
                selectedBlebSensor = value;
                OnPropertyChanged(nameof(SelectedBlebSensor));
            }
        }
        private ObservableCollection<ConnectionSettings> connectionSettings;
        public ObservableCollection<ConnectionSettings> ConnectionSettings
        {
            get { return connectionSettings; }
            set
            {
                connectionSettings = value;
                OnPropertyChanged(nameof(ConnectionSettings));
                
            }
        }
        private ConnectionSettings selectedConnectionSettings;
        public ConnectionSettings SelectedConnectionSettings
        {
            get { return selectedConnectionSettings; }
            set
            {
                selectedConnectionSettings = value;
                OnPropertyChanged(nameof(SelectedConnectionSettings));
                if(SelectedConnectionSettings != null)
                {
                    _ = GetBlebSensorsAsync();
                }
            }
        }
        private List<string> topics;
        public List<string> Topics
        {
            get { return topics; }
            set
            {
                topics = value;
                OnPropertyChanged(nameof(Topics));

            }
        }
        private string selectedTopic;
        public string SelectedTopic
        {
            get { return selectedTopic; }
            set
            {
                selectedTopic = value;
                OnPropertyChanged(nameof(SelectedTopic));
                if (SelectedTopic != null)
                    GetTopicBlebSensors();
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
        private string lastMessage;
        public string LastMessage
        {
            get { return lastMessage; }
            set
            {
                lastMessage = value;
                OnPropertyChanged(nameof(LastMessage));
            }
        }
        #endregion

        #region Builder
        public async Task LoadAsync()
        {
            JsonManager jsonManager = new JsonManager();
            List<ConnectionSettings> connectionSettings = await jsonManager.ReadJsonAsync();
            ConnectionSettings = new ObservableCollection<ConnectionSettings>(connectionSettings);

            InitializeTaskTimer();
        }
        #endregion

        #region Methods
        private async Task GetBlebSensorsAsync()
        {
            var reader = new ConfigurationXmlReader();
            TagVariables = reader.ReadVariables(SelectedConnectionSettings.TagVariablesFileName);
            BlebSensorsAll = new List<BlebSensor>();
            foreach (var tag in TagVariables)
            {
                CustomData config = CustomDataDeserializer.DeserializeFromXmlAttribute(tag.CustomData);
                BlebSensorsAll.Add(new BlebSensor()
                {
                    Topic = tag.Address,
                    PlaceId = tag.Id,
                    Sensor_Location = config.Sensor,
                    Gateway_ID = EnumRandomizer.GetRandomAlphanumeric(),
                    Sensor_ID = EnumRandomizer.GetRandomAlphanumeric(),
                    Sensor_Type = EnumRandomizer.GetRandomBlebSensorType().ToString(),
                    Sensor_Area = EnumRandomizer.GetRandomSensorLocation().ToString(),
                    Sensor_Communication = "BLE",
                    Sensor_Status = "Offline",
                    Sensor_Value = EnumRandomizer.GetRandomInt(1000),
                    Presence = false,
                });
            }
            TextLeftUp?.Invoke($"Loaded {BlebSensorsAll.Count} Bleb Sensors from {SelectedConnectionSettings.TagVariablesFileName}");
            Topics = BlebSensorsAll.Select(v => v.Topic).Distinct().ToList();
            TextLeftDown?.Invoke($"Loaded {Topics.Count} Topics from Bleb Sensors");
        }
        private void GetTopicBlebSensors()
        {
            BlebSensors = BlebSensorsAll.Where(v => v.Topic == SelectedTopic).ToList();
            TextLeftDown?.Invoke($"Loaded {BlebSensors.Count} Bleb Sensors for Topic " + SelectedTopic);
        }
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
                TextLeftUp?.Invoke("Publisher Task Stopped..");
            }
        }
        private string GetRandomMessage(string topic)
        {
            int numTotalTagsForTopic = TagVariables.Where(t => t.Address.ToLower() == topic.ToLower()).Count();
            int randomIndex = EnumRandomizer.GetRandomInt(numTotalTagsForTopic);
            List<VariableData> matchingTags = TagVariables.Where(t => t.Address.ToLower() == topic.ToLower()).ToList();
            string randomSensorCustomData = matchingTags[randomIndex].CustomData;
            CustomData config = CustomDataDeserializer.DeserializeFromXmlAttribute(randomSensorCustomData);
            bool randomBusy = random.Next(2) == 0;
            LastMessage = "{\r\n    \"sensor_location\": \"" + config.Sensor + "\",\r\n    \"sensor_status\": \"" + EnumRandomizer.GetRandomStatusString() + "\",\r\n    \"presence\": " + randomBusy.ToString().ToLower() + ",\r\n    \"timestamp\": \"" + TimestampRoundTrip.GetTimeStamp() + "\" \r\n}";
            return LastMessage;
        }
        #endregion

        #region Publisher
        private TaskTimer taskTimer;
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private void InitializeTaskTimer()
        {
            taskTimer = new TaskTimer(PublisherTimerIntervalMSec);
            taskTimer.Elapsed += TaskTimer_Elapsed;
            taskTimer.AutoReset = true;
        }
        private async void TaskTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
                    Error?.Invoke("Errore: connectionSettings non ancora caricate.");
                    return;
                }

                var manager = new MqttPublisherManager(SelectedConnectionSettings.Address, SelectedConnectionSettings.Port);

                bool isConnected = await manager.ConnectAsync();

                if (isConnected)
                {
                    string randomTopic = Global.GetRandomTestTopic();
                    string randomMessage = GetRandomMessage(randomTopic);
                    await manager.PublishMessageAsync(randomTopic, randomMessage);
                    TextLeftUp?.Invoke("Scrittura Eseguita Correttamente su Topic: " + randomTopic);
                    await manager.DisconnectAsync();

                    TextLeftUp?.Invoke("Scrittura Eseguita Correttamente alle " + DateTime.Now.ToString());
                }
                else
                {
                    Error?.Invoke($"Connessione fallita per la pubblicazione.");
                }
            }
            catch (MqttCommunicationException ex)
            {
                dispatcher.Invoke(() =>
                {
                    Error?.Invoke($"Errore comunicazione MQTT: {ex.Message}. Controlla il broker ({SelectedConnectionSettings.Address}:{SelectedConnectionSettings.Port})");
                });
            }
            catch (Exception ex)
            {
                dispatcher.Invoke(() =>
                {
                    Error?.Invoke($"Errore grave nel task: {ex.Message}");
                });
            }
        }
        #endregion
    }
}
