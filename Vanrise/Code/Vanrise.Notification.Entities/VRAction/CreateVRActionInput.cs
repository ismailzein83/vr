using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class CreateVRActionInput
    {
        public VRAction Action { get; set; }

        public IVRActionEventPayload EventPayload { get; set; }

        public IVRActionRollbackEventPayload RollbackEventPayload { get; set; }
    }
}
