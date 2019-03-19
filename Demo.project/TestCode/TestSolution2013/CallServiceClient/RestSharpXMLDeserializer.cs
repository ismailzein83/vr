//#region License
////   Copyright 2010 John Sheehan
////
////   Licensed under the Apache License, Version 2.0 (the "License");
////   you may not use this file except in compliance with the License.
////   You may obtain a copy of the License at
////
////     http://www.apache.org/licenses/LICENSE-2.0
////
////   Unless required by applicable law or agreed to in writing, software
////   distributed under the License is distributed on an "AS IS" BASIS,
////   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
////   See the License for the specific language governing permissions and
////   limitations under the License. 
//#endregion
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Reflection;
//using System.Xml;
//using System.Xml.Linq;
////using RestSharp.Extensions;
//using System.ComponentModel;
//using System.Text;
//using System.Web;
//using System.Text.RegularExpressions;
//namespace CallServiceClient
//{
//    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = false)]
//    public sealed class VRXmlDeserializeAsAttribute : Attribute
//    {
//        /// <summary>
//        /// The name to use for the serialized element
//        /// </summary>
//        public string Name { get; set; }
//        /// <summary>
//        /// Sets if the property to Deserialize is an Attribute or Element (Default: false)
//        /// </summary>
//        public bool Attribute { get; set; }
//        /// <summary>
//        /// Sets if the property to Deserialize is a content of current Element (Default: false)
//        /// </summary>
//        public bool Content { get; set; }
//    }

//    public static class RestSharpXmlExtensions
//    {
//        /// <summary>
//        /// Returns the name of an element with the namespace if specified
//        /// </summary>
//        /// <param name="name">Element name</param>
//        /// <param name="namespace">XML Namespace</param>
//        /// <returns></returns>
//        public static XName AsNamespaced(this string name, string @namespace)
//        {
//            XName xName = name;
//            if (@namespace.HasValue())
//            {
//                xName = XName.Get(name, @namespace);
//            }
//            return xName;
//        }
//    }

//    public static class StringExtensions
//    {
//        public static string UrlDecode(this string input)
//        {
//            return System.Web.HttpUtility.UrlDecode(input);
//        }
//        /// <summary>
//        ///     Uses Uri.EscapeDataString() based on recommendations on MSDN
//        ///     http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
//        /// </summary>
//        public static string UrlEncode(this string input)
//        {
//            const int maxLength = 32766;
//            if (input == null)
//                throw new ArgumentNullException("input");
//            if (input.Length <= maxLength)
//                return Uri.EscapeDataString(input);
//            var sb = new StringBuilder(input.Length * 2);
//            var index = 0;
//            while (index < input.Length)
//            {
//                var length = Math.Min(input.Length - index, maxLength);
//                var subString = input.Substring(index, length);
//                sb.Append(Uri.EscapeDataString(subString));
//                index += subString.Length;
//            }
//            return sb.ToString();
//        }
//        public static string HtmlDecode(this string input)
//        {
//            return HttpUtility.HtmlDecode(input);
//        }
//        public static string HtmlEncode(this string input)
//        {
//            return HttpUtility.HtmlEncode(input);
//        }
//        public static string UrlEncode(this string input, Encoding encoding)
//        {
//            return HttpUtility.UrlEncode(input, encoding);
//        }
//        public static string HtmlAttributeEncode(this string input)
//        {
//            return HttpUtility.HtmlAttributeEncode(input);
//        }
//        /// <summary>
//        ///     Check that a string is not null or empty
//        /// </summary>
//        /// <param name="input">String to check</param>
//        /// <returns>bool</returns>
//        public static bool HasValue(this string input)
//        {
//            return !string.IsNullOrEmpty(input);
//        }
//        /// <summary>
//        ///     Remove underscores from a string
//        /// </summary>
//        /// <param name="input">String to process</param>
//        /// <returns>string</returns>
//        public static string RemoveUnderscoresAndDashes(this string input)
//        {
//            return input.Replace("_", "").Replace("-", "");
//        }

