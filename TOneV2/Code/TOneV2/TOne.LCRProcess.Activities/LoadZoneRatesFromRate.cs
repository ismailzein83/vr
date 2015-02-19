using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Entities;
using TOne.Entities;
using TOne.LCR.Data;
using Vanrise.Queueing;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class LoadZoneRatesFromRateInput
    {
        public DateTime EffectiveTime { get; set; }

        public bool IsFuture { get; set; }

        public BaseQueue<ZoneRateBatch> OutputQueueSupplierZR { get; set; }

        public BaseQueue<ZoneRateBatch> OutputQueueCustomerZR { get; set; }
    }

    #endregion

    public sealed class LoadZoneRatesFromRate : BaseAsyncActivity<LoadZoneRatesFromRateInput>
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveTime { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<ZoneRateBatch>> OutputQueueSupplierZR { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<ZoneRateBatch>> OutputQueueCustomerZR { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueueSupplierZR.Get(context) == null)
                this.OutputQueueSupplierZR.Set(context, new MemoryQueue<ZoneRateBatch>());
            if (this.OutputQueueCustomerZR.Get(context) == null)
                this.OutputQueueCustomerZR.Set(context, new MemoryQueue<ZoneRateBatch>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadZoneRatesFromRateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadZoneRatesFromRateInput
            {
                EffectiveTime = this.EffectiveTime.Get(context),
                IsFuture = this.IsFuture.Get(context),
                OutputQueueSupplierZR = this.OutputQueueSupplierZR.Get(context),
                OutputQueueCustomerZR = this.OutputQueueCustomerZR.Get(context)
            };
        }

        protected override void DoWork(LoadZoneRatesFromRateInput inputArgument, AsyncActivityHandle handle)
        {
            IRateDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRateDataManager>();
            dataManager.LoadZoneRates(inputArgument.EffectiveTime, inputArgument.IsFuture, 10000,
                (zoneRateBatch) =>
                {
                    if (zoneRateBatch.IsSupplierZoneRateBatch)
                        inputArgument.OutputQueueSupplierZR.Enqueue(zoneRateBatch);
                    else
                        inputArgument.OutputQueueCustomerZR.Enqueue(zoneRateBatch);
                });
        }
    }
}
