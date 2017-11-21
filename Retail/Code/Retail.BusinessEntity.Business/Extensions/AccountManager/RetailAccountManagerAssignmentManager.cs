using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.AccountManager.Entities;
using Vanrise.AccountManager.Business;
using Vanrise.Rules;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class RetailAccountManagerAssignmentManager
    {
        #region Public Methods
        Vanrise.AccountManager.Business.AccountManagerAssignmentManager manager = new Vanrise.AccountManager.Business.AccountManagerAssignmentManager();
        public IDataRetrievalResult<AccountManagerAssignmentDetail> GetFilteredAccountManagerAssignments(DataRetrievalInput<AccountManagerAssignmentQuery> input)
        {
            var accountManagerAssignments = manager.GetAccountManagerAssignments(input.Query.AccountManagerAssignementDefinitionId);
            Func<AccountManagerAssignment, bool> filterExpression = (prod) =>
            {
                if (input.Query.AccountManagerId != null && !input.Query.AccountManagerId.Equals(prod.AccountManagerId))
                    return false;
                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, accountManagerAssignments.ToBigResult(input, filterExpression, AccountManagerDetailMapper));

        }
        public AccountManagerAssignmentRuntimeEditor GetAccountManagerAssignmentRuntimeEditor(AccountManagerAssignmentRuntimeInput accountManagerAssignmentRuntimeInput)
        {
            AccountManagerDefinitionManager accountManagerDefinitionManager = new AccountManagerDefinitionManager();
            AccountManagerAssignmentRuntimeEditor accountManagerAssignmentRuntime = new AccountManagerAssignmentRuntimeEditor();
            if (accountManagerAssignmentRuntimeInput.AccountManagerDefinitionId != null)
            {
                accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition = accountManagerDefinitionManager.GetAccountManagerAssignmentDefinition(accountManagerAssignmentRuntimeInput.AccountManagerDefinitionId, accountManagerAssignmentRuntimeInput.AssignmentDefinitionId);
            }
            if (accountManagerAssignmentRuntimeInput.AccountManagerAssignementId.HasValue)
            {
                var accountManagerAssignmentId = accountManagerAssignmentRuntimeInput.AccountManagerAssignementId.Value;
                accountManagerAssignmentRuntime.AccountManagerAssignment = manager.GetAccountManagerAssignment(accountManagerAssignmentId, accountManagerAssignmentRuntimeInput.AssignmentDefinitionId);
                accountManagerAssignmentRuntime.AccountName = accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition.Settings.GetAccountName(accountManagerAssignmentRuntime.AccountManagerAssignment.AccountId);
            }
            return accountManagerAssignmentRuntime;
        }
        public InsertOperationOutput<AccountManagerAssignmentDetail> AddAccountManagerAssignment(AssignAccountManagerToAccountsInput accountManagerAssignment)
        {
            string errorMessage;
            InsertOperationOutput<AccountManagerAssignmentDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountManagerAssignmentDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            bool insertActionSucc = manager.AssignAccountManagerToAccounts(accountManagerAssignment, out errorMessage);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            }
            else
            {
                insertOperationOutput.Message = errorMessage;
                insertOperationOutput.ShowExactMessage = true;
            }
            return insertOperationOutput;
        }
        public UpdateOperationOutput<AccountManagerAssignmentDetail> UpdateAccountManagerAssignment(UpdateAccountManagerAssignmentInput accountManagerAssignment)
        {
            string errorMessage;
            UpdateOperationOutput<AccountManagerAssignmentDetail> updateOperationOutput = new UpdateOperationOutput<AccountManagerAssignmentDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSucc = manager.UpdateAccountManagerAssignment(accountManagerAssignment, out errorMessage);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                var accountManagerAssignmentUpdated = manager.GetAccountManagerAssignment(accountManagerAssignment.AccountManagerAssignmentId, accountManagerAssignment.AccountManagerAssignmentDefinitionId);
                updateOperationOutput.UpdatedObject = AccountManagerDetailMapper(accountManagerAssignmentUpdated);
            }
            else
            {
                updateOperationOutput.Message = errorMessage;
                updateOperationOutput.ShowExactMessage = true;

            }


            return updateOperationOutput;
        }
        public Dictionary<Guid, AccountManagerDefInfo> GetAccountManagerDefsInfo()
        {
            AccountManagerDefinitionManager accountManagerDefinitionManager = new AccountManagerDefinitionManager();
            return accountManagerDefinitionManager.GetCachedOrCreate("GetCachedAccountManagerDefsInfo",
              () =>
              {
                  var accountManagerDefinitionSettings = accountManagerDefinitionManager.GetAccountManagerDefinitionSettings();
                  Dictionary<Guid, AccountManagerDefInfo> accountManagerDefsInfo = new Dictionary<Guid, AccountManagerDefInfo>();
                  foreach (var accountManagerDefinitionSetting in accountManagerDefinitionSettings)
                  {
                      if (accountManagerDefinitionSetting.Value.AssignmentDefinitions != null)
                      {
                          foreach (var accountManagerAssignment in accountManagerDefinitionSetting.Value.AssignmentDefinitions)
                          {
                              var accountManagerAssignmentSetings = accountManagerAssignment.Settings as RetailAccountAssignmentDefinition;
                              if (accountManagerAssignmentSetings != null)
                              {
                                  AccountManagerDefInfo accountManagerDefInfo = new AccountManagerDefInfo
                                  {
                                      AccountManagerDefinitionId = accountManagerDefinitionSetting.Key,
                                      AccountManagerDefinitionSettings = accountManagerDefinitionSetting.Value,
                                      AccountManagerAssignmentDefinition = accountManagerAssignment
                                  };
                                  if (!accountManagerDefsInfo.ContainsKey(accountManagerAssignmentSetings.AccountBEDefinitionId))
                                      accountManagerDefsInfo.Add(accountManagerAssignmentSetings.AccountBEDefinitionId, accountManagerDefInfo);
                              }
                          }
                      }

                  }
                  return accountManagerDefsInfo;
              });
          
        }
        public AccountManagerDefInfo GetAccountManagerDefInfoByAccountBeDefinitionId(Guid accountBeDefinitionId)
        {
            var accountManagerDefsInfo = GetAccountManagerDefsInfo();
            return accountManagerDefsInfo.GetRecord(accountBeDefinitionId);
        }
        #endregion

        #region Mappers
        private AccountManagerAssignmentDetail AccountManagerDetailMapper(AccountManagerAssignment accountManagerAssignment)
        {

            AccountManagerAssignmentDetail accountManagerAssignmentDetail = new AccountManagerAssignmentDetail
            {
                AccountManagerAssignementId = accountManagerAssignment.AccountManagerAssignementId,
                AccountManagerAssignementDefinitionId = accountManagerAssignment.AccountManagerAssignementDefinitionId,
                AccountId = accountManagerAssignment.AccountId,
                AccountManagerId = accountManagerAssignment.AccountManagerId,
                BED = accountManagerAssignment.BED,
                EED = accountManagerAssignment.EED
            };
            AccountManagerManager accountManagerManager = new AccountManagerManager();
            var accountManagerDefinitionId = accountManagerManager.GetAccountManagerDefinitionId(accountManagerAssignment.AccountManagerId);
            AccountManagerDefinitionManager accountManagerDefinitionManager = new AccountManagerDefinitionManager();
            var accountManagerDefinitionSetting = accountManagerDefinitionManager.GetAccountManagerAssignmentDefinition(accountManagerDefinitionId, accountManagerAssignment.AccountManagerAssignementDefinitionId);
            if (accountManagerDefinitionSetting != null)
            {
                accountManagerAssignmentDetail.AccountName = accountManagerDefinitionSetting.Settings.GetAccountName(accountManagerAssignment.AccountId);
            }
            return accountManagerAssignmentDetail;
        }
        #endregion

        #region Private Classes
     

        #endregion
    }
}
