using MQTT.Sharing.Models;
using MQTT.Sharing.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MQTT.Subscriber.ViewModels
{
    public class SubscriberVM : INotifyPropertyChanged
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
        #endregion

        #region Properties
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
                if (SelectedConnectionSettings != null)
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
                    GetTopicBlebSensors();
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
        #endregion

        #region Builder
        public async Task LoadAsync()
        {
            JsonManager jsonManager = new JsonManager();
            List<ConnectionSettings> connectionSettings = await jsonManager.ReadJsonAsync();
            ConnectionSettings = new ObservableCollection<ConnectionSettings>(connectionSettings);
        }
        #endregion

        #region Methods
        private async Task GetBlebSensorsAsync()
        {
            //BlebSensorsPayloads = new List<BlebSensor>();
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
        private void GetTopicBlebSensors()
        {
            BlebSensors = BlebSensorsAll.Where(v => v.Topic == SelectedTopic.Name).ToList();
            TextRightUp?.Invoke($"Loaded {BlebSensors.Count} Bleb Sensors for Topic " + SelectedTopic.Name);
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
        #endregion
    }
}
