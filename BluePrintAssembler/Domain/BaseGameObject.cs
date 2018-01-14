using System.Collections.Generic;
using Newtonsoft.Json;

namespace BluePrintAssembler.Domain
{
    public class BaseGameObject : IWithIcon, INamed
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("localised_name")]
        public LocalisedString LocalisedName { get; set; }

        protected bool Equals(BaseGameObject other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Type, other.Type);
        }

        private sealed class NameTypeEqualityComparer : IEqualityComparer<BaseGameObject>
        {
            public bool Equals(BaseGameObject x, BaseGameObject y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Name, y.Name) && string.Equals(x.Type, y.Type);
            }

            public int GetHashCode(BaseGameObject obj)
            {
                unchecked
                {
                    return ((obj.Name != null ? obj.Name.GetHashCode() : 0) * 397) ^ (obj.Type != null ? obj.Type.GetHashCode() : 0);
                }
            }
        }

        public static IEqualityComparer<BaseGameObject> NameTypeComparer { get; } = new NameTypeEqualityComparer();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseGameObject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Type != null ? Type.GetHashCode() : 0);
            }
        }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("type")]
        public virtual string Type { get; set; }

        [JsonProperty("icon_size")]
        public float IconSize { get; set; }

        [JsonProperty("icons")]
        public Dictionary<string, IconPart> Icons { get; set; }

        [JsonIgnore]
        public IWithIcon FallbackIcon => null;

        /*[JsonExtensionData]
        public Dictionary<string,JToken> ExtraData { get; set; }*/
    }
}