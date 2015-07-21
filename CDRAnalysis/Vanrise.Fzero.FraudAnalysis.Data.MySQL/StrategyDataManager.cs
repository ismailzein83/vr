using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class StrategyDataManager : BaseMySQLDataManager, IStrategyDataManager 
    {
        public StrategyDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

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

        public bool AddStrategy(Strategy strategy, out int insertedId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }

        public Strategy GetStrategy(int strategyId)
        {
            throw new NotImplementedException();
        }

        public List<CallClass> GetAllCallClasses()
        {
            throw new NotImplementedException();
        }

        public List<Period> GetPeriods()
        {
            throw new NotImplementedException();
        }

        public List<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            throw new NotImplementedException();
        }

        public List<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber)
        {
            throw new NotImplementedException();
        }

        public List<SubscriberThreshold> GetSubscriberThresholds(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            throw new NotImplementedException();
        }
    }
}
