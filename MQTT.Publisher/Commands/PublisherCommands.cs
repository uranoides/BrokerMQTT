using System.Windows.Input;

namespace MQTT.Publisher.Commands
{
    public static class PublisherCommands
    {
        public static readonly RoutedCommand WriteTopics = new RoutedCommand("WriteTopics", typeof(PublisherCommands));
    }
}
