using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeNode
    {
        public Guid RuntimeNodeId { get; set; }

        public Guid RuntimeNodeConfigurationId { get; set; }

        public string Name { get; set; }

        public RuntimeNodeSettings Settings { get; set; }
    }

    public class RuntimeNodeSettings
    {

    }
}