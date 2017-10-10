using System;
using Vanrise.Invoice.Entities;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CustomerGenerationCustomSection : GenerationCustomSection
    {
        public override string GenerationCustomSectionDirective
        {
            get
            {
                return "whs-invoicetype-generationcustomsection-customer";
            }

        }

        public override dynamic GetGenerationCustomPayload(IGetGenerationCustomPayloadContext context)
        {
            WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();

            var financialAccount = whsFinancialAccountManager.GetFinancialAccount(int.Parse(context.PartnerId));
            int timeZoneId;

            if (financialAccount.CarrierAccountId.HasValue)
            {
                timeZoneId = new CarrierAccountManager().GetCustomerTimeZoneId(financialAccount.CarrierAccountId.Value);
            }
            else
            {
                timeZoneId = new CarrierProfileManager().GetCustomerTimeZoneId(financialAccount.CarrierProfileId.Value);
            }

            return new CustomerGenerationCustomSectionPayload() { TimeZoneId = timeZoneId };
        }
    }
}
