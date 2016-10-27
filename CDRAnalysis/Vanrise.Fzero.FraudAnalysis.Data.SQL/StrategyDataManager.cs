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
                Vanrise.Common.Serializer.Serialize(strategyObject.Settings)
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
                Vanrise.Common.Serializer.Serialize(strategyObject.Settings));
            return (recordsEffected > 0);
        }

        public bool AreStrategiesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("FraudAnalysis.Strategy", ref updateHandle);
        }

        #region Mappers

        private Strategy StrategyMapper(IDataReader reader)
        {
            return new Strategy
            {
                Id = (int)reader["Id"],
                Description = reader["Description"] as string,
                LastUpdatedOn = GetReaderValue<DateTime>(reader, "LastUpdatedOn"),
                UserId = GetReaderValue<int>(reader, "UserId"),
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<StrategySettings>(reader["Settings"] as string)
            };
        }
       
        #endregion
    }
}
