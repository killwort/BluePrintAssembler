using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BluePrintAssembler.Domain
{
    public class LocalisedString
    {
        [JsonProperty("1")]
        public string Format { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> Arguments { get; set; }
    }
}