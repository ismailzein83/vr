using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;
using System.Linq;

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


        public Vanrise.Entities.IDataRetrievalResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input, IEnumerable<User> users)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            BigResult<Strategy> strategies = manager.GetFilteredStrategies(input);

            foreach ( var strategy in strategies.Data)
            {
                strategy.Analyst = users.Where(x => x.UserId == strategy.UserId).First().Name;
            }


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, strategies);
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
                insertOperationOutput.InsertedObject = strategyObject;
            }
            else
                insertOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.InsertOperationResult.SameExists;
            return insertOperationOutput;
        }

        public void DeleteStrategyResults(int StrategyId, DateTime FromDate, DateTime ToDate)
        {

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            manager.DeleteStrategyResults(StrategyId, FromDate, ToDate);

        }


    }
}
