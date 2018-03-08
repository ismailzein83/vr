using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeHostConfiguration
    {
        public Guid RuntimeHostConfigurationId { get; set; }

        public string Name { get; set; }

        public Guid RuntimeConfigurationSetId { get; set; }

        public RuntimeHostConfigurationSettings Settings { get; set; }
    }

    public class RuntimeHostConfigurationSettings
    {

    }
}
