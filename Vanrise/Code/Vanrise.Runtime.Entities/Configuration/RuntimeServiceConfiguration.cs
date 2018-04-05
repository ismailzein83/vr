using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeServiceConfiguration
    {
        public string Name { get; set; }

        public RuntimeServiceConfigurationSettings Settings { get; set; }
    }

    public class RuntimeServiceConfigurationSettings
    {
        public RuntimeService RuntimeService { get; set; }
    }
}
