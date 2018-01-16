using System;

namespace BluePrintAssembler.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class AspDataFieldsAttribute : Attribute { }
}