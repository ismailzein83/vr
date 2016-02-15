using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyExecutionItemManager
    {
        public bool LinkItemToCase(string accountNumber, int accountCaseID, CaseStatus caseStatus)
        {
            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            return dataManager.LinkItemToCase(accountNumber, accountCaseID, caseStatus);
        }
        public BigResult<AccountSuspicionDetail> GetFilteredItemsByCaseId(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input)
        {
            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            return dataManager.GetFilteredDetailsByCaseID(input);
        }
       
    }
}
