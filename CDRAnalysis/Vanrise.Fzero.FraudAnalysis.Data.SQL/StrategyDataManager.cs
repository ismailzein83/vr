using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Linq;

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
            return GetItemsSP("mainmodule.sp_Strategy_GetStrategy", StrategyMapper, strategyId).FirstOrDefault();
        }

        public List<Strategy> GetAllStrategies()
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetAll", StrategyMapper);
        }

        public List<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description)
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetFilteredStrategies", StrategyMapper, fromRow, toRow, name, description);
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
                Vanrise.Common.Serializer.Serialize(strategyObject)

            );
            insertedId = (int)id;
            if (recordesEffected > 0)
                return true;
            return false;
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
                Vanrise.Common.Serializer.Serialize(strategyObject));
            if (recordesEffected > 0)
                return true;
            return false;
        }
        
        #region Private Methods

        private Strategy StrategyMapper(IDataReader reader)
        {
            var strategy = Vanrise.Common.Serializer.Deserialize<Strategy>(GetReaderValue<string>(reader, "StrategyContent"));
            strategy.Id = (int)reader["Id"];
            return strategy;
        }

        #endregion

    }
}
