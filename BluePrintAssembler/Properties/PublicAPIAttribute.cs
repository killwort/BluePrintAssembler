﻿using System;

namespace BluePrintAssembler.Annotations
{
    /// <summary>
    /// This attribute is intended to mark publicly available API
    /// which should not be removed and so is treated as used.
    /// </summary>
    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    public sealed class PublicAPIAttribute : Attribute
    {
        public PublicAPIAttribute() { }

        public PublicAPIAttribute([NotNull] string comment)
        {
            Comment = comment;
        }

        [CanBeNull] public string Comment { get; private set; }
    }
}