using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities.ResultQuery;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AccountCaseHistoryManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AccountCaseHistory> GetFilteredAccountCaseHistoryByCaseID(Vanrise.Entities.DataRetrievalInput<AccountCaseHistoryQuery> input)
        {
            IAccountCaseHistoryDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountCaseHistoryDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredAccountCaseHistoryByCaseID(input));
        }

        public bool InsertAccountCaseHistory(int caseID, int? userID, CaseStatus caseStatus, string reason)
        {
            IAccountCaseHistoryDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAccountCaseHistoryDataManager>();
            return dataManager.InsertAccountCaseHistory(caseID,userID,caseStatus,reason);
        }
    }
}
