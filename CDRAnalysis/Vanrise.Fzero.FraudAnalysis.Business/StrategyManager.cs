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

        public Vanrise.Entities.IDataRetrievalResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetFilteredStrategies(input));
        }

        public UpdateOperationOutput<Strategy> UpdateStrategy(Strategy strategyObject)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            strategyObject.UserId=Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            bool updateActionSucc = manager.UpdateStrategy(strategyObject);
            UpdateOperationOutput<Strategy> updateOperationOutput = new UpdateOperationOutput<Strategy>();

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = strategyObject;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
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
