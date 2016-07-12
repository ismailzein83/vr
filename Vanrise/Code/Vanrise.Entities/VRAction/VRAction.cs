using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRAction
    {
        public int ConfigId { get; set; }

        public IVRActionEventPayload EventPayload { get; set; }

        public abstract void Execute(IVRActionContext context);

        public VRActionValidity Validity { get; set; }
    }

    public interface IVRActionContext
    {
        IVRActionEventPayload EventPayload { get; }

        int NumberOfExecutions { get; }

        DateTime? NextExecutionTime { set; }
    }

    public interface IVRActionEventPayload
    {

    }

    public abstract class VRActionValidity
    {
        public abstract bool IsValid(IVRActionValidityContext context);
    }

    public interface IVRActionValidityContext
    {
        VRAction Action { get; }
    }
}
