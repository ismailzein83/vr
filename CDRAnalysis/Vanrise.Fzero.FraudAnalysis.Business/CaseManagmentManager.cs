using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;
using System.Linq;
using Vanrise.Security.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class CaseManagmentManager
    {

        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<AccountCase> SaveAccountCase(AccountCase accountCaseObject)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            accountCaseObject.UserID= Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            manager.SaveAccountCase(accountCaseObject);
            Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<AccountCase> updateOperationOutput = new Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<AccountCase>();

            updateOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationResult.Succeeded;
            updateOperationOutput.UpdatedObject = accountCaseObject;
            return updateOperationOutput;
        }


        //public Vanrise.Entities.IDataRetrievalResult<AccountCase> GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input, IEnumerable<UserInfo> users)
        //{
        //    //ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

        //    //BigResult<AccountCase> accountCases = manager.GetFilteredAccountCases(input);

        //    //foreach (var accountCase in accountCases.Data)
        //    //{
        //    //    UserInfo userinfo = users.Where(x => x.UserId == accountCase.UserID).FirstOrDefault();
        //    //    if (userinfo != null)
        //    //    {
        //    //        //accountCase.UserName = userinfo.Name;
        //    //    }
        //    //}


        //    //return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, accountCases);
        //}


    }
}
