using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;


namespace TOne.WhS.Sales.BP.Activities
{
    public class InsertSubscriberPreview:CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SubscriberId { get; set; }

        [RequiredArgument]
        public InArgument<bool> TerminatedDueBusinessRulesViolation { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            long rootProcessInstanceId = ratePlanContext.RootProcessInstanceId;
            int subscriberId = SubscriberId.Get(context);
            bool terminatedDueBusinessRulesViolation = TerminatedDueBusinessRulesViolation.Get(context);
            string description;
            SubscriberProcessStatus status;

            if (terminatedDueBusinessRulesViolation)
            {
                status = SubscriberProcessStatus.Failed;
                long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
                description = getTerminatedDueBusinessRulesViolationDescription(processInstanceId);
            }
            else if (ratePlanContext.ProcessHasChanges)
            {
                status = SubscriberProcessStatus.Success;
                description = "Success";
            }
            else
            {
                status = SubscriberProcessStatus.NoChange;
                description = "Subscriber has no changes";
            }
            var subscriberPreview = new SubscriberPreview
            {
                SubscriberId = subscriberId,
                Status = status,
                Description = description
            };
            var subscriberPreviewDataManager = SalesDataManagerFactory.GetDataManager<ISubscriberPreviewDataManager>();
            subscriberPreviewDataManager.ProcessInstanceId = rootProcessInstanceId;
            subscriberPreviewDataManager.InsertSubscriberPreview(subscriberPreview);
        }
        private string getTerminatedDueBusinessRulesViolationDescription(long processInstanceId)
        {
            var bPInstanceTrackingManager = new BPInstanceTrackingManager();
            IEnumerable<BPTrackingMessage> bPTrackingMessages = bPInstanceTrackingManager.GetBPInstanceTrackingMessages(processInstanceId, new List<LogEntryType> { LogEntryType.Error });
            return string.Join(", ", bPTrackingMessages.Select(p => p.TrackingMessage));
        }
    }
}
