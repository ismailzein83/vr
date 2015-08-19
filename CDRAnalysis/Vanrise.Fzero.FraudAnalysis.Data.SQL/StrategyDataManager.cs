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

        public StrategyDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public Strategy GetStrategy(int strategyId)
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetStrategy", StrategyMapper, strategyId).FirstOrDefault();
        }

        public List<Strategy> GetStrategies(int PeriodId, bool? IsEnabled)
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetStrategies", StrategyMapper, PeriodId, IsEnabled);
        }

        public BigResult<Strategy> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyResultQuery> input)
        {
            bool? IsDefault = null;
            if (input.Query.IsDefaultList.Contains("true"))
                IsDefault = true;
            else if (input.Query.IsDefaultList.Contains("false"))
                IsDefault = false;


            bool? IsEnabled = null;
            if (input.Query.IsEnabledList.Contains("true"))
                IsEnabled = true;
            else if (input.Query.IsEnabledList.Contains("false"))
                IsEnabled = false;

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_CreateTempForFilteredStrategies", tempTableName, input.Query.Name, input.Query.Description, input.Query.PeriodsList, input.Query.UsersList, IsDefault, IsEnabled, input.Query.FromDate, input.Query.ToDate);
            };

          
            return RetrieveData(input, createTempTableAction, StrategyMapper);
        }


        public bool AddStrategy(Strategy strategyObject, out int insertedId, int userId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Insert", out id,
                userId,
                !string.IsNullOrEmpty(strategyObject.Name) ? strategyObject.Name : null,
                !string.IsNullOrEmpty(strategyObject.Description) ? strategyObject.Description : null,
                DateTime.Now,
                strategyObject.IsDefault,
                strategyObject.IsEnabled,
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

        public bool UpdateStrategy(Strategy strategyObject, int userId)
        {

            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Update",
                strategyObject.Id,
                 userId,
                !string.IsNullOrEmpty(strategyObject.Name) ? strategyObject.Name : null,
                !string.IsNullOrEmpty(strategyObject.Description) ? strategyObject.Description : null,
                DateTime.Now,
                strategyObject.IsDefault,
                strategyObject.IsEnabled,
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


        public void DeleteStrategyResults(int StrategyId, DateTime FromDate, DateTime ToDate)
        {
            ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_DeleteStrategyResults", StrategyId, FromDate, ToDate);
        }


        #region Private Methods

        private Strategy StrategyMapper(IDataReader reader)
        {
            var strategy = Vanrise.Common.Serializer.Deserialize<Strategy>(GetReaderValue<string>(reader, "StrategyContent"));
            strategy.Id = (int)reader["Id"];
            strategy.UserId = (int)reader["UserId"];
            return strategy;
        }

        private String StrategyNameMapper(IDataReader reader)
        {
            return reader["Name"] as string;
        }

        #endregion







        
    }
}
