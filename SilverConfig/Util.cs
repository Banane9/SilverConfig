using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SilverConfig
{
    internal static class Util
    {
        public static object GetDefaultValue(this Type t)
        {
            if (t == null)
                return null;

            return t.GetTypeInfo().IsValueType ? Activator.CreateInstance(t) : null;
        }

        public static Type GetMemberType(this MemberInfo member)
        {
            if (member as PropertyInfo != null)
                return ((PropertyInfo)member).PropertyType;
            else if (member as FieldInfo != null)
                return ((FieldInfo)member).FieldType;
            else
                throw new Exception("Member has to be PropertyInfo or FieldInfo.");
        }

        public static bool InheritsOrImplements(this TypeInfo child, TypeInfo parent)
        {
            if (child == null || parent == null)
                return false;

            parent = resolveGenericTypeDefinition(parent);

            var currentChild = child.IsGenericType
                                   ? child.GetGenericTypeDefinition().GetTypeInfo()
                                   : child;

            while (currentChild != typeof(object).GetTypeInfo())
            {
                if (parent == currentChild || hasAnyInterfaces(parent, currentChild))
                    return true;

                currentChild = currentChild.BaseType != null
                               && currentChild.BaseType.GetTypeInfo().IsGenericType
                                   ? currentChild.BaseType.GetTypeInfo().GetGenericTypeDefinition().GetTypeInfo()
                                   : currentChild.BaseType.GetTypeInfo();

                if (currentChild == null)
                    return false;
            }
            return false;
        }

        public static string Times(this string str, uint times)
        {
            if (str == null)
                throw new ArgumentNullException("str", "String can't be null.");

            if (times == 0)
                return string.Empty;

            var stringBuilder = new StringBuilder(str);
            var timesDone = 1;

            while (timesDone < times)
            {
                stringBuilder.Append(str);
                timesDone++;
            }

            return stringBuilder.ToString();
        }

        private static bool hasAnyInterfaces(TypeInfo parent, TypeInfo child)
        {
            return child.ImplementedInterfaces
                        .Any(childInterface =>
                             {
                                 var currentInterface = childInterface.GetTypeInfo().IsGenericType
                                                            ? childInterface.GetGenericTypeDefinition()
                                                            : childInterface;

                                 return currentInterface.GetTypeInfo() == parent;
                             });
        }

        private static TypeInfo resolveGenericTypeDefinition(TypeInfo parent)
        {
            var shouldUseGenericType = !(parent.IsGenericType && parent.GetGenericTypeDefinition().GetTypeInfo() != parent);

            if (parent.IsGenericType && shouldUseGenericType)
                parent = parent.GetGenericTypeDefinition().GetTypeInfo();

            return parent;
        }
    }
}