using System;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.BusinessEntity.BP.Activities
{
    public class GetMaxSaleRateTimeStampInput
    {

    }

    public class GetMaxSaleRateTimeStampOutput
    {
        public object MaxSaleRateTimeStamp { get; set; }
    }

    public sealed class GetMaxSaleRateTimeStamp : BaseAsyncActivity<GetMaxSaleRateTimeStampInput, GetMaxSaleRateTimeStampOutput>
    {
        [RequiredArgument]
        public OutArgument<object> MaxSaleRateTimeStamp { get; set; }

        protected override GetMaxSaleRateTimeStampOutput DoWorkWithResult(GetMaxSaleRateTimeStampInput inputArgument, AsyncActivityHandle handle)
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            object maxSaleRateTimeStamp = saleRateManager.GetMaximumTimeStamp();

            return new GetMaxSaleRateTimeStampOutput() { MaxSaleRateTimeStamp = maxSaleRateTimeStamp };
        }

        protected override GetMaxSaleRateTimeStampInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetMaxSaleRateTimeStampInput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetMaxSaleRateTimeStampOutput result)
        {
            this.MaxSaleRateTimeStamp.Set(context, result.MaxSaleRateTimeStamp);
        }
    }
}
