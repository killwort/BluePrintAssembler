using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class BaseEntity : BaseGameObject
    {
        [JsonProperty("selection_box")]
        public Box Box { get; set; }
    }
}