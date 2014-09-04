using SilverConfig.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace SilverConfig
{
    public sealed class SilverConfigXmlSerializer<TConfig> : SilverConfigSerializer<TConfig> where TConfig : new()
    {
        private static MethodInfo toArray = typeof(Enumerable).GetTypeInfo().GetDeclaredMethod("ToArray");
        private string indentation = "  ";

        /// <summary>
        /// Gets or sets the string used for indentation.
        /// </summary>
        [NotNull]
        public string Indentation
        {
            get { return indentation; }
            set
            {
                indentation = value ?? string.Empty;
            }
        }

        protected override TConfig deserialize(string source)
        {
            var config = new TConfig();

            foreach (var element in serializationInfos)
                deserializeElement(element, config, XDocument.Parse(source).Root);

            return config;
        }

        protected override string serialize(TConfig config)
        {
            var root = new XElement(configAttribute.Name ?? configTypeInfo.Name, new XText(Environment.NewLine));

            foreach (var element in serializationInfos)
                root.Add(serializeElement(element, config, 1).Cast<object>().ToArray());

            return root.ToString(SaveOptions.DisableFormatting);
        }

        private void deserializeElement(SerializationInfo element, object obj, XElement root)
        {
            var xElement = root.Element(element.Name);

            if (xElement == null)
                throw new Exception("Element with name [" + element.Name + "] not found.");

            // Simple element(s)
            if (element.SerializationInfos.Count == 0)
            {
                if (!element.Member.GetMemberType().GetTypeInfo().IsArray)
                    element.Member.SetMemberValue(obj, parseValue(element.Member.GetMemberType(), xElement.Value));
                else
                {
                    var elementType = element.Member.GetMemberType().GetTypeInfo().GetElementType();
                    var valueList = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                    var addMethod = valueList.GetType().GetTypeInfo().GetDeclaredMethod("Add");
                    foreach (var item in xElement.Elements().Select(valueElement => parseValue(elementType, valueElement.Value)))
                        addMethod.Invoke(valueList, new object[] { item });

                    element.Member.SetMemberValue(obj, toArray.MakeGenericMethod(elementType).Invoke(null, new object[] { valueList }));
                }
            }
            // Complex element(s)
            else
            {
                if (!element.Member.GetMemberType().GetTypeInfo().IsArray)
                {
                    var elementObj = Activator.CreateInstance(element.Member.GetMemberType());
                    foreach (var subElement in element.SerializationInfos)
                        deserializeElement(subElement, elementObj, xElement);

                    element.Member.SetMemberValue(obj, elementObj);
                }
                else
                {
                    var elementType = element.Member.GetMemberType().GetTypeInfo().GetElementType();
                    var elementObjs = xElement.Elements().Select(valueElement =>
                        {
                            var elementObj = Activator.CreateInstance(elementType);
                            foreach (var subElement in element.SerializationInfos)
                                deserializeElement(subElement, elementObj, xElement);

                            return elementObj;
                        }).ToArray();

                    element.Member.SetMemberValue(obj, elementObjs);
                }
            }
        }

        private IEnumerable<XNode> serializeElement(SerializationInfo element, object obj, uint level)
        {
            if (element.AttributeData.NewLineBefore)
                yield return new XText(Environment.NewLine);

            if (!string.IsNullOrWhiteSpace(element.AttributeData.Comment))
            {
                var comment = string.Join(Environment.NewLine + indentation.Times(level) + "     ", element.AttributeData.Comment.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

                yield return new XText(indentation);
                yield return new XComment(" " + comment + " ");
                yield return new XText(Environment.NewLine);
            }

            yield return new XText(indentation.Times(level));

            var child = new XElement(element.Name);
            var value = element.Member.GetMemberValue(obj);

            // Simple element(s)
            if (element.SerializationInfos.Count == 0)
            {
                if (!element.Member.GetMemberType().GetTypeInfo().IsArray)
                    child.Value = value.ToString();
                else
                {
                    foreach (var item in (IEnumerable)value)
                    {
                        child.Add(new XText(Environment.NewLine + indentation.Times(level + 1)),
                                  new XElement(element.ArrayItemName, item.ToString()));
                    }

                    child.Add(new XText(Environment.NewLine + indentation.Times(level)));
                }
            }
            // Complex element(s)
            else
            {
                if (!element.Member.GetMemberType().GetTypeInfo().IsArray)
                {
                    foreach (var subElement in element.SerializationInfos)
                        child.Add(serializeElement(subElement, value, level + 1));

                    child.Add(new XText(indentation.Times(level)));
                }
                else
                {
                    foreach (var item in (IEnumerable)value)
                    {
                        var subChild = new XElement(element.ArrayItemName);

                        foreach (var subElement in element.SerializationInfos)
                            subChild.Add(serializeElement(subElement, value, level + 2));

                        subChild.Add(new XText(indentation.Times(level + 1)));

                        child.Add(subChild, new XText(Environment.NewLine + indentation.Times(level)));
                    }
                }
            }

            yield return child;
            yield return new XText(Environment.NewLine);
        }
    }
}