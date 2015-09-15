using System;
using System.Configuration;
using Vanrise.Common;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.Business
{
    public class ConfigParameterManager
    {
        #region Singleton

        static ConfigParameterManager _current = new ConfigParameterManager();

        private ConfigParameterManager()
        {

        }

        #endregion

        #region Public Methods

        public static OperatorTypeEnum GetOperatorType()
        {
            return (OperatorTypeEnum) ConfigParameterName.OperatorType;
        }

        #endregion

        #region Private

        delegate bool TryParseDelegate<T>(string value, out T result);


        private static int GetIntValue(ConfigParameterName prmName)
        {
            return GetTypedValue<int>(prmName, int.TryParse);
        }


        private static T GetTypedValue<T>(ConfigParameterName prmName, TryParseDelegate<T> tryParse)
        {
            string valAsString = GetParameterValueFromConfig(prmName);
            if (valAsString == null)
            {
                var attribute = Utilities.GetEnumAttribute<ConfigParameterName, ConfigParameterNameAttribute>(prmName);
                if (attribute == null)
                    throw new Exception(String.Format("Could not find ConfigParameterNameAttribute for parameter", prmName));
                valAsString = attribute.DefaultValue;
            }
            T typedValue;
            if (!tryParse(valAsString, out typedValue))
                throw new Exception(String.Format("Configuration Parameter value {0} for parameter name {1} is not parsable to {2}", valAsString, prmName, (typeof(T)).Name));
            return typedValue;
        }

        static string  GetParameterValueFromConfig(ConfigParameterName prmName)
        {
            return ConfigurationManager.AppSettings[prmName.ToString()];
        }

        #endregion
    }
}
