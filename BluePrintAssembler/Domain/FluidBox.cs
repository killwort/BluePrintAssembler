using System.Collections.Generic;
using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class FluidBox
    {
        [JsonProperty("production_type")]
        public string Type { get; set; }
        [JsonProperty("pipe_connections")]
        public Dictionary<string,PositionContainer> Connections { get;set; }
    }
}