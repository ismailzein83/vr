using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Configuration
{
    public class RuntimeConfig : ConfigurationSection
    {
        public static RuntimeConfig GetConfig()
        {
            return System.Configuration.ConfigurationManager.GetSection("vanriseRuntime") as RuntimeConfig;
        }

        [ConfigurationProperty("runtimeNodeId", IsRequired = true)]
        public Guid RuntimeNodeId
        {
            get
            {
                return new Guid(this["runtimeNodeId"].ToString());
            }
        }

        [System.Configuration.ConfigurationProperty("runtimeServiceGroups")]
        [ConfigurationCollection(typeof(RuntimeServiceGroupCollection), AddItemName = "runtimeServiceGroup")]
        public RuntimeServiceGroupCollection RuntimeServiceGroups
        {
            get
            {
                return this["runtimeServiceGroups"] as RuntimeServiceGroupCollection;
            }
        }
    }
}
