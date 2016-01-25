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
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        public StrategyDataManager()
            : base("CDRDBConnectionString")
        {

        }

        static StrategyDataManager()
        {
            _columnMapper.Add("IsDefaultText", "PeriodId");
            _columnMapper.Add("Analyst", "UserID");
            _columnMapper.Add("LastUpdatedOn", "LastUpdatedOn");
        }

        public Strategy GetStrategy(int strategyId)
        {
            return GetItemSP("FraudAnalysis.sp_Strategy_GetByID", StrategyMapper, strategyId);
        }

        public List<Strategy> GetStrategies()
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetAll", StrategyMapper);
        }

        public bool AddStrategy(Strategy strategyObject, out int insertedId)
        {
            object id;
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Insert", out id,
                strategyObject.UserId,
                strategyObject.Name,
                strategyObject.Description,
                DateTime.Now,
                strategyObject.IsDefault,
                strategyObject.IsEnabled,
                strategyObject.PeriodId,
                Vanrise.Common.Serializer.Serialize(strategyObject)

            );

            if (recordsEffected > 0)
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
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Update",
                strategyObject.Id,
                strategyObject.UserId,
                strategyObject.Name,
                strategyObject.Description,
                DateTime.Now,
                strategyObject.IsDefault,
                strategyObject.IsEnabled,
                strategyObject.PeriodId,
                Vanrise.Common.Serializer.Serialize(strategyObject));

            return (recordsEffected > 0);
        }

        public List<String> GetStrategyNames(List<int> strategyIds)
        {
            string strategyIDs = (strategyIds != null && strategyIds.Count > 0) ? string.Join(",", strategyIds) : null;

            return GetItemsSP("FraudAnalysis.sp_Strategy_GetNamesByIDs", StrategyNameMapper, strategyIDs);
        }

        public bool AreStrategiesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("FraudAnalysis.Strategy", ref updateHandle);
        }

        #region Private Methods

        private Strategy StrategyMapper(IDataReader reader)
        {
            var strategy = Vanrise.Common.Serializer.Deserialize<Strategy>(reader["StrategyContent"] as string);
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
