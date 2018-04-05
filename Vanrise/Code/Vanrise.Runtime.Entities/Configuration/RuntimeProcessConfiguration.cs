using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeProcessConfiguration
    {
        public string Name { get; set; }

        public RuntimeProcessConfigurationSettings Settings { get; set; }
    }

    public class RuntimeProcessConfigurationSettings
    {
        public bool IsEnabled { get; set; }

        public int NbOfInstances { get; set; }

        public Dictionary<Guid, RuntimeServiceConfiguration> Services { get; set; }
    }
}
