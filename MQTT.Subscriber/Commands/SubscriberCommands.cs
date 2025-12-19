using System.Windows.Input;

namespace MQTT.Subscriber.Commands
{
    public static class SubscriberCommands
    {
        public static readonly RoutedCommand ReadTopics = new RoutedCommand("ReadTopics", typeof(SubscriberCommands));
    }
}
