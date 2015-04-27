using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.MySQL;

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
            string query = "SELECT `MaxValue`, `CriteriaID` FROM StrategyThreshold WHERE StrategyId = @StrategyId";
            string query2 ="SELECT `LevelId`, `CriteriaId1`, `Cr1Per`, `CriteriaId2`, `Cr2Per`, `CriteriaId3`,"
               + " `Cr3Per`, `CriteriaId4`, `Cr4Per`, `CriteriaId5`, `Cr5Per`, `CriteriaId6`, `Cr6Per`, `CriteriaId7`, `Cr7Per`,"
               +" `CriteriaId8`, `Cr8Per`, `CriteriaId9`, `Cr9Per`, `CriteriaId10`, `Cr10Per`, `CriteriaId11`, `Cr11Per`,"
               +" `CriteriaId12`, `Cr12Per`, `CriteriaId13`, `Cr13Per`, `CriteriaId14`, `Cr14Per`, `CriteriaId15`, `Cr15Per` "
               + " FROM Strategy_Suspicion_Level WHERE StrategyId = @StrategyId";
            
            Strategy st = new Strategy();
            

            MySQLManager manager = new MySQLManager();

            st.Criterias = manager.GetItems(query, (cmd) =>
           {

               cmd.Parameters.AddWithValue("@StrategyId", strategyId);

           }, (reader) =>
           {

               StrategyCriteria sc = new StrategyCriteria();
               sc.Threshold = ParseDecimal(reader["MaxValue"].ToString());
               sc.CriteriaId = ParseInt(reader["CriteriaID"].ToString());
               return sc;
           });


            st.Levels = manager.GetItems(query2, (cmd) =>
            {

                cmd.Parameters.AddWithValue("@StrategyId", strategyId);

            }, (reader) =>
            {
                StrategyLevel sl = new StrategyLevel();

                List<StrategyLevelCriteria> Lstslc = new List<StrategyLevelCriteria>();

                sl.SuspectionLevel = ParseInt(reader["LevelId"].ToString());

                for (int i = 1; i <= 15; i++)
                {
                    StrategyLevelCriteria slc = new StrategyLevelCriteria();
                    string CrId = "CriteriaId" + i.ToString();
                    string CrPer = "Cr" + i.ToString() + "Per";
                    slc.CriteriaId = ParseInt(reader[CrId].ToString());
                    slc.Percentage = ParseDecimal(reader[CrPer].ToString());
                    Lstslc.Add(slc);
                }
                sl.Criterias = Lstslc;
                return sl;
            });

            return st;
        }

        public static decimal ParseDecimal(string value)
        {
            Decimal d = 0;
            Decimal.TryParse(value, out d);
            return d;
        }

        public static int ParseInt(string value)
        {
            int d = 0;
            int.TryParse(value, out d);
            return d;
        } 
    }
}
