using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Box
    {
        [JsonProperty("1")]
        public Point TopLeft { get;set; }

        [JsonProperty("2")]
        public Point BottomRight { get;set; }

        public float Width => BottomRight.X - TopLeft.X;
        public float Height=> BottomRight.Y - TopLeft.Y;
    }
}