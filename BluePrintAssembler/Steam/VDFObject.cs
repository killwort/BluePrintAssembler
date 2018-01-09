using System;
using System.Collections.Generic;

namespace BluePrintAssembler.Steam
{
    internal class VDFObject
    {
        public string Value;

        public string UnescapedValue => Value.Substring(1, Value.Length - 2).Replace("\\\"", "\"").Replace("\\\\", "\\");
        public List<Tuple<string, VDFObject>> Values;
    }
}
