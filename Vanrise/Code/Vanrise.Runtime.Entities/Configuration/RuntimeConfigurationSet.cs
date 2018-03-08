using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeConfigurationSet
    {
        public Guid RuntimeConfigurationSetId { get; set; }

        public string Name { get; set; }

        public RuntimeConfigurationSetSettings Settings { get; set; }
    }

    public class RuntimeConfigurationSetSettings
    {

    }
}
