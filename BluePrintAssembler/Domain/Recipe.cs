using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Recipe:RecipeInputsOutputs,IWithIcon
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("normal")]
        public RecipeInputsOutputs Normal { get; set; }
        [JsonProperty("expensive")]
        public RecipeInputsOutputs Expensive { get; set; }

        public bool HasResult(BaseProducibleObject result)
        {
            RecipeInputsOutputs rio = this;
            if (rio.Results == null) rio = Normal;
            if (rio?.Results == null) rio = Expensive;
            if (rio?.Results == null) return false;
            return rio.Results.Any(x => x.Value.Name == result.Name && x.Value.Type == result.Type);
        }
        public bool HasSource(BaseProducibleObject source)
        {
            RecipeInputsOutputs rio = this;
            if (rio.Sources == null) rio = Normal;
            if (rio?.Sources == null) rio = Expensive;
            if (rio?.Sources == null) return false;
            return rio.Sources.Any(x => x.Value.Name == source.Name && x.Value.Type == source.Type);
        }
        [JsonIgnore]
        public RecipeInputsOutputs CurrentMode => Normal ?? this;
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("icon_size")]
        public float IconSize { get; set; }
        [JsonProperty("icons")]
        public Dictionary<string, IconPart> Icons { get; set; }
        [JsonIgnore]
        public IWithIcon FallbackIcon
        {
            get
            {
                var firstResult = CurrentMode.Results.FirstOrDefault();
                if (firstResult.Value?.Name == null) return null;
                return Configuration.Instance.RawData.Get(firstResult.Value.Type, firstResult.Value.Name);
            }
        }
    }
}