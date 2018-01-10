using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Point
    {
        [JsonProperty("1")]
        public float X { get;set; }
        [JsonProperty("2")]
        public float Y { get;set; }
    }
}