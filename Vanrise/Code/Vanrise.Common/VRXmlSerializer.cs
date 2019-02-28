﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Vanrise.Common.VRXmlSerializerExtensions;

namespace Vanrise.Common
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = false)]
    public sealed class VRXmlDeserializeAsAttribute : Attribute
    {
        /// <summary>
        /// The name to use for the serialized element
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Sets if the property to Deserialize is an Attribute or Element (Default: false)
        /// </summary>
        public bool Attribute { get; set; }
        /// <summary>
        /// Sets if the property to Deserialize is a content of current Element (Default: false)
        /// </summary>
        public bool Content { get; set; }
    }

    public class VRXmlSerializer //: IDeserializer
    {
        public VRXmlSerializer()
        {
            Culture = CultureInfo.InvariantCulture;
        }
        public CultureInfo Culture { get; set; }
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        //public virtual T Deserialize<T>(IRestResponse response)
        //{
        //    return Deserialize<T>(response.Content);
        //}

        public T Deserialize<T>(string content)
        {
            if (string.IsNullOrEmpty(content))
                return default(T);
            var doc = XDocument.Parse(content);
            var root = doc.Root;
            if (RootElement.HasValue() && doc.Root != null)
                root = doc.Root.DescendantsAndSelf(RootElement.AsNamespaced(Namespace)).SingleOrDefault();
            // autodetect xml namespace
            if (!Namespace.HasValue())
                RemoveNamespace(doc);
            var x = Activator.CreateInstance<T>();
            var objType = x.GetType();
            if (objType.IsSubclassOfRawGeneric(typeof(List<>)))
                x = (T)HandleListDerivative(root, objType.Name, objType);
            else
                x = (T)Map(x, root);
            return x;
        }

        public string Serialize(Object o)
        {
            return new DotNetXmlSerializer().Serialize(o);
        }

        public string Serialize(Object o, Dictionary<string, string> namespaces)
        {
            return new DotNetXmlSerializer().Serialize(o, namespaces);
        }

        private static void RemoveNamespace(XDocument xdoc)
        {
            if (xdoc.Root == null) return;

            foreach (var e in xdoc.Root.DescendantsAndSelf())
            {
                if (e.Name.Namespace != XNamespace.None)
                    e.Name = XNamespace.None.GetName(e.Name.LocalName);
                if (e.Attributes()
                    .Any(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
                    e.ReplaceAttributes(
                        e.Attributes()
                            .Select(a => a.IsNamespaceDeclaration
                                ? null
                                : a.Name.Namespace != XNamespace.None
                                    ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value)
                                    : a));
            }
        }
        //Complex Method  (complexity = 42)
        protected virtual object Map(object x, XElement root)
        {
            Type objType = x.GetType();
            PropertyInfo[] props = objType.GetProperties();
            bool deserializeFromContentAttributeAlreadyUsed = false;
            foreach (var prop in props)
            {
                var type = prop.PropertyType.GetTypeInfo();
                var typeIsPublic = type.IsPublic || type.IsNestedPublic;
                if (!typeIsPublic || !prop.CanWrite)
                    continue;
                bool deserializeFromContent = false;
                bool isNameDefinedInAttribute = false;
                XName name = null;
                var attributes = prop.GetCustomAttributes(typeof(VRXmlDeserializeAsAttribute), false);
                if (attributes.Any())
                {
                    VRXmlDeserializeAsAttribute attribute = (VRXmlDeserializeAsAttribute)attributes.First();
                    name = attribute.Name.AsNamespaced(Namespace);
                    isNameDefinedInAttribute = name != null && !string.IsNullOrEmpty(name.LocalName);
                    deserializeFromContent = attribute.Content;
                    if (deserializeFromContentAttributeAlreadyUsed && deserializeFromContent)
                    {
                        throw new ArgumentException("Class cannot have two properties marked with " +
                            "SerializeAs(Content = true) attribute.");
                    }
                    deserializeFromContentAttributeAlreadyUsed |= deserializeFromContent;
                }
                if (name == null)
                {
                    name = prop.Name.AsNamespaced(Namespace);
                }
                var value = GetValueFromXml(root, name, prop, isNameDefinedInAttribute);
                if (value == null)
                {
                    // special case for text content node
                    if (deserializeFromContent)
                    {
                        var textNode = root.Nodes().FirstOrDefault(n => n is XText);
                        if (textNode != null)
                        {
                            value = ((XText)textNode).Value;
                            prop.SetValue(x, value, null);
                        }
                        continue;
                    }
                    // special case for inline list items
                    if (type.IsGenericType)
                    {
                        var genericType = type.GetGenericArguments()[0];
                        var first = GetElementByName(root, genericType.Name);
                        var list = (IList)Activator.CreateInstance(type.AsType());
                        if (first != null && root != null)
                        {
                            var elements = root.Elements(first.Name);
                            PopulateListFromElements(genericType, elements, list);
                        }
                        prop.SetValue(x, list, null);
                    }
                    continue;
                }
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // if the value is empty, set the property to null...
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        prop.SetValue(x, null, null);
                        continue;
                    }
                    type = type.GetGenericArguments()[0].GetTypeInfo();
                }
                var asType = type.AsType();
                if (asType == typeof(bool))
                {
                    var toConvert = value.ToString()
                        .ToLower();
                    prop.SetValue(x, XmlConvert.ToBoolean(toConvert), null);
                }
                else if (type.IsPrimitive)
                {
                    prop.SetValue(x, value.ChangeType(asType, Culture), null);
                }
                else if (type.IsEnum)
                {
                    var converted = type.AsType().FindEnumValue(value.ToString(), Culture);
                    prop.SetValue(x, converted, null);
                }
                else if (asType == typeof(Uri))
                {
                    var uri = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
                    prop.SetValue(x, uri, null);
                }
                else if (asType == typeof(string))
                {
                    prop.SetValue(x, value, null);
                }
                else if (asType == typeof(DateTime))
                {
                    value = DateFormat.HasValue()
                        ? DateTime.ParseExact(value.ToString(), DateFormat, Culture)
                        : DateTime.Parse(value.ToString(), Culture);
                    prop.SetValue(x, value, null);
                }
                else if (asType == typeof(DateTimeOffset))
                {
                    var toConvert = value.ToString();
                    if (string.IsNullOrEmpty(toConvert)) continue;
                    DateTimeOffset deserialisedValue;
                    try
                    {
                        deserialisedValue = XmlConvert.ToDateTimeOffset(toConvert);
                        prop.SetValue(x, deserialisedValue, null);
                    }
                    catch (Exception)
                    {
                        object result;
                        if (TryGetFromString(toConvert, out result, asType))
                        {
                            prop.SetValue(x, result, null);
                        }
                        else
                        {
                            //fallback to parse
                            deserialisedValue = DateTimeOffset.Parse(toConvert);
                            prop.SetValue(x, deserialisedValue, null);
                        }
                    }
                }
                else if (asType == typeof(decimal))
                {
                    value = decimal.Parse(value.ToString(), Culture);
                    prop.SetValue(x, value, null);
                }
                else if (asType == typeof(Guid))
                {
                    var raw = value.ToString();
                    value = string.IsNullOrEmpty(raw)
                        ? Guid.Empty
                        : new Guid(value.ToString());
                    prop.SetValue(x, value, null);
                }
                else if (asType == typeof(TimeSpan))
                {
                    var timeSpan = XmlConvert.ToTimeSpan(value.ToString());
                    prop.SetValue(x, timeSpan, null);
                }
                else if (type.IsGenericType)
                {
                    var t = type.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(asType);
                    var container = this.GetElementByName(root, name);
                    if (container.HasElements)
                    {
                        var first = container.Elements().FirstOrDefault();
                        if (first != null)
                        {
                            var elements = container.Elements(first.Name);
                            PopulateListFromElements(t, elements, list);
                        }
                    }
                    prop.SetValue(x, list, null);
                }
                else if (asType.IsSubclassOfRawGeneric(typeof(List<>)))
                {
                    // handles classes that derive from List<T>
                    // e.g. a collection that also has attributes
                    var list = HandleListDerivative(root, prop.Name, asType);
                    prop.SetValue(x, list, null);
                }
                else
                {
                    object result;
                    //fallback to type converters if possible
                    if (TryGetFromString(value.ToString(), out result, asType))
                    {
                        prop.SetValue(x, result, null);
                    }
                    else
                    {
                        // nested property classes
                        if (root == null) continue;
                        var element = GetElementByName(root, name);
                        if (element == null) continue;
                        var item = CreateAndMap(asType, element);
                        prop.SetValue(x, item, null);
                    }
                }
            }
            return x;
        }
        private static bool TryGetFromString(string inputString, out object result, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                result = converter.ConvertFromInvariantString(inputString);
                return true;
            }
            result = null;
            return false;
        }
        private void PopulateListFromElements(Type t, IEnumerable<XElement> elements, IList list)
        {
            foreach (var item in elements.Select(element => CreateAndMap(t, element)))
                list.Add(item);
        }
        private object HandleListDerivative(XElement root, string propName, Type type)
        {
            var t = type.IsGenericType
                ? type.GetGenericArguments()[0]
                : type.BaseType.GetGenericArguments()[0];
            var list = (IList)Activator.CreateInstance(type);
            IList<XElement> elements = root.Descendants(t.Name.AsNamespaced(Namespace))
                .ToList();
            var name = t.Name;
            var attribute = t.GetAttribute<VRXmlDeserializeAsAttribute>();
            if (attribute != null)
                name = attribute.Name;
            if (!elements.Any())
            {
                var lowerName = name.ToLower().AsNamespaced(Namespace);
                elements = root.Descendants(lowerName).ToList();
            }
            if (!elements.Any())
            {
                var camelName = name.ToCamelCase(Culture).AsNamespaced(Namespace);
                elements = root.Descendants(camelName).ToList();
            }
            if (!elements.Any())
                elements = root.Descendants()
                    .Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == name)
                    .ToList();
            if (!elements.Any())
            {
                var lowerName = name.ToLower().AsNamespaced(Namespace);
                elements = root.Descendants()
                    .Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == lowerName)
                    .ToList();
            }
            PopulateListFromElements(t, elements, list);
            // get properties too, not just list items
            // only if this isn't a generic type
            if (!type.IsGenericType)
                Map(list, root.Element(propName.AsNamespaced(Namespace)) ?? root);
            return list;
        }
        protected virtual object CreateAndMap(Type t, XElement element)
        {
            object item;
            if (t == typeof(string))
            {
                item = element.Value;
            }
            else if (t.GetTypeInfo().IsPrimitive)
            {
                item = element.Value.ChangeType(t, Culture);
            }
            else
            {
                item = Activator.CreateInstance(t);
                Map(item, element);
            }
            return item;
        }
        protected virtual object GetValueFromXml(XElement root, XName name, PropertyInfo prop, bool useExactName)
        {
            object val = null;
            if (root == null) return val;
            var element = GetElementByName(root, name);
            if (element == null)
            {
                var attribute = GetAttributeByName(root, name, useExactName);
                if (attribute != null)
                    val = attribute.Value;
            }
            else
            {
                if (!element.IsEmpty || element.HasElements || element.HasAttributes)
                    val = element.Value;
            }
            return val;
        }
        protected virtual XElement GetElementByName(XElement root, XName name)
        {
            var lowerName = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
            var camelName = name.LocalName.ToCamelCase(Culture).AsNamespaced(name.NamespaceName);
            if (root.Element(name) != null)
                return root.Element(name);
            if (root.Element(lowerName) != null)
                return root.Element(lowerName);
            if (root.Element(camelName) != null)
                return root.Element(camelName);
            // try looking for element that matches sanitized property name (Order by depth)
            var element = root.Descendants()
                       .OrderBy(d => d.Ancestors().Count())
                       .FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName) ??
                   root.Descendants()
                       .OrderBy(d => d.Ancestors().Count())
                       .FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName.ToLower());
            return element == null && name == "Value".AsNamespaced(name.NamespaceName) &&
                   (!root.HasAttributes || root.Attributes().All(x => x.Name != name))
                ? root
                : element;
        }
        protected virtual XAttribute GetAttributeByName(XElement root, XName name, bool useExactName)
        {
            var names = useExactName
                ? null
                : new List<XName>
                {
                    name.LocalName,
                    name.LocalName.ToLower()
                        .AsNamespaced(name.NamespaceName),
                    name.LocalName.ToCamelCase(Culture)
                        .AsNamespaced(name.NamespaceName)
                };
            return root.DescendantsAndSelf()
                .OrderBy(d => d.Ancestors().Count())
                .Attributes()
                .FirstOrDefault(
                    d => useExactName
                        ? d.Name == name
                        : names.Contains(d.Name.LocalName.RemoveUnderscoresAndDashes()));
        }
    }

    internal class DotNetXmlSerializer
    {
        /// <summary>
        ///     Default constructor, does not specify namespace
        /// </summary>
        public DotNetXmlSerializer()
        {
            Encoding = Encoding.UTF8;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Specify the namespaced to be used when serializing
        /// </summary>
        /// <param name="namespace">XML namespace</param>
        public DotNetXmlSerializer(string @namespace)
            : this()
        {
            Namespace = @namespace;
        }

        /// <summary>
        ///     Encoding for serialized content
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Serialize the object as XML
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>XML as string</returns>
        public string Serialize(object obj)
        {
            var ns = new System.Xml.Serialization.XmlSerializerNamespaces();

            ns.Add(string.Empty, Namespace);

            var serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            var writer = new EncodingStringWriter(Encoding);

            serializer.Serialize(writer, obj, ns);

            return writer.ToString();
        }

        public string Serialize(object obj, Dictionary<string, string> namespaces)
        {
            var ns = new System.Xml.Serialization.XmlSerializerNamespaces();

            if (namespaces != null && namespaces.Count > 0)
            {
                foreach (var item in namespaces)
                {
                    ns.Add(item.Key, item.Value);
                }
            }

            var serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            var writer = new EncodingStringWriter(Encoding);

            serializer.Serialize(writer, obj, ns);

            return writer.ToString();
        }

        /// <summary>
        ///     Name of the root element to use when serializing
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        ///     XML namespace to use when serializing
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Format string to use when serializing dates
        /// </summary>
        public string DateFormat { get; set; }


        private class EncodingStringWriter : System.IO.StringWriter
        {
            // Need to subclass StringWriter in order to override Encoding
            public EncodingStringWriter(Encoding encoding) => Encoding = encoding;

            public override Encoding Encoding { get; }
        }
    }
}
