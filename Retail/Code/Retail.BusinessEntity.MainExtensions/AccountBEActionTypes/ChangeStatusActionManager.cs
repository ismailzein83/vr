using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class ChangeStatusActionManager
    {
        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> ChangeAccountStatus(Guid accountBEDefinitionId, long accountId, Guid actionDefinitionId)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            var accountBEManager = new AccountBEManager();
            var actionDefinition = new AccountBEDefinitionManager().GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId);
            actionDefinition.ThrowIfNull("actionDefinition",actionDefinitionId);

            var actionDefinitionSettings = actionDefinition.ActionDefinitionSettings as ChangeStatusActionSettings;
            actionDefinitionSettings.ThrowIfNull("actionDefinitionSettings");
            if (accountBEManager.EvaluateAccountCondition(accountBEDefinitionId,accountId,actionDefinition.AvailabilityCondition))
            {
                if (accountBEManager.UpdateStatus(accountBEDefinitionId, accountId, actionDefinitionSettings.StatusId))
                {
                    long accountStatusHistoryId;
                    new AccountStatusHistoryManager().TryAddAccountStatusHistory(new AccountStatusHistory
                    {
                        AccountId = accountId,
                        StatusChangedDate = DateTime.Now,
                        StatusId = actionDefinitionSettings.StatusId

                    }, out accountStatusHistoryId);
                    FinancialAccountManager financialAccountManager = new FinancialAccountManager();
                    financialAccountManager.UpdateAccountStatus(accountBEDefinitionId, accountId);
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = accountBEManager.GetAccountDetail(accountBEDefinitionId, accountId);
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
            }
            return updateOperationOutput;
        }

    }
}
