using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Inserter : BaseEntity
    {
        [JsonProperty("pickup_position")]
        public Point PickupPosition { get; set; }

        [JsonProperty("insert_position")]
        public Point InsertPosition { get; set; }
        [JsonProperty("energy_source")]
        public EnergySource EnergySource { get; set; }

        [JsonProperty("extension_speed")]
        public float ExtensionSpeed { get; set; }

        [JsonProperty("rotation_speed")]
        public float RotationSpeed { get; set; }

        [JsonProperty("stack")]
        public bool Stack { get; set; }

        [JsonProperty("filter_count")]
        public float FilterCount { get; set; }

    }
}