using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Item:BaseProducibleObject
    {
        public override string Type => "item";

        [JsonProperty("stack_size")]
        public int StackSize { get; set; }
    }
}
