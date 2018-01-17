using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class BaseProducibleObject : BaseGameObject
    {
        [JsonProperty("subgroup")]
        public string Subgroup { get; set; }
        [JsonProperty("order")]
        public string Order { get; set; }

    }
}