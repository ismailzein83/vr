using System;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareCustomerZoneDetailForDBApplyInput
    {
        public DateTime? EffectiveOn { get; set; }
        public bool IsFuture { get; set; }
        public int RoutingDatabaseId { get; set; }
        public BaseQueue<CustomerZoneDetailBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareCustomerZoneDetailForDBApply : DependentAsyncActivity<PrepareCustomerZoneDetailForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<CustomerZoneDetailBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareCustomerZoneDetailForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerZoneDetailsDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerZoneDetailsDataManager>();
            dataManager.EffectiveDate = inputArgument.EffectiveOn;
            dataManager.IsFuture = inputArgument.IsFuture;
            dataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, CustomerZoneDetailsBatch => CustomerZoneDetailsBatch.CustomerZoneDetails);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Customer Zone Detail For DB Apply is done", null);
        }

        protected override PrepareCustomerZoneDetailForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCustomerZoneDetailForDBApplyInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsFuture = this.IsFuture.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Object>());
            base.OnBeforeExecute(context, handle);
        }
    }
}