using System.Linq;
using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class Recipe:RecipeInputsOutputs
    {
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
        [JsonIgnore]
        public RecipeInputsOutputs CurrentMode => Normal ?? this;
    }
}