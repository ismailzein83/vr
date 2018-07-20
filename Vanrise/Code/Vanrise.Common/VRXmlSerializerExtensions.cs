using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Vanrise.Common.VRXmlSerializerExtensions
{
    public static class RestSharpXmlExtensions
    {
        /// <summary>
        /// Returns the name of an element with the namespace if specified
        /// </summary>
        /// <param name="name">Element name</param>
        /// <param name="namespace">XML Namespace</param>
        /// <returns></returns>
        public static XName AsNamespaced(this string name, string @namespace)
        {
            XName xName = name;
            if (@namespace.HasValue())
            {
                xName = XName.Get(name, @namespace);
            }
            return xName;
        }
    }

    public static class StringExtensions
    {
        public static string UrlDecode(this string input)
        {
            return System.Web.HttpUtility.UrlDecode(input);
        }
        /// <summary>
        ///     Uses Uri.EscapeDataString() based on recommendations on MSDN
        ///     http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
        /// </summary>
        public static string UrlEncode(this string input)
        {
            const int maxLength = 32766;
            if (input == null)
                throw new ArgumentNullException("input");
            if (input.Length <= maxLength)
                return Uri.EscapeDataString(input);
            var sb = new StringBuilder(input.Length * 2);
            var index = 0;
            while (index < input.Length)
            {
                var length = Math.Min(input.Length - index, maxLength);
                var subString = input.Substring(index, length);
                sb.Append(Uri.EscapeDataString(subString));
                index += subString.Length;
            }
            return sb.ToString();
        }
        public static string HtmlDecode(this string input)
        {
            return HttpUtility.HtmlDecode(input);
        }
        public static string HtmlEncode(this string input)
        {
            return HttpUtility.HtmlEncode(input);
        }
        public static string UrlEncode(this string input, Encoding encoding)
        {
            return HttpUtility.UrlEncode(input, encoding);
        }
        public static string HtmlAttributeEncode(this string input)
        {
            return HttpUtility.HtmlAttributeEncode(input);
        }
        /// <summary>
        ///     Check that a string is not null or empty
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns>bool</returns>
        public static bool HasValue(this string input)
        {
            return !string.IsNullOrEmpty(input);
        }
        /// <summary>
        ///     Remove underscores from a string
        /// </summary>
        /// <param name="input">String to process</param>
        /// <returns>string</returns>
        public static string RemoveUnderscoresAndDashes(this string input)
        {
            return input.Replace("_", "").Replace("-", "");
        }

        private static readonly Regex DateRegex = new Regex(@"\\?/Date\((-?\d+)(-|\+)?([0-9]{4})?\)\\?/");
        private static readonly Regex NewDateRegex = new Regex(@"newDate\((-?\d+)*\)");
        /// <summary>
        ///     Parses most common JSON date formats
        /// </summary>
        /// <param name="input">JSON value to parse</param>
        /// <param name="culture"></param>
        /// <returns>DateTime</returns>
        public static DateTime ParseJsonDate(this string input, CultureInfo culture)
        {
            const long maxAllowedTimestamp = 253402300799;
            input = input.Replace("\n", "");
            input = input.Replace("\r", "");
            input = input.RemoveSurroundingQuotes();
            long unix;
            if (long.TryParse(input, out unix))
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return unix > maxAllowedTimestamp ? epoch.AddMilliseconds(unix) : epoch.AddSeconds(unix);
            }
            if (input.Contains("/Date("))
                return ExtractDate(input, DateRegex, culture);
            if (input.Contains("new Date("))
            {
                input = input.Replace(" ", "");
                // because all whitespace is removed, match against newDate( instead of new Date(
                return ExtractDate(input, NewDateRegex, culture);
            }
            return ParseFormattedDate(input, culture);
        }
        /// <summary>
        ///     Remove leading and trailing " from a string
        /// </summary>
        /// <param name="input">String to parse</param>
        /// <returns>String</returns>
        public static string RemoveSurroundingQuotes(this string input)
        {
            if (input.StartsWith("\"") && input.EndsWith("\""))
                input = input.Substring(1, input.Length - 2);
            return input;
        }
        private static DateTime ParseFormattedDate(string input, CultureInfo culture)
        {
            string[] formats =
            {
                "u",
                "s",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-dd HH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:sszzzzzz",
                "M/d/yyyy h:mm:ss tt" // default format for invariant culture
            };
            DateTime date;
            if (DateTime.TryParseExact(input, formats, culture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out date))
                return date;
            return DateTime.TryParse(input, culture, DateTimeStyles.None, out date) ? date : default(DateTime);
        }
        private static DateTime ExtractDate(string input, Regex regex, CultureInfo culture)
        {
            var dt = DateTime.MinValue;
            if (!regex.IsMatch(input)) return dt;
            var matches = regex.Matches(input);
            var match = matches[0];
            var ms = Convert.ToInt64(match.Groups[1].Value);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = epoch.AddMilliseconds(ms);
            // adjust if time zone modifier present
            if (match.Groups.Count <= 2 || string.IsNullOrEmpty(match.Groups[3].Value)) return dt;
            var mod = DateTime.ParseExact(match.Groups[3].Value, "HHmm", culture);
            dt = match.Groups[2].Value == "+"
                ? dt.Add(mod.TimeOfDay)
                : dt.Subtract(mod.TimeOfDay);
            return dt;
        }
        /// <summary>
        ///     Converts a string to pascal case
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">String to convert</param>
        /// <param name="culture"></param>
        /// <returns>string</returns>
        public static string ToPascalCase(this string lowercaseAndUnderscoredWord, CultureInfo culture)
        {
            return ToPascalCase(lowercaseAndUnderscoredWord, true, culture);
        }
        /// <summary>
        ///     Converts a string to pascal case with the option to remove underscores
        /// </summary>
        /// <param name="text">String to convert</param>
        /// <param name="removeUnderscores">Option to remove underscores</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string text, bool removeUnderscores, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            text = text.Replace("_", " ");
            var joinString = removeUnderscores
                ? string.Empty
                : "_";
            var words = text.Split(' ');
            if (words.Length <= 1 && !words[0].IsUpperCase())
                return string.Concat(words[0].Substring(0, 1).ToUpper(culture), words[0].Substring(1));
            for (var i = 0; i < words.Length; i++)
            {
                if (words[i].Length <= 0) continue;
                var word = words[i];
                var restOfWord = word.Substring(1);
                if (restOfWord.IsUpperCase())
                    restOfWord = restOfWord.ToLower(culture);
                var firstChar = char.ToUpper(word[0], culture);
                words[i] = string.Concat(firstChar, restOfWord);
            }
            return string.Join(joinString, words);
        }
        /// <summary>
        ///     Converts a string to camel case
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">String to convert</param>
        /// <param name="culture"></param>
        /// <returns>String</returns>
        public static string ToCamelCase(this string lowercaseAndUnderscoredWord, CultureInfo culture)
        {
            return MakeInitialLowerCase(ToPascalCase(lowercaseAndUnderscoredWord, culture));
        }
        /// <summary>
        ///     Convert the first letter of a string to lower case
        /// </summary>
        /// <param name="word">String to convert</param>
        /// <returns>string</returns>
        public static string MakeInitialLowerCase(this string word)
        {
            return string.Concat(word.Substring(0, 1).ToLower(), word.Substring(1));
        }
        private static readonly Regex IsUpperCaseRegex = new Regex(@"^[A-Z]+$");
        /// <summary>
        ///     Checks to see if a string is all uppper case
        /// </summary>
        /// <param name="inputString">String to check</param>
        /// <returns>bool</returns>
        public static bool IsUpperCase(this string inputString)
        {

            return IsUpperCaseRegex.IsMatch(inputString);
        }
        private static readonly Regex AddUnderscoresRegex1 = new Regex(@"[-\s]");
        private static readonly Regex AddUnderscoresRegex2 = new Regex(@"([a-z\d])([A-Z])");
        private static readonly Regex AddUnderscoresRegex3 = new Regex(@"([A-Z]+)([A-Z][a-z])");
        /// <summary>
        ///     Add underscores to a pascal-cased string
        /// </summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>string</returns>
        public static string AddUnderscores(this string pascalCasedWord)
        {
            return AddUnderscoresRegex1.Replace(
                AddUnderscoresRegex2.Replace(
                    AddUnderscoresRegex3.Replace(pascalCasedWord, "$1_$2"),
                    "$1_$2"),
                "_");
        }
        private static readonly Regex AddDashesRegex1 = new Regex(@"[\s]");
        private static readonly Regex AddDashesRegex2 = new Regex(@"([a-z\d])([A-Z])");
        private static readonly Regex AddDashesRegex3 = new Regex(@"([A-Z]+)([A-Z][a-z])");
        /// <summary>
        ///     Add dashes to a pascal-cased string
        /// </summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>string</returns>
        public static string AddDashes(this string pascalCasedWord)
        {
            return AddDashesRegex1.Replace(
                   AddDashesRegex2.Replace(
                       AddDashesRegex3.Replace(pascalCasedWord, "$1-$2"),
                       "$1-$2"),
                   "-");
        }
        /// <summary>
        ///     Add an undescore prefix to a pascasl-cased string
        /// </summary>
        /// <param name="pascalCasedWord"></param>
        /// <returns></returns>
        public static string AddUnderscorePrefix(this string pascalCasedWord)
        {
            return string.Format("_{0}", pascalCasedWord);
        }
        private static readonly Regex AddSpacesRegex1 = new Regex(@"[-\s]");
        private static readonly Regex AddSpacesRegex2 = new Regex(@"([a-z\d])([A-Z])");
        private static readonly Regex AddSpacesRegex3 = new Regex(@"([A-Z]+)([A-Z][a-z])");
        /// <summary>
        ///     Add spaces to a pascal-cased string
        /// </summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>string</returns>
        public static string AddSpaces(this string pascalCasedWord)
        {
            return AddSpacesRegex1.Replace(
                   AddSpacesRegex2.Replace(
                       AddSpacesRegex3.Replace(pascalCasedWord, "$1 $2"),
                       "$1 $2"),
                   " ");
        }

        /// <summary>
        ///     Return possible variants of a name for name matching.
        /// </summary>
        /// <param name="name">String to convert</param>
        /// <param name="culture">The culture to use for conversion</param>
        /// <returns>IEnumerable&lt;string&gt;</returns>
        public static IEnumerable<string> GetNameVariants(this string name, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(name))
                yield break;
            yield return name;
            // try camel cased name
            yield return name.ToCamelCase(culture);
            // try lower cased name
            yield return name.ToLower(culture);
            // try name with underscores
            yield return name.AddUnderscores();
            // try name with underscores with lower case
            yield return name.AddUnderscores().ToLower(culture);
            // try name with dashes
            yield return name.AddDashes();
            // try name with dashes with lower case
            yield return name.AddDashes().ToLower(culture);
            // try name with underscore prefix
            yield return name.AddUnderscorePrefix();
            // try name with underscore prefix, using camel case
            yield return name.ToCamelCase(culture).AddUnderscorePrefix();
            // try name with spaces
            yield return name.AddSpaces();
            // try name with spaces with lower case
            yield return name.AddSpaces().ToLower(culture);
        }
    }

    public static class ReflectionExtensions
    {
        /// <summary>
        /// Retrieve an attribute from a member (property)
        /// </summary>
        /// <typeparam name="T">Type of attribute to retrieve</typeparam>
        /// <param name="prop">Member to retrieve attribute from</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo prop) where T : Attribute
        {
            return Attribute.GetCustomAttribute(prop, typeof(T)) as T;
        }
        /// <summary>
        /// Retrieve an attribute from a type
        /// </summary>
        /// <typeparam name="T">Type of attribute to retrieve</typeparam>
        /// <param name="type">Type to retrieve attribute from</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return Attribute.GetCustomAttribute(type, typeof(T)) as T;
        }
        /// <summary>
        /// Checks a type to see if it derives from a raw generic (e.g. List[[]])
        /// </summary>
        /// <param name="toCheck"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                Type cur = toCheck.GetTypeInfo().IsGenericType
                    ? toCheck.GetGenericTypeDefinition()
                    : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.GetTypeInfo().BaseType;
            }
            return false;
        }
        public static object ChangeType(this object source, TypeInfo newType)
        {
            return Convert.ChangeType(source, newType.AsType());
        }
        public static object ChangeType(this object source, Type newType, CultureInfo culture)
        {
            return Convert.ChangeType(source, newType);
        }
        /// <summary>
        /// Find a value from a System.Enum by trying several possible variants
        /// of the string value of the enum.
        /// </summary>
        /// <param name="type">Type of enum</param>
        /// <param name="value">Value for which to search</param>
        /// <param name="culture">The culture used to calculate the name variants</param>
        /// <returns></returns>
        public static object FindEnumValue(this Type type, string value, CultureInfo culture)
        {
            Enum ret = Enum.GetValues(type)
                .Cast<Enum>()
                .FirstOrDefault(v => v.ToString()
                    .GetNameVariants(culture)
                    .Contains(value, StringComparer.Create(culture, true)));
            if (ret != null) return ret;
            object enumValueAsUnderlyingType = Convert.ChangeType(value, Enum.GetUnderlyingType(type), culture);
            if (enumValueAsUnderlyingType != null && Enum.IsDefined(type, enumValueAsUnderlyingType))
            {
                ret = (Enum)Enum.ToObject(type, enumValueAsUnderlyingType);
            }
            return ret;
        }
    }
}
