using System;
using System.Linq;
using System.Activities;
using Vanrise.BusinessProcess;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.BP.Activities
{
    public class GetSalePricelistInput
    {
        public SalePriceList SalePriceList { get; set; }
    }
    public class GetSalePricelistOutput
    {
        public IEnumerable<SalePricelistVRFile> SalePricelistVRFiles { get; set; }
    }
    public sealed class PreparePricelistFile : BaseAsyncActivity<GetSalePricelistInput, GetSalePricelistOutput>
    {
        [RequiredArgument]
        public InArgument<SalePriceList> SalePriceList { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SalePricelistVRFile>> SalePricelistVRFiles { get; set; }

        protected override GetSalePricelistOutput DoWorkWithResult(GetSalePricelistInput inputArgument, AsyncActivityHandle handle)
        {
            var carrierAccountManager = new CarrierAccountManager();
            var customerPriceListTemplateId = carrierAccountManager.GetCustomerPriceListTemplateId(inputArgument.SalePriceList.OwnerId);
            var salePricelistManager = new SalePriceListManager();
            IEnumerable<SalePricelistVRFile> files = salePricelistManager.PreparePriceListVrFiles(inputArgument.SalePriceList, SalePriceListType.Full
                                                    , customerPriceListTemplateId, out _, RateChangeType.NotChanged);


            return new GetSalePricelistOutput
            {
                SalePricelistVRFiles = files
            };
        }

        protected override GetSalePricelistInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetSalePricelistInput
            {
                SalePriceList = this.SalePriceList.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetSalePricelistOutput result)
        {
            SalePricelistVRFiles.Set(context, result.SalePricelistVRFiles);
        }
    }
}
