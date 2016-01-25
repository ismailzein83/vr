using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyExecutionManager
    {
        public InsertOperationOutput<StrategyExecution> ExecuteStrategy(StrategyExecution strategyExecutionObject)
        {
            InsertOperationOutput<StrategyExecution> insertOperationOutput = new InsertOperationOutput<StrategyExecution>();

            int strategyExecutionId = -1;

            IStrategyExecutionDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            manager.ExecuteStrategy(strategyExecutionObject, out strategyExecutionId);

            strategyExecutionObject.ID = strategyExecutionId;
            insertOperationOutput.InsertedObject = strategyExecutionObject;

            return insertOperationOutput;
        }

        public void OverrideStrategyExecution(List<int> strategyIDs, DateTime from, DateTime to)
        {
            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            ICaseManagementDataManager caseManagementDataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            foreach (var strategyID in strategyIDs)
            {
                strategyExecutionDataManager.OverrideStrategyExecution(strategyID, from, to);
            }


            List<int> CaseIDs = strategyExecutionDataManager.GetCasesIDsofStrategyExecutionDetails(null, from, to, strategyIDs);

            if (CaseIDs != null && CaseIDs.Count > 0)
            {
                strategyExecutionDataManager.DeleteStrategyExecutionDetails_ByFilters(null, from, to, strategyIDs);

                caseManagementDataManager.DeleteAccountCases_ByCaseIDs(CaseIDs);
            }
        }
    }
}