//        private static readonly Regex DateRegex = new Regex(@"\\?/Date\((-?\d+)(-|\+)?([0-9]{4})?\)\\?/");
//        private static readonly Regex NewDateRegex = new Regex(@"newDate\((-?\d+)*\)");
//        /// <summary>
//        ///     Parses most common JSON date formats
//        /// </summary>
//        /// <param name="input">JSON value to parse</param>
//        /// <param name="culture"></param>
//        /// <returns>DateTime</returns>
//        public static DateTime ParseJsonDate(this string input, CultureInfo culture)
//        {
//            const long maxAllowedTimestamp = 253402300799;
//            input = input.Replace("\n", "");
//            input = input.Replace("\r", "");
//            input = input.RemoveSurroundingQuotes();
//            long unix;
//            if (long.TryParse(input, out unix))
//            {
//                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//                return unix > maxAllowedTimestamp ? epoch.AddMilliseconds(unix) : epoch.AddSeconds(unix);
//            }
//            if (input.Contains("/Date("))
//                return ExtractDate(input, DateRegex, culture);
//            if (input.Contains("new Date("))
//            {
//                input = input.Replace(" ", "");
//                // because all whitespace is removed, match against newDate( instead of new Date(
//                return ExtractDate(input, NewDateRegex, culture);
//            }
//            return ParseFormattedDate(input, culture);
//        }
//        /// <summary>
//        ///     Remove leading and trailing " from a string
//        /// </summary>
//        /// <param name="input">String to parse</param>
//        /// <returns>String</returns>
//        public static string RemoveSurroundingQuotes(this string input)
//        {
//            if (input.StartsWith("\"") && input.EndsWith("\""))
//                input = input.Substring(1, input.Length - 2);
//            return input;
//        }
//        private static DateTime ParseFormattedDate(string input, CultureInfo culture)
//        {
//            string[] formats =
//            {
//                "u",
//                "s",
//                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
//                "yyyy-MM-ddTHH:mm:ssZ",
//                "yyyy-MM-dd HH:mm:ssZ",
//                "yyyy-MM-ddTHH:mm:ss",
//                "yyyy-MM-ddTHH:mm:sszzzzzz",
//                "M/d/yyyy h:mm:ss tt" // default format for invariant culture
//            };
//            DateTime date;
//            if (DateTime.TryParseExact(input, formats, culture,
//                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out date))
//                return date;
//            return DateTime.TryParse(input, culture, DateTimeStyles.None, out date) ? date : default(DateTime);
//        }
//        private static DateTime ExtractDate(string input, Regex regex, CultureInfo culture)
//        {
//            var dt = DateTime.MinValue;
//            if (!regex.IsMatch(input)) return dt;
//            var matches = regex.Matches(input);
//            var match = matches[0];
//            var ms = Convert.ToInt64(match.Groups[1].Value);
//            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//            dt = epoch.AddMilliseconds(ms);
//            // adjust if time zone modifier present
//            if (match.Groups.Count <= 2 || string.IsNullOrEmpty(match.Groups[3].Value)) return dt;
//            var mod = DateTime.ParseExact(match.Groups[3].Value, "HHmm", culture);
//            dt = match.Groups[2].Value == "+"
//                ? dt.Add(mod.TimeOfDay)
//                : dt.Subtract(mod.TimeOfDay);
//            return dt;
//        }
//        /// <summary>
//        ///     Converts a string to pascal case
//        /// </summary>
//        /// <param name="lowercaseAndUnderscoredWord">String to convert</param>
//        /// <param name="culture"></param>
//        /// <returns>string</returns>
//        public static string ToPascalCase(this string lowercaseAndUnderscoredWord, CultureInfo culture)
//        {
//            return ToPascalCase(lowercaseAndUnderscoredWord, true, culture);
//        }
//        /// <summary>
//        ///     Converts a string to pascal case with the option to remove underscores
//        /// </summary>
//        /// <param name="text">String to convert</param>
//        /// <param name="removeUnderscores">Option to remove underscores</param>
//        /// <param name="culture"></param>
//        /// <returns></returns>
//        public static string ToPascalCase(this string text, bool removeUnderscores, CultureInfo culture)
//        {
//            if (string.IsNullOrEmpty(text))
//                return text;
//            text = text.Replace("_", " ");
//            var joinString = removeUnderscores
//                ? string.Empty
//                : "_";
//            var words = text.Split(' ');
//            if (words.Length <= 1 && !words[0].IsUpperCase())
//                return string.Concat(words[0].Substring(0, 1).ToUpper(culture), words[0].Substring(1));
//            for (var i = 0; i < words.Length; i++)
//            {
//                if (words[i].Length <= 0) continue;
//                var word = words[i];
//                var restOfWord = word.Substring(1);
//                if (restOfWord.IsUpperCase())
//                    restOfWord = restOfWord.ToLower(culture);
//                var firstChar = char.ToUpper(word[0], culture);
//                words[i] = string.Concat(firstChar, restOfWord);
//            }
//            return string.Join(joinString, words);
//        }
//        /// <summary>
//        ///     Converts a string to camel case
//        /// </summary>
//        /// <param name="lowercaseAndUnderscoredWord">String to convert</param>
//        /// <param name="culture"></param>
//        /// <returns>String</returns>
//        public static string ToCamelCase(this string lowercaseAndUnderscoredWord, CultureInfo culture)
//        {
//            return MakeInitialLowerCase(ToPascalCase(lowercaseAndUnderscoredWord, culture));
//        }
//        /// <summary>
//        ///     Convert the first letter of a string to lower case
//        /// </summary>
//        /// <param name="word">String to convert</param>
//        /// <returns>string</returns>
//        public static string MakeInitialLowerCase(this string word)
//        {
//            return string.Concat(word.Substring(0, 1).ToLower(), word.Substring(1));
//        }
//        private static readonly Regex IsUpperCaseRegex = new Regex(@"^[A-Z]+$");
//        /// <summary>
//        ///     Checks to see if a string is all uppper case
//        /// </summary>
//        /// <param name="inputString">String to check</param>
//        /// <returns>bool</returns>
//        public static bool IsUpperCase(this string inputString)
//        {

