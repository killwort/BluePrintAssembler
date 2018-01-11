using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class IconPart
    {
        [JsonProperty("shift")]
        public Point Shift { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("scale")]
        public float Scale { get; set; } = 1;
        [JsonProperty("tint")]
        public ARGB Tint { get; set; }
    }
}