using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.InvToAccBalanceRelation.Business;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.Invoice.Business;
namespace TOne.WhS.InvToAccBalanceRelation.Business
{
    public class CarrierInvToAccBalanceRelationDefinitionExtendedSettings : InvToAccBalanceRelationDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("F5CD8367-A6DC-421E-B93C-0567ED769150"); }
        }

        public override List<Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo> GetBalanceInvoiceAccounts(IInvToAccBalanceRelGetBalanceInvoiceAccountsContext context)
        {

            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.AccountId));
            financialAccount.ThrowIfNull("financialAccount", context.AccountId);

            WHSFinancialAccountDefinitionManager financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var financialAccountDefinitionSettings = financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            if (financialAccountDefinitionSettings.FinancialAccountInvoiceTypes != null)
            {
                List<Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo> invoiceAccountInfo = new List<Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo>();
                foreach (var financialAccountInvoiceType in financialAccountDefinitionSettings.FinancialAccountInvoiceTypes)
                {
                    if (!financialAccountInvoiceType.IgnoreFromBalance)
                    {
                        invoiceAccountInfo.Add(new InvoiceAccountInfo
                        {
                            InvoiceTypeId = financialAccountInvoiceType.InvoiceTypeId,
                            PartnerId = context.AccountId
                        });
                    }
                   
                }
                return invoiceAccountInfo;

            }
            else
            {
                return null;
            }
        }

        public override List<BalanceAccountInfo> GetInvoiceBalanceAccounts(IInvToAccBalanceRelGetInvoiceBalanceAccountsContext context)
        {
            throw new NotImplementedException();

        }

    }
}
