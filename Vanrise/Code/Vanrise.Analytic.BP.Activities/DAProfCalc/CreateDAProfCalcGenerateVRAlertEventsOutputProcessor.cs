using System;
using System.Activities;
using Vanrise.Analytic.Business;
using Vanrise.BusinessProcess;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    public sealed class CreateDAProfCalcGenerateVRAlertEventsOutputProcessor : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<DAProfCalcGenerateVRAlertEventsOutputProcessor> OutputProcessor { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectivceDate = EffectiveDate.Get(context);
            var sharedInstance = context.GetSharedInstanceData();

            var outputProcessor = new DAProfCalcGenerateVRAlertEventsOutputProcessor(effectivceDate,
             (logEntryType, message) => { sharedInstance.WriteTrackingMessage(logEntryType, message); });

            OutputProcessor.Set(context, outputProcessor);
        }
    }
}