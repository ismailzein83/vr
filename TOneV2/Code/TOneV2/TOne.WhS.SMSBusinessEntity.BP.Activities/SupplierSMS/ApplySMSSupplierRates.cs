using System.Activities;
using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{
    public sealed class ApplySMSSupplierRates : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<Dictionary<int, SupplierSMSRateChange>> SupplierSMSRateChangesByMobileNetworkID { get; set; }

        [RequiredArgument]
        public InArgument<SupplierSMSPriceList> SupplierSMSPriceList { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            SupplierSMSPriceList supplierSMSPriceList = this.SupplierSMSPriceList.Get(context.ActivityContext);
            var supplierSMSRateChangesByMobileNetworkID = this.SupplierSMSRateChangesByMobileNetworkID.Get(context.ActivityContext);

            ISupplierSMSRateDataManager supplierSMSRateDataManager = SMSBEDataFactory.GetDataManager<ISupplierSMSRateDataManager>();

            bool isApplied = supplierSMSRateDataManager.ApplySupplierRates(supplierSMSPriceList, supplierSMSRateChangesByMobileNetworkID);
        }
    }
}