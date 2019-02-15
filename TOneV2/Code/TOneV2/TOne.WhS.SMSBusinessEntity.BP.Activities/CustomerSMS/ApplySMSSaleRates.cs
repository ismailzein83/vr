using System.Activities;
using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{
    public sealed class ApplySMSSaleRates : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<Dictionary<int, CustomerSMSRateChange>> CustomerSMSRateChangesByMobileNetworkID { get; set; }

        [RequiredArgument]
        public InArgument<CustomerSMSPriceList> CustomerSMSPriceList { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            CustomerSMSPriceList customerSMSPriceList = this.CustomerSMSPriceList.Get(context.ActivityContext);
            var customerSMSRateChangesByMobileNetworkID = this.CustomerSMSRateChangesByMobileNetworkID.Get(context.ActivityContext);

            ICustomerSMSRateDataManager customerSMSRateDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSRateDataManager>();

            bool isApplied = customerSMSRateDataManager.ApplySaleRates(customerSMSPriceList, customerSMSRateChangesByMobileNetworkID);
        }
    }
}