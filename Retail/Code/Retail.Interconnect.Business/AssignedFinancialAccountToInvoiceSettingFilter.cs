using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business.InvoicePartnerFilter;
using Retail.BusinessEntity.Business;
using Vanrise.Invoice.Business;
using Vanrise.Common;

namespace Retail.Interconnect.Business
{
    public class AssignedFinancialAccountToInvoiceSettingFilter : PartnerInvoiceSettingFilter, IAccountFilter, IFinancialAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            FinancialAccountManager manager = new FinancialAccountManager();
            var invoiceTypeExtendedSettings = invoiceTypeManager.GetInvoiceTypeExtendedSettings(base.InvoiceTypeId);
            var interconnectInvoiceSettings = invoiceTypeExtendedSettings as InterconnectInvoiceSettings;
            if (interconnectInvoiceSettings != null)
            {
                string classification;
                if (interconnectInvoiceSettings.Type == InterconnectInvoiceType.Customer)
                    classification = "Customer";
                else
                    classification = "Supplier";
                var financialAccounts = manager.GetFinancialAccounts(context.AccountBEDefinitionId, context.Account.AccountId, false, classification);
                if (financialAccounts != null && financialAccounts.All(x => !base.IsMatched(x.FinancialAccountId)))
                    return true;
                return false;
            }

            var interconnectSettlementInvoiceSettings = invoiceTypeExtendedSettings as InterconnectSettlementInvoiceSettings;
            if (interconnectSettlementInvoiceSettings != null)
            {
                var customerFinancialAccounts = manager.GetFinancialAccounts(context.AccountBEDefinitionId, context.Account.AccountId, false, "Customer");
                if (customerFinancialAccounts != null && customerFinancialAccounts.All(x => !base.IsMatched(x.FinancialAccountId)))
                    return true;
                var supplierFinancialAccounts = manager.GetFinancialAccounts(context.AccountBEDefinitionId, context.Account.AccountId, false, "Supplier");
                if (supplierFinancialAccounts != null && supplierFinancialAccounts.All(x => !base.IsMatched(x.FinancialAccountId)))
                    return true;
                return false;
            }
            return false;
        }

        public bool IsMatched(IFinancialAccountFilterContext context)
        {
            var financialAccountMananger = new FinancialAccountManager();
            var financialAccountData = financialAccountMananger.GetFinancialAccountData(context.AccountBEDefinitionId, context.FinancialAccountId);
            financialAccountData.ThrowIfNull("financialAccountData", string.Format("AccountBEDefinitionId:{0}, FinancialAccountId:{1}", context.AccountBEDefinitionId, context.FinancialAccountId));
            if (financialAccountData.InvoiceTypeIds == null || !financialAccountData.InvoiceTypeIds.Contains(base.InvoiceTypeId))
                return false;   
            if (this.EditablePartnerId != null && context.FinancialAccountId == this.EditablePartnerId)
                return true;
            if (!base.IsMatched(context.FinancialAccountId))
                return false;
            return true;
        }
    }
}
