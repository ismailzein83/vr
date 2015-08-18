using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface ICaseManagementDataManager : IDataManager 
    {
        bool SaveAccountCase(AccountCase accountCaseObject);
        BigResult<AccountCase> GetFilteredAccountCases(Vanrise.Entities.DataRetrievalInput<AccountCaseResultQuery> input);
    }
}
