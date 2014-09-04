using SilverConfig.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SilverConfig
{
    internal static class MemberInfoUtil
    {
        public static Type GetMemberType([NotNull] this MemberInfo member)
        {
            if (member as PropertyInfo != null)
                return ((PropertyInfo)member).PropertyType;
            else if (member as FieldInfo != null)
                return ((FieldInfo)member).FieldType;
            else
                throw new Exception("Member has to be PropertyInfo or FieldInfo and not null.");
        }

        public static object GetMemberValue([NotNull] this MemberInfo member, [NotNull] object obj)
        {
            if (member as PropertyInfo != null)
                return ((PropertyInfo)member).GetValue(obj);
            else if (member as FieldInfo != null)
                return ((FieldInfo)member).GetValue(obj);
            else
                throw new Exception("Member has to be PropertyInfo or FieldInfo and not null.");
        }

        public static void SetMemberValue([NotNull] this MemberInfo member, [NotNull] object obj, [NotNull] object value)
        {
            if (member as PropertyInfo != null)
                ((PropertyInfo)member).SetValue(obj, value);
            else if (member as FieldInfo != null)
                ((FieldInfo)member).SetValue(obj, value);
            else
                throw new Exception("Member has to be PropertyInfo or FieldInfo and not null.");
        }
    }
}