using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SilverConfig
{
    public abstract class SilverConfigSerializer<TConfig>
    {
        protected SilverConfigAttribute configAttribute;
        protected TypeInfo configTypeInfo;
        private Lazy<SortedSet<SerializationInfo>> _serializationInfos;

        protected SortedSet<SerializationInfo> serializationInfos
        {
            get { return _serializationInfos.Value; }
        }

        protected SilverConfigSerializer()
        {
            configTypeInfo = typeof(TConfig).GetTypeInfo();

            if (configTypeInfo.GetCustomAttribute<SilverConfigAttribute>() == null)
                throw new ArgumentException("TConfig has to have the SilverConfigAttribute.", "TConfig");

            _serializationInfos = new Lazy<SortedSet<SerializationInfo>>(makeSerializationInfo);
            configAttribute = configTypeInfo.GetCustomAttribute<SilverConfigAttribute>();
        }

        protected object resolveValue(MemberInfo member, string value)
        {
            Type memberType;
            if (member is PropertyInfo)
                memberType = ((PropertyInfo)member).PropertyType;
            else
                memberType = ((FieldInfo)member).FieldType;

            if (memberType == typeof(string))
                return value;
            else if (memberType == typeof(bool)
                || memberType == typeof(sbyte) || memberType == typeof(byte)
                || memberType == typeof(short) || memberType == typeof(ushort)
                || memberType == typeof(int) || memberType == typeof(uint)
                || memberType == typeof(long) || memberType == typeof(ulong)
                || memberType == typeof(float) || memberType == typeof(double))
                return memberType.GetTypeInfo().GetDeclaredMethods("Parse").Single(method => method.GetParameters().Length == 1).Invoke(null, new object[] { value });
            else
                throw new NotSupportedException("MemberType [" + memberType.Name + "] is not supported.");
        }

        private SortedSet<SerializationInfo> makeSerializationInfo()
        {
            var sortedSet = new SortedSet<SerializationInfo>(new SerializationInfo.Comparer());
            foreach (var item in configTypeInfo.GetSilverConfigElements().Select(element => new SerializationInfo(element)))
                sortedSet.Add(item);

            return sortedSet;
        }
    }
}