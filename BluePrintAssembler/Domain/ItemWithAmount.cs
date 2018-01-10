using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class ItemWithAmount
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "item";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; } = 1;

        [JsonProperty("probability")]
        public float Probability { get; set; } = 1;
    }
}