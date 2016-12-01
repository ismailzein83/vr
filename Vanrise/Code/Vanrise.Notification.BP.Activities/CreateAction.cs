using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.Business;
using Vanrise.BusinessProcess;

namespace Vanrise.Notification.BP.Activities
{
    public sealed class CreateAction : CodeActivity
    {
        public InArgument<CreateVRActionInput> CreateVRActionInput { get; set; }
        public InArgument<int> UserID { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var createVRActionInput = CreateVRActionInput.Get(context);
            VRActionExecutionContext vrActionExecutionContext = new VRActionExecutionContext
            {
                EventPayload = createVRActionInput.EventPayload,
                UserID = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId
            };
            createVRActionInput.Action.Execute(vrActionExecutionContext);
        }
    }

    #region Async CreateAction Implementation

    //public sealed class CreateAction : NativeActivity
    //{
    //    public InArgument<CreateVRActionInput> CreateVRActionInput { get; set; }
    //    public InArgument<int> UserID { get; set; }        

    //    protected override bool CanInduceIdle
    //    {
    //        get
    //        {
    //            return true;
    //        }
    //    }
              
    //    protected override void Execute(NativeActivityContext context)
    //    {
    //        var createVRActionInput = CreateVRActionInput.Get(context);
    //        Func<VRActionProgressReporter> createProgressReporter = () =>
    //            {
    //                string bookmarkName = String.Format("SetVRActionCompleted_{0}", Guid.NewGuid());
    //                context.CreateBookmark(bookmarkName, BookmarkResumed);
    //                return new ContinueWorkflowVRActionProgressReporter
    //                {
    //                    ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
    //                    BookmarkName = bookmarkName
    //                };
    //            };
    //        VRActionExecutionContext vrActionExecutionContext = new VRActionExecutionContext(createProgressReporter)
    //        {
    //            BPProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
    //            EventPayload = createVRActionInput.EventPayload,
    //            UserID = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId
    //        };
    //        createVRActionInput.Action.Execute(vrActionExecutionContext);
    //    }

    //    private void BookmarkResumed(NativeActivityContext context,
    //          Bookmark bookmark,
    //          object value)
    //    {
            
    //    }

    //    #region Classes

    //    public class ContinueWorkflowVRActionProgressReporter : VRActionProgressReporter
    //    {
    //        public long ProcessInstanceId { get; set; }

    //        public string BookmarkName { get; set; }

    //        public override void SetActionExecutionCompleted(VRActionExecutionCompletedPayload payload)
    //        {
    //            var triggerProcessEventInput = new TriggerProcessEventInput
    //            {
    //                ProcessInstanceId = this.ProcessInstanceId,
    //                BookmarkName = this.BookmarkName,
    //                EventData = payload
    //            };
    //            new BPEventManager().TriggerProcessEvent(triggerProcessEventInput);
    //        }
    //    }

    //    #endregion
    //}

    #endregion
}
