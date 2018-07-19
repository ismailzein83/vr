using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class UtilityManager
    {
        public DateTimeRange GetDateTimeRange()
        {
            IUtilityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IUtilityDataManager>();
            return dataManager.GetDateTimeRange();
        }

        public bool CheckIfDefaultOrInvalid(DateTime? dateTime)
        {
            IUtilityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IUtilityDataManager>();
            return dataManager.CheckIfDefaultOrInvalid(dateTime);
        }

        public static void GenerateDocumentationForEnums(IEnumerable<Assembly> assemblies)
        {
            List<Enumeration> allEnumerations = new List<Enumeration>();
            foreach (var assembly in assemblies)
            {
                allEnumerations.AddRange(GetEnumerations(assembly));
            }
            if (!allEnumerations.Any())
                return;
            IEnumerationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IEnumerationDataManager>();
            dataManager.ClearEnumerations();
            dataManager.SaveEnumerationsToDb(allEnumerations);
        }

        public static List<Enumeration> GetEnumerations(Assembly assembly)
        {
            List<Enumeration> result = new List<Enumeration>();
            if (assembly == null)
                return result;
            
            var assemblyTypes = assembly.GetTypes();
            foreach (Type type in assemblyTypes)
            {
                if (type.IsEnum)
                {
                    if (type.Namespace == null)
                        continue;
                    Enumeration enumeration = new Enumeration();
                    enumeration.NameSpace = type.Namespace;
                    enumeration.Name = type.Name;

                    var enumerationValues = new List<string>(); ;
                    foreach (var enumValue in type.GetEnumValues())
                    {
                        int enumerationValueInteger = Convert.ToInt32(enumValue);
                        string enumerationValueName = enumValue.ToString();
                        enumerationValues.Add(string.Format("{0}:{1}", enumerationValueName, enumerationValueInteger));
                    }
                    enumeration.Description = string.Join(", ", enumerationValues);
                    result.Add(enumeration);
                }
            }
            return result;
        }
    }
}