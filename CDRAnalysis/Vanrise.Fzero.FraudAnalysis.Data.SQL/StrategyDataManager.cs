using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class StrategyDataManager : BaseSQLDataManager, IStrategyDataManager
    {

        int DefaultUserId = 1;

        public StrategyDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public Strategy GetStrategy(int strategyId)
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetStrategy", StrategyMapper, strategyId).FirstOrDefault();
        }

        public List<Strategy> GetStrategies(int PeriodId)
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetStrategies", StrategyMapper, PeriodId);
        }

        public BigResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_CreateTempForFilteredStrategies", tempTableName, input.Query.Name, input.Query.Description);
            };
            return RetrieveData(input, createTempTableAction, StrategyMapper);
        }


        public bool AddStrategy(Strategy strategyObject, out int insertedId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Insert", out id,
                DefaultUserId,
                !string.IsNullOrEmpty(strategyObject.Name) ? strategyObject.Name : null,
                !string.IsNullOrEmpty(strategyObject.Description) ? strategyObject.Description : null,
                DateTime.Now,
                strategyObject.IsDefault,
                strategyObject.PeriodId,
                Vanrise.Common.Serializer.Serialize(strategyObject)

            );

            if (recordesEffected > 0)
            {
                insertedId = (int)id;
                return true;
            }
            else
            {
                insertedId = 0;
                return false;
            }


        }

        public bool UpdateStrategy(Strategy strategyObject)
        {

            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Update",
                strategyObject.Id,
                 DefaultUserId,
                !string.IsNullOrEmpty(strategyObject.Name) ? strategyObject.Name : null,
                !string.IsNullOrEmpty(strategyObject.Description) ? strategyObject.Description : null,
                DateTime.Now,
                strategyObject.IsDefault,
                strategyObject.PeriodId,
                Vanrise.Common.Serializer.Serialize(strategyObject));
            if (recordesEffected > 0)
                return true;
            return false;
        }


        public List<String> GetStrategyNames(List<int> strategyIds)
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetStrategyNames", StrategyNameMapper, string.Join(",", strategyIds));
        }


        #region Private Methods

        private Strategy StrategyMapper(IDataReader reader)
        {
            var strategy = Vanrise.Common.Serializer.Deserialize<Strategy>(GetReaderValue<string>(reader, "StrategyContent"));
            strategy.Id = (int)reader["Id"];
            return strategy;
        }

        private String StrategyNameMapper(IDataReader reader)
        {
            return reader["Name"] as string;
        }

        #endregion




       
    }
}
