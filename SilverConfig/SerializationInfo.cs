using SilverConfig.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SilverConfig
{
    /// <summary>
    /// Contains information about a Member of a class or struct that will be (de)serialized.
    /// </summary>
    public sealed class SerializationInfo
    {
        /// <summary>
        /// Gets the SilverConfigElementAttribute for the Member.
        /// </summary>
        [NotNull]
        public SilverConfigElementAttribute AttributeData { get; private set; }

        /// <summary>
        /// Gets the MemberInfo for the Member.
        /// </summary>
        public MemberInfo Member { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializationInfo"/> class for the given Member.
        /// </summary>
        /// <param name="member">The Member that the info is for.</param>
        public SerializationInfo(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<SilverConfigElementAttribute>();

            if (attribute == null)
                throw new ArgumentException("Member has to have the SilverConfigElementAttribute.", "member");

            AttributeData = attribute;
            Member = member;
        }

        /// <summary>
        /// Comparer for sorting the serialization infos based on their index.
        /// </summary>
        public sealed class Comparer : IComparer<SerializationInfo>
        {
            public int Compare(SerializationInfo x, SerializationInfo y)
            {
                if (x.AttributeData.Index > y.AttributeData.Index)
                    return 1;
                else if (x.AttributeData.Index < y.AttributeData.Index)
                    return -1;

                return 0;
            }
        }
    }
}