using System;
using System.Activities;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Security.Business;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{

    public sealed class PrepareCustomerSMSPriceListToInsert : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CustomerID { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<CustomerSMSPriceList> CustomerSMSPriceList { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            int customerID = this.CustomerID.Get(context.ActivityContext);
            DateTime effectiveDate = this.EffectiveDate.Get(context.ActivityContext);

            Currency currency = new CurrencyManager().GetCurrencyByCarrierId(customerID.ToString());

            CustomerSMSPriceList CustomerSMSPriceList = new CustomerSMSPriceList()
            {
                CustomerID = customerID,
                CurrencyID = currency.CurrencyID,
                EffectiveOn = effectiveDate,
                UserID = SecurityContext.Current.GetLoggedInUserId(),
                ProcessInstanceID = null,
            };

            this.CustomerSMSPriceList.Set(context.ActivityContext, CustomerSMSPriceList);
        }
    }
}
