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

        public VRMailMessageTypeSettings Settings { get; set; }
    }

    public class VRMailMessageTypeSettings
    {
        public VRObjectVariableCollection Objects { get; set; }
    }
}
