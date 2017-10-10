using System;
using Vanrise.Invoice.Entities;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SupplierGenerationCustomSection : GenerationCustomSection
    {
        public override string GenerationCustomSectionDirective
        {
            get
            {
                return "whs-invoicetype-generationcustomsection-supplier";
            }
        }

        public override dynamic GetGenerationCustomPayload(IGetGenerationCustomPayloadContext context)
        {
            WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();

            var financialAccount = whsFinancialAccountManager.GetFinancialAccount(int.Parse(context.PartnerId));
            int timeZoneId;

            if (financialAccount.CarrierAccountId.HasValue)
            {
                timeZoneId = new CarrierAccountManager().GetSupplierTimeZoneId(financialAccount.CarrierAccountId.Value);
            }
            else
            {
                timeZoneId = new CarrierProfileManager().GetSupplierTimeZoneId(financialAccount.CarrierProfileId.Value);
            }

            return new CustomerGenerationCustomSectionPayload() { TimeZoneId = timeZoneId };
        }
    }
}
