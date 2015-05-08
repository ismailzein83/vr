using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using System.Data.SqlClient;
using System.IO;
using Vanrise.Data.SQL;

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
            
            Strategy st = new Strategy();

            st.Id = GetItemText<int>(query0, (reader) =>
            {
                return ParseInt(reader["Id"].ToString());
            } ,(cmd) =>
            {
                //cmd.Parameters.AddWithValue("@StrategyId", strategyId);
            });


            st.StrategyCriterias = GetItemsText<StrategyCriteria>(query, (reader) =>
           {

               StrategyCriteria sc = new StrategyCriteria();
               sc.Threshold = ParseDecimal(reader["MaxValue"].ToString());
               sc.CriteriaId = ParseInt(reader["CriteriaID"].ToString());
               return sc;
           }, (cmd) =>
           {

               //cmd.Parameters.AddWithValue("@StrategyId", strategyId);

           });


            st.StrategyLevels = GetItemsText<StrategyLevel>(query2, (reader) =>
            {
                StrategyLevel sl = new StrategyLevel();

                List<StrategyLevelCriteria> Lstslc = new List<StrategyLevelCriteria>();

                sl.SuspectionLevelId = ParseInt(reader["LevelId"].ToString());

                for (int i = 1; i <= 15; i++)
                {
                    StrategyLevelCriteria slc = new StrategyLevelCriteria();
                    string CrId = "CriteriaId" + i.ToString();
                    string CrPer = "Cr" + i.ToString() + "Per";
                    if (reader[CrId].ToString() != "0")
                    {
                        //slc.CriteriaId = ParseInt(reader[CrId].ToString());
                        slc.CriteriaId = i;
                        slc.Percentage = ParseDecimal(reader[CrPer].ToString());
                        Lstslc.Add(slc);
                    }
                }
                sl.StrategyLevelCriterias = Lstslc;
                return sl;
            }, (cmd) =>
            {

                //cmd.Parameters.AddWithValue("@StrategyId", strategyId);

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
