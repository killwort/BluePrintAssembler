using System.Collections.Generic;
using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Module : BaseGameObject,IWithIcon,INamed
    {
        [JsonProperty("ties")]
        public float Tier;
        [JsonProperty("subgroup")]
        public string Subgroup;
        [JsonProperty("category")]
        public string Category;
        [JsonProperty("effect")]
        public Dictionary<string,Effect> Effects { get; set; }
    }
}