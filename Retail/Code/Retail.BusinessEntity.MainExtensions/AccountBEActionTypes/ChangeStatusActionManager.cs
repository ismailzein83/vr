using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class ChangeStatusActionManager
    {
        AccountBEManager s_accountBEManager = new AccountBEManager();
        AccountBEDefinitionManager s_accountBEDefinitionManager = new AccountBEDefinitionManager();


        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> ChangeAccountStatus(Guid accountBEDefinitionId, long accountId, Guid actionDefinitionId, DateTime statusChangedDate)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var actionDefinition = s_accountBEDefinitionManager.GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId);
            actionDefinition.ThrowIfNull("actionDefinition",actionDefinitionId);

            var actionDefinitionSettings = actionDefinition.ActionDefinitionSettings as ChangeStatusActionSettings;
            actionDefinitionSettings.ThrowIfNull("actionDefinitionSettings");

            List<Account> accounts = new List<Account>();
            if (s_accountBEManager.EvaluateAccountCondition(accountBEDefinitionId, accountId, actionDefinition.AvailabilityCondition))
            {
                var account = s_accountBEManager.GetAccount(accountBEDefinitionId, accountId);
                accounts.Add(account);
            }
            if (actionDefinitionSettings.ApplyToChildren)
            {
                var childAccounts = s_accountBEManager.GetChildAccounts(accountBEDefinitionId, accountId, true);
                if (childAccounts != null)
                {
                    foreach (var childAccount in childAccounts)
                    {
                        if (s_accountBEManager.EvaluateAccountCondition(childAccount, actionDefinition.AvailabilityCondition))
                        {
                            accounts.Add(childAccount);
                        }
                    }
                }
            }
            string errorMessage;
            s_accountBEManager.UpdateStatuses(accountBEDefinitionId, accounts, actionDefinitionSettings.StatusId, statusChangedDate, actionDefinitionSettings.AllowOverlapping, actionDefinitionSettings.ApplicableOnStatuses,out errorMessage, true, actionDefinition.Name);
            if (errorMessage != null )
            {
                updateOperationOutput.Message = errorMessage;
                updateOperationOutput.ShowExactMessage = true;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = s_accountBEManager.GetAccountDetail(accountBEDefinitionId, accountId);
            }
            return updateOperationOutput;
        }
    }
}
