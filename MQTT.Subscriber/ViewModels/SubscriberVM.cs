using MQTT.Sharing.Enumerations;
using MQTT.Sharing.Helpers;
using MQTT.Sharing.Models;
using MQTT.Sharing.Utilities;
using MQTTnet;
using MQTTnet.Protocol;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
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
        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool isDisconnectingIntentional = false;
        private IMqttClient mqttClient;
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
                else
                    BlebSensors = new List<BlebSensor>();
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
        private bool isBlebRunning = false;
        public bool IsBlebRunning
        {
            get { return isBlebRunning; }
            set
            {
                isBlebRunning = value;
                OnPropertyChanged(nameof(IsBlebRunning));
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
        private bool isConnectionSelectionEnable = true;
        public bool IsConnectionSelectionEnable
        {
            get { return isConnectionSelectionEnable; }
            set
            {
                isConnectionSelectionEnable = value;
                OnPropertyChanged(nameof(IsConnectionSelectionEnable));
            }
        }
        #endregion

        #region Builder
        public async Task LoadAsync()
        {
            JsonManager jsonManager = new JsonManager();
            List<ConnectionSettings> connectionSettings = await jsonManager.ReadJsonAsync();
            ConnectionSettings = new ObservableCollection<ConnectionSettings>(connectionSettings);
            BlebSensorsPayloads = new List<BlebSensor>();
        }
        #endregion

        #region Subscriber
        public void ToggleTimerBleb()
        {
            if (!IsBlebRunning)
            {
                IsConnectionSelectionEnable = false;
                _ = StartAsync();
            }
            else
            {
                IsConnectionSelectionEnable = true;
                _ = StopAsync();
            }
        }
        public async Task StartAsync()
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            if (cts.IsCancellationRequested)
            {
                cts.Dispose();
                cts = new CancellationTokenSource();
            }
            string nuovoGuidString = Guid.NewGuid().ToString();

            MqttClientFactory mqttFactory = new();
            mqttClient = mqttFactory.CreateMqttClient();

            mqttClient.ApplicationMessageReceivedAsync += OnReceivedMessageAsync;
            mqttClient.ConnectedAsync += OnConnectedAsync;
            mqttClient.DisconnectedAsync += OnDisconnectedAsync;

            var mqttClientOptions = (MqttClientOptions)MqttClientBuilderHelper.ConfigureMqttOptions(SelectedConnectionSettings);

            try
            {
                MqttClientConnectResult result = await mqttClient.ConnectAsync(mqttClientOptions, timeoutCts.Token);
                if (result.ResultCode != MqttClientConnectResultCode.Success)
                {
                    string connectionError = string.Empty;
                    connectionError = $"Connessione fallita. Codice: {result.ResultCode} - ";

                    if ((int)result.ResultCode == 1)
                    {
                        connectionError += "Versione del Protocollo Inaccettabile";
                    }
                    else
                    {
                        connectionError += "Errore Generico";
                    }
                    TextLeftDown?.Invoke(connectionError);
                    await DisconnectOnError();
                }
                else
                {
                    TextLeftUp?.Invoke("Connessione al Broker Riuscita!");
                }
            }
            catch (OperationCanceledException)
            {
                TextLeftDown?.Invoke("ERRORE: Timeout di Connessione (5\") Raggiunto");
                await DisconnectOnError();
            }
            catch (Exception ex)
            {
                TextLeftDown?.Invoke($"ERRORE: Errore di Connessione o Esecuzione: {ex.Message}");
                await DisconnectOnError();
            }
        }
        public async Task StopAsync()
        {
            isDisconnectingIntentional = true;

            if (mqttClient != null)
            {
                mqttClient.ApplicationMessageReceivedAsync -= OnReceivedMessageAsync;
                mqttClient.ConnectedAsync -= OnConnectedAsync;
                mqttClient.DisconnectedAsync -= OnDisconnectedAsync;
                if (mqttClient.IsConnected)
                {
                    await mqttClient.DisconnectAsync();
                }
                mqttClient.Dispose();

                TextLeftUp?.Invoke("Client MQTT disconnesso e risorse liberate.");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsBlebRunning = false;
            });
            SetBlebSensorsOffline();
            ResetAllCounters();
            SelectedTopic = null;
            isDisconnectingIntentional = false;
        }
        private Task OnConnectedAsync(MqttClientConnectedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsBlebRunning = true;
            });

            return TopicSubscriptionAsync();
        }
        private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            if (isDisconnectingIntentional)
            {
                TextLeftUp?.Invoke("⛔️ Disconnessione dal Broker completata (richiesta dall'utente).");
            }
            else
            {
                string reason = e.ReasonString ?? e.Reason.ToString();
                string errorMessage = $"⛔️ Disconnessione Imprevista dal Broker. Causa: {reason}";

                TextLeftDown?.Invoke(errorMessage);

                SetBlebSensorsOffline();
                OnPropertyChanged(nameof(BlebSensors));
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsBlebRunning = false;
            });
            cts.Cancel();

            return Task.CompletedTask;
        }
        private async Task DisconnectOnError()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
            await mqttClient.DisconnectAsync();
        }
        private async Task TopicSubscriptionAsync()
        {
            MqttClientFactory mqttFactory = new();
            var subscribeOptionsBuilder = mqttFactory.CreateSubscribeOptionsBuilder();

            if (TagVariables == null || TagVariables.Count == 0)
            {
                TextLeftDown?.Invoke("⚠️ DIAGNOSTICA: TagVariables è NULL o vuota. Nessun topic da sottoscrivere.");
                return;
            }

            List<string> uniqueAddresses = TagVariables
                .Select(v => v.Address)
                .Distinct()
                .ToList();

            TextLeftDown?.Invoke($"🔍 DIAGNOSTICA: {TagVariables.Count} variabili lette, {uniqueAddresses.Count} topic univoci: [{string.Join(", ", uniqueAddresses)}]");

            if (uniqueAddresses.Count == 0)
            {
                TextLeftDown?.Invoke("⚠️ DIAGNOSTICA: Nessun topic trovato nelle TagVariables. Controlla il file XML.");
                return;
            }

            foreach (string address in uniqueAddresses)
            {
                var mqttTopicFilter = new MqttTopicFilterBuilder()
                    .WithTopic(address)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                subscribeOptionsBuilder.WithTopicFilter(mqttTopicFilter);
            }

            var subscribeOptions = subscribeOptionsBuilder.Build();

            MqttClientSubscribeResult subscribeResult = await mqttClient.SubscribeAsync(subscribeOptions, CancellationToken.None);

            foreach (var item in subscribeResult.Items)
            {
                if (item.ResultCode > MqttClientSubscribeResultCode.GrantedQoS2)
                    TextLeftDown?.Invoke($"⚠️ DIAGNOSTICA: Sottoscrizione FALLITA per topic '{item.TopicFilter.Topic}' → Codice: {item.ResultCode}");
                else
                    TextLeftDown?.Invoke($"✅ DIAGNOSTICA: Sottoscritto '{item.TopicFilter.Topic}' → QoS: {item.ResultCode}");
            }

            if (BlebSensorsAll == null || BlebSensorsAll.Count == 0)
                TextRightDown?.Invoke("⚠️ DIAGNOSTICA: BlebSensorsAll è NULL o vuota. I messaggi non verranno abbinati ad alcun sensore.");
            else
                TextRightDown?.Invoke($"🔍 DIAGNOSTICA: {BlebSensorsAll.Count} sensori in BlebSensorsAll");

            TextLeftUp?.Invoke("Connessione e Sottoscrizione ai Topic avvenuta con Successo..");
        }
        private Task OnReceivedMessageAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            string payload = e.ApplicationMessage.ConvertPayloadToString();
            string topic = e.ApplicationMessage.Topic;

            TextRightUp?.Invoke($"📨 DIAGNOSTICA: Messaggio ricevuto su topic '{topic}'");

            TextLeftUp?.Invoke("Last Reading at " + DateTime.Now.ToString());

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    using (JsonDocument document = JsonDocument.Parse(payload))
                    {
                        JsonElement root = document.RootElement;

                        if (root.TryGetProperty("sensor_location", out JsonElement locElem) && locElem.ValueKind == JsonValueKind.String)
                        {
                            string location = locElem.GetString();
                            var area = root.TryGetProperty("sensor_area", out var areaElem) && areaElem.ValueKind == JsonValueKind.String ? areaElem.GetString() : null;

                            IncrementTopic(topic + " - " + area);

                            BlebSensor blebSensorToUpdate = BlebSensorsAll.FirstOrDefault(f => f.Sensor_Location == location && f.Sensor_Area == area);
                            if (blebSensorToUpdate != null)
                            {
                                if (root.TryGetProperty("gateway_ID", out var gtw))
                                    blebSensorToUpdate.Gateway_ID = gtw.GetString();

                                if (root.TryGetProperty("sensor_ID", out var id))
                                    blebSensorToUpdate.Sensor_ID = id.GetString();

                                if (root.TryGetProperty("sensor_type", out var type))
                                    blebSensorToUpdate.Sensor_Type = type.GetString();

                                if (root.TryGetProperty("sensor_communication", out var comm))
                                    blebSensorToUpdate.Sensor_Communication = comm.GetString();

                                if (root.TryGetProperty("battery", out var battery))
                                    blebSensorToUpdate.Battery = battery.GetInt32();

                                blebSensorToUpdate.Sensor_Area = area;
                                blebSensorToUpdate.Sensor_Location = location;
                                blebSensorToUpdate.Sensor_Status = root.TryGetProperty("sensor_status", out var status) ? status.GetString() : "Unknown";

                                if (root.TryGetProperty("sensor_threshold", out var threshold))
                                    blebSensorToUpdate.Sensor_Threshold = threshold.GetInt64();

                                if (root.TryGetProperty("sensor_value", out var val))
                                    blebSensorToUpdate.Sensor_Value = val.GetInt64();

                                if (status.GetString() == "Offline")
                                    blebSensorToUpdate.Presence = null;
                                else
                                    blebSensorToUpdate.Presence = root.TryGetProperty("presence", out var pres) ? pres.GetBoolean() : null;

                                if (root.TryGetProperty("timestamp", out var ts))
                                {
                                    if (DateTime.TryParse(ts.GetString(), out DateTime parsedDate))
                                        blebSensorToUpdate.Timestamp = parsedDate;
                                }
                                OnPropertyChanged(nameof(BlebSensors));
                                UpdateBlebSensorPayloads(blebSensorToUpdate);
                            }
                            else
                            {
                                TextLeftDown?.Invoke($"⚠️ Sensor with Location '{location}' NOT FOUND on BlebSensors' list.");
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    TextLeftDown?.Invoke($"Errore di Parsing JSON per il Topic '{topic}'. Payload non valido: '{payload}'. Errore: {ex.Message}");
                }
                catch (Exception ex)
                {
                    TextLeftDown?.Invoke($"Errore Generico durante l'Elaborazione del Messaggio sul Topic '{topic}'. Errore: {ex.Message}");
                }
            });
            return Task.CompletedTask;
        }
        #endregion

        #region Methods
        private async Task GetBlebSensorsAsync()
        {
            Topics = new ObservableCollection<TopicCounter>();
            var reader = new ConfigurationXmlReader();
            TagVariables = reader.ReadVariables(SelectedConnectionSettings.TagVariablesFileName);
            BlebSensorsAll = new List<BlebSensor>();
            foreach (var tag in TagVariables)
            {
                BlebSensorsAll.Add(new BlebSensor()
                {
                    Topic = tag.Address,
                    PlaceId = tag.Id,
                    Sensor_Location = tag.CustomData,
                    Gateway_ID = EnumRandomizer.GetRandomAlphanumeric(),
                    Sensor_ID = EnumRandomizer.GetRandomAlphanumeric(),
                    Sensor_Type = BlebSensorType.Radar.ToString().Substring(0, 3),
                    Sensor_Area = tag.AdvancedProperties[3].Value,
                    Sensor_Communication = "Radar",
                    Sensor_Status = "Offline",
                    Sensor_Value = EnumRandomizer.GetRandomInt(1000),
                    Battery = 100,
                    Presence = false,
                });
            }
            TextLeftUp?.Invoke($"Loaded {BlebSensorsAll.Count} Bleb Sensors from {SelectedConnectionSettings.TagVariablesFileName}");
            InitializeTopics();
            TextRightUp?.Invoke($"Loaded {Topics.Count} Topics from Bleb Sensors");
        }
        private void UpdateBlebSensorPayloads(BlebSensor sensor)
        {
            var existingSensor = BlebSensorsPayloads.FirstOrDefault(s => s.Sensor_Location == sensor.Sensor_Location);
            if (existingSensor != null)
            {
                BlebSensorsPayloads.Remove(existingSensor);
            }
            BlebSensorsPayloads.Add(sensor);
            BlebSensorsPayloads = BlebSensorsPayloads.OrderByDescending(s => s.Timestamp).ToList();
            OnPropertyChanged(nameof(BlebSensorsPayloads));
        }
        private void SetBlebSensorsOffline()
        {
            List<BlebSensor> blebSensorsList = this.BlebSensorsAll.ToList();
            foreach (var sensor in blebSensorsList)
            {
                sensor.Sensor_Status = "Offline";
                sensor.Presence = null;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                BlebSensors = new List<BlebSensor>();
            });
        }
        private void GetTopicBlebSensors()
        {
            BlebSensors = BlebSensorsAll.Where(v => (v.Topic + " - " + v.Sensor_Area) == SelectedTopic.Name).ToList();
            TextRightUp?.Invoke($"Loaded {BlebSensors.Count} Bleb Sensors for Topic " + SelectedTopic.Name);
        }
        public void InitializeTopics()
        {
            var uniqueTopics = BlebSensorsAll.Select(v => v.Topic + " - " + v.Sensor_Area).Distinct();

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
        #endregion
    }
}
