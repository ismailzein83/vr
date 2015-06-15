using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes

    public class GenerateDailyTrafficStatisticsInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBillingBatch> InputQueue { get; set; }

        public BaseQueue<TrafficStatisticDailyBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class GenerateDailyTrafficStatistics : DependentAsyncActivity<GenerateDailyTrafficStatisticsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBillingBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TrafficStatisticDailyBatch>> OutputQueue { get; set; }

        protected override void DoWork(GenerateDailyTrafficStatisticsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override GenerateDailyTrafficStatisticsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GenerateDailyTrafficStatisticsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<TrafficStatisticDailyBatch>());

            base.OnBeforeExecute(context, handle);
        }

    }
}
