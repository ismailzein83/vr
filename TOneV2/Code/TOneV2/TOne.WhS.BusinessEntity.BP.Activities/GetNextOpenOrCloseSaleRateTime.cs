using System;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.BusinessEntity.BP.Activities
{
    public class GetNextOpenOrCloseSaleRateTimeInput
    {
        public DateTime EffectiveDate { get; set; }
    }

    public class GetNextOpenOrCloseSaleRateTimeOutput
    {
        public DateTime? NextOpenOrCloseSaleRateTime { get; set; }
    }

    public sealed class GetNextOpenOrCloseSaleRateTime : BaseAsyncActivity<GetNextOpenOrCloseSaleRateTimeInput, GetNextOpenOrCloseSaleRateTimeOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime?> NextOpenOrCloseSaleRateTime { get; set; }

        protected override GetNextOpenOrCloseSaleRateTimeOutput DoWorkWithResult(GetNextOpenOrCloseSaleRateTimeInput inputArgument, AsyncActivityHandle handle)
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            DateTime? nextOpenOrCloseSaleRateTime = saleRateManager.GetNextOpenOrCloseTime(inputArgument.EffectiveDate);

            return new GetNextOpenOrCloseSaleRateTimeOutput() { NextOpenOrCloseSaleRateTime = nextOpenOrCloseSaleRateTime };
        }

        protected override GetNextOpenOrCloseSaleRateTimeInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetNextOpenOrCloseSaleRateTimeInput()
            {
                EffectiveDate = this.EffectiveDate.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetNextOpenOrCloseSaleRateTimeOutput result)
        {
            this.NextOpenOrCloseSaleRateTime.Set(context, result.NextOpenOrCloseSaleRateTime);
        }
    }
}
