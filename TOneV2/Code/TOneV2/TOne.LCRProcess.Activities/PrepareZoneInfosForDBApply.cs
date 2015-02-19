using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Collections.Concurrent;
using TOne.Entities;
using Vanrise.BusinessProcess;
using System.Threading;
using TOne.LCR.Data;
using Vanrise.Queueing;
using TOne.BusinessEntity.Entities;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes
    public class PrepareZoneInfosForDBApplyInput
    {
        public BaseQueue<List<ZoneInfo>> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareZoneInfosForDBApply : DependentAsyncActivity<PrepareZoneInfosForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<List<ZoneInfo>>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareZoneInfosForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareZoneInfosForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(PrepareZoneInfosForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IZoneInfoDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneInfoDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            TimeSpan totalTime = default(TimeSpan);
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (zoneInfos) =>
                        {
                            DateTime start = DateTime.Now;
                            Object preparedCodeMatches = dataManager.PrepareZoneInfosForDBApply(zoneInfos);
                            inputArgument.OutputQueue.Enqueue(preparedCodeMatches);
                            totalTime += (DateTime.Now - start);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }
    }
}
