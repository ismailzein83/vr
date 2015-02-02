using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;




namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class vw_ReportedNumberNormalCDR
    {
        public static List<vw_ReportedNumberNormalCDR> GetList(int ReportID)
        {

            List<vw_ReportedNumberNormalCDR> unNormalizedCalls = new List<vw_ReportedNumberNormalCDR>();

            try
            {
                var _ReportID = new SqlParameter("@ReportID", ReportID);


                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    unNormalizedCalls = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vw_ReportedNumberNormalCDR>("db_GetReportedNumberNormalCDR @ReportID", _ReportID).ToList();

                }




            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.vw_ReportedNumberNormalCDR.GetList(" + ReportID.ToString() + ")", err);
            }
            return unNormalizedCalls;
        }
    }
}