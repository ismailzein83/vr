using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.BusinessEntity.MainExtensions
{
    public class ChangeStatusPostAction : AccountProvisionPostAction
    {
        public override void ExecutePostAction(IAccountProvisionPostActionContext context)
        {
            var definitionSettings = context.DefinitionPostAction as ChangeStatusDefinitionPostAction;
            definitionSettings.ThrowIfNull("definitionSettings");
            AccountBEManager accountBEManager = new AccountBEManager();
            var account = accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);
            account.ThrowIfNull("account", context.AccountId);
            var childAccounts = accountBEManager.GetChildAccounts(account, true);
            List<long> accountIdsToUpdateStatus = new List<long>();
            if (definitionSettings.ExistingStatusDefinitionIds == null || definitionSettings.ExistingStatusDefinitionIds.Contains(account.StatusId))
            {
                accountIdsToUpdateStatus.Add(context.AccountId);
            }
            if (childAccounts != null)
            {
                foreach (var childAccount in childAccounts)
                {
                    if (definitionSettings.ExistingStatusDefinitionIds == null || definitionSettings.ExistingStatusDefinitionIds.Contains(childAccount.StatusId))
                    {
                        accountIdsToUpdateStatus.Add(childAccount.AccountId);
                    }
                }
            }
            string errorMessage;
            accountBEManager.UpdateStatuses(context.AccountBEDefinitionId, accountIdsToUpdateStatus, definitionSettings.NewStatusDefinitionId, out errorMessage);
        }
    }
}
