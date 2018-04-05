using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeNodeConfiguration
    {
        public Guid RuntimeNodeConfigurationId { get; set; }

        public string Name { get; set; }

        public RuntimeNodeConfigurationSettings Settings { get; set; }
    }

    public class RuntimeNodeConfigurationSettings
    {
        public Dictionary<Guid, RuntimeProcessConfiguration> Processes { get; set; }
    }
}
