using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class GenerateCustomerZoneDetailsInput
    {
        public BaseQueue<CustomerZoneDetailBatch> OutputQueue { get; set; }
        public IEnumerable<RoutingCustomerInfo> CustomerInfos { get; set; }
        public DateTime? EffectiveOn { get; set; }
        public bool IsEffectiveInFuture { get; set; }
        public int VersionNumber { get; set; }
    }

    public sealed class GenerateCustomerZoneDetails : BaseAsyncActivity<GenerateCustomerZoneDetailsInput>
    {
        [RequiredArgument]
        public InOutArgument<BaseQueue<CustomerZoneDetailBatch>> OutputQueue { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<RoutingCustomerInfo>> CustomerInfos { get; set; }
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }
        [RequiredArgument]
        public InArgument<bool> IsEffectiveInFuture { get; set; }
        [RequiredArgument]
        public InArgument<int> VersionNumber { get; set; }

        protected override void DoWork(GenerateCustomerZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {
            CustomerZoneDetailBatch customerZoneDetailBatch = new CustomerZoneDetailBatch();
            customerZoneDetailBatch.CustomerZoneDetails = new List<CustomerZoneDetail>();
            ZoneDetailBuilder zoneDetailBuilder = new ZoneDetailBuilder();

            zoneDetailBuilder.BuildCustomerZoneDetails(inputArgument.CustomerInfos, inputArgument.EffectiveOn, inputArgument.IsEffectiveInFuture, inputArgument.VersionNumber, () => { return ShouldStop(handle); }, (customerZoneDetail) =>
            {
                customerZoneDetailBatch.CustomerZoneDetails.Add(customerZoneDetail);
                //TODO: Batch Count Should be configuration parameter
                if (customerZoneDetailBatch.CustomerZoneDetails.Count >= 10000)
                {
                    inputArgument.OutputQueue.Enqueue(customerZoneDetailBatch);
                    customerZoneDetailBatch = new CustomerZoneDetailBatch();
                    customerZoneDetailBatch.CustomerZoneDetails = new List<CustomerZoneDetail>();
                }
            });

            if (customerZoneDetailBatch.CustomerZoneDetails.Count > 0)
                inputArgument.OutputQueue.Enqueue(customerZoneDetailBatch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Generating Customer Zone Details is done", null);
        }

        protected override GenerateCustomerZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GenerateCustomerZoneDetailsInput()
            {
                CustomerInfos = this.CustomerInfos.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsEffectiveInFuture = this.IsEffectiveInFuture.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                VersionNumber = this.VersionNumber.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CustomerZoneDetailBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
