using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace Vanrise.Fzero.CDRAnalysis
{
    public class CallsStatistics
    {
        public int CallsCount { get; set; }
        public decimal CallsDurations { get; set; }
        public string Prefix { get; set; }
        public int Length { get; set; }
        public string Party { get; set; }
        public string TrunckName { get; set; }
        //public int SwitchId { get; set; }


        public static string UnnormalizedCallFieldValue = "-1";

        public static List<CallsStatistics> GetUnNormalizedCalls(int switchId, string party, DateTime? fromDate, DateTime? toDate)
        {
            string DatabaseName = Switch_DatabaseConnections.GetDatabaseName(switchId);
            return GetUnNormalizedCalls(DatabaseName, party, fromDate, toDate);
        }
        public static List<CallsStatistics> GetUnNormalizedCalls(string database, string party, DateTime? fromDate, DateTime? toDate)
        {

            List<CallsStatistics> unNormalizedCalls = new List<CallsStatistics>();

            try
            {
                var _FromDate = new SqlParameter("@FromDate", fromDate);
                var _ToDate = new SqlParameter("@ToDate", toDate);
                var _Database = new SqlParameter("@Database", database);


                using (Entities context = new Entities())
                {
                    if (party == Constants.CGPN)
                    {
                        unNormalizedCalls = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<CallsStatistics>("db_GetUnNormalizedCGPN @FromDate, @ToDate, @Database", _FromDate, _ToDate, _Database).ToList();
                    }
                    else if (party == Constants.CDPN)
                    {
                        unNormalizedCalls = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<CallsStatistics>("db_GetUnNormalizedCDPN @FromDate, @ToDate, @Database", _FromDate, _ToDate, _Database).ToList();
                    }
                }
                         


             
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.CallStatistics.GetUnNormalizedCalls(" + database + ")", err);
            }
            return unNormalizedCalls;
        }
    }
}
