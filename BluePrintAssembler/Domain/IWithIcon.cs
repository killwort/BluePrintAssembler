using System.Collections.Generic;

namespace BluePrintAssembler.Domain
{
    public interface IWithIcon
    {
        string Icon { get; set; }
        float IconSize { get; set; }
        Dictionary<string, IconPart> Icons { get; set; }

        IWithIcon FallbackIcon { get; }
    }
}