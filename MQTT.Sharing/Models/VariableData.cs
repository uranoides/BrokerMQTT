namespace MQTT.Sharing.Models
{
    public class VariableData
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string VariableName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CustomData { get; set; } = string.Empty;
        public List<AdvancedProperty> AdvancedProperties { get; set; }

    }
    public class AdvancedProperty
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
