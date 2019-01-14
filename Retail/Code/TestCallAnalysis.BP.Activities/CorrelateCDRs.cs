using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    public class CorrelateCDRsInput
    {
        public MemoryQueue<RecordBatch> InputQueue { get; set; }
        public TimeSpan DateTimeMargin { get; set; }
        public MemoryQueue<Entities.CDRCorrelationBatch> OutputQueue { get; set; }
        public MemoryQueue<Entities.CDRCorrelationBatch> OutputCaseQueue { get; set; }

    }
    public sealed class CorrelateCDRs : DependentAsyncActivity<CorrelateCDRsInput>
    {
        [RequiredArgument]
        public InArgument<MemoryQueue<RecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> DateTimeMargin { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> OutputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<MemoryQueue<Entities.CDRCorrelationBatch>> OutputCaseQueue { get; set; }

        protected override CorrelateCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CorrelateCDRsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                DateTimeMargin = this.DateTimeMargin.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                OutputCaseQueue = this.OutputCaseQueue.Get(context)
            };
        }

        protected override void DoWork(CorrelateCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan dateTimeMargin = inputArgument.DateTimeMargin;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                    {
                        DateTime batchStartTime = DateTime.Now;

                        if (recordBatch.Records != null && recordBatch.Records.Count > 0)
                        {
                            DateTime maxDateTime = DateTime.MinValue;
                            foreach (var cdr in recordBatch.Records)
                            {
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItems);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Correlate CDRs is done.");
        }
    }
}
