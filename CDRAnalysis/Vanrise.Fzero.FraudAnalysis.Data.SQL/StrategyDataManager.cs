using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class StrategyDataManager : BaseSQLDataManager, IStrategyDataManager
    {
        public StrategyDataManager()
            : base("CDRDBConnectionString")
        {

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

        public bool AreStrategiesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("FraudAnalysis.Strategy", ref updateHandle);
        }

        #region Mappers

        private Strategy StrategyMapper(IDataReader reader)
        {
            var strategy = Vanrise.Common.Serializer.Deserialize<Strategy>(reader["StrategyContent"] as string);
            strategy.Id = (int)reader["Id"];
            strategy.UserId = (int)reader["UserId"];
            return strategy;
        }
       
        #endregion
    }
}
