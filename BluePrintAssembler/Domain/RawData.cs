using System;
using System.Collections.Generic;
using System.Windows.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BluePrintAssembler.Domain
{
    public class RawData
    {
        [JsonProperty("item")]
        public Dictionary<string, Item> Items { get; set; }

        [JsonProperty("fluid")]
        public Dictionary<string, Fluid> Fluids { get; set; }

        [JsonProperty("electric-pole")]
        public Dictionary<string, ElectricPole> ElectricPoles { get; set; }

        [JsonProperty("mining-drill")]
        public Dictionary<string, MiningDrill> MiningDrills { get; set; }

        [JsonProperty("transport-belt")]
        public Dictionary<string, TransportBelt> TransportBelts { get; set; }

        [JsonProperty("recipe")]
        public Dictionary<string, Recipe> Recipes { get; set; }

        [JsonProperty("furnace")]
        public Dictionary<string, Furnace> Furnaces { get; set; }

        [JsonProperty("container")]
        public Dictionary<string, Container> Containers { get; set; }

        [JsonProperty("inserter")]
        public Dictionary<string, Inserter> Inserters { get; set; }

        [JsonProperty("assembling-machine")]
        public Dictionary<string, Assembler> Assemblers { get; set; }

        [JsonProperty("locale")]
        public Dictionary<string, JToken> LocaleData { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> ExtraData { get; set; }

        public BaseProducibleObject Get(string valueType, string valueName)
        {
            switch (valueType)
            {
                case "fluid":
                    return Fluids[valueName];
                case "item":
                    return Items.TryGetValue(valueName, out var v) ? v : new BaseProducibleObject {Type = "void", Name = valueName};
            }

            throw new NotSupportedException();
        }
    }
}