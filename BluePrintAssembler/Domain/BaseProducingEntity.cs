using System.Collections.Generic;
using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class BaseProducingEntity : BaseEntity
    {
        [JsonProperty("input_fluid_box")]
        public FluidBox InputFluidPipes { get;set; }
        [JsonProperty("output_fluid_box")]
        public FluidBox OutputFluidPipes { get;set; }
        [JsonProperty("module_specification")]
        public ModuleSpecification ModuleSpecification { get; set; }
        [JsonProperty("energy_source")]
        public EnergySource EnergySource { get; set; }
        [JsonProperty("crafting_categories")]
        public Dictionary<string,string> CraftingCategories { get; set; }
        [JsonProperty("crafting_speed")]
        public float CraftingSpeed { get; set; }

        [JsonProperty("ingredient_count")]
        public float IngredientCount { get; set; }

        [JsonProperty("allowed_effects")]
        public Dictionary<string,string> AllowedEffects { get; set; }
    }
}