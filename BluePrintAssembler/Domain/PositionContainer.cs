using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class PositionContainer
    {
        [JsonProperty("position")]
        public Point Position { get; set; }
    }
}