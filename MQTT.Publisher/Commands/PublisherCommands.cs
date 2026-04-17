using System.Windows.Input;

namespace MQTT.Publisher.Commands
{
    public static class PublisherCommands
    {
        public static readonly RoutedCommand WriteTopics = new RoutedCommand("WriteTopics", typeof(PublisherCommands));
        public static readonly RoutedCommand WriteSingleTopic = new RoutedCommand("WriteSingleTopic", typeof(PublisherCommands));
    }
}
