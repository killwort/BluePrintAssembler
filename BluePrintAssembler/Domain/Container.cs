using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Container : BaseEntity
    {
        [JsonProperty("logistic_mode")]
        public string LogisticMode { get; set; }
        [JsonProperty("inventory_size")]
        public float InventorySize { get; set; }
    }
}