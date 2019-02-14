using System.Activities;
using TOne.WhS.SMSBusinessEntity.BP.Arguments;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{
    public sealed class GetCustomerSMSRateDraftData : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<SMSSaleRateInput> SMSSaleRateInput { get; set; }

        [RequiredArgument]
        public OutArgument<CustomerSMSRateDraft> CustomerSMSRateDraft { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            long processDraftID = this.SMSSaleRateInput.Get(context.ActivityContext).ProcessDraftID;

            CustomerSMSRateDraft customerSMSRateDraft = new CustomerSMSRateDraftManager().GetCustomerSMSRateDraft(processDraftID);
            customerSMSRateDraft.ThrowIfNull("customerSMSRateDraft", processDraftID);

            this.CustomerSMSRateDraft.Set(context.ActivityContext, customerSMSRateDraft);
        }
    }
}