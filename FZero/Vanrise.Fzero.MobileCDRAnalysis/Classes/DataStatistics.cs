
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.CommonLibrary;




namespace Vanrise.Fzero.MobileCDRAnalysis
{
    [Serializable]
    public class DataStatistics
    {
        #region Properties
        public int ReportDetails { get; set; }
        public int ReportsCount { get; set; }
        public int SwitchesCount { get; set; }
        public int RulesCount { get; set; }
        public int StrategyCount { get; set; }
        public DateTime? CDRsLastDate { get; set; }
        public int NewCDRsCount { get; set; }
        public int CGPNCount { get; set; }
        public int CDPNCount { get; set; }

        #endregion

        public static DataStatistics GetStatistics()
        {
            DataStatistics dataStatistics = new DataStatistics();
            try 
            {
                using (MobileEntities context = new MobileEntities())
                {
                    dataStatistics.ReportDetails = context.ReportDetails.Count();
                    dataStatistics.ReportsCount = context.Reports.Count();
                    dataStatistics.SwitchesCount = context.SwitchProfiles.Count();
                    dataStatistics.RulesCount= context.NormalizationRules.Count();
                    dataStatistics.StrategyCount = context.Strategies.Count();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.DataStatistics.GetStatistics()", err);
            }
            return dataStatistics;
        }

        public static DataStatistics GetCDRsStatistics(string database)
        {
            vw_Dashboard dashboard = new vw_Dashboard();
            DataStatistics dataStatistics = new DataStatistics();
            try
            {
                var _Database = new SqlParameter("@Database", database);

                using (MobileEntities context = new MobileEntities())
                {
                    ((IObjectContextAdapter)context).ObjectContext.CommandTimeout = 18000;
                    dashboard = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vw_Dashboard>("db_Dashboard @Database", _Database).ToList().FirstOrDefault();
                    dataStatistics.CDRsLastDate = dashboard.MaxAttemptDateTime;
                    dataStatistics.NewCDRsCount = dashboard.CountNewCDRs;
                    dataStatistics.CDPNCount = dashboard.CountCDPN;
                    dataStatistics.CGPNCount = dashboard.CountCGPN;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.CallStatistics.GetCDRsStatistics(" + database + ")", err);
            }
            return dataStatistics;
        }
    }
}
