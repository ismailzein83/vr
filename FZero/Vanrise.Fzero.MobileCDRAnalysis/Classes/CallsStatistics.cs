using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace Vanrise.Fzero.MobileCDRAnalysis
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
            List<CallsStatistics> unNormalizedCalls = new List<CallsStatistics>();

            try
            {
                var _FromDate = new SqlParameter("@FromDate", fromDate);
                var _ToDate = new SqlParameter("@ToDate", toDate);
                var _SwitchID = new SqlParameter("@SwitchID", switchId);


                using (MobileEntities context = new MobileEntities())
                {
                    if (party == Constants.MSISDN)
                    {
                        unNormalizedCalls = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<CallsStatistics>("prGetUnNormalizedCGPN @FromDate, @ToDate, @SwitchID", _FromDate, _ToDate, _SwitchID).ToList();
                    }
                    else if (party == Constants.Destination)
                    {
                        unNormalizedCalls = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<CallsStatistics>("prGetUnNormalizedCDPN @FromDate, @ToDate, @SwitchID", _FromDate, _ToDate, _SwitchID).ToList();
                    }
                }




            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.CallStatistics.GetUnNormalizedCalls()", err);
            }
            return unNormalizedCalls;
        }
       
    }
}
