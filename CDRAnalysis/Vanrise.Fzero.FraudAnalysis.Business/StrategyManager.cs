using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyManager
    {

        public List<Period> GetPeriods()
        {
            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return dataManager.GetPeriods();
        }


        public Strategy GetStrategy(int StrategyId)
        {

            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return dataManager.GetStrategy(StrategyId);

        }


        public IEnumerable<SubscriberThreshold> GetSubscriberThresholds(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {

            ISuspiciousNumberDataManager manager = FraudDataManagerFactory.GetDataManager<ISuspiciousNumberDataManager>();

            return manager.GetSubscriberThresholds(fromRow, toRow, fromDate, toDate, msisdn);

        }



        public IEnumerable<Strategy> GetAllStrategies()
        {

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return manager.GetAllStrategies();

        }


        public IEnumerable<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description)
        {

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return manager.GetFilteredStrategies(fromRow, toRow, name, description);

        }


        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<Strategy> UpdateStrategy(Strategy strategyObject)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            bool updateActionSucc = manager.UpdateStrategy(strategyObject);
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
            bool insertActionSucc = manager.AddStrategy(strategyObject, out strategyId);

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




    }
}
