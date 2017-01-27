using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountBEStatusDefinitionFilter : IStatusDefinitionFilter
    {
        public Guid AccountBEDefinitionId { get; set; }
        public bool IsMatched(IStatusDefinitionFilterContext context)
        {
            var accountBEDefinitionSettings = new AccountBEDefinitionManager().GetAccountBEDefinitionSettings(this.AccountBEDefinitionId);
            if (accountBEDefinitionSettings.StatusBEDefinitionId != context.BusinessEntityDefinitionId)
                return false;
            return true;
        }
    }
}
