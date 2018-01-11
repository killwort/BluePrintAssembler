using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class ARGB
    {
        [JsonProperty("a")]
        public float A { get; set; }
        [JsonProperty("r")]
        public float R { get; set; }
        [JsonProperty("g")]
        public float G { get; set; }
        [JsonProperty("b")]
        public float B { get; set; }
    }
}