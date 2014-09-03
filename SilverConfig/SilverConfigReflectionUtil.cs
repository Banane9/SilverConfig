using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SilverConfig
{
    internal static class SilverConfigReflectionUtil
    {
        public static IEnumerable<MemberInfo> GetSilverConfigElements(this TypeInfo type)
        {
            if (type == null)
                throw new ArgumentNullException("type", "Type can't be null.");

            if (!type.IsSilverConfigType())
                throw new ArgumentException("Type has to have the SilverConfigAttribute.", "type");

            while (type.BaseType != null)
            {
                foreach (var member in type.DeclaredMembers.Where(member => member.IsSilverConfigMember()))
                    yield return member;

                type = type.BaseType.GetTypeInfo();
            }
        }

        public static bool IsSilverConfigMember(this MemberInfo member)
        {
            return member.CustomAttributes.Any(attributeData => attributeData.AttributeType == typeof(SilverConfigElementAttribute));
        }

        public static bool IsSilverConfigType(this TypeInfo type)
        {
            return type.CustomAttributes.Any(attributeData => attributeData.AttributeType == typeof(SilverConfigAttribute));
        }
    }
}