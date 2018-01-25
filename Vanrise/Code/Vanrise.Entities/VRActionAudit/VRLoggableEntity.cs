using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRLoggableEntity
    {
        public Guid VRLoggableEntityId { get; set; }

        public string UniqueName { get; set; }

        public VRLoggableEntitySettings Settings { get; set; }
    }

    public class VRLoggableEntitySettings
    {
        public string ViewHistoryItemClientActionName { get; set; }

        public VRActionAuditChangeInfoDefinition ChangeInfoDefinition { get; set; }

    }
}
