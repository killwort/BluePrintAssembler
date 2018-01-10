using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class RecipeInputsOutputs
    {
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

        /*[JsonProperty("enabled")]
        public bool Enabled { get; set; }*/
    }
}