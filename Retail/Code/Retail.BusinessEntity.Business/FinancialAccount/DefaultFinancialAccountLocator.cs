using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class DefaultFinancialAccountLocator : FinancialAccountLocator
    {
        public override Guid ConfigId { get { return new Guid("63E3987B-302B-42BA-8E61-A7762FA7BFD3"); } }

        public override bool TryGetFinancialAccountId(IFinancialAccountLocatorContext context)
        {
            long? financialAccountId = new AccountBEManager().GetFinancialAccountId(context.AccountDefinitionId, context.AccountId);
            if (!financialAccountId.HasValue)
                return false;

            context.FinancialAccountId = financialAccountId.Value;
            context.BalanceAccountId = financialAccountId.Value.ToString();
            context.BalanceAccountTypeId = new AccountBalanceManager().GetAccountBalanceTypeId(context.AccountDefinitionId);
            return true;
        }
    }
}
