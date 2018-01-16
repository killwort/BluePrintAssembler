using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

        private static Regex FormattingRegex=new Regex(@"__(?<num>\d+)__",RegexOptions.Compiled);

        private class AltNameWrapper : INamed
        {
            public AltNameWrapper(Tuple<string, string> src)
            {
                Type = src.Item1;
                Name = src.Item2;
            }
            public string Type { get; }
            public string Name { get; }
            public LocalisedString LocalisedName { get; }
        }
        public string LocalisedName(INamed obj, CultureInfo cultureInfo)
        {
            if(cultureInfo==null)cultureInfo=CultureInfo.CurrentUICulture;
            var cultureList = new[] {cultureInfo.Name, cultureInfo.Name.Split('/')[0], cultureInfo.TwoLetterISOLanguageName, "en"};
            LocalisedString str = obj.LocalisedName;
            if (str == null) str = new LocalisedString {Format = $"{obj.Type}-name.{obj.Name}"};

            string GetString(string key)
            {
                foreach (var dict in cultureList)
                {
                    if (LocaleData.TryGetValue(dict, out var dic))
                    {
                        var val = ((JObject) dic).SelectToken(key);
                        if (val != null) return val.Value<string>();
                    }
                }
                return null;
            }

            var fmt = GetString(str.Format);
            if (fmt == null && obj is IAltNames anames)
            {
                var altName=anames.AlternativeNames.Select(x => LocalisedName(new AltNameWrapper(x), cultureInfo)).FirstOrDefault(x => x != null);
                if (altName != null) return altName;
            }
            if (fmt == null) return str.Format;
            if (str.Arguments == null) return fmt;
            return FormattingRegex.Replace(fmt, (m) =>
            {
                if (str.Arguments.TryGetValue((1 + int.Parse(m.Groups["num"].Value)).ToString(), out var arg))
                {
                    return GetString(((JObject) arg)["1"].Value<string>());
                }

                return m.Value;
            });
        }
    }
}