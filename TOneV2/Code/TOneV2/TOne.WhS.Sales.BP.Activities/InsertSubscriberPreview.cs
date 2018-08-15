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
        public InArgument<List<ExcludedChange>> ExcludedCountries { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            long rootProcessInstanceId = ratePlanContext.RootProcessInstanceId;
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            int subscriberId = SubscriberId.Get(context);
            List<ExcludedChange> excludedCountries = ExcludedCountries.Get(context);
            bool terminatedDueBusinessRulesViolation = TerminatedDueBusinessRulesViolation.Get(context);
            SubscriberProcessStatus status;

            if (terminatedDueBusinessRulesViolation)
            {
                status = SubscriberProcessStatus.Failed;
            }
            else if (ratePlanContext.ProcessHasChanges)
            {
                status = SubscriberProcessStatus.Success;
            }
            else
            {
                status = SubscriberProcessStatus.NoChange;
            }
            var subscriberPreview = new SubscriberPreview
            {
                SubscriberId = subscriberId,
                Status = status,
                SubscriberProcessInstanceId = processInstanceId,
                ExcludedCountries = excludedCountries
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
