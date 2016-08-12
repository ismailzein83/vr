using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;

namespace Vanrise.Notification.BP.Activities
{
    public class ProcessRulesMatchedEventsInput
    {

    }

    public class ProcessRulesMatchedEventsOutput
    {

    }

    public sealed class ProcessRulesMatchedEvents : DependentAsyncActivity<ProcessRulesMatchedEventsInput, ProcessRulesMatchedEventsOutput>
    {
        [RequiredArgument]
        public InArgument<Guid> AlertRuleTypeId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<VRAlertRuleMatchedEvent>> InputQueue { get; set; }

        protected override ProcessRulesMatchedEventsOutput DoWorkWithResult(ProcessRulesMatchedEventsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override ProcessRulesMatchedEventsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessRulesMatchedEventsOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