//            return IsUpperCaseRegex.IsMatch(inputString);
//        }
//        private static readonly Regex AddUnderscoresRegex1 = new Regex(@"[-\s]");
//        private static readonly Regex AddUnderscoresRegex2 = new Regex(@"([a-z\d])([A-Z])");
//        private static readonly Regex AddUnderscoresRegex3 = new Regex(@"([A-Z]+)([A-Z][a-z])");
//        /// <summary>
//        ///     Add underscores to a pascal-cased string
//        /// </summary>
//        /// <param name="pascalCasedWord">String to convert</param>
//        /// <returns>string</returns>
//        public static string AddUnderscores(this string pascalCasedWord)
//        {
//            return AddUnderscoresRegex1.Replace(
//                AddUnderscoresRegex2.Replace(
//                    AddUnderscoresRegex3.Replace(pascalCasedWord, "$1_$2"),
//                    "$1_$2"),
//                "_");
//        }
//        private static readonly Regex AddDashesRegex1 = new Regex(@"[\s]");
//        private static readonly Regex AddDashesRegex2 = new Regex(@"([a-z\d])([A-Z])");
//        private static readonly Regex AddDashesRegex3 = new Regex(@"([A-Z]+)([A-Z][a-z])");
//        /// <summary>
//        ///     Add dashes to a pascal-cased string
//        /// </summary>
//        /// <param name="pascalCasedWord">String to convert</param>
//        /// <returns>string</returns>
//        public static string AddDashes(this string pascalCasedWord)
//        {
//            return AddDashesRegex1.Replace(
//                   AddDashesRegex2.Replace(
//                       AddDashesRegex3.Replace(pascalCasedWord, "$1-$2"),
//                       "$1-$2"),
//                   "-");
//        }
//        /// <summary>
//        ///     Add an undescore prefix to a pascasl-cased string
//        /// </summary>
//        /// <param name="pascalCasedWord"></param>
//        /// <returns></returns>
//        public static string AddUnderscorePrefix(this string pascalCasedWord)
//        {
//            return string.Format("_{0}", pascalCasedWord);
//        }
//        private static readonly Regex AddSpacesRegex1 = new Regex(@"[-\s]");
//        private static readonly Regex AddSpacesRegex2 = new Regex(@"([a-z\d])([A-Z])");
//        private static readonly Regex AddSpacesRegex3 = new Regex(@"([A-Z]+)([A-Z][a-z])");
//        /// <summary>
//        ///     Add spaces to a pascal-cased string
//        /// </summary>
//        /// <param name="pascalCasedWord">String to convert</param>
//        /// <returns>string</returns>
//        public static string AddSpaces(this string pascalCasedWord)
//        {
//            return AddSpacesRegex1.Replace(
//                   AddSpacesRegex2.Replace(
//                       AddSpacesRegex3.Replace(pascalCasedWord, "$1 $2"),
//                       "$1 $2"),
//                   " ");
//        }

