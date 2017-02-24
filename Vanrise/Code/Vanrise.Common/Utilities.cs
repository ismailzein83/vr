using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public static class Utilities
    {
        public static Dictionary<T, Q> GetEnumAttributes<T, Q>()
            where T : struct
            where Q : Attribute
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception(String.Format("{0} is not an Enum type", enumType));

            Type attributeType = typeof(Q);
            Dictionary<T, Q> enumAttributes = new Dictionary<T, Q>();
            foreach (var member in enumType.GetFields())
            {
                Q mbrAttribute = member.GetCustomAttributes(attributeType, true).FirstOrDefault() as Q;
                if (mbrAttribute != null)
                    enumAttributes.Add((T)Enum.Parse(enumType, member.Name), mbrAttribute);
            }
            return enumAttributes;
        }

        public static Q GetEnumAttribute<T, Q>(T enumItem)
            where T : struct
            where Q : Attribute
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception(String.Format("{0} is not an Enum type", enumType));

            Type attributeType = typeof(Q);
            Dictionary<T, Q> enumAttributes = new Dictionary<T, Q>();
            foreach (var member in enumType.GetFields())
            {
                T memberAsEnum;
                if (Enum.TryParse<T>(member.Name, true, out memberAsEnum) && memberAsEnum.Equals(enumItem))
                {
                    return member.GetCustomAttributes(attributeType, true).FirstOrDefault() as Q;
                }

            }
            return default(Q);
        }

        public static string GetEnumDescription<T>(T enumItem) where T : struct
        {
            System.ComponentModel.DescriptionAttribute descriptionAttribute = GetEnumAttribute<T, System.ComponentModel.DescriptionAttribute>(enumItem);
            return descriptionAttribute != null ? descriptionAttribute.Description : enumItem.ToString();
        }

        public static IEnumerable<Type> GetAllImplementations(Type baseType)
        {
            List<Type> lst = new List<Type>();
            List<string> loadedAssemblies = new List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (loadedAssemblies.Contains(assembly.FullName))
                    continue;
                loadedAssemblies.Add(assembly.FullName);
                foreach (Type t in assembly.GetLoadableTypes())
                {
                    if (baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    {
                        lst.Add(t);
                    }
                }
            }
            return lst;
        }

        public static IEnumerable<Type> GetAllImplementations<T>()
        {
            return GetAllImplementations(typeof(T));
        }

        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return date1 > date2 ? date1 : date2;
        }

        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return date1 < date2 ? date1 : date2;
        }

        public static void ActivateAspose()
        {

            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
        }

        public static bool IsNumeric(string value)
        {
            return IsNumeric(value, null);
        }

        public static bool IsNumeric(string value, long? minValue)
        {
            long temp;
            if (!long.TryParse(value, out temp))
                return false;

            if (minValue != null && temp < minValue)
                return false;

            return true;
        }

        static ConcurrentDictionary<string, IPropValueReader> s_cachedProbValueReaders = new ConcurrentDictionary<string, IPropValueReader>();

        public static IPropValueReader GetPropValueReader(string propertyPath)
        {
            IPropValueReader propValueReader;
            string key = propertyPath;
            if (!s_cachedProbValueReaders.TryGetValue(key, out propValueReader))
            {
                StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Common.IPropValueReader
                    {   
                        public Object GetPropertyValue(dynamic target)
                        {
                            return target.#PROPERTYPATH#;
                        }
                    }
                }
                ");

                classDefinitionBuilder.Replace("#PROPERTYPATH#", propertyPath);

                string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Common");
                string className = "PropValueReader";
                classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
                classDefinitionBuilder.Replace("#CLASSNAME#", className);
                string fullTypeName = String.Format("{0}.{1}", classNamespace, className);
                CSharpCompilationOutput compilationOutput;
                if (!CSharpCompiler.TryCompileClass(classDefinitionBuilder.ToString(), out compilationOutput))
                {
                    StringBuilder errorsBuilder = new StringBuilder();
                    if (compilationOutput.ErrorMessages != null)
                    {
                        foreach (var errorMessage in compilationOutput.ErrorMessages)
                        {
                            errorsBuilder.AppendLine(errorMessage);
                        }
                    }
                    throw new Exception(String.Format("Compile Error when building executor type for PropValueReader. Errors: {0}",
                        errorsBuilder));
                }
                var runtimeType = compilationOutput.OutputAssembly.GetType(fullTypeName);
                if (runtimeType == null)
                    throw new NullReferenceException("runtimeType");
                propValueReader = Activator.CreateInstance(runtimeType) as IPropValueReader;
                s_cachedProbValueReaders.TryAdd(key, propValueReader);
            }
            return propValueReader;
        }

        public static void CompilePredefinedPropValueReaders()
        {
            var compilationStepTypes = GetAllImplementations<IPropValueReaderCompilationStep>();
            HashSet<string> propNames = new HashSet<string>();
            foreach (var stepType in compilationStepTypes)
            {
                foreach (var propName in (Activator.CreateInstance(stepType) as IPropValueReaderCompilationStep).GetPropertiesToCompile(null))
                {
                    propNames.Add(propName);
                }
            }
            AddPropValueReaders(propNames);
        }

        static void AddPropValueReaders(HashSet<string> propNames)
        {
            List<string> propNamesToInclude = propNames.Where(itm => !s_cachedProbValueReaders.ContainsKey(itm)).ToList();
            if (propNamesToInclude.Count > 0)
            {
                string classDefinitionTemplate = @"public class #CLASSNAME# : Vanrise.Common.IPropValueReader
                    {   
                        public Object GetPropertyValue(dynamic target)
                        {
                            return target.#PROPNAME#;
                        }
                    }";

                Dictionary<string, string> classNamesByProperties = new Dictionary<string, string>();
                StringBuilder classDefinitionsBuilder = new StringBuilder();

                foreach (var propName in propNamesToInclude)
                {
                    string className = string.Format("PropValueReader_{0}", propName.Replace(".", "_").Replace("[", "_").Replace("]", "_"));

                    classNamesByProperties.Add(propName, className);
                    StringBuilder builder = new StringBuilder(classDefinitionTemplate);
                    builder.Replace("#CLASSNAME#", className);
                    builder.Replace("#PROPNAME#", propName);
                    classDefinitionsBuilder.AppendLine();
                    classDefinitionsBuilder.AppendLine(builder.ToString());
                }

                StringBuilder fullCodeBuilder = new StringBuilder(@" 
                using System;

                namespace #NAMESPACE#
                {
                    #CLASSES#
                }
                ");

                string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Common");
                fullCodeBuilder.Replace("#NAMESPACE#", classNamespace);
                fullCodeBuilder.Replace("#CLASSES#", classDefinitionsBuilder.ToString());
                CSharpCompilationOutput compilationOutput;
                if (!CSharpCompiler.TryCompileClass(fullCodeBuilder.ToString(), out compilationOutput))
                {
                    StringBuilder errorsBuilder = new StringBuilder();
                    if (compilationOutput.ErrorMessages != null)
                    {
                        foreach (var errorMessage in compilationOutput.ErrorMessages)
                        {
                            errorsBuilder.AppendLine(errorMessage);
                        }
                    }
                    throw new Exception(String.Format("Compile Error when building executor type for PropValueReader. Errors: {0}",
                        errorsBuilder));
                }
                foreach (var propEntry in classNamesByProperties)
                {
                    var runtimeType = compilationOutput.OutputAssembly.GetType(String.Format("{0}.{1}", classNamespace, propEntry.Value));
                    if (runtimeType == null)
                        throw new NullReferenceException(String.Format("runtimeType '{0}'", propEntry.Key));
                    var propValueReader = Activator.CreateInstance(runtimeType) as IPropValueReader;
                    s_cachedProbValueReaders.TryAdd(propEntry.Key, propValueReader);
                }
            }
        }
        public static string GetExposedConnectionString(string connectionStringName)
        {
            if (!IsConnectionStringExposed(connectionStringName))
                throw new Exception(String.Format("Connection String '{0}' is not exposed", connectionStringName));
            var connStringEntry = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connStringEntry == null)
                throw new NullReferenceException(String.Format("connStringEntry. connectionStringName '{0}'", connectionStringName));
            return connStringEntry.ConnectionString;
        }

        static HashSet<string> s_exposedConnectionStringNames;
        static Object s_lockObj = new object();

        private static bool IsConnectionStringExposed(string connectionStringName)
        {
            if (s_exposedConnectionStringNames == null)
            {
                lock (s_lockObj)
                {
                    if (s_exposedConnectionStringNames == null)
                    {
                        var exposedConnectionStringNames = ConfigurationManager.AppSettings["ExposedConnectionStringNames"];
                        if (exposedConnectionStringNames != null)
                            s_exposedConnectionStringNames = new HashSet<string>(exposedConnectionStringNames.Split(',').Select(itm => itm.Trim()));
                        else
                            s_exposedConnectionStringNames = new HashSet<string>();
                    }
                }
            }
            return s_exposedConnectionStringNames.Contains(connectionStringName);
        }


        public static string GetDateTimeFormat(Vanrise.Entities.DateTimeType dateTimeType)
        {
            switch (dateTimeType)
            {
                case Vanrise.Entities.DateTimeType.LongDateTime: return "yyyy-MM-dd HH:mm:ss";
                case Vanrise.Entities.DateTimeType.DateTime: return "yyyy-MM-dd HH:mm";
                case Vanrise.Entities.DateTimeType.Date: return "yyyy-MM-dd";
                default: throw new NotSupportedException(String.Format("dateTimeType '{0}'", dateTimeType));
            }
        }

        public static List<T> GetDictionaryKeys<T, Q>(Dictionary<T, Q> dict)
        {
            if (dict == null || dict.Count == 0)
                return null;
            return dict.Keys.ToList();
        }

        public static T DeserializeAndValidate<T>(string serialized)
        {
            if (serialized == null)
                throw new ArgumentNullException("serialized");
            Object deserializedAsObject = Serializer.Deserialize(serialized);
            if (!(deserializedAsObject is T))
                throw new Exception(String.Format("handler is not of type {0}. it is of type '{1}'", typeof(T).FullName, deserializedAsObject.ToString()));
            return (T)deserializedAsObject;
        }

        public static bool IsTextMatched(string target, string match, TextFilterType textFilterType)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (match == null)
                throw new ArgumentNullException("match");

            target = target.ToLower();
            match = match.ToLower();

            switch (textFilterType)
            {
                case TextFilterType.StartsWith:
                    return target.StartsWith(match);
                case TextFilterType.Contains:
                    return target.Contains(match);
            }
            throw new ArgumentException("textFilterType");
        }
        public static object GetTypeDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public static Object GetPropValue(string fieldName, dynamic obj)
        {
            Resolver resolver = new Resolver();
            return resolver.Resolve(obj, fieldName);
            //if (fieldName == null)
            //    return null;
            //string[] arr = fieldName.Split('.');
            //PropertyInfo propertyInfo;
            //dynamic value = obj;
            //for (var i = 0; i < arr.Length; i++)
            //{
            //    string field = arr[i];
            //    if (value != null)
            //    {
            //        propertyInfo = value.GetType().GetProperty(field);
            //        value = propertyInfo.GetValue(value, null);
            //        if (i == arr.Length - 1)
            //        {
            //            return value;
            //        }
            //    }

            //}
            //return null;
        }

        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            Regex regularExpression = new Regex(validEmailPattern, RegexOptions.IgnoreCase);
            return regularExpression.IsMatch(email);
        }

        public static T CloneObject<T>(T obj) where T : class
        {
            if (obj == null)
                return null;

            return Vanrise.Common.Serializer.Deserialize<T>(Vanrise.Common.Serializer.Serialize(obj));
        }

        public static IEnumerable<R> MergeUnionWithQForce<T, Q, R>(List<T> Tlist, List<Q> Qlist, Action<T, R> mapTtoR, Action<Q, R> mapQtoR)
            where T : class, IDateEffectiveSettingsEditable
            where Q : class, IDateEffectiveSettingsEditable
            where R : class, IDateEffectiveSettingsEditable
        {
            int Tindex = 0;
            int Qindex = 0;

            R lastRecord = default(R);
            List<R> totalRecords = new List<R>();

            int TlistCount = Tlist.Count;
            int QlistCount = Qlist.Count;

            while (Tindex < TlistCount || Qindex < QlistCount)
            {
                Q Qitem = Qindex < QlistCount ? Qlist[Qindex] : null;
                T Titem = Tindex < TlistCount ? Tlist[Tindex] : null;

                if (Qitem != null && (Titem == null || Qitem.BED <= Titem.BED || (lastRecord != null && lastRecord.EED.HasValue && lastRecord.EED.Value == Qitem.BED)))
                {
                    lastRecord = Activator.CreateInstance<R>();
                    lastRecord.BED = Qitem.BED;
                    lastRecord.EED = Qitem.EED;

                    mapQtoR(Qitem, lastRecord);
                    totalRecords.Add(lastRecord);

                    Qindex++;
                }
                else
                {
                    lastRecord = Activator.CreateInstance<R>();
                    lastRecord.BED = lastRecord == null || !lastRecord.EED.HasValue ? Titem.BED : Utilities.Max(lastRecord.EED.Value, Titem.BED);
                    lastRecord.EED = Qitem == null ? Titem.EED : Titem.EED.MinDate(Qitem.BED);

                    mapTtoR(Titem, lastRecord);
                    totalRecords.Add(lastRecord);
                }

                while (Titem != null && Tindex < TlistCount && Titem.EED.VRLessThanOrEqual(lastRecord.EED))
                {
                    Tindex++;
                    Titem = Tindex < TlistCount ? Tlist[Tindex] : null;
                }
            }

            return totalRecords;
        }

        public static IEnumerable<R> GetQIntersectT<T, Q, R>(List<T> Tlist, List<Q> Qlist, Action<Q, R> mapQtoR)
            where T : class, IDateEffectiveSettingsEditable
            where Q : class, IDateEffectiveSettingsEditable
            where R : class, IDateEffectiveSettingsEditable
        {
            int Tindex = 0;
            int Qindex = 0;

            R lastRecord = default(R);
            List<R> totalRecords = new List<R>();

            int TlistCount = Tlist.Count;
            int QlistCount = Qlist.Count;

            while (Tindex < TlistCount && Qindex < QlistCount)
            {
                Q Qitem = Qindex < QlistCount ? Qlist[Qindex] : null;
                T Titem = Tindex < TlistCount ? Tlist[Tindex] : null;

                if (Qitem.EED.VRGreaterThan(Titem.BED) && Titem.EED.VRGreaterThan(Qitem.BED))
                {
                    lastRecord = Activator.CreateInstance<R>();
                    lastRecord.BED = Utilities.Max(Titem.BED, Qitem.BED);
                    lastRecord.EED = Titem.EED.MinDate(Qitem.EED);

                    mapQtoR(Qitem, lastRecord);
                    totalRecords.Add(lastRecord);

                    if (Qitem.EED.VRLessThanOrEqual(lastRecord.EED))
                        Qindex++;

                    if (Titem.EED.VRLessThanOrEqual(lastRecord.EED))
                        Tindex++;
                }

                while (Qindex < QlistCount && Qitem.EED.VRLessThanOrEqual(Titem.BED))
                {
                    Qindex++;
                    Qitem = Qindex < QlistCount ? Qlist[Qindex] : null;
                }

                while (Tindex < TlistCount && Titem.EED.VRLessThanOrEqual(Qitem.BED))
                {
                    Tindex++;
                    Titem = Tindex < TlistCount ? Tlist[Tindex] : null;
                }
            }

            return totalRecords;
        }

        public static string ReplaceString(string inputString, string stringToReplace, string newStringValue, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(inputString))
                return null;

            if (string.IsNullOrEmpty(stringToReplace))
                return inputString;

            int startIndex = 0;
            while (true)
            {
                startIndex = inputString.IndexOf(stringToReplace, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                inputString = inputString.Substring(0, startIndex) + newStringValue + inputString.Substring(startIndex + stringToReplace.Length);

                startIndex += newStringValue.Length;
            }

            return inputString;
        }
    }

    public interface IPropValueReader
    {
        Object GetPropertyValue(dynamic target);
    }
    public interface IPropValueReaderCompilationStep
    {
        HashSet<string> GetPropertiesToCompile(IPropValueReaderCompilationStepContext context);
    }

    public interface IPropValueReaderCompilationStepContext
    {

    }

    public class CommonPropPropValueReaderCompilationStep : IPropValueReaderCompilationStep
    {
        public HashSet<string> GetPropertiesToCompile(IPropValueReaderCompilationStepContext context)
        {
            return new HashSet<string> { "ID", "Entity.ID", "Name", "Entity.Name", "Description", "Entity.Description", "CreatedTime", "Entity.CreatedTime" };
        }
    }

}
