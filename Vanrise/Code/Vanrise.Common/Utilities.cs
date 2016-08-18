﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {

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
                            s_exposedConnectionStringNames = new HashSet<string>(exposedConnectionStringNames.Split(','));
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
    }

    public interface IPropValueReader
    {
        Object GetPropertyValue(dynamic target);
    }        
}
