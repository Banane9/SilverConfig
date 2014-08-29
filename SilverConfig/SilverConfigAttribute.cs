using SilverConfig.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilverConfig
{
    /// <summary>
    /// Marks a class as being a SilverConfig serializable class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class SilverConfigAttribute : Attribute
    {
        /// <summary>
        /// The name that the Class will be serialized to (if applicable).
        /// </summary>
        [CanBeNull, UsedImplicitly]
        public string Name { get; set; }
    }
}