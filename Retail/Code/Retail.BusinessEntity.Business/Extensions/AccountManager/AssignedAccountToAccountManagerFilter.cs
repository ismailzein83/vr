using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Business;

namespace Retail.BusinessEntity.Business
{
    public class AssignedAccountToAccountManagerFilter : IAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            RetailAccountManagerAssignmentManager retailAccountManagerAssignmnetManager = new RetailAccountManagerAssignmentManager();
            AccountManagerAssignmentManager accountManagerAssignmnetManager = new AccountManagerAssignmentManager();

            var accountManagerDefInfo = retailAccountManagerAssignmnetManager.GetAccountManagerDefInfoByAccountBeDefinitionId(context.AccountBEDefinitionId);
            if (accountManagerDefInfo == null || accountManagerDefInfo.AccountManagerAssignmentDefinition == null)
                throw new Exception("Account Manager Definition Info is Null");

            var assignmnetDefinitionId = accountManagerDefInfo.AccountManagerAssignmentDefinition.AccountManagerAssignementDefinitionId;
            if (accountManagerAssignmnetManager.AreAccountAssignableToAccountManager(assignmnetDefinitionId, context.Account.AccountId.ToString()))
                return false;
            return true;

        }
    }
}
