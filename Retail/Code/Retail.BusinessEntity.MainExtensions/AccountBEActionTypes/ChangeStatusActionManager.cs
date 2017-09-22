﻿using Retail.BusinessEntity.Business;
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
            if (accountBEManager.EvaluateAccountCondition(accountBEDefinitionId, accountId, actionDefinition.AvailabilityCondition))
            {
                accountBEManager.UpdateStatuses(accountBEDefinitionId, new List<long> { accountId }, actionDefinitionSettings.StatusId, actionDefinition.Name);
                FinancialAccountManager financialAccountManager = new FinancialAccountManager();
                financialAccountManager.UpdateAccountStatus(accountBEDefinitionId, accountId);
                bool isSucceeded = true;
                if (actionDefinitionSettings.ApplyToChildren)
                {
                    if (!AppyChangeStatusToChilds(actionDefinitionSettings, actionDefinition.AvailabilityCondition, accountBEDefinitionId, accountId, actionDefinition.Name))
                    {
                        isSucceeded = false;
                    };
                }
                if (isSucceeded)
                {
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
        private bool AppyChangeStatusToChilds(ChangeStatusActionSettings actionDefinitionSettings,AccountCondition actionCondition, Guid accountBEDefinitionId, long accountId,string actionName)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            AccountBEManager accountBEManager = new AccountBEManager();
            var childAccouts = accountBEManager.GetChildAccounts(accountBEDefinitionId, accountId,true);
            if(childAccouts != null)
            {
                foreach(var childAccout in childAccouts)
                {
                    if (accountBEManager.EvaluateAccountCondition(childAccout, actionCondition))
                    {
                        accountBEManager.UpdateStatuses(accountBEDefinitionId, new List<long> { childAccout.AccountId }, actionDefinitionSettings.StatusId, actionName);
                        financialAccountManager.UpdateAccountStatus(accountBEDefinitionId, childAccout.AccountId);
                    }
                }
            }
            return true;
        }

    }
}
