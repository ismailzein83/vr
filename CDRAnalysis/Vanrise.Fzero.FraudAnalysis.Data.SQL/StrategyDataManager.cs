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



        //public Strategy GetStrategy(int strategyId)
        //{
        //    return GetItemsSP("mainmodule.sp_User_GetUser", StrategyMapper, strategyId).FirstOrDefault();
        //}




        public Strategy GetStrategy(int strategyId)
        {
            string query0 = "SELECT Id, StrategyContent FROM Strategy WHERE Id = @StrategyId";
            string query = "SELECT MaxValue, CriteriaID FROM StrategyThreshold Where StrategyId = @StrategyId";
            string query1 = "SELECT PeriodId, Value, CriteriaID FROM StrategyPeriods Where StrategyId = @StrategyId";
            string query2 = "SELECT LevelId, CriteriaId1, Cr1Per ,  CriteriaId2 ,  Cr2Per ,  CriteriaId3 ,  Cr3Per ,  CriteriaId4 ,  Cr4Per ,  CriteriaId5 ,  Cr5Per ,  CriteriaId6 ,  Cr6Per ,  CriteriaId7 ,  Cr7Per ,  CriteriaId8 ,  Cr8Per ,  CriteriaId9 ,  Cr9Per ,  CriteriaId10 ,  Cr10Per ,  CriteriaId11 ,  Cr11Per ,  CriteriaId12 ,  Cr12Per ,  CriteriaId13 ,  Cr13Per ,  CriteriaId14 ,  Cr14Per ,  CriteriaId15 ,  Cr15Per,  CriteriaId16 ,  Cr16Per   FROM  Strategy_Suspicion_Level Where StrategyId = @StrategyId and LevelId<>1  ";
            
            Strategy strategy = GetItemText<Strategy>(query0, StrategyMapper
            ,(cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter(){ParameterName="@StrategyId", Value=strategyId});
            });


            strategy.StrategyCriterias = GetItemsText<StrategyCriteria>(query, (reader) =>
           {

               StrategyCriteria strategyCriteria = new StrategyCriteria();
               strategyCriteria.Threshold = GetReaderValue<decimal>(reader, "MaxValue") ;
               strategyCriteria.CriteriaId = GetReaderValue<int>(reader, "CriteriaID")  ;
               return strategyCriteria;
           }, (cmd) =>
           {

               cmd.Parameters.Add(new SqlParameter(){ParameterName="@StrategyId", Value=strategyId});

           });



            strategy.StrategyPeriods = GetItemsText<StrategyPeriod>(query1, (reader) =>
            {
                StrategyPeriod strategyPeriod = new StrategyPeriod();
                strategyPeriod.Period = (Enums.Period)Enum.ToObject(typeof(Enums.Period), GetReaderValue<int>(reader, "PeriodId"));
                strategyPeriod.Value = GetReaderValue<int>(reader, "Value");
                strategyPeriod.CriteriaId = GetReaderValue<int>(reader, "CriteriaID");
                return strategyPeriod;
            }, (cmd) =>
            {

                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@StrategyId", Value = strategyId });

            });




            strategy.StrategyLevels = GetItemsText<StrategyLevel>(query2, (reader) =>
            {
                StrategyLevel strategyLevel = new StrategyLevel();

                List<StrategyLevelCriteria> Lstslc = new List<StrategyLevelCriteria>();

                strategyLevel.SuspectionLevelId = GetReaderValue<int>(reader, "LevelId");

                for (int i = 1; i <= 15; i++)
                {
                    StrategyLevelCriteria slc = new StrategyLevelCriteria();
                    string CrId = "CriteriaId" + i.ToString();
                    string CrPer = "Cr" + i.ToString() + "Per";
                    if (reader[CrId].ToString() != "0")
                    {
                        slc.CriteriaId = i;
                        slc.Percentage = GetReaderValue<decimal>(reader, CrPer);
                        Lstslc.Add(slc);
                    }
                }
                strategyLevel.StrategyLevelCriterias = Lstslc;
                return strategyLevel;
            }, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@StrategyId", Value = strategyId });
            });

            return strategy;
        }

        public List<Strategy> GetAllStrategies()
        {
            string query_GetStrategies = @"SELECT Id, StrategyContent  FROM Strategy; ";
            List<Strategy> strategies = new List<Strategy>();


            ExecuteReaderText(query_GetStrategies, (reader) =>
            {
                while (reader.Read())
                {
                    strategies.Add(Vanrise.Common.Serializer.Deserialize<Strategy>(GetReaderValue<string>(reader, "StrategyContent")));
                }
             }, null);
            return strategies;
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
