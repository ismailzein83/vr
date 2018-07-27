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
    public class ProcessCDRsInput
    {
        public BaseQueue<RecordBatch> InputQueue { get; set; }

        public TimeSpan DateTimeMargin { get; set; }

        public TimeSpan DurationMargin { get; set; }

        public CDRCorrelationDefinition CDRCorrelationDefinition { get; set; }

        public BaseQueue<RecordBatch> OutputQueue { get; set; }
    }

    public sealed class ProcessCDRs : DependentAsyncActivity<ProcessCDRsInput>
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
        public OutArgument<BaseQueue<RecordBatch>> OutputQueue { get; set; }

        protected override void DoWork(ProcessCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override ProcessCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessCDRsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                CDRCorrelationDefinition = this.CDRCorrelationDefinition.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                DateTimeMargin = this.DateTimeMargin.Get(context),
                DurationMargin = this.DurationMargin.Get(context)
            };
        }
    }
}
