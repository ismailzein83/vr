using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes
    public class PrepareTrafficStatsForDBApplyInput
    {
        public BaseQueue<TOne.CDR.Entities.TrafficStatisticBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareTrafficStatsForDBApply : DependentAsyncActivity<PrepareTrafficStatsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.TrafficStatisticBatch>> InputQueue { get; set; }


        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareTrafficStatsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareTrafficStatsForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(PrepareTrafficStatsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TimeSpan totalTime = default(TimeSpan);
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (Stats) =>
                        {
                            DateTime start = DateTime.Now;
                            Object preparedMainCDRs = dataManager.PrepareTrafficStatsForDBApply(Stats.TrafficStatistics.Values.ToList());
                            inputArgument.OutputQueue.Enqueue(preparedMainCDRs);
                            totalTime += (DateTime.Now - start);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }
    }
}
