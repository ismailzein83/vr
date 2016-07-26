using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRMailMessageType
    {
        public Guid VRMailMessageTypeId { get; set; }

        public string Name { get; set; }

        public List<int> BusinessEntityDefinitionIds { get; set; }
    }
}
