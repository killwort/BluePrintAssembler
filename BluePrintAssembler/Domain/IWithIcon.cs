using System.Collections.Generic;

namespace BluePrintAssembler.Domain
{
    public interface IWithIcon
    {
        string Icon { get;  }
        float IconSize { get;  }
        Dictionary<string, IconPart> Icons { get;  }

        IWithIcon FallbackIcon { get; }
    }
}