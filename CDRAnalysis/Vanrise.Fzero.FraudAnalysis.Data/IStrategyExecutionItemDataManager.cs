using System;
using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyExecutionItemDataManager : IDataManager, IBulkApplyDataManager<StrategyExecutionItem>
    {
        bool LinkItemToCase(string accountNumber, int accountCaseId, CaseStatus caseStatus);

        BigResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input);

        void GetStrategyExecutionItemsbyExecutionId(long ExecutionId, out List<StrategyExecutionItem> outAllStrategyExecutionItems, out List<AccountCase> outAccountCases, out List<StrategyExecutionItem> outStrategyExecutionItemRelatedtoCases);

        bool CancelStrategyExecutionItemBatch(List<long> ItemIds, int userId, SuspicionOccuranceStatus status);

        bool UpdateStrategyExecutionItemBatch(Dictionary<string, int> accountNumberCaseIds);

        void ApplyStrategyExecutionItemsToDB(object preparedStrategyExecutionItems);

        void LoadStrategyExecutionItemSummaries(Action<StrategyExecutionItemSummary> onBatchReady);

        void GetStrategyExecutionItemsbyNULLCaseId(out List<AccountInfo> outItems, out List<AccountCase> outCases, out List<AccountInfo> outInfos);
    }
}
