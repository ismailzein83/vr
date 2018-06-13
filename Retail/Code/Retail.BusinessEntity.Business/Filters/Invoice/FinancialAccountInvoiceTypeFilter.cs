using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountInvoiceTypeFilter : IFinancialAccountFilter,IAccountFilter
    {
        public Guid InvoiceTypeId { get; set; }
        public bool IsMatched(IFinancialAccountFilterContext context)
        {
            FinancialAccountManager manager = new FinancialAccountManager();
            var financialAccount = manager.GetFinancialAccountData(context.AccountBEDefinitionId, context.FinancialAccountId);
            financialAccount.ThrowIfNull("financialAccount", context.FinancialAccountId);
            if(financialAccount.InvoiceTypeIds == null || !financialAccount.InvoiceTypeIds.Contains(InvoiceTypeId))
                return false;
            return true;
        }

        public bool IsExcluded(IAccountFilterContext context)
        {
            return false;
        }
    }
    public class FinancialAccountBalanceTypeFilter : IFinancialAccountFilter, IAccountFilter
    {
        public Guid BalanceAccountTypeId { get; set; }
        public bool IsMatched(IFinancialAccountFilterContext context)
        {
            FinancialAccountManager manager = new FinancialAccountManager();
            var financialAccount = manager.GetFinancialAccountData(context.AccountBEDefinitionId, context.FinancialAccountId);
            financialAccount.ThrowIfNull("financialAccount", context.FinancialAccountId);
            if (!financialAccount.BalanceAccountTypeId.HasValue || BalanceAccountTypeId != financialAccount.BalanceAccountTypeId.Value)
                return false;
            return true;
        }

        public bool IsExcluded(IAccountFilterContext context)
        {
            return false;
        }
    }

}
