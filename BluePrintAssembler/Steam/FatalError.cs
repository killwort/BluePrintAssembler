using System;

namespace BluePrintAssembler.Steam
{
    internal class FatalError: Exception {
        public FatalError(string m): base(m) {}
    }
}