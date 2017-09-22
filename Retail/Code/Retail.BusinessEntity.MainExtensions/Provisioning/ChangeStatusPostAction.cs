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
            if (definitionSettings.ExistingStatusDefinitionIds == null || definitionSettings.ExistingStatusDefinitionIds.Contains(account.StatusId))
            {
                var childAccounts = accountBEManager.GetChildAccounts(account, true);
                accountBEManager.UpdateStatus(context.AccountBEDefinitionId, context.AccountId, definitionSettings.NewStatusDefinitionId);
                if (childAccounts != null)
                {
                    foreach (var childAccount in childAccounts)
                    {
                        if (definitionSettings.ExistingStatusDefinitionIds == null || definitionSettings.ExistingStatusDefinitionIds.Contains(childAccount.StatusId))
                        {
                            accountBEManager.UpdateStatus(context.AccountBEDefinitionId, childAccount.AccountId, definitionSettings.NewStatusDefinitionId);
                        }
                    }
                }
              
            }
        }
    }
}
