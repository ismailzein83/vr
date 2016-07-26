using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRAction
    {
        public int ConfigId { get; set; }
        public string ActionName { get; set; }
        public abstract void Execute(IVRActionContext context);
    }

    public interface IVRActionContext
    {
        IVRActionEventPayload EventPayload { get; }

        int NumberOfExecutions { get; }

        DateTime? NextExecutionTime { set; }
    }
}
