using System;
using System.Collections.Generic;
using System.Linq;

namespace SilverConfig
{
    /// <summary>
    /// Extends the standard Element Attribute with information specific to arrays.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SilverConfigArrayElementAttribute : SilverConfigElementAttribute
    {
        /// <summary>
        /// The name for items in the array (if applicable).
        /// </summary>
        public string ArrayItemName { get; set; }
    }
}