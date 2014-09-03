using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SilverConfig
{
    public sealed class SerializationInfo
    {
        public SilverConfigElementAttribute AttributeData { get; private set; }

        public MemberInfo Member { get; private set; }

        public SerializationInfo(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<SilverConfigElementAttribute>();

            if (attribute == null)
                throw new ArgumentException("Member has to have the SilverConfigElementAttribute.", "member");

            AttributeData = attribute;
            Member = member;
        }

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