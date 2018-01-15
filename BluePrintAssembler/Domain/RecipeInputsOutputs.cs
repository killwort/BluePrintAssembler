using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class RecipeInputsOutputs:IAltNames
    {
        [JsonProperty("energy_required")]
        public float BaseProductionTime { get;set; }
        [JsonProperty("results")]
        public Dictionary<string,ItemWithAmount> Results { get; set; }
        [JsonProperty("ingredients")]
        public Dictionary<string,ItemWithAmount> Sources { get; set; }
        [JsonProperty("ingredient")]
        public string SingleSource
        {
            get => null;
            set
            {
                if(Sources==null && value!=null)
                    Sources = new Dictionary<string, ItemWithAmount> { { "1", new ItemWithAmount { Name = value } } };
            }
        }

        [JsonProperty("result")]
        public string SingleResult
        {
            get => null;
            set
            {
                if(Results==null && value!=null)
                Results = new Dictionary<string, ItemWithAmount> { { "1", new ItemWithAmount { Name = value } } };
            }
        }
        public virtual IEnumerable<Tuple<string, string>> AlternativeNames => Results != null && Results.Count == 1 ? new[] {Tuple.Create(Results.First().Value.Type, Results.First().Value.Name)} : new Tuple<string, string>[0];

        /*[JsonProperty("enabled")]
        public bool Enabled { get; set; }*/
    }
}