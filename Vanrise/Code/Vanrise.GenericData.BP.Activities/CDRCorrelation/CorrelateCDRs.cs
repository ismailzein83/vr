using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace Vanrise.GenericData.BP.Activities
{
    public class CorrelateCDRsInput
    {
        public BaseQueue<RecordBatch> InputQueue { get; set; }

        public TimeSpan DateTimeMargin { get; set; }

        public TimeSpan DurationMargin { get; set; }

        public CDRCorrelationDefinition CDRCorrelationDefinition { get; set; }

        public BaseQueue<CDRCorrelationBatch> OutputQueue { get; set; }
    }

    public sealed class CorrelateCDRs : DependentAsyncActivity<CorrelateCDRsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<RecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> DateTimeMargin { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> DurationMargin { get; set; }

        [RequiredArgument]
        public InArgument<CDRCorrelationDefinition> CDRCorrelationDefinition { get; set; }

        [RequiredArgument]
        public OutArgument<BaseQueue<CDRCorrelationBatch>> OutputQueue { get; set; }

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
           // CDRCorrelationDefinition cdrCorrelationDefinition = inputArgument.CDRCorrelationDefinition;

           //TimeSpan dateTimeMargin = inputArgument.DateTimeMargin;
           // TimeSpan durationMargin = inputArgument.DurationMargin;

           // List<dynamic> uncorrelatedCDRs = new List<dynamic>(); 
           // List<CDRCorrelationBatch> outputQueue = new List<CDRCorrelationBatch>();

           // DoWhilePreviousRunning(previousActivityStatus, handle, () =>
           // {
           //     bool hasItems = false;
           //     do
           //     {
           //         hasItems = inputArgument.InputQueue.TryDequeue(
           //             (recordBatch) =>
           //             {
           //                 if (recordBatch.Records != null && recordBatch.Records.Count > 0)
           //                 {
           //                     foreach(var cdr in recordBatch.Records)
           //                     {
           //                         DateTime cdrAttemptDateTime = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.DatetimeFieldName);

           //                         foreach(var uncorrelatedCDR in uncorrelatedCDRs)
           //                         {
           //                             DateTime uncorrelatedCDRAttemptDateTime = cdr.GetFieldValue(cdrCorrelationDefinition.Settings.DatetimeFieldName);

           //                             if((cdrAttemptDateTime - dateTimeMargin) > uncorrelatedCDRAttemptDateTime)
           //                             {

           //                             }
           //                         }
           //                     }
           //                 }
           //             });
           //     } while (!ShouldStop(handle) && hasItems);
           // });
        }

        protected override CorrelateCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CorrelateCDRsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                DateTimeMargin = this.DateTimeMargin.Get(context),
                DurationMargin = this.DurationMargin.Get(context),
                CDRCorrelationDefinition = this.CDRCorrelationDefinition.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
