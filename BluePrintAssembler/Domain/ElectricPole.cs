using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class ElectricPole : BaseEntity
    {
        [JsonProperty("supply_area_distance")]
        public float SupplyAreaDistance { get; set; }
    }
}