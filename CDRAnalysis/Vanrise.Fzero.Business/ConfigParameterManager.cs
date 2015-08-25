using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.Entities;
using Vanrise.Common;

namespace Vanrise.Fzero.Business
{
    public class ConfigParameterManager
    {
        #region Singleton

        static ConfigParameterManager _current = new ConfigParameterManager();

        public static ConfigParameterManager Current
        {
            get
            {
                return _current;
            }
        }

        private ConfigParameterManager()
        {

        }

        #endregion

        #region Public Methods

        public int GetOperatorType()
        {
            return GetIntValue(ConfigParameterName.OperatorType);
        }

        #endregion

        #region Private

        delegate bool TryParseDelegate<T>(string value, out T result);


        private int GetIntValue(ConfigParameterName prmName)
        {
            return GetTypedValue<int>(prmName, int.TryParse);
        }

        private bool GetBoolValue(ConfigParameterName prmName)
        {
            return GetTypedValue<bool>(prmName, bool.TryParse);
        }

        private Decimal GetDecimalValue(ConfigParameterName prmName)
        {
            return GetTypedValue<Decimal>(prmName, Decimal.TryParse);
        }

        private DateTime GetDateTimeValue(ConfigParameterName prmName)
        {
            return GetTypedValue<DateTime>(prmName, DateTime.TryParse);
        }

        private TimeSpan GetTimeSpanValue(ConfigParameterName prmName)
        {
            return GetTypedValue<TimeSpan>(prmName, TimeSpan.TryParse);
        }

        private T GetTypedValue<T>(ConfigParameterName prmName, TryParseDelegate<T> tryParse)
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

        string GetParameterValueFromConfig(ConfigParameterName prmName)
        {
            return ConfigurationManager.AppSettings[prmName.ToString()];
        }

        #endregion
    }
}
