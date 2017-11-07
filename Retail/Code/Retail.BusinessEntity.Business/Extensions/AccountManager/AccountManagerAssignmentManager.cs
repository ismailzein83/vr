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
           Func<AccountManagerAssignment, bool> filterExpression = null;
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, accountManagerAssignments.ToBigResult(input, filterExpression, AccountManagerDetailMapper));

       }
       public AccountManagerAssignmentRuntime GetAccountManagerAssignmentRuntimeEditor(AccountManagerAssignmentRuntimeInput accountManagerAssignmentRuntimeInput)
       {
           Vanrise.AccountManager.Business.AccountManagerDefinitionManager definitionManager = new Vanrise.AccountManager.Business.AccountManagerDefinitionManager();
           AccountManagerAssignmentRuntime accountManagerAssignmentRuntime = new AccountManagerAssignmentRuntime();
           var accountManagerDefinitionSettings = definitionManager.GetAccountManagerDefinitionSettings(accountManagerAssignmentRuntimeInput.AccountManagerDefinitionId);
           var assignmentDefinitions = accountManagerDefinitionSettings.AssignmentDefinitions;
           foreach (var assignmentDefinition in assignmentDefinitions)
           {

               if (assignmentDefinition.AccountManagerAssignementDefinitionId == accountManagerAssignmentRuntimeInput.AssignmentDefinitionId)
               {
                   accountManagerAssignmentRuntime.AccountManagrAssignmentDefinition = assignmentDefinition;
               }
           }
           if (accountManagerAssignmentRuntimeInput.AccountManagerAssignementId != null)
           {
               var accountManagerAssignmentId = accountManagerAssignmentRuntimeInput.AccountManagerAssignementId.Value;
               accountManagerAssignmentRuntime.AccountManagerAssignment = manager.GetAccountManagerAssignment(accountManagerAssignmentId);
           }
           return accountManagerAssignmentRuntime;
       }
       public Vanrise.Entities.InsertOperationOutput<AccountManagerAssignmentDetail> AddAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment)
       {
           string errorMessage;
           int assignmentId;
           InsertOperationOutput<AccountManagerAssignmentDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountManagerAssignmentDetail>();
           bool insertActionSucc = manager.TryAddAccountManagerAssignment(accountManagerAssignment, out assignmentId, out errorMessage);
           if (insertActionSucc)
           {
               accountManagerAssignment.AccountManagerAssignementId = assignmentId;
               insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
               insertOperationOutput.InsertedObject = null;
               insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
               insertOperationOutput.InsertedObject = AccountManagerDetailMapper(accountManagerAssignment);
           }
           else
           {
               insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
           }
           return insertOperationOutput;
       }
       public Vanrise.Entities.UpdateOperationOutput<AccountManagerAssignmentDetail> UpdateAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment)
       {
           string errorMessage;
           UpdateOperationOutput<AccountManagerAssignmentDetail> updateOperationOutput = new UpdateOperationOutput<AccountManagerAssignmentDetail>();
           bool updateActionSucc = manager.TryUpdateAccountManagerAssignment(accountManagerAssignment, out errorMessage);
           if (updateActionSucc)
           {
               updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
               updateOperationOutput.UpdatedObject = AccountManagerDetailMapper(accountManagerAssignment);
           }
           else
           {
               updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
           }
           return updateOperationOutput;
       }
       private AccountManagerAssignmentDetail AccountManagerDetailMapper(Vanrise.AccountManager.Entities.AccountManagerAssignment accountManagerAssignment)
       {
           return new AccountManagerAssignmentDetail()
           {
               AccountManagerAssignementId = accountManagerAssignment.AccountManagerAssignementId,
               AccountManagerAssignementDefinitionId = accountManagerAssignment.AccountManagerAssignementDefinitionId,
               BED = accountManagerAssignment.BED,
               EED = accountManagerAssignment.EED
           };

       }
    }
}
