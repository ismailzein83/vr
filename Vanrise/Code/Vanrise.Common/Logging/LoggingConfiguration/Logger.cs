using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.LoggingConfiguration
{
    public class Logger : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("logLevel", IsRequired = false)]
        public LogEntryType? LogLevel
        {
            get
            {
                return (LogEntryType?)this["logLevel"];
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return this["type"] as string;
            }
        }

        [ConfigurationProperty("parameters", IsRequired = false)]
        [ConfigurationCollection(typeof(ConfigurationParameterCollection), AddItemName = "parameter")]
        public ConfigurationParameterCollection Parameters
        {
            get
            {
                return this["parameters"] as ConfigurationParameterCollection;
            }
        }
    }

    public class LoggerCollection : ConfigurationElementCollection
    {        

        public Logger this[int index]
        {
            get
            {
                return base.BaseGet(index) as Logger;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new Logger this[string responseString]
        {
            get { return (Logger)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new Logger();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Logger)element).Name;
        }
    }

}
