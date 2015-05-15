using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class StrategyDataManager : BaseMySQLDataManager, IStrategyDataManager 
    {
        public StrategyDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public Strategy GetDefaultStrategy()
        {
            string query0 = "SELECT `Id` FROM Strategy s WHERE s.IsDefault = 1";
            string query = "SELECT `MaxValue`, `CriteriaID` FROM StrategyThreshold sl inner join Strategy s on sl.StrategyId=s.Id  WHERE s.IsDefault = 1";
            string query2 = "SELECT `LevelId`, `CriteriaId1`, `Cr1Per`, `CriteriaId2`, `Cr2Per`, `CriteriaId3`, `Cr3Per`, `CriteriaId4`, `Cr4Per`, `CriteriaId5`, `Cr5Per`, `CriteriaId6`, `Cr6Per`, `CriteriaId7`, `Cr7Per`, `CriteriaId8`, `Cr8Per`, `CriteriaId9`, `Cr9Per`, `CriteriaId10`, `Cr10Per`, `CriteriaId11`, `Cr11Per`, `CriteriaId12`, `Cr12Per`, `CriteriaId13`, `Cr13Per`, `CriteriaId14`, `Cr14Per`, `CriteriaId15`, `Cr15Per`  FROM  Strategy_Suspicion_Level sl inner join  Strategy s on sl.StrategyId=s.Id  WHERE s.IsDefault = 1 and sl.LevelId<>1  ";

            Strategy strategy = new Strategy();
            

            MySQLManager manager = new MySQLManager();

            strategy.Id = manager.GetItem(query0, (cmd) =>
            {

                //cmd.Parameters.AddWithValue("@StrategyId", strategyId);

            }, (reader) =>
            {
                return GetReaderValue<int>(reader, "Id") ;
            });


            strategy.StrategyCriterias = manager.GetItems(query, (cmd) =>
           {

               //cmd.Parameters.AddWithValue("@StrategyId", strategyId);

           }, (reader) =>
           {

               StrategyCriteria strategyCriteria = new StrategyCriteria();
               strategyCriteria.Threshold = GetReaderValue<decimal>(reader, "MaxValue") ;
               strategyCriteria.CriteriaId = GetReaderValue<int>(reader, "CriteriaID") ;
               return strategyCriteria;
           });


            strategy.StrategyLevels = manager.GetItems(query2, (cmd) =>
            {

                //cmd.Parameters.AddWithValue("@StrategyId", strategyId);

            }, (reader) =>
            {
                StrategyLevel strategyLevel = new StrategyLevel();

                List<StrategyLevelCriteria> StrategyLevelCriterias = new List<StrategyLevelCriteria>();

                strategyLevel.SuspectionLevelId = GetReaderValue<int>(reader, "LevelId")  ;

                for (int i = 1; i <= 15; i++)
                {
                    StrategyLevelCriteria strategyLevelCriteria = new StrategyLevelCriteria();
                    string CrId = "CriteriaId" + i.ToString();
                    string CrPer = "Cr" + i.ToString() + "Per";
                    if (reader[CrId].ToString() != "0")
                    {
                        //slc.CriteriaId = ParseInt(reader[CrId].ToString());
                        strategyLevelCriteria.CriteriaId = i;
                        strategyLevelCriteria.Percentage =  GetReaderValue<decimal>(reader, CrPer);
                        StrategyLevelCriterias.Add(strategyLevelCriteria);
                    }
                }
                strategyLevel.StrategyLevelCriterias = StrategyLevelCriterias;
                return strategyLevel;
            });

            return strategy;
        }

       
    }
}
