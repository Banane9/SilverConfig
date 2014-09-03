using SilverConfig.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace SilverConfig
{
    public sealed class SilverConfigXmlSerializer<TConfig> : SilverConfigSerializer<TConfig> where TConfig : new()
    {
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
                if (value == null)
                    throw new ArgumentNullException("value", "Value can't be null.");

                indentation = value;
            }
        }

        [UsedImplicitly]
        public TConfig Deserialize(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return default(TConfig);

            return Deserialize(XDocument.Parse(source).Root);
        }

        public TConfig Deserialize(XElement root)
        {
            if (root == null)
                return default(TConfig);

            var config = new TConfig();

            foreach (var element in serializationInfos)
                deserialize(element, config, root);

            return config;
        }

        [UsedImplicitly]
        public string Serialize([NotNull] TConfig config)
        {
            var root = new XElement(configAttribute.Name ?? configTypeInfo.Name, new XText(Environment.NewLine));

            foreach (var element in serializationInfos)
                root.Add(serialize(element, config, 1).Cast<object>().ToArray());

            return root.ToString(SaveOptions.DisableFormatting);
        }

        private void deserialize(SerializationInfo element, object obj, XElement root)
        {
            var xElement = root.Element(element.AttributeData.Name ?? element.Member.Name);

            if (xElement == null)
                throw new Exception("Element with name [" + element.AttributeData.Name ?? element.Member.Name + "] not found.");

            if (element.Member is PropertyInfo)
                ((PropertyInfo)element.Member).SetValue(obj, xElement.Value);
            else
                ((FieldInfo)element.Member).SetValue(obj, xElement.Value);
        }

        private IEnumerable<XNode> serialize(SerializationInfo element, object obj, uint level)
        {
            if (element.AttributeData.NewLineBefore)
                yield return new XText(Environment.NewLine);

            if (!string.IsNullOrWhiteSpace(element.AttributeData.Comment))
            {
                var comment = string.Join(Environment.NewLine + indentation.Times(1) + "     ", element.AttributeData.Comment.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

                yield return new XText(indentation);
                yield return new XComment(" " + comment + " ");
                yield return new XText(Environment.NewLine);
            }

            var child = new XElement(element.AttributeData.Name ?? element.Member.Name);

            if (element.Member is PropertyInfo)
                child.Add(((PropertyInfo)element.Member).GetValue(obj));
            else
                child.Add(((FieldInfo)element.Member).GetValue(obj));

            yield return new XText(indentation);
            yield return child;
            yield return new XText(Environment.NewLine);
        }
    }
}