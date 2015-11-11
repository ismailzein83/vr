using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public class GenerateSupplierZoneDetailsInput
    {
        public BaseQueue<SupplierZoneDetailBatch> OutputQueue { get; set; }

        public DateTime? EffectiveOn { get; set; }
        public bool IsEffectiveInFuture { get; set; }

    }
    public sealed class GenerateSupplierZoneDetails : BaseAsyncActivity<GenerateSupplierZoneDetailsInput>
    {
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }
        [RequiredArgument]
        public InArgument<bool> IsEffectiveInFuture { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<SupplierZoneDetailBatch>> OutputQueue { get; set; }

        protected override void DoWork(GenerateSupplierZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierZoneDetailBatch supplierZoneDetailBatch = new SupplierZoneDetailBatch();
            supplierZoneDetailBatch.SupplierZoneDetails = new List<SupplierZoneDetail>();
            ZoneDetailBuilder zoneDetailBuilder = new ZoneDetailBuilder();
            zoneDetailBuilder.BuildSupplierZoneDetails(inputArgument.EffectiveOn, inputArgument.IsEffectiveInFuture, (supplierZoneDetail) =>
            {
                supplierZoneDetailBatch.SupplierZoneDetails.Add(supplierZoneDetail);
                if (supplierZoneDetailBatch.SupplierZoneDetails.Count >= 10000)
                {
                    inputArgument.OutputQueue.Enqueue(supplierZoneDetailBatch);
                    supplierZoneDetailBatch = new SupplierZoneDetailBatch();
                    supplierZoneDetailBatch.SupplierZoneDetails = new List<SupplierZoneDetail>();
                }
            });

            if (supplierZoneDetailBatch.SupplierZoneDetails.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(supplierZoneDetailBatch);
                supplierZoneDetailBatch = new SupplierZoneDetailBatch();
                supplierZoneDetailBatch.SupplierZoneDetails = new List<SupplierZoneDetail>();
            }
        }

        protected override GenerateSupplierZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GenerateSupplierZoneDetailsInput()
            {
                EffectiveOn = this.EffectiveOn.Get(context),
                IsEffectiveInFuture = this.IsEffectiveInFuture.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
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
