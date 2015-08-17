using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

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

    }
}
