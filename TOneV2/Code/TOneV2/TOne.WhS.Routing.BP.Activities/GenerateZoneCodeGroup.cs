using System;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public class GenerateZoneCodeGroupInput
    {
        public BaseQueue<ZoneCodeGroupBatch> OutputQueue { get; set; }
        public DateTime? EffectiveOn { get; set; }
        public bool IsFuture { get; set; }
    }

    public sealed class GenerateZoneCodeGroup : BaseAsyncActivity<GenerateZoneCodeGroupInput>
    {
        [RequiredArgument]
        public InOutArgument<BaseQueue<ZoneCodeGroupBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        protected override void DoWork(GenerateZoneCodeGroupInput inputArgument, AsyncActivityHandle handle)
        {
            ZoneCodeGroupManager zoneCodeGroupManager = new ZoneCodeGroupManager();

            ZoneCodeGroupBatch saleZoneCodeGroupBatch = new ZoneCodeGroupBatch();
            saleZoneCodeGroupBatch.ZoneCodeGroups = zoneCodeGroupManager.GetSaleZoneCodeGroups(inputArgument.EffectiveOn, inputArgument.IsFuture);
            if (saleZoneCodeGroupBatch.ZoneCodeGroups != null && saleZoneCodeGroupBatch.ZoneCodeGroups.Count > 0)
                inputArgument.OutputQueue.Enqueue(saleZoneCodeGroupBatch);

            ZoneCodeGroupBatch costZoneCodeGroupBatch = new ZoneCodeGroupBatch();
            costZoneCodeGroupBatch.ZoneCodeGroups = zoneCodeGroupManager.GetCostZoneCodeGroups(inputArgument.EffectiveOn, inputArgument.IsFuture);
            if (costZoneCodeGroupBatch.ZoneCodeGroups != null && costZoneCodeGroupBatch.ZoneCodeGroups.Count > 0)
                inputArgument.OutputQueue.Enqueue(costZoneCodeGroupBatch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Generating Zone Code Group is done", null);
        }

        protected override GenerateZoneCodeGroupInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GenerateZoneCodeGroupInput()
            {
                EffectiveOn = this.EffectiveOn.Get(context),
                IsFuture = this.IsFuture.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<ZoneCodeGroupBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}