//        /// <summary>
//        ///     Return possible variants of a name for name matching.
//        /// </summary>
//        /// <param name="name">String to convert</param>
//        /// <param name="culture">The culture to use for conversion</param>
//        /// <returns>IEnumerable&lt;string&gt;</returns>
//        public static IEnumerable<string> GetNameVariants(this string name, CultureInfo culture)
//        {
//            if (string.IsNullOrEmpty(name))
//                yield break;
//            yield return name;
//            // try camel cased name
//            yield return name.ToCamelCase(culture);
//            // try lower cased name
//            yield return name.ToLower(culture);
//            // try name with underscores
//            yield return name.AddUnderscores();
//            // try name with underscores with lower case
//            yield return name.AddUnderscores().ToLower(culture);
//            // try name with dashes
//            yield return name.AddDashes();
//            // try name with dashes with lower case
//            yield return name.AddDashes().ToLower(culture);
//            // try name with underscore prefix
//            yield return name.AddUnderscorePrefix();
//            // try name with underscore prefix, using camel case
//            yield return name.ToCamelCase(culture).AddUnderscorePrefix();
//            // try name with spaces
//            yield return name.AddSpaces();
//            // try name with spaces with lower case
//            yield return name.AddSpaces().ToLower(culture);
//        }
//    }

//    public static class ReflectionExtensions
//    {
//        /// <summary>
//        /// Retrieve an attribute from a member (property)
//        /// </summary>
//        /// <typeparam name="T">Type of attribute to retrieve</typeparam>
//        /// <param name="prop">Member to retrieve attribute from</param>
//        /// <returns></returns>
//        public static T GetAttribute<T>(this MemberInfo prop) where T : Attribute
//        {
//            return Attribute.GetCustomAttribute(prop, typeof(T)) as T;
//        }
//        /// <summary>
//        /// Retrieve an attribute from a type
//        /// </summary>
//        /// <typeparam name="T">Type of attribute to retrieve</typeparam>
//        /// <param name="type">Type to retrieve attribute from</param>
//        /// <returns></returns>
//        public static T GetAttribute<T>(this Type type) where T : Attribute
//        {
//            return Attribute.GetCustomAttribute(type, typeof(T)) as T;
//        }
//        /// <summary>
//        /// Checks a type to see if it derives from a raw generic (e.g. List[[]])
//        /// </summary>
//        /// <param name="toCheck"></param>
//        /// <param name="generic"></param>
//        /// <returns></returns>
//        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
//        {
//            while (toCheck != null && toCheck != typeof(object))
//            {
//                Type cur = toCheck.GetTypeInfo().IsGenericType
//                    ? toCheck.GetGenericTypeDefinition()
//                    : toCheck;
//                if (generic == cur)
//                {
//                    return true;
//                }
//                toCheck = toCheck.GetTypeInfo().BaseType;
//            }
//            return false;
//        }
//        public static object ChangeType(this object source, TypeInfo newType)
//        {
//            return Convert.ChangeType(source, newType.AsType());
//        }
//        public static object ChangeType(this object source, Type newType, CultureInfo culture)
//        {
//            return Convert.ChangeType(source, newType);
//        }
//        /// <summary>
//        /// Find a value from a System.Enum by trying several possible variants
//        /// of the string value of the enum.
//        /// </summary>
//        /// <param name="type">Type of enum</param>
//        /// <param name="value">Value for which to search</param>
//        /// <param name="culture">The culture used to calculate the name variants</param>
//        /// <returns></returns>
//        public static object FindEnumValue(this Type type, string value, CultureInfo culture)
//        {
//            Enum ret = Enum.GetValues(type)
//                .Cast<Enum>()
//                .FirstOrDefault(v => v.ToString()
//                    .GetNameVariants(culture)
//                    .Contains(value, StringComparer.Create(culture, true)));
//            if (ret != null) return ret;
//            object enumValueAsUnderlyingType = Convert.ChangeType(value, Enum.GetUnderlyingType(type), culture);
//            if (enumValueAsUnderlyingType != null && Enum.IsDefined(type, enumValueAsUnderlyingType))
//            {
//                ret = (Enum)Enum.ToObject(type, enumValueAsUnderlyingType);
//            }
//            return ret;
//        }
//    }

//    public class VRXmlDeserializer //: IDeserializer
//    {
//        public VRXmlDeserializer()
//        {
//            Culture = CultureInfo.InvariantCulture;
//        }
//        public CultureInfo Culture { get; set; }
//        public string RootElement { get; set; }
//        public string Namespace { get; set; }
//        public string DateFormat { get; set; }
//        //public virtual T Deserialize<T>(IRestResponse response)
//        //{
//        //    return Deserialize<T>(response.Content);
//        //}

//        public T Deserialize<T>(string content)
//        {
//            if (string.IsNullOrEmpty(content))
//                return default(T);
//            var doc = XDocument.Parse(content);
//            var root = doc.Root;
//            if (RootElement.HasValue() && doc.Root != null)
//                root = doc.Root.DescendantsAndSelf(RootElement.AsNamespaced(Namespace)).SingleOrDefault();
//            // autodetect xml namespace
//            if (!Namespace.HasValue())
//                RemoveNamespace(doc);
//            var x = Activator.CreateInstance<T>();
//            var objType = x.GetType();
//            if (objType.IsSubclassOfRawGeneric(typeof(List<>)))
//                x = (T)HandleListDerivative(root, objType.Name, objType);
//            else
//                x = (T)Map(x, root);
//            return x;
//        }

//        private static void RemoveNamespace(XDocument xdoc)
//        {
//            if (xdoc.Root == null) return;
            
//            foreach (var e in xdoc.Root.DescendantsAndSelf())
//            {
//                if (e.Name.Namespace != XNamespace.None)
//                    e.Name = XNamespace.None.GetName(e.Name.LocalName);
//                if (e.Attributes()
//                    .Any(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
//                    e.ReplaceAttributes(
//                        e.Attributes()
//                            .Select(a => a.IsNamespaceDeclaration
//                                ? null
//                                : a.Name.Namespace != XNamespace.None
//                                    ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value)
//                                    : a));
//            }
//        }
// //Complex Method  (complexity = 42)
//        protected virtual object Map(object x, XElement root)
//        {
//            Type objType = x.GetType();
//            PropertyInfo[] props = objType.GetProperties();
//            bool deserializeFromContentAttributeAlreadyUsed = false;
//            foreach (var prop in props)
//            {
//                var type = prop.PropertyType.GetTypeInfo();
//                var typeIsPublic = type.IsPublic || type.IsNestedPublic;
//                if (!typeIsPublic || !prop.CanWrite)
//                    continue;
//                bool deserializeFromContent = false;
//                bool isNameDefinedInAttribute = false;
//                XName name = null;
//                var attributes = prop.GetCustomAttributes(typeof(VRXmlDeserializeAsAttribute), false);
//                if (attributes.Any())
//                {
//                    VRXmlDeserializeAsAttribute attribute = (VRXmlDeserializeAsAttribute)attributes.First();
//                    name = attribute.Name.AsNamespaced(Namespace);
//                    isNameDefinedInAttribute = name != null && !string.IsNullOrEmpty(name.LocalName);
//                    deserializeFromContent = attribute.Content;
//                    if(deserializeFromContentAttributeAlreadyUsed && deserializeFromContent)
//                    {
//                        throw new ArgumentException("Class cannot have two properties marked with " +
//                            "SerializeAs(Content = true) attribute.");
//                    }
//                    deserializeFromContentAttributeAlreadyUsed |= deserializeFromContent;
//                }
//                if (name == null)
//                {
//                    name = prop.Name.AsNamespaced(Namespace);
//                }
//                var value = GetValueFromXml(root, name, prop, isNameDefinedInAttribute);
//                if (value == null)
//                {
//                    // special case for text content node
//                    if (deserializeFromContent)
//                    {
//                        var textNode = root.Nodes().FirstOrDefault(n => n is XText);
//                        if(textNode != null)
//                        {
//                            value = ((XText)textNode).Value;
//                            prop.SetValue(x, value, null);
//                        }
//                        continue;
//                    }
//                    // special case for inline list items
//                    if (type.IsGenericType)
//                    {
//                        var genericType = type.GetGenericArguments()[0];
//                        var first = GetElementByName(root, genericType.Name);
//                        var list = (IList) Activator.CreateInstance(type.AsType());
//                        if (first != null && root != null)
//                        {
//                            var elements = root.Elements(first.Name);
//                            PopulateListFromElements(genericType, elements, list);
//                        }
//                        prop.SetValue(x, list, null);
//                    }
//                    continue;
//                }
//                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
//                {
//                    // if the value is empty, set the property to null...
//                    if (string.IsNullOrEmpty(value.ToString()))
//                    {
//                        prop.SetValue(x, null, null);
//                        continue;
//                    }
//                    type = type.GetGenericArguments()[0].GetTypeInfo();
//                }
//                var asType = type.AsType();
//                if (asType == typeof(bool))
//                {
//                    var toConvert = value.ToString()
//                        .ToLower();
//                    prop.SetValue(x, XmlConvert.ToBoolean(toConvert), null);
//                }
//                else if (type.IsPrimitive)
//                {
//                    prop.SetValue(x, value.ChangeType(asType, Culture), null);
//                }
//                else if (type.IsEnum)
//                {
//                    var converted = type.AsType().FindEnumValue(value.ToString(), Culture);
//                    prop.SetValue(x, converted, null);
//                }
//                else if (asType == typeof(Uri))
//                {
//                    var uri = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
//                    prop.SetValue(x, uri, null);
//                }
//                else if (asType == typeof(string))
//                {
//                    prop.SetValue(x, value, null);
//                }
//                else if (asType == typeof(DateTime))
//                {
//                    value = DateFormat.HasValue()
//                        ? DateTime.ParseExact(value.ToString(), DateFormat, Culture)
//                        : DateTime.Parse(value.ToString(), Culture);
//                    prop.SetValue(x, value, null);
//                }
//                else if (asType == typeof(DateTimeOffset))
//                {
//                    var toConvert = value.ToString();
//                    if (string.IsNullOrEmpty(toConvert)) continue;
//                    DateTimeOffset deserialisedValue;
//                    try
//                    {
//                        deserialisedValue = XmlConvert.ToDateTimeOffset(toConvert);
//                        prop.SetValue(x, deserialisedValue, null);
//                    }
//                    catch (Exception)
//                    {
//                        object result;
//                        if (TryGetFromString(toConvert, out result, asType))
//                        {
//                            prop.SetValue(x, result, null);
//                        }
//                        else
//                        {
//                            //fallback to parse
//                            deserialisedValue = DateTimeOffset.Parse(toConvert);
//                            prop.SetValue(x, deserialisedValue, null);
//                        }
//                    }
//                }
//                else if (asType == typeof(decimal))
//                {
//                    value = decimal.Parse(value.ToString(), Culture);
//                    prop.SetValue(x, value, null);
//                }
//                else if (asType == typeof(Guid))
//                {
//                    var raw = value.ToString();
//                    value = string.IsNullOrEmpty(raw)
//                        ? Guid.Empty
//                        : new Guid(value.ToString());
//                    prop.SetValue(x, value, null);
//                }
//                else if (asType == typeof(TimeSpan))
//                {
//                    var timeSpan = XmlConvert.ToTimeSpan(value.ToString());
//                    prop.SetValue(x, timeSpan, null);
//                }
//                else if (type.IsGenericType)
//                {
//                    var t = type.GetGenericArguments()[0];
//                    var list = (IList) Activator.CreateInstance(asType);
//                    var container = this.GetElementByName(root, name);
//                    if (container.HasElements)
//                    {
//                        var first = container.Elements().FirstOrDefault();
//                        if (first != null)
//                        {
//                            var elements = container.Elements(first.Name);
//                            PopulateListFromElements(t, elements, list);
//                        }
//                    }
//                    prop.SetValue(x, list, null);
//                }
//                else if (asType.IsSubclassOfRawGeneric(typeof(List<>)))
//                {
//                    // handles classes that derive from List<T>
//                    // e.g. a collection that also has attributes
//                    var list = HandleListDerivative(root, prop.Name, asType);
//                    prop.SetValue(x, list, null);
//                }
//                else
//                {
//                    object result;
//                    //fallback to type converters if possible
//                    if (TryGetFromString(value.ToString(), out result, asType))
//                    {
//                        prop.SetValue(x, result, null);
//                    }
//                    else
//                    {
//                        // nested property classes
//                        if (root == null) continue;
//                        var element = GetElementByName(root, name);
//                        if (element == null) continue;
//                        var item = CreateAndMap(asType, element);
//                        prop.SetValue(x, item, null);
//                    }
//                }
//            }
//            return x;
//        }
//        private static bool TryGetFromString(string inputString, out object result, Type type)
//        {
//            var converter = TypeDescriptor.GetConverter(type);
//            if (converter.CanConvertFrom(typeof(string)))
//            {
//                result = converter.ConvertFromInvariantString(inputString);
//                return true;
//            }
//            result = null;
//            return false;
//        }
//        private void PopulateListFromElements(Type t, IEnumerable<XElement> elements, IList list)
//        {
//            foreach (var item in elements.Select(element => CreateAndMap(t, element)))
//                list.Add(item);
//        }
//        private object HandleListDerivative(XElement root, string propName, Type type)
//        {
//            var t = type.IsGenericType
//                ? type.GetGenericArguments()[0]
//                : type.BaseType.GetGenericArguments()[0];
//            var list = (IList) Activator.CreateInstance(type);
//            IList<XElement> elements = root.Descendants(t.Name.AsNamespaced(Namespace))
//                .ToList();
//            var name = t.Name;
//            var attribute = t.GetAttribute<VRXmlDeserializeAsAttribute>();
//            if (attribute != null)
//                name = attribute.Name;
//            if (!elements.Any())
//            {
//                var lowerName = name.ToLower().AsNamespaced(Namespace);
//                elements = root.Descendants(lowerName).ToList();
//            }
//            if (!elements.Any())
//            {
//                var camelName = name.ToCamelCase(Culture).AsNamespaced(Namespace);
//                elements = root.Descendants(camelName).ToList();
//            }
//            if (!elements.Any())
//                elements = root.Descendants()
//                    .Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == name)
//                    .ToList();
//            if (!elements.Any())
//            {
//                var lowerName = name.ToLower().AsNamespaced(Namespace);
//                elements = root.Descendants()
//                    .Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == lowerName)
//                    .ToList();
//            }
//            PopulateListFromElements(t, elements, list);
//            // get properties too, not just list items
//            // only if this isn't a generic type
//            if (!type.IsGenericType)
//                Map(list, root.Element(propName.AsNamespaced(Namespace)) ?? root);
//            return list;
//        }
//        protected virtual object CreateAndMap(Type t, XElement element)
//        {
//            object item;
//            if (t == typeof(string))
//            {
//                item = element.Value;
//            }
//            else if (t.GetTypeInfo().IsPrimitive)
//            {
//                item = element.Value.ChangeType(t, Culture);
//            }
//            else
//            {
//                item = Activator.CreateInstance(t);
//                Map(item, element);
//            }
//            return item;
//        }
//        protected virtual object GetValueFromXml(XElement root, XName name, PropertyInfo prop, bool useExactName)
//        {
//            object val = null;
//            if (root == null) return val;
//            var element = GetElementByName(root, name);
//            if (element == null)
//            {
//                var attribute = GetAttributeByName(root, name, useExactName);
//                if (attribute != null)
//                    val = attribute.Value;
//            }
//            else
//            {
//                if (!element.IsEmpty || element.HasElements || element.HasAttributes)
//                    val = element.Value;
//            }
//            return val;
//        }
//        protected virtual XElement GetElementByName(XElement root, XName name)
//        {
//            var lowerName = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
//            var camelName = name.LocalName.ToCamelCase(Culture).AsNamespaced(name.NamespaceName);
//            if (root.Element(name) != null)
//                return root.Element(name);
//            if (root.Element(lowerName) != null)
//                return root.Element(lowerName);
//            if (root.Element(camelName) != null)
//                return root.Element(camelName);
//            // try looking for element that matches sanitized property name (Order by depth)
//            var element = root.Descendants()
//                       .OrderBy(d => d.Ancestors().Count())
//                       .FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName) ??
//                   root.Descendants()
//                       .OrderBy(d => d.Ancestors().Count())
//                       .FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName.ToLower());
//            return element == null && name == "Value".AsNamespaced(name.NamespaceName) &&
//                   (!root.HasAttributes || root.Attributes().All(x => x.Name != name))
//                ? root
//                : element;
//        }
//        protected virtual XAttribute GetAttributeByName(XElement root, XName name, bool useExactName)
//        {
//            var names = useExactName
//                ? null
//                : new List<XName>
//                {
//                    name.LocalName,
//                    name.LocalName.ToLower()
//                        .AsNamespaced(name.NamespaceName),
//                    name.LocalName.ToCamelCase(Culture)
//                        .AsNamespaced(name.NamespaceName)
//                };
//            return root.DescendantsAndSelf()
//                .OrderBy(d => d.Ancestors().Count())
//                .Attributes()
//                .FirstOrDefault(
//                    d => useExactName
//                        ? d.Name == name
//                        : names.Contains(d.Name.LocalName.RemoveUnderscoresAndDashes()));
//        }
//    }
//}