using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Business;
using System.Threading.Tasks;
namespace TOne.RepricingProcess.Activities
{

    public sealed class ClearTimeRangeBillingRecords : AsyncCodeActivity
    {
        
        [RequiredArgument]
        public InArgument<TimeRange> TimeRange { get; set; }

       
        

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Action<TimeRange> executeAction = new Action<TimeRange>(DoWork);
            context.UserState = executeAction;

            return executeAction.BeginInvoke(this.TimeRange.Get(context), callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Action<TimeRange> executeAction = (Action<TimeRange>)context.UserState;
            executeAction.EndInvoke(result);
        }

        void DoWork(TimeRange timeRange)
        {            
            CDRManager cdrManager = new CDRManager();
            DateTime deletionStart = DateTime.Now;
            DateTime from = timeRange.From;
            DateTime to = timeRange.To;
            Action[] deleteActions = new Action[]
                {
                    ()=> cdrManager.DeleteCDRMain(from,to),
                     ()=> cdrManager.DeleteCDRInvalid(from, to),
                     ()=> cdrManager.DeleteCDRCost(from, to), 
                     ()=> cdrManager.DeleteCDRSale(from, to), 
                     ()=> cdrManager.DeleteTrafficStats(from, to)
                };


            Parallel.ForEach(deleteActions, (action) =>
            {
                action();
            });

            //Console.WriteLine("{0}: {1} to delete records in interval {2: HH:mm} - {3: HH:mm}", DateTime.Now, (DateTime.Now - deletionStart), from, to);
        }
    }
}
