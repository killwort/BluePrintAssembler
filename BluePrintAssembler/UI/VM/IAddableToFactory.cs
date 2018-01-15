using System;
using BluePrintAssembler.Domain;

namespace BluePrintAssembler.UI.VM
{
    public interface IAddableToFactory
    {
        event EventHandler<BaseProducibleObject> AddedToFactory;
    }
}