using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities.ResultQuery;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAccountCaseHistoryDataManager : IDataManager
    {
        bool InsertAccountCaseHistory(int caseID, int? userID, CaseStatus caseStatus, string reason);

        void SavetoDB(List<AccountCaseHistory> records);

        BigResult<AccountCaseLog> GetFilteredAccountCaseHistoryByCaseID(Vanrise.Entities.DataRetrievalInput<AccountCaseLogQuery> input);
    }
}
