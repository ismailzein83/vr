using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Configuration
{
    public class RuntimeServiceGroup : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("nbOfRuntimeInstances", IsRequired = true)]
        public int NbOfRuntimeInstances
        {
            get
            {
                return (int)this["nbOfRuntimeInstances"];
            }
        }

        [System.Configuration.ConfigurationProperty("runtimeServices")]
        [ConfigurationCollection(typeof(RuntimeServiceCollection), AddItemName = "runtimeService")]
        public RuntimeServiceCollection RuntimeServices
        {
            get
            {
                return this["runtimeServices"] as RuntimeServiceCollection;
            }
        }
    }

    public class RuntimeServiceGroupCollection : ConfigurationElementCollection
    {

        public RuntimeServiceGroup this[int index]
        {
            get
            {
                return base.BaseGet(index) as RuntimeServiceGroup;
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

        public new RuntimeServiceGroup this[string serviceName]
        {
            get { return (RuntimeServiceGroup)BaseGet(serviceName); }
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
            return new RuntimeServiceGroup();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RuntimeServiceGroup)element).Name;
        }
    }
}
