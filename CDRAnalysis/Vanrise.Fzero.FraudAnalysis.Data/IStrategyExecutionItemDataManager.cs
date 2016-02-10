using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyExecutionItemDataManager:IDataManager
    {
        bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus);

        BigResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input);

        void GetStrategyExecutionItemsbyExecutionId(long ExecutionId, out List<StrategyExecutionItem> outAllStrategyExecutionItems, out List<AccountCase> outAccountCases, out List<StrategyExecutionItem> outStrategyExecutionItemRelatedtoCases);

        bool UpdateStrategyExecutionItemBatch(List<long> ItemIds, int userId, SuspicionOccuranceStatus status);

    }
}
