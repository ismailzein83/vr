using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyManager
    {
        public Strategy GetStrategy(int StrategyId)
        {

            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return dataManager.GetStrategy(StrategyId);

        }

        public IEnumerable<Strategy> GetStrategies(int PeriodId, bool? IsEnabled)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            return manager.GetStrategies(PeriodId, IsEnabled);
        }

        public Vanrise.Entities.IDataRetrievalResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyQuery> input)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetFilteredStrategies(input));
        }

        public UpdateOperationOutput<Strategy> UpdateStrategy(Strategy strategyObject)
        {
            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            strategyObject.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            bool updateActionSucc = dataManager.UpdateStrategy(strategyObject);
            
            UpdateOperationOutput<Strategy> updateOperationOutput = new UpdateOperationOutput<Strategy>();
            
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = strategyObject;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public InsertOperationOutput<Strategy> AddStrategy(Strategy strategyObject)
        {
            InsertOperationOutput<Strategy> insertOperationOutput = new InsertOperationOutput<Strategy>();

            int strategyId = -1;

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            strategyObject.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            bool insertActionSucc = manager.AddStrategy(strategyObject, out strategyId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                strategyObject.Id = strategyId;
                insertOperationOutput.InsertedObject = strategyObject;
            }
            else
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            return insertOperationOutput;
        }

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

            if (CaseIDs !=null && CaseIDs.Count > 0)
            {
                strategyExecutionDataManager.DeleteStrategyExecutionDetails_ByFilters(null, from, to, strategyIDs);

                caseManagementDataManager.DeleteAccountCases_ByCaseIDs(CaseIDs);
            }
        }
    }
}
