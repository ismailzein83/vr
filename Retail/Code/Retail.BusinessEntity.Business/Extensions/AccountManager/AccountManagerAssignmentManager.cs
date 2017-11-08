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
   public class AccountManagerAssignmentManager
    {
       Vanrise.AccountManager.Business.AccountManagerAssignmentManager manager = new Vanrise.AccountManager.Business.AccountManagerAssignmentManager();
       public Vanrise.Entities.IDataRetrievalResult<AccountManagerAssignmentDetail> GetFilteredAccountManagerAssignments(Vanrise.Entities.DataRetrievalInput<AccountManagerAssignmentQuery> input)
       {
           var accountManagerAssignments = manager.GetAccountManagerAssignments();
           Func<AccountManagerAssignment, bool> filterExpression = (prod) =>
             (input.Query.AccountManagerId == null || prod.AccountManagerId.Equals(input.Query.AccountManagerId));
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, accountManagerAssignments.ToBigResult(input, filterExpression, AccountManagerDetailMapper));

       }
       public AccountManagerAssignmentRuntimeEditor GetAccountManagerAssignmentRuntimeEditor(AccountManagerAssignmentRuntimeInput accountManagerAssignmentRuntimeInput)
       {
           AccountManagerDefinitionManager accountManagerDefinitionManager = new AccountManagerDefinitionManager();
           AccountManagerAssignmentRuntimeEditor accountManagerAssignmentRuntime = new AccountManagerAssignmentRuntimeEditor();
           var accountManagerDefinitionSettings = accountManagerDefinitionManager.GetAccountManagerDefinitionSettings(accountManagerAssignmentRuntimeInput.AccountManagerDefinitionId);
           var assignmentDefinitions = accountManagerDefinitionSettings.AssignmentDefinitions;
           accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition = accountManagerDefinitionManager.GetAccountManagerAssignmentDefinition(accountManagerAssignmentRuntimeInput.AccountManagerDefinitionId, accountManagerAssignmentRuntimeInput.AssignmentDefinitionId);
           if (accountManagerAssignmentRuntimeInput.AccountManagerAssignementId != null)
           {
               var accountManagerAssignmentId = accountManagerAssignmentRuntimeInput.AccountManagerAssignementId.Value;
               accountManagerAssignmentRuntime.AccountManagerAssignment = manager.GetAccountManagerAssignment(accountManagerAssignmentId);
           }
           return accountManagerAssignmentRuntime;
       }
           public Vanrise.Entities.InsertOperationOutput<AccountManagerAssignmentDetail> AddAccountManagerAssignment(AssignAccountManagerToAccountsInput accountManagerAssignment)
       {
           string errorMessage;
           InsertOperationOutput<AccountManagerAssignmentDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountManagerAssignmentDetail>();
           insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
           insertOperationOutput.InsertedObject = null;
           bool insertActionSucc = manager.AssignAccountManagerToAccounts(accountManagerAssignment,out errorMessage);
           return insertOperationOutput;
   }
       public Vanrise.Entities.UpdateOperationOutput<AccountManagerAssignmentDetail> UpdateAccountManagerAssignment(UpdateAccountManagerAssignmentInput accountManagerAssignment)
       {
           string errorMessage;
           UpdateOperationOutput<AccountManagerAssignmentDetail> updateOperationOutput = new UpdateOperationOutput<AccountManagerAssignmentDetail>();
           updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
           updateOperationOutput.UpdatedObject = null;
           manager.UpdateAccountManagerAssignment(accountManagerAssignment, out errorMessage);
           return updateOperationOutput;
       }
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
    }
}
