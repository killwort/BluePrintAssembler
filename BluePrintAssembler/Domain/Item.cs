using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Item:BaseProducibleObject
    {
        public override string Type => "item";

        [JsonProperty("stack_size")]
        public int StackSize { get; set; }
        [JsonProperty("subgroup")]
        public string Subgroup { get; set; }
        [JsonProperty("order")]
        public string Order { get; set; }
    }
}
