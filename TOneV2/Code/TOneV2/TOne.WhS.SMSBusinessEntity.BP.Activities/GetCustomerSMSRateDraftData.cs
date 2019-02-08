using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.BP.Arguments;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{
    public sealed class GetCustomerSMSRateDraftData : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<SMSSaleRateInput> SMSSaleRateInput { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<int, CustomerSMSRateChange>> CustomerSMSRateChangesByMobileNetworkID { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> EffectiveDate { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            int customerID = this.SMSSaleRateInput.Get(context.ActivityContext).CustomerID;
            CustomerSMSRateDraft customerSMSRateDraft = new CustomerSMSRateDraftManager().GetCustomerSMSRateDraft(customerID);
            if (customerSMSRateDraft != null && customerSMSRateDraft.SMSRates != null && customerSMSRateDraft.SMSRates.Count != 0)
            {
                this.CustomerSMSRateChangesByMobileNetworkID.Set(context.ActivityContext, customerSMSRateDraft);
                this.EffectiveDate.Set(context.ActivityContext, customerSMSRateDraft.EffectiveDate);
            }
        }
    }
}