using System;

namespace Vanrise.Notification.Entities
{
    public abstract class VRAction
    {
        //public abstract Guid ConfigId { get; }
        public string ActionName { get; set; }
        public virtual void Execute(IVRActionExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryConvertToBPInputArgument(IVRActionConvertToBPInputArgumentContext context)
        {
            return false;
        }
        public Guid DefinitionId { get; set; }
    }

    public interface IVRActionExecutionContext
    {
        long? BPProcessInstanceId { get; }
        IVRActionEventPayload EventPayload { get; }
        IVRActionRollbackEventPayload RollbackEventPayload { get; }
        int UserID { get; }
        int NumberOfExecutions { get; }
        DateTime? NextExecutionTime { set; }
        long? AlertRuleId { get; }

        //void SetExecutionAsynchronous(out VRActionProgressReporter progressReporter);
    }

    public interface IVRActionConvertToBPInputArgumentContext
    {
        IVRActionEventPayload EventPayload { get; }
        IVRActionRollbackEventPayload RollbackEventPayload { get; }

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
