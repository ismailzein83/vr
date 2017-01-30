using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRConnectionQuery
    {
        public string Name { get; set; }

        public List<Guid> ExtensionConfigIds { get; set; }
    }
}
