using System.Activities;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{

    public sealed class PrepareCustomerSMSPriceListToInsert : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<CustomerSMSRateDraft> CustomerSMSRateDraft { get; set; }
        
        [RequiredArgument]
        public OutArgument<CustomerSMSPriceList> CustomerSMSPriceList { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            CustomerSMSRateDraft customerSMSRateDraft = this.CustomerSMSRateDraft.Get(context.ActivityContext);
            long processInstanceID = context.ActivityContext.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            int userID = context.ActivityContext.GetSharedInstanceData().InstanceInfo.InitiatorUserId;

            CustomerSMSPriceList CustomerSMSPriceList = new CustomerSMSPriceListManager().CreateCustomerSMSPriceList(customerSMSRateDraft.CustomerID, customerSMSRateDraft.CurrencyId, customerSMSRateDraft.EffectiveDate, processInstanceID, userID);

            this.CustomerSMSPriceList.Set(context.ActivityContext, CustomerSMSPriceList);
        }
    }
}
