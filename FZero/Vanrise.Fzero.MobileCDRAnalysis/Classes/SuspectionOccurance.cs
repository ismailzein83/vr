using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using Vanrise.CommonLibrary;
using System.Data.SqlClient;






namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public class SuspectionOccurance
    {
        public string SubscriberNumber { get; set; }
        public Suspection_Level Suspection_Level { get; set; }
        public int? NumberOfOccurance { get; set; }
        public Strategy Strategy { get; set; }
        public string LastReport { get; set; }


        public static List<vwSuspectionAnalysi> GetList(int strategyId, DateTime? fromDate, DateTime? toDate, string suspectionList, int minimumOccurance)
        {
            List<vwSuspectionAnalysi> suspectionOccurance = new List<vwSuspectionAnalysi>();
         
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 18000;

                    var _fromDate = new SqlParameter("@fromDate", fromDate);
                    var _toDate = new SqlParameter("@ToDate", toDate);
                    var _strategyId = new SqlParameter("@strategyId", strategyId);
                    var _suspectionList = new SqlParameter("@SuspectionList", suspectionList);
                    var _minimumOccurance = new SqlParameter("@MinimumOccurance", minimumOccurance);

                    suspectionOccurance = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwSuspectionAnalysi>("db_findSuspectionOccurance @fromDate,  @ToDate, @strategyId, @SuspectionList, @MinimumOccurance", _fromDate, _toDate, _strategyId, _suspectionList, _minimumOccurance).ToList();
                 
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
