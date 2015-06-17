﻿using System.Collections.Generic;
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
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetStrategy", StrategyMapper, strategyId).FirstOrDefault();
        }
       
        public List<Strategy> GetAllStrategies()
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetAll", StrategyMapper);
        }

        public List<Result> GetFilteredSuspiciousNumbers(int fromRow, int toRow, DateTime fromDate, DateTime toDate, int strategyId, string suspicionList)
        {
            return GetItemsSP("FraudAnalysis.sp_Results_GetFilteredSuspiciousNumbers", ResultMapper, fromRow, toRow, fromDate, toDate, strategyId, suspicionList);
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
                Vanrise.Common.Serializer.Serialize(strategyObject));
            if (recordesEffected > 0)
                return true;
            return false;
        }
        
        #region Private Methods

        private Result ResultMapper(IDataReader reader)
        {
            var result = new Result();
            result.DateDay = (DateTime)reader["DateDay"];
            result.SubscriberNumber = reader["SubscriberNumber"] as string;
            result.SuspicionLevelName = reader["SuspicionLevelName"] as string;
            return result;
        }

        private Strategy StrategyMapper(IDataReader reader)
        {
            var strategy = Vanrise.Common.Serializer.Deserialize<Strategy>(GetReaderValue<string>(reader, "StrategyContent"));
            strategy.Id = (int)reader["Id"];
            return strategy;
        }

        private CallClass CallClassMapper(IDataReader reader)
        {
            var callClass = new CallClass();
            callClass.Id = (int)reader["Id"];
            callClass.Description = reader["Description"] as string;
            callClass.NetType = (Enums.NetType)Enum.ToObject(typeof(Enums.NetType), GetReaderValue<int>(reader, "NetType"));
            return callClass;
        }

        public List<CallClass> GetAllCallClasses()
        {
            return GetItemsSP("FraudAnalysis.sp_CallClass_GetAll", CallClassMapper);
        }

        public List<Period> GetPeriods()
        {

            var enumerationType = typeof(Enums.Period);
            List<Period> periods = new List<Period>();

            foreach (int value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                periods.Add(new Period() { Id = value, Name = name });
            }

            return periods;
        }


        #endregion

    }
}
