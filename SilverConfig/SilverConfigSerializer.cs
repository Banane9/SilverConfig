using SilverConfig.Annotations;
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

        /// <summary>
        /// Trys to deserialize the given string into a Config Object.
        /// </summary>
        /// <param name="source">The string containing the serialized data.</param>
        /// <returns>The deserialized Config Object, or null.</returns>
        [CanBeNull, UsedImplicitly]
        public TConfig Deserialize(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return default(TConfig);

            return deserialize(source);
        }

        /// <summary>
        /// Serializes the given Config Object into a string.
        /// </summary>
        /// <param name="config">The Config Object to be serialized.</param>
        /// <returns>The string containing the data.</returns>
        [NotNull, UsedImplicitly]
        public string Serialize([NotNull] TConfig config)
        {
            if (config == null)
                return string.Empty;

            return serialize(config);
        }

        protected abstract TConfig deserialize([NotNull] string source);

        protected object parseValue(Type type, string value)
        {
            if (type == typeof(string))
                return value;
            else if (type == typeof(bool)
                || type == typeof(sbyte) || type == typeof(byte)
                || type == typeof(short) || type == typeof(ushort)
                || type == typeof(int) || type == typeof(uint)
                || type == typeof(long) || type == typeof(ulong)
                || type == typeof(float) || type == typeof(double))
                return type.GetTypeInfo().GetDeclaredMethods("Parse").Single(method => method.GetParameters().Length == 1).Invoke(null, new object[] { value });
            else
                throw new NotSupportedException("MemberType [" + type.Name + "] is not supported.");
        }

        protected abstract string serialize([NotNull] TConfig config);

        private SortedSet<SerializationInfo> makeSerializationInfo()
        {
            var sortedSet = new SortedSet<SerializationInfo>(new SerializationInfo.Comparer());
            foreach (var item in configTypeInfo.GetSilverConfigElements().Select(element => new SerializationInfo(element)))
                sortedSet.Add(item);

            return sortedSet;
        }
    }
}