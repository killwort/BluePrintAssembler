using System;
using System.Collections.Generic;

namespace BluePrintAssembler.Domain
{
    public interface IAltNames
    {
        IEnumerable<Tuple<string, string>> AlternativeNames { get; }
    }
}