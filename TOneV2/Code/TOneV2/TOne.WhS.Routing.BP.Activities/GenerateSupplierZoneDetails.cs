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
    public sealed class GenerateSupplierZoneDetails : DependentAsyncActivity<GenerateSupplierZoneDetailsInput>
    {
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }
        [RequiredArgument]
        public InArgument<bool> IsEffectiveInFuture { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<SupplierZoneDetailBatch>> OutputQueue { get; set; }

        protected override void DoWork(GenerateSupplierZoneDetailsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            SupplierZoneDetailBatch supplierZoneDetailBatch = new SupplierZoneDetailBatch();
            supplierZoneDetailBatch.SupplierZoneDetails = new List<SupplierZoneDetail>();
            ZoneDetailBuilder zoneDetailBuilder = new ZoneDetailBuilder();
            zoneDetailBuilder.BuildSupplierZoneDetails(inputArgument.EffectiveOn, inputArgument.IsEffectiveInFuture, (supplierZoneDetail) =>
            {
                supplierZoneDetailBatch.SupplierZoneDetails.Add(supplierZoneDetail);

            });


        }

        protected override GenerateSupplierZoneDetailsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new GenerateSupplierZoneDetailsInput()
            {
                EffectiveOn = this.EffectiveOn.Get(context),
                IsEffectiveInFuture = this.IsEffectiveInFuture.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
