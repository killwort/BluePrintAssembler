using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class BaseProducibleObject : BaseGameObject
    {
        [JsonProperty("subgroup")]
        public string Subgroup { get; set; }
    }
}