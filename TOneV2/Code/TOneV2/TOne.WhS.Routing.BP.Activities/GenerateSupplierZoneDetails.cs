using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class GenerateSupplierZoneDetailsInput
    {
        public BaseQueue<SupplierZoneDetailBatch> OutputQueue { get; set; }
        public IEnumerable<RoutingSupplierInfo> SupplierInfos { get; set; }
        public DateTime? EffectiveOn { get; set; }
        public bool IsEffectiveInFuture { get; set; }
        public int VersionNumber { get; set; }
    }

    public sealed class GenerateSupplierZoneDetails : BaseAsyncActivity<GenerateSupplierZoneDetailsInput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<RoutingSupplierInfo>> SupplierInfos { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsEffectiveInFuture { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<SupplierZoneDetailBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> VersionNumber { get; set; }

        protected override void DoWork(GenerateSupplierZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierZoneDetailBatch supplierZoneDetailBatch = new SupplierZoneDetailBatch();
            supplierZoneDetailBatch.SupplierZoneDetails = new List<SupplierZoneDetail>();
            ZoneDetailBuilder zoneDetailBuilder = new ZoneDetailBuilder();

            zoneDetailBuilder.BuildSupplierZoneDetails(inputArgument.EffectiveOn, inputArgument.IsEffectiveInFuture, inputArgument.SupplierInfos, inputArgument.VersionNumber, () => { return ShouldStop(handle); }, (supplierZoneDetail) =>
            {
                supplierZoneDetailBatch.SupplierZoneDetails.Add(supplierZoneDetail);
                //TODO: Batch Count Should be configuration parameter
                if (supplierZoneDetailBatch.SupplierZoneDetails.Count >= 10000)
                {
                    inputArgument.OutputQueue.Enqueue(supplierZoneDetailBatch);
                    supplierZoneDetailBatch = new SupplierZoneDetailBatch();
                    supplierZoneDetailBatch.SupplierZoneDetails = new List<SupplierZoneDetail>();
                }
            });

            if (supplierZoneDetailBatch.SupplierZoneDetails.Count > 0)
                inputArgument.OutputQueue.Enqueue(supplierZoneDetailBatch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Generating Supplier Zone Details is done", null);
        }

        protected override GenerateSupplierZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GenerateSupplierZoneDetailsInput()
            {
                SupplierInfos = this.SupplierInfos.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsEffectiveInFuture = this.IsEffectiveInFuture.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                VersionNumber = this.VersionNumber.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<SupplierZoneDetailBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
