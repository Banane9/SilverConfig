using SilverConfig.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilverConfig
{
    /// <summary>
    /// Marks a Field or Property as having to be serialized in a <see cref="SilverConfigAttribute"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SilverConfigElementAttribute : Attribute
    {
        /// <summary>
        /// The comment that will be written over the Element.
        /// </summary>
        [CanBeNull, UsedImplicitly]
        public string Comment { get; set; }

        /// <summary>
        /// The index that the Element will have in the serialized Config.
        /// The order of colliding indexes will be determined by how they come up.
        /// Will be sorted in Ascending (0, 1, 2, ...) order.
        /// </summary>
        [UsedImplicitly]
        public uint Index { get; set; }

        /// <summary>
        /// The name that the Field or Property will be serialized to.
        /// </summary>
        [CanBeNull, UsedImplicitly]
        public string Name { get; set; }

        /// <summary>
        /// Whether there will be an empty line before the Element in the serialized Config or not.
        /// </summary>
        [UsedImplicitly]
        public bool NewLineBefore { get; set; }
    }
}