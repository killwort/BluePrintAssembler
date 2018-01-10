using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class TransportBelt : BaseEntity
    {
        [JsonProperty("speed")]
        public float Speed { get; set; }
    }
}