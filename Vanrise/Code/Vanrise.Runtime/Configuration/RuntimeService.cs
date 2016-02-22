using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Configuration
{
    public class RuntimeService : ConfigurationElement
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

        [ConfigurationProperty("interval", IsRequired = true)]
        public TimeSpan Interval
        {
            get
            {
                return (TimeSpan)this["interval"];
            }
        }

        //[ConfigurationProperty("parameters", IsRequired = false)]
        //[ConfigurationCollection(typeof(ConfigurationParameterCollection), AddItemName = "parameter")]
        //public ConfigurationParameterCollection Parameters
        //{
        //    get
        //    {
        //        return this["parameters"] as ConfigurationParameterCollection;
        //    }
        //}
    }

    public class RuntimeServiceCollection : ConfigurationElementCollection
    {

        public RuntimeService this[int index]
        {
            get
            {
                return base.BaseGet(index) as RuntimeService;
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

        public new RuntimeService this[string serviceName]
        {
            get { return (RuntimeService)BaseGet(serviceName); }
            set
            {
                if (BaseGet(serviceName) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(serviceName)));
                }
                BaseAdd(value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new RuntimeService();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RuntimeService)element).Name;
        }
    }
}
