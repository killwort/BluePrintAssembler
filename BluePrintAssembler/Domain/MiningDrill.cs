using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class MiningDrill : BaseProducingEntity
    {
        [JsonProperty("mining_power")]
        public float MiningPower { get; set; }
        [JsonProperty("mining_speed")]
        public float MiningSpeed { get; set; }
    }
}