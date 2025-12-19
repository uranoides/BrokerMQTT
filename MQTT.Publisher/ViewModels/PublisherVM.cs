using MQTT.Publisher.Models;
using MQTT.Sharing;
using MQTT.Sharing.Helpers;
using MQTT.Sharing.Models;
using MQTT.Sharing.Utilities;
using MQTTnet;
using MQTTnet.Exceptions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
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
        private Dictionary<string, TopicCounter> _topicLookup;
        private ObservableCollection<TopicCounter> topics;
        public ObservableCollection<TopicCounter> Topics
        {
            get { return topics; }
            set
            {
                topics = value;
                OnPropertyChanged(nameof(Topics));

            }
        }
        private TopicCounter selectedTopic;
        public TopicCounter SelectedTopic
        {
            get { return selectedTopic; }
            set
            {
                selectedTopic = value;
                OnPropertyChanged(nameof(SelectedTopic));
                if (SelectedTopic != null)
                    GetTopicBlebSensorsCount();
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
            JsonManager jsonManager = new JsonManager();
            List<ConnectionSettings> connectionSettings = await jsonManager.ReadJsonAsync();
            ConnectionSettings = new ObservableCollection<ConnectionSettings>(connectionSettings);

            InitializeTaskTimer();
        }
        #endregion

        #region Methods
        private async Task GetBlebSensorsAsync()
        {
            BlebSensorsPayloads = new List<BlebSensor>();
            Topics = new ObservableCollection<TopicCounter>();
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
            InitializeTopics();
            TextRightUp?.Invoke($"Loaded {Topics.Count} Topics from Bleb Sensors");
        }
        public void InitializeTopics()
        {
            var uniqueTopics = BlebSensorsAll.Select(v => v.Topic).Distinct();

            Topics.Clear();
            _topicLookup = new Dictionary<string, TopicCounter>();

            foreach (var name in uniqueTopics)
            {
                var item = new TopicCounter { Name = name, Count = 0 };
                Topics.Add(item);
                _topicLookup.Add(name, item); 
            }
        }
        public void IncrementTopic(string topicName)
        {
            if (_topicLookup.ContainsKey(topicName))
            {
                Application.Current.Dispatcher.Invoke(() => _topicLookup[topicName].Count++);
            }
        }
        public void ResetAllCounters()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var topic in Topics)
                {
                    topic.Count = 0;
                }
            });
        }
        private void GetTopicBlebSensorsCount()
        {
            BlebSensors = BlebSensorsAll.Where(v => v.Topic == SelectedTopic.Name).ToList();
            TextRightUp?.Invoke($"Loaded {BlebSensors.Count} Bleb Sensors for Topic " + SelectedTopic.Name);
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
                BlebSensorsPayloads = new List<BlebSensor>();
                ResetAllCounters();
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
            PublisherPayload publisherPayload = new PublisherPayload{
                Presence = randomBusy,
                SensorLocation = config.Sensor, 
                SensorStatus = EnumRandomizer.GetRandomStatusString(),
                Timestamp = TimestampRoundTrip.GetTimeStamp()
            };
            IncrementTopic(topic);
            BlebSensor blebSensorToUpdate = BlebSensorsAll.FirstOrDefault(s => s.Sensor_Location == config.Sensor);
            blebSensorToUpdate.Sensor_Status = publisherPayload.SensorStatus;
            blebSensorToUpdate.Presence = publisherPayload.Presence;
            blebSensorToUpdate.Timestamp = DateTime.Now;
            LastMessage = JsonHelper.ToJson(publisherPayload, true);
            UpdateBlebSensorPayloads(blebSensorToUpdate);

            return LastMessage;
        }
        private void UpdateBlebSensorPayloads(BlebSensor sensor)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var existingSensor = BlebSensorsPayloads.FirstOrDefault(s => s.Sensor_Location == sensor.Sensor_Location);
                if (existingSensor != null)
                {
                    BlebSensorsPayloads.Remove(existingSensor);
                }
                BlebSensorsPayloads.Add(sensor);
                BlebSensorsPayloads = BlebSensorsPayloads.OrderByDescending(s => s.Timestamp).ToList();
                OnPropertyChanged(nameof(BlebSensorsPayloads));
                if (SelectedBlebSensor != null)
                {
                    if (SelectedBlebSensor.Sensor_Location == sensor.Sensor_Location)
                    {
                        SelectedBlebSensor = sensor;
                    }
                }
            });
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
                    await manager.DisconnectAsync();
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
