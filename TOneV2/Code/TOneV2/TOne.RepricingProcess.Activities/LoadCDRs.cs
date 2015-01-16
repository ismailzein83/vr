using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Collections.Concurrent;
using TOne.Business;

namespace TOne.RepricingProcess.Activities
{

    public sealed class LoadCDRs : AsyncCodeActivity<int>
    {
        [RequiredArgument]
        public InArgument<DateTime> From { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> To { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentQueue<CDRBatch>> QueueLoadedCDRs { get; set; }


        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Func<DateTime, DateTime, ConcurrentQueue<CDRBatch>, int> executeAction = new Func<DateTime, DateTime, ConcurrentQueue<CDRBatch>, int>(DoWork);
            context.UserState = executeAction;
            
            return executeAction.BeginInvoke(this.From.Get(context), this.To.Get(context),this.QueueLoadedCDRs.Get(context), callback, state);
        }

        protected override int EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Func<DateTime, DateTime, ConcurrentQueue<CDRBatch>, int> executeAction = (Func<DateTime, DateTime, ConcurrentQueue<CDRBatch>, int>)context.UserState;
            return executeAction.EndInvoke(result);
        }

        int DoWork(DateTime from, DateTime to, ConcurrentQueue<CDRBatch> queue)
        {
            int cdrCount = 0;
            CDRManager manager = new CDRManager();
            manager.LoadCDRRange(from, to, 10000, (cdrs) =>
                {
                    cdrCount += cdrs.Count;
                    queue.Enqueue(new CDRBatch { CDRs = cdrs });                    
                });
            return cdrCount;
        }
    }
}
