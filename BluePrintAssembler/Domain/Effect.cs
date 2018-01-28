using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Effect
    {
        [JsonProperty("bonus")]
        public float Bonus { get; set; }
    }
}