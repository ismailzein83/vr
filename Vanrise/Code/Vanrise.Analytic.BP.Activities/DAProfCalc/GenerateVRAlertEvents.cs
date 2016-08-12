using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Analytic.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    #region Arguments

    public class GenerateVRAlertEventsInput
    {

    }

    public class GenerateVRAlertEventsOutput
    {

    }

    #endregion

    public sealed class GenerateVRAlertEvents : DependentAsyncActivity<GenerateVRAlertEventsInput, GenerateVRAlertEventsOutput>
    {
        [RequiredArgument]
        public InArgument<List<VRAlertRule<DAProfCalcAlertRuleCriteria>>> AlertRules { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<DAProfCalcOutputRecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<VRAlertRuleMatchedEvent>> OutputQueue { get; set; }

        protected override GenerateVRAlertEventsOutput DoWorkWithResult(GenerateVRAlertEventsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override GenerateVRAlertEventsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GenerateVRAlertEventsOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
