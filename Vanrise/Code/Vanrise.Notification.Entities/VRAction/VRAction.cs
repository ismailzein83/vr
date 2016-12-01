using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRAction
    {
        public abstract Guid ConfigId { get; }
        public string ActionName { get; set; }
        public virtual void Execute(IVRActionExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryConvertToBPInputArgument(IVRActionConvertToBPInputArgumentContext context)
        {
            return false;
        }
    }

    public interface IVRActionExecutionContext
    {
        long? BPProcessInstanceId { get; }
        IVRActionEventPayload EventPayload { get; }
        int UserID { get; }
        int NumberOfExecutions { get; }

        DateTime? NextExecutionTime { set; }

        //void SetExecutionAsynchronous(out VRActionProgressReporter progressReporter);
    }

    public interface IVRActionConvertToBPInputArgumentContext
    {
        IVRActionEventPayload EventPayload { get; }   
        
        int NumberOfExecutions { get; }

        DateTime? NextExecutionTime { set; }

        Vanrise.BusinessProcess.Entities.BaseProcessInputArgument BPInputArgument { set; }
    }

    #region Async CreateAction Implementation

    //public abstract class VRActionProgressReporter
    //{
    //    public abstract void SetActionExecutionCompleted(VRActionExecutionCompletedPayload payload);
    //}

    //public enum VRActionExecutionResult { Succeded = 0, Failed = 10 }

    //public class VRActionExecutionCompletedPayload
    //{
    //    public VRActionExecutionResult Result { get; set; }

    //    public string Message { get; set; }
    //}

    #endregion
}
