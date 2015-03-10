using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.LoggingConfiguration
{
    public class ConfigurationParameter : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return this["value"] as string;
            }
        }
    }

    public class ConfigurationParameterCollection : ConfigurationElementCollection
    {
        public ConfigurationParameter this[int index]
        {
            get
            {
                return base.BaseGet(index) as ConfigurationParameter;
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

        public new ConfigurationParameter this[string responseString]
        {
            get { return (ConfigurationParameter)BaseGet(responseString); }
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
            return new ConfigurationParameter();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigurationParameter)element).Name;
        }
    }

}
