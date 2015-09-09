using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

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

        public Vanrise.Entities.IDataRetrievalResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetFilteredStrategies(input));
        }

        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<Strategy> UpdateStrategy(Strategy strategyObject)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            bool updateActionSucc = manager.UpdateStrategy(strategyObject, Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId());
            Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<Strategy> updateOperationOutput = new Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<Strategy>();

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = strategyObject;
                strategyObject.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            }
            else
                updateOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationResult.Failed;
            return updateOperationOutput;
        }

        public Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationOutput<Strategy> AddStrategy(Strategy strategyObject)
        {
            Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationOutput<Strategy> insertOperationOutput = new Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationOutput<Strategy>();

            int strategyId = -1;

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            bool insertActionSucc = manager.AddStrategy(strategyObject, out strategyId, Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId());

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationResult.Succeeded;
                strategyObject.Id = strategyId;
                strategyObject.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
                insertOperationOutput.InsertedObject = strategyObject;
            }
            else
                insertOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationResult.SameExists;
            return insertOperationOutput;
        }


        public Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationOutput<StrategyExecution> ExecuteStrategy(StrategyExecution strategyExecutionObject)
        {
            Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationOutput<StrategyExecution> insertOperationOutput = new Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationOutput<StrategyExecution>();

            int strategyExecutionId = -1;

            IStrategyExecutionDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            manager.ExecuteStrategy(strategyExecutionObject, out strategyExecutionId);

            strategyExecutionObject.ID = strategyExecutionId;
            insertOperationOutput.InsertedObject = strategyExecutionObject;

            return insertOperationOutput;
        }


        public void OverrideStrategyExecution(int StrategyId, DateTime FromDate, DateTime ToDate)
        {

            IStrategyExecutionDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();

            int StrategyExecutionId;

            bool OverridenSuccessfully = manager.OverrideStrategyExecution(StrategyId, FromDate, ToDate,out StrategyExecutionId);

            if (OverridenSuccessfully)
            {
                manager.DeleteStrategyExecutionDetails(StrategyExecutionId);
            }

        }

    }
}
