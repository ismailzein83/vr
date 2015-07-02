using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using Vanrise.CommonLibrary;
using System.Data.SqlClient;
using MySql.Data.MySqlClient ;
using System.Configuration;






namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public class SuspectionOccurance
    {
        public string SubscriberNumber { get; set; }
        public Suspicion_Level Suspicion_Level { get; set; }
        public int? NumberOfOccurance { get; set; }
        public Strategy Strategy { get; set; }
        public string LastReport { get; set; }


        public static List<vwSuspectionAnalysi> GetList(int strategyId, DateTime? fromDate, DateTime? toDate, string suspectionList, int minimumOccurance, string SQLType)
        {
            List<vwSuspectionAnalysi> suspectionOccurance = new List<vwSuspectionAnalysi>();
         
            try
            {
                using (Entities context = new Entities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 18000;


                    if (SQLType == "MySQL")
                    {
                        MySqlParameter MySQL_fromDate = new MySqlParameter("@fromDate", fromDate);
                        MySqlParameter MySQL_toDate = new MySqlParameter("@ToDate", toDate);
                        MySqlParameter MySQL_strategyId = new MySqlParameter("@strategyId", strategyId);
                        MySqlParameter MySQL_suspectionList = new MySqlParameter("@SuspectionList", suspectionList);
                        MySqlParameter MySQL_minimumOccurance = new MySqlParameter("@MinimumOccurance", minimumOccurance);

                        suspectionOccurance = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwSuspectionAnalysi>("call prfindSuspicionOccurrence (@fromDate,  @ToDate, @strategyId, @SuspectionList, @MinimumOccurance)", MySQL_fromDate, MySQL_toDate, MySQL_strategyId, MySQL_suspectionList, MySQL_minimumOccurance).ToList();


                    }

                    else
                    {
                        SqlParameter SQL_fromDate = new SqlParameter("@fromDate", fromDate);
                        SqlParameter SQL_toDate = new SqlParameter("@ToDate", toDate);
                        SqlParameter SQL_strategyId = new SqlParameter("@strategyId", strategyId);
                        SqlParameter SQL_suspectionList = new SqlParameter("@SuspectionList", suspectionList);
                        SqlParameter SQL_minimumOccurance = new SqlParameter("@MinimumOccurance", minimumOccurance);

                        suspectionOccurance = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwSuspectionAnalysi>("prfindSuspicionOccurrence @fromDate,  @ToDate, @strategyId, @SuspectionList, @MinimumOccurance", SQL_fromDate, SQL_toDate, SQL_strategyId, SQL_suspectionList, SQL_minimumOccurance).ToList();

                    }
                    
                   

                 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SuspectionOccurance.GetSuspectionOccurance", err);
            }
            return suspectionOccurance;
        }




    }
}
