using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.AccountManager.Entities;
using Vanrise.AccountManager.Business;
using Vanrise.Rules;

namespace Retail.BusinessEntity.Business
{
   public class AccountManagerAssignmentManager
    {
       Vanrise.AccountManager.Business.AccountManagerAssignmentManager manager = new Vanrise.AccountManager.Business.AccountManagerAssignmentManager();
       public List<AccountManagerAssignmentDetail> GetAccountManagerAssignments()
       {
           var accountManagerAssignments = manager.GetAccountManagerAssignments();
           List<AccountManagerAssignmentDetail> accountManagerAssignmentDetails = new List<AccountManagerAssignmentDetail>();
           foreach (var accountManagerAssignment in accountManagerAssignments)
           {
               var accountManagerAssignmentDetail = AccountManagerDetailMapper(accountManagerAssignment);
               accountManagerAssignmentDetails.Add(accountManagerAssignmentDetail);
 
           }
           return accountManagerAssignmentDetails;

       }
       public Vanrise.Entities.InsertOperationOutput<AccountManagerAssignmentDetail> AddAccountManagerAssignment(AccountManagerAssignment accountManagerAssignment)
       {
           string errorMessage;
           int assignmentId;
           InsertOperationOutput<AccountManagerAssignmentDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountManagerAssignmentDetail>();
           bool isAccountManagerAssignmentAdded = manager.TryAddAccountManagerAssignment(accountManagerAssignment,out assignmentId,out errorMessage);
           if (isAccountManagerAssignmentAdded)
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
           bool isAccountManagerAssignmentUpdated = manager.TryUpdateAccountManagerAssignment(accountManagerAssignment, out errorMessage);
           if (isAccountManagerAssignmentUpdated)
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
