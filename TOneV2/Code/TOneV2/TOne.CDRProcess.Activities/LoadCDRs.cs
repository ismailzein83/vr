using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Collections.Concurrent;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess.Activities
{

    #region Arguments Classes

    public class LoadCDRsInput
    {
        public DateTime From { get; set; }
   
        public DateTime To { get; set; }

        public ConcurrentQueue<CDRBatch> QueueLoadedCDRs { get; set; }
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
        public InArgument<ConcurrentQueue<CDRBatch>> QueueLoadedCDRs { get; set; }

        [RequiredArgument]
        public OutArgument<int> Result { get; set; }

        protected override LoadCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsInput
            {
                From = this.From.Get(context),
                To = this.To.Get(context),
                QueueLoadedCDRs = this.QueueLoadedCDRs.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsOutput result)
        {
            this.Result.Set(context, result.Result);
        }

        protected override LoadCDRsOutput DoWorkWithResult(LoadCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            int cdrCount = 0;
            CDRManager manager = new CDRManager();
            manager.LoadCDRRange(inputArgument.From, inputArgument.To, 10000, (cdrs) =>
            {
                cdrCount += cdrs.Count;
                inputArgument.QueueLoadedCDRs.Enqueue(new CDRBatch { CDRs = cdrs });
            });

            return new LoadCDRsOutput
            {
                Result = cdrCount
            };
        }
    }
}
