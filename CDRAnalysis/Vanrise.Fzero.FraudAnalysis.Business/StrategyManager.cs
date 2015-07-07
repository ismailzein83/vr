﻿using System;
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

        public IEnumerable<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return ((IEnumerable<CDR>)(manager.GetNormalCDRs(fromRow, toRow, fromDate, toDate, msisdn)));

        }


        public IEnumerable<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber)
        {

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return ((IEnumerable<NumberProfile>)(manager.GetNumberProfiles(fromRow, toRow, fromDate, toDate, subscriberNumber)));

        }


        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(string tempTableKey, int fromRow, int toRow, DateTime fromDate, DateTime toDate, int? strategyId, string suspicionLevelsList)
        {
            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return manager.GetFilteredSuspiciousNumbers(fromRow, toRow, fromDate, toDate, strategyId, suspicionLevelsList);

        }


        public IEnumerable<Strategy> GetAllStrategies()
        {

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return ((IEnumerable<Strategy>)(manager.GetAllStrategies()));

        }


        public IEnumerable<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description)
        {

            IStrategyDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();

            return ((IEnumerable<Strategy>)(manager.GetFilteredStrategies(fromRow, toRow, name, description)));

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
