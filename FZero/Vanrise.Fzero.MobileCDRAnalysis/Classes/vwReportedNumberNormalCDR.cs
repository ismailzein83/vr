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




namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class vwReportedNumberNormalCDR
    {
        public static List<vwReportedNumberNormalCDR> GetList(int ReportID)
        {

            List<vwReportedNumberNormalCDR> unNormalizedCalls = new List<vwReportedNumberNormalCDR>();

            try
            {
                var _ReportID = new SqlParameter("@ReportID", ReportID);


                using (MobileEntities context = new MobileEntities())
                {
                    unNormalizedCalls = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwReportedNumberNormalCDR>("prGetReportedNumberNormalCDR @ReportID", _ReportID).ToList();

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