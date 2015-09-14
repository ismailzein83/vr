using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Collections.Concurrent;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{

    #region Arguments Classes

    public class LoadCDRsInput
    {
        public DateTime From { get; set; }
   
        public DateTime To { get; set; }

        public BaseQueue<TOne.CDR.Entities.CDRBatch> OutputQueue { get; set; }
    }

    public class LoadCDRsOutput
    {
        public int Result { get; set; }
    }

    #endregion

    public sealed class LoadCDRs : BaseAsyncActivity<LoadCDRsInput, LoadCDRsOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime> From { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> To { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TOne.CDR.Entities.CDRBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public OutArgument<int> Result { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override LoadCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsInput
            {
                From = this.From.Get(context),
                To = this.To.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsOutput result)
        {
            this.Result.Set(context, result.Result);
        }

        protected override LoadCDRsOutput DoWorkWithResult(LoadCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            int cdrCount = 0;
            TOne.CDR.Business.CDRManager cdrmanager = new TOne.CDR.Business.CDRManager();
            cdrmanager.LoadCDRRange(inputArgument.From, inputArgument.To, 10000, (cdrs) =>
            {
                cdrCount += cdrs.Count;
                inputArgument.OutputQueue.Enqueue(new TOne.CDR.Entities.CDRBatch { CDRs = cdrs });
            });

            return new LoadCDRsOutput
            {
                Result = cdrCount
            };
        }
    }
}
