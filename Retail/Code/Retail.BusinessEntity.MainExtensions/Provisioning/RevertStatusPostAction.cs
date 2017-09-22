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
    public class RevertStatusPostAction : AccountProvisionPostAction
    {
        public override void ExecutePostAction(IAccountProvisionPostActionContext context)
        {

            var definitionSettings = context.DefinitionPostAction as RevertStatusDefinitionPostAction;
            definitionSettings.ThrowIfNull("definitionSettings");
            AccountBEManager accountBEManager = new AccountBEManager();
            var account = accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);
            account.ThrowIfNull("account", context.AccountId);
            var childAccounts = accountBEManager.GetChildAccounts(context.AccountBEDefinitionId, context.AccountId, true);
            HashSet<AccountDefinition> accountDefinitions = new HashSet<AccountDefinition>();
            if (account.StatusId == definitionSettings.RevertFromStatusDefinitionId)
            {
                accountDefinitions.Add(new AccountDefinition
                {
                    AccountBEDefinitionId = context.AccountBEDefinitionId,
                    AccountId = context.AccountId
                });
            }
            if (childAccounts != null)
            {
                foreach (var childAccount in childAccounts)
                {
                    if (childAccount.StatusId == definitionSettings.RevertFromStatusDefinitionId)
                    {
                        accountDefinitions.Add(new AccountDefinition
                        {
                            AccountBEDefinitionId = context.AccountBEDefinitionId,
                            AccountId = childAccount.AccountId
                        });
                    }
                }
            }

            var accountStatusHistoryList = new AccountStatusHistoryManager().GetAccountStatusHistoryListByAccountDefinition(accountDefinitions);
            if (accountStatusHistoryList != null)
            {
                var accountIdsToUpdateStatus = new List<UpdateAccountStatusInput>();
                foreach (var accountDefinition in accountDefinitions)
                {
                    var accountStatusHistoryOrderedList = accountStatusHistoryList.GetRecord(accountDefinition);
                    if (accountStatusHistoryOrderedList != null)
                    {
                        var accountStatusHistory = accountStatusHistoryOrderedList.FirstOrDefault();
                        if (accountStatusHistory != null && accountStatusHistory.StatusId == definitionSettings.RevertFromStatusDefinitionId)
                        {
                            if (accountStatusHistory.PreviousStatusId.HasValue)
                            {
                                accountIdsToUpdateStatus.Add(new UpdateAccountStatusInput
                                {
                                    StatusId = accountStatusHistory.PreviousStatusId.Value,
                                    AccountId = accountDefinition.AccountId
                                });
                            }
                        }
                    }
                }
                accountBEManager.UpdateStatuses(context.AccountBEDefinitionId, accountIdsToUpdateStatus);
            }
        }
    }
}
