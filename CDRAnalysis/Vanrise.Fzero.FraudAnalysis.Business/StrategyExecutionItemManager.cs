﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyExecutionItemManager
    {
        public bool LinkDetailToCase(string accountNumber, int caseID, CaseStatus caseStatus)
        {
            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            return dataManager.LinkDetailToCase(accountNumber, caseID, caseStatus);
        }
        public BigResult<AccountSuspicionDetail> GetFilteredDetailsByCaseID(Vanrise.Entities.DataRetrievalInput<CaseDetailQuery> input)
        {
            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            return dataManager.GetFilteredDetailsByCaseID(input);
        }

        public void GetStrategyExecutionbyExecutionId(int ExecutionId, out List<StrategyExecutionItem> outAllStrategyExecutionItems, out List<AccountCase> outAccountCases, out List<StrategyExecutionItem> outStrategyExecutionItemRelatedtoCases)
        {
            IStrategyExecutionItemDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionItemDataManager>();
            dataManager.GetStrategyExecutionbyExecutionId(ExecutionId, out outAllStrategyExecutionItems, out outAccountCases, out outStrategyExecutionItemRelatedtoCases);
        }
    }
}
