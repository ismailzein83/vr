using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeServiceGroupConfiguration
    {
        public Guid RuntimeServiceGroupConfigurationId { get; set; }

        public Guid ConfigurationSetId { get; set; }

        public string Name { get; set; }

        public RuntimeServiceGroupConfigurationSettings Settings { get; set; }
    }

    public class RuntimeServiceGroupConfigurationSettings
    {
        public int NbOfInstances { get; set; }
    }
}
