using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;
using System.Linq;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class CaseManagmentManager
    {

        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<AccountCase> SaveAccountCase(AccountCase accountCaseObject)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            accountCaseObject.UserId= Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            manager.SaveAccountCase(accountCaseObject);
            Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<AccountCase> updateOperationOutput = new Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<AccountCase>();

            updateOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationResult.Succeeded;
            updateOperationOutput.UpdatedObject = accountCaseObject;
            return updateOperationOutput;
        }


        public Vanrise.Entities.IDataRetrievalResult<AccountCase> GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input, IEnumerable<User> users)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            BigResult<AccountCase> accountCases = manager.GetFilteredAccountCases(input);

            foreach (var accountCase in accountCases.Data)
            {
                User user =users.Where(x => x.UserId == accountCase.UserId).FirstOrDefault();
                if (user !=null)
                {
                    accountCase.UserName = user.Name;
                }
            }


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, accountCases);
        }


    }
}
