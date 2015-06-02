using System;
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

        public Strategy GetStrategy(int strategyId)
        {
            string query0 = "SELECT `Id` FROM Strategy  WHERE Id = @StrategyId";
            string query = "SELECT `MaxValue`, `CriteriaID` FROM StrategyThreshold Where StrategyId = @StrategyId";
            string query1 = "SELECT `PeriodId`, `Value`, `CriteriaID` FROM StrategyPeriods Where StrategyId = @StrategyId";
            string query2 = "SELECT `LevelId`, `CriteriaId1`, `Cr1Per`, `CriteriaId2`, `Cr2Per`, `CriteriaId3`, `Cr3Per`, `CriteriaId4`, `Cr4Per`, `CriteriaId5`, `Cr5Per`, `CriteriaId6`, `Cr6Per`, `CriteriaId7`, `Cr7Per`, `CriteriaId8`, `Cr8Per`, `CriteriaId9`, `Cr9Per`, `CriteriaId10`, `Cr10Per`, `CriteriaId11`, `Cr11Per`, `CriteriaId12`, `Cr12Per`, `CriteriaId13`, `Cr13Per`, `CriteriaId14`, `Cr14Per`, `CriteriaId15`, `Cr15Per`, `CriteriaId16`, `Cr16Per`  FROM  Strategy_Suspicion_Level Where StrategyId = @StrategyId and LevelId<>1  ";

            Strategy strategy = new Strategy();
            

            MySQLManager manager = new MySQLManager();

            strategy = manager.GetItem<Strategy>(query0, (cmd) =>
            {
                cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter() { ParameterName = "@StrategyId", Value = strategyId });
            }, (reader) =>
            {
                return new Strategy()
                {
                    Id = GetReaderValue<int>(reader, "Id"),
                    Name = GetReaderValue<string>(reader, "Name"),
                    Description = GetReaderValue<string>(reader, "Description"),
                    IsDefault = GetReaderValue<bool>(reader, "IsDefault")
                };
            });


            strategy.StrategyCriterias = manager.GetItems(query, (cmd) =>
           {

               cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter() { ParameterName = "@StrategyId", Value = strategyId });

           }, (reader) =>
           {

               StrategyCriteria strategyCriteria = new StrategyCriteria();
               strategyCriteria.Threshold = GetReaderValue<decimal>(reader, "MaxValue") ;
               strategyCriteria.CriteriaId = GetReaderValue<int>(reader, "CriteriaID") ;
               return strategyCriteria;
           });




            strategy.StrategyPeriods = manager.GetItems(query1, (cmd) =>
            {

                cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter() { ParameterName = "@StrategyId", Value = strategyId });

            }, (reader) =>
            {

                StrategyPeriod strategyPeriod = new StrategyPeriod();
                strategyPeriod.Period =  (Enums.Period)Enum.ToObject(typeof(Enums.Period), GetReaderValue<int>(reader, "PeriodId"));
                strategyPeriod.Value = GetReaderValue<int>(reader, "Value");
                strategyPeriod.CriteriaId = GetReaderValue<int>(reader, "CriteriaID");
                return strategyPeriod;
            });

                      



            strategy.StrategyLevels = manager.GetItems(query2, (cmd) =>
            {

                cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter() { ParameterName = "@StrategyId", Value = strategyId });

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


        public List<Strategy> GetAllStrategies()
        {
            MySQLManager manager = new MySQLManager();
            string query_GetStrategies = @"SELECT Id, Name, Description FROM Strategy; ";
            List<Strategy> strategies = new List<Strategy>();


            manager.ExecuteReader(query_GetStrategies,
                null, (reader) =>
                {

                    while (reader.Read())
                    {
                        Strategy strategy = new Strategy();
                        strategy.Id = (int)reader["Id"];
                        strategy.Name = reader["Name"] as string;
                        strategy.Description = reader["Description"] as string;
                        strategies.Add(strategy);
                    }



                });


            return strategies;
        }

        public List<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description)
        {
            MySQLManager manager = new MySQLManager();
            string query_GetStrategies = @"SELECT Id, Name, Description FROM Strategy where Name like '%' +@Name+ '%' and Description like '%' +@Description+ '%'; ";
            List<Strategy> strategies = new List<Strategy>();


            manager.ExecuteReader(query_GetStrategies, 
             (cmd) =>
            {
                if (name == null)
                    name = string.Empty;

                if (description == null)
                    description = string.Empty;

                cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter() { ParameterName = "@Name", Value = name });
                cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter() { ParameterName = "@Description", Value = description });
            }, (reader) =>
            {
                while (reader.Read())
                {
                    Strategy strategy = new Strategy();
                    strategy.Id = (int)reader["Id"];
                    strategy.Name = reader["Name"] as string;
                    strategy.Description = reader["Description"] as string;
                    strategies.Add(strategy);
                }
            }
           
            );
            return strategies;
        }

        public bool AddStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }
    }
}
