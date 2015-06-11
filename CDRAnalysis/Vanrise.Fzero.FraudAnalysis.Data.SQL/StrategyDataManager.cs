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
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetStrategy", StrategyMapper, strategyId).FirstOrDefault();
        }
        //public Strategy GetStrategy(int strategyId)
        //{
        //    string query0 = "SELECT Id FROM Strategy WHERE Id = @StrategyId";
        //    string query = "SELECT MaxValue, CriteriaID FROM StrategyThreshold Where StrategyId = @StrategyId";
        //    string query1 = "SELECT PeriodId, Value, CriteriaID FROM StrategyPeriods Where StrategyId = @StrategyId";
        //    string query2 = "SELECT LevelId, CriteriaId1, Cr1Per ,  CriteriaId2 ,  Cr2Per ,  CriteriaId3 ,  Cr3Per ,  CriteriaId4 ,  Cr4Per ,  CriteriaId5 ,  Cr5Per ,  CriteriaId6 ,  Cr6Per ,  CriteriaId7 ,  Cr7Per ,  CriteriaId8 ,  Cr8Per ,  CriteriaId9 ,  Cr9Per ,  CriteriaId10 ,  Cr10Per ,  CriteriaId11 ,  Cr11Per ,  CriteriaId12 ,  Cr12Per ,  CriteriaId13 ,  Cr13Per ,  CriteriaId14 ,  Cr14Per ,  CriteriaId15 ,  Cr15Per,  CriteriaId16 ,  Cr16Per,  CriteriaId17 ,  Cr17Per,  CriteriaId18 ,  Cr18Per   FROM  Strategy_Suspicion_Level Where StrategyId = @StrategyId and LevelId<>1  ";

        //    Strategy strategy = new Strategy();

        //    strategy.Id = GetItemText<int>(query0, (reader) =>
        //    {
        //        return GetReaderValue<int>(reader, "Id");
        //    }, (cmd) =>
        //    {
        //        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@StrategyId", Value = strategyId });
        //    });


        //    strategy.StrategyFilters = GetItemsText<StrategyFilter>(query, (reader) =>
        //    {

        //        StrategyFilter strategyFilter = new StrategyFilter();
        //        strategyFilter.Threshold = GetReaderValue<decimal>(reader, "MaxValue");
        //        strategyFilter.FilterId = GetReaderValue<int>(reader, "CriteriaID");
        //        return strategyFilter;
        //    }, (cmd) =>
        //    {

        //        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@StrategyId", Value = strategyId });

        //    });





        //    List<StrategyFilter> strategyFilters = GetItemsText<StrategyFilter>(query1, (reader) =>
        //    {
        //        StrategyFilter strategyFilter = new StrategyFilter();
        //        strategyFilter.PeriodId = GetReaderValue<int>(reader, "PeriodId");
        //        strategyFilter.FilterId = GetReaderValue<int>(reader, "CriteriaID");
        //        return strategyFilter;
        //    }, (cmd) =>
        //    {

        //        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@StrategyId", Value = strategyId });

        //    });



        //    foreach (var i in strategy.StrategyFilters)
        //    {
        //        i.PeriodId = strategyFilters.Where(x => x.FilterId == i.FilterId).First().PeriodId;
        //    }




        //    strategy.StrategyLevels = GetItemsText<StrategyLevel>(query2, (reader) =>
        //    {
        //        StrategyLevel strategyLevel = new StrategyLevel();

        //        List<StrategyLevelCriteria> Lstslc = new List<StrategyLevelCriteria>();

        //        strategyLevel.SuspectionLevelId = GetReaderValue<int>(reader, "LevelId");

        //        for (int i = 1; i <= 18; i++)
        //        {
        //            StrategyLevelCriteria slc = new StrategyLevelCriteria();
        //            string CrId = "CriteriaId" + i.ToString();
        //            string CrPer = "Cr" + i.ToString() + "Per";
        //            if (reader[CrId].ToString() != "0")
        //            {
        //                slc.FilterId = i;
        //                slc.Percentage = GetReaderValue<decimal>(reader, CrPer);
        //                Lstslc.Add(slc);
        //            }
        //        }
        //        strategyLevel.StrategyLevelCriterias = Lstslc;
        //        return strategyLevel;
        //    }, (cmd) =>
        //    {
        //        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@StrategyId", Value = strategyId });
        //    });

        //    return strategy;
        //}
        public List<Strategy> GetAllStrategies2()
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetAll", StrategyMapper);
        }


        public List<Strategy> GetAllStrategies()
        {
            string query_GetStrategies = @"SELECT Id, Name, Description FROM Strategy; ";
            List<Strategy> strategies = new List<Strategy>();


            ExecuteReaderText(query_GetStrategies, (reader) =>
            {
                while (reader.Read())
                {
                    Strategy strategy = new Strategy();
                    strategy.Id = (int)reader["Id"];
                    strategy.Name = reader["Name"] as string;
                    strategy.Description = reader["Description"] as string;
                    strategies.Add(strategy);
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

        private Strategy StrategyMapper(IDataReader reader)
        {
            var strategy = Vanrise.Common.Serializer.Deserialize<Strategy>(GetReaderValue<string>(reader, "StrategyContent"));
            strategy.Id = (int)reader["Id"];
            return strategy;
        }

        


        #endregion

    }
}
