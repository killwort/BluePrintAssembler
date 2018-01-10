using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class EnergySource
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("fuel_category")]
        public string FuelCategory { get; set; }
    }
}