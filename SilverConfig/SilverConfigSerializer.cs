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

        private SortedSet<SerializationInfo> makeSerializationInfo()
        {
            var sortedSet = new SortedSet<SerializationInfo>(new SerializationInfo.Comparer());
            foreach (var item in configTypeInfo.GetSilverConfigElements().Select(element => new SerializationInfo(element)))
                sortedSet.Add(item);

            return sortedSet;
        }
    }
}