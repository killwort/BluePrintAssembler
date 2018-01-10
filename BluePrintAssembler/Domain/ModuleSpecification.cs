using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class ModuleSpecification
    {
        [JsonProperty("module_slots")]
        public float Slots { get; set; }
    }
}