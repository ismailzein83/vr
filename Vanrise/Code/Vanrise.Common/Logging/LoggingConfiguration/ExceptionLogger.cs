using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.LoggingConfiguration
{
    public class ExceptionLogger : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
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

    public class ExceptionLoggerCollection : ConfigurationElementCollection
    {

        public ExceptionLogger this[int index]
        {
            get
            {
                return base.BaseGet(index) as ExceptionLogger;
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

        public new ExceptionLogger this[string responseString]
        {
            get { return (ExceptionLogger)BaseGet(responseString); }
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
            return new ExceptionLogger();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ExceptionLogger)element).Name;
        }
    }

}
