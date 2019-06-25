using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;

namespace Retail.Billing.Business
{
    public class BillingInvoiceSettings : GenericInvoiceSettings
    {
        public override Guid ConfigId { get { return new Guid("1535DBF0-01E9-4E8B-9566-A278F357CC80"); } }
        public Guid? VatRuleDefinitionId { get; set; }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            return null;
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            GenericFinancialAccountManager financialAccountManager = new GenericFinancialAccountManager(this.Configuration);
            var financialAccount = financialAccountManager.GetFinancialAccount(context.PartnerId);
            context.PartnerCreationDate = financialAccount.BED.Value;
        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new BillingInvoiceGenerator(this.Configuration, this.InvoiceTransactionTypeId, this.UsageToOverrideTransactionTypeIds, this.VatRuleDefinitionId);
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            switch (context.PartnerRetrievalType)
            {
                case PartnerRetrievalType.GetActive:
                case PartnerRetrievalType.GetAll:
                    GenericFinancialAccountManager financialAccountManager = new GenericFinancialAccountManager(this.Configuration);
                    var financialAccounts = financialAccountManager.GetFinancialAccounts();
                    if (financialAccounts == null)
                        return null;
                    return financialAccounts.Select(x => x.FinancialAccountId.ToString());
                default:
                    return null;
            }
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return new BillingInvoicePartnerSettings(this.Configuration);
        }
    }
}
