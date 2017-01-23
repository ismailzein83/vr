using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountAccountTypeFilter : IAccountTypeFilter
    {
        public Guid AccountBEDefinitionId { get; set; }

        public bool IsMatched(IAccountTypeFilterContext context)
        {
            if (context != null && context.AccountType != null && context.AccountType.AccountBEDefinitionId != this.AccountBEDefinitionId)
                    return false;
            return true;
        }
    }
}
