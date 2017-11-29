using System;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.BusinessEntity.BP.Activities
{
    public class GetMaxSupplierRateTimeStampInput
    {

    }

    public class GetMaxSupplierRateTimeStampOutput
    {
        public object MaxSupplierRateTimeStamp { get; set; }
    }

    public sealed class GetMaxSupplierRateTimeStamp : BaseAsyncActivity<GetMaxSupplierRateTimeStampInput, GetMaxSupplierRateTimeStampOutput>
    {
        [RequiredArgument]
        public OutArgument<object> MaxSupplierRateTimeStamp { get; set; }

        protected override GetMaxSupplierRateTimeStampOutput DoWorkWithResult(GetMaxSupplierRateTimeStampInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            object maxSupplierRateTimeStamp = supplierRateManager.GetMaximumTimeStamp();

            return new GetMaxSupplierRateTimeStampOutput() { MaxSupplierRateTimeStamp = maxSupplierRateTimeStamp };
        }

        protected override GetMaxSupplierRateTimeStampInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetMaxSupplierRateTimeStampInput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetMaxSupplierRateTimeStampOutput result)
        {
            this.MaxSupplierRateTimeStamp.Set(context, result.MaxSupplierRateTimeStamp);
        }
    }
}
