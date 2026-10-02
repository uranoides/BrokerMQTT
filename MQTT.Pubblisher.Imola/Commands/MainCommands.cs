using System.Windows.Input;

namespace MQTT.Pubblisher.Imola.Commands
{
    public class MainCommands
    {
        public static readonly RoutedCommand WriteTopics = new RoutedCommand("WriteTopics", typeof(MainCommands));
    }
}
