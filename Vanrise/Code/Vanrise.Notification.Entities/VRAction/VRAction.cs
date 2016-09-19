using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRAction
    {
        public virtual Guid ConfigId { get; set; }
        public string ActionName { get; set; }
        public abstract void Execute(IVRActionExecutionContext context);

        public virtual bool IsStillValid(IVRActionValidityContext context)
        {
            return false;
        }
    }

    public interface IVRActionExecutionContext
    {
        IVRActionEventPayload EventPayload { get; }
        int UserID { get; }
        int NumberOfExecutions { get; }

        DateTime? NextExecutionTime { set; }
    }

    public interface IVRActionValidityContext
    {
        int NumberOfExecutions { get; }

        DateTime LastExecutionTime { get; }
    }
}
