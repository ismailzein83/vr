using System.Collections.Generic;
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

        public Strategy GetDefaultStrategy()
        {
            string query0 = "SELECT Id FROM Strategy s WHERE s.IsDefault = 1";
            string query = "SELECT MaxValue, CriteriaID FROM StrategyThreshold sl inner join Strategy s on sl.StrategyId=s.Id  WHERE s.IsDefault = 1";
            string query2 = "SELECT LevelId, CriteriaId1, Cr1Per ,  CriteriaId2 ,  Cr2Per ,  CriteriaId3 ,  Cr3Per ,  CriteriaId4 ,  Cr4Per ,  CriteriaId5 ,  Cr5Per ,  CriteriaId6 ,  Cr6Per ,  CriteriaId7 ,  Cr7Per ,  CriteriaId8 ,  Cr8Per ,  CriteriaId9 ,  Cr9Per ,  CriteriaId10 ,  Cr10Per ,  CriteriaId11 ,  Cr11Per ,  CriteriaId12 ,  Cr12Per ,  CriteriaId13 ,  Cr13Per ,  CriteriaId14 ,  Cr14Per ,  CriteriaId15 ,  Cr15Per   FROM  Strategy_Suspicion_Level sl inner join  Strategy s on sl.StrategyId=s.Id  WHERE s.IsDefault = 1 and sl.LevelId<>1  ";
            
            Strategy strategy = new Strategy();

            strategy.Id = GetItemText<int>(query0, (reader) =>
            {
                return Helper.AsInt(reader["Id"].ToString());
            } ,(cmd) =>
            {
                //cmd.Parameters.AddWithValue("@StrategyId", strategyId);
            });


            strategy.StrategyCriterias = GetItemsText<StrategyCriteria>(query, (reader) =>
           {

               StrategyCriteria strategyCriteria = new StrategyCriteria();
               strategyCriteria.Threshold = Helper.AsDecimal(reader["MaxValue"].ToString());
               strategyCriteria.CriteriaId = Helper.AsInt(reader["CriteriaID"].ToString());
               return strategyCriteria;
           }, (cmd) =>
           {

               //cmd.Parameters.AddWithValue("@StrategyId", strategyId);

           });


            strategy.StrategyLevels = GetItemsText<StrategyLevel>(query2, (reader) =>
            {
                StrategyLevel strategyLevel = new StrategyLevel();

                List<StrategyLevelCriteria> Lstslc = new List<StrategyLevelCriteria>();

                strategyLevel.SuspectionLevelId = Helper.AsInt(reader["LevelId"].ToString());

                for (int i = 1; i <= 15; i++)
                {
                    StrategyLevelCriteria slc = new StrategyLevelCriteria();
                    string CrId = "CriteriaId" + i.ToString();
                    string CrPer = "Cr" + i.ToString() + "Per";
                    if (reader[CrId].ToString() != "0")
                    {
                        slc.CriteriaId = i;
                        slc.Percentage = Helper.AsDecimal(reader[CrPer].ToString());
                        Lstslc.Add(slc);
                    }
                }
                strategyLevel.StrategyLevelCriterias = Lstslc;
                return strategyLevel;
            }, (cmd) =>
            {

                //cmd.Parameters.AddWithValue("@StrategyId", strategyId);

            });

            return strategy;
        }

      
    }
}
