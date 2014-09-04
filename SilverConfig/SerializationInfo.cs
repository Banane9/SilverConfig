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
        private Lazy<SortedSet<SerializationInfo>> _serializationInfos;

        private TypeInfo memberTypeInfo;

        /// <summary>
        /// Gets the name for array-items for the Member.
        /// </summary>
        public string ArrayItemName { get; private set; }

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
        /// Gets the name for elements for the Member.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the SerializationInfos for the MemberType.
        /// </summary>
        public SortedSet<SerializationInfo> SerializationInfos
        {
            get { return _serializationInfos.Value; }
        }

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

            var memberTypeInfoTemp = member.GetMemberType().GetTypeInfo();

            if (memberTypeInfoTemp.IsSilverConfigType())
                memberTypeInfo = memberTypeInfoTemp;
            else if (memberTypeInfoTemp.IsArray && memberTypeInfoTemp.GetElementType().GetTypeInfo().IsSilverConfigType())
                memberTypeInfo = memberTypeInfoTemp.GetElementType().GetTypeInfo();

            if (memberTypeInfo != null)
                _serializationInfos = new Lazy<SortedSet<SerializationInfo>>(makeSerializationInfo);
            else
                _serializationInfos = new Lazy<SortedSet<SerializationInfo>>(() => new SortedSet<SerializationInfo>());

            Name = (attribute.Name ?? member.Name).Replace(' ', '_');

            if (attribute as SilverConfigArrayElementAttribute != null)
                ArrayItemName = ((SilverConfigArrayElementAttribute)attribute).ArrayItemName;

            if (ArrayItemName == null)
                ArrayItemName = Name + "Item";
        }

        private SortedSet<SerializationInfo> makeSerializationInfo()
        {
            var sortedSet = new SortedSet<SerializationInfo>(new SerializationInfo.Comparer());

            foreach (var item in memberTypeInfo.GetSilverConfigElements().Select(element => new SerializationInfo(element)))
                sortedSet.Add(item);

            return sortedSet;
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