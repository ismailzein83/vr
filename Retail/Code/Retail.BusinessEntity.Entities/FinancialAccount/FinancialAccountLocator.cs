using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class FinancialAccountLocator
    {
        public abstract Guid ConfigId { get; }

        public abstract bool TryGetFinancialAccountId(IFinancialAccountLocatorContext context);
    }

    public interface IFinancialAccountLocatorContext
    {
        Guid AccountDefinitionId { get; }

        long AccountId { get; }

        DateTime EffectiveOn { get; }

        long FinancialAccountId { set; }

        string BalanceAccountId { set; }

        Guid BalanceAccountTypeId { set; }
        string Classification { get; }
    }
}
