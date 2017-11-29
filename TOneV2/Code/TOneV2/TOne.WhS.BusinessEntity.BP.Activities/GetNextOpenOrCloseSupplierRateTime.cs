using System;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.BusinessEntity.BP.Activities
{
    public class GetNextOpenOrCloseSupplierRateTimeInput
    {
        public DateTime EffectiveDate { get; set; }
    }

    public class GetNextOpenOrCloseSupplierRateTimeOutput
    {
        public DateTime? NextOpenOrCloseSupplierRateTime { get; set; }
    }

    public sealed class GetNextOpenOrCloseSupplierRateTime : BaseAsyncActivity<GetNextOpenOrCloseSupplierRateTimeInput, GetNextOpenOrCloseSupplierRateTimeOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime?> NextOpenOrCloseSupplierRateTime { get; set; }

        protected override GetNextOpenOrCloseSupplierRateTimeOutput DoWorkWithResult(GetNextOpenOrCloseSupplierRateTimeInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            DateTime? nextOpenOrCloseSupplierRateTime = supplierRateManager.GetNextOpenOrCloseTime(inputArgument.EffectiveDate);

            return new GetNextOpenOrCloseSupplierRateTimeOutput() { NextOpenOrCloseSupplierRateTime = nextOpenOrCloseSupplierRateTime };
        }

        protected override GetNextOpenOrCloseSupplierRateTimeInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetNextOpenOrCloseSupplierRateTimeInput()
            {
                EffectiveDate = this.EffectiveDate.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetNextOpenOrCloseSupplierRateTimeOutput result)
        {
            this.NextOpenOrCloseSupplierRateTime.Set(context, result.NextOpenOrCloseSupplierRateTime);
        }
    }
}
