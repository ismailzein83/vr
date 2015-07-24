using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class StrategyDataManager : BaseMySQLDataManager, IStrategyDataManager 
    {
        public StrategyDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }



        public Strategy GetStrategy(int strategyId)
        {
            throw new NotImplementedException();
        }

        public List<Strategy> GetAllStrategies()
        {
            throw new NotImplementedException();
        }

        public List<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description)
        {
            throw new NotImplementedException();
        }

        public bool AddStrategy(Strategy strategy, out int insertedId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }

        public List<CallClass> GetAllCallClasses()
        {
            throw new NotImplementedException();
        }

        public List<Period> GetPeriods()
        {
            throw new NotImplementedException();
        }

        public List<SubscriberThreshold> GetSubscriberThresholds(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            throw new NotImplementedException();
        }

        public List<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            throw new NotImplementedException();
        }

        public List<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber)
        {
            throw new NotImplementedException();
        }
    }
}
