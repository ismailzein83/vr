﻿using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Data.SqlClient;
using System.Data;

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
            string query0 = "SELECT Id FROM Strategy WHERE Id = @StrategyId";
            string query = "SELECT MaxValue, CriteriaID FROM StrategyThreshold Where StrategyId = @StrategyId";
            string query2 = "SELECT LevelId, CriteriaId1, Cr1Per ,  CriteriaId2 ,  Cr2Per ,  CriteriaId3 ,  Cr3Per ,  CriteriaId4 ,  Cr4Per ,  CriteriaId5 ,  Cr5Per ,  CriteriaId6 ,  Cr6Per ,  CriteriaId7 ,  Cr7Per ,  CriteriaId8 ,  Cr8Per ,  CriteriaId9 ,  Cr9Per ,  CriteriaId10 ,  Cr10Per ,  CriteriaId11 ,  Cr11Per ,  CriteriaId12 ,  Cr12Per ,  CriteriaId13 ,  Cr13Per ,  CriteriaId14 ,  Cr14Per ,  CriteriaId15 ,  Cr15Per   FROM  Strategy_Suspicion_Level Where StrategyId = @StrategyId and LevelId<>1  ";
            
            Strategy strategy = new Strategy();

            strategy.Id = GetItemText<int>(query0, (reader) =>
            {
                return GetReaderValue<int>(reader, "Id") ;
            } ,(cmd) =>
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

      
    }
}
