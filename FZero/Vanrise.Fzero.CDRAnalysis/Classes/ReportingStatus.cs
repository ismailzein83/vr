using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.CDRAnalysis.Providers;
namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class ReportingStatu
    {
        public static List<ReportingStatu> GetAll()
        {
            List<ReportingStatu> ListReportingStatus = new List<ReportingStatu>();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    ListReportingStatus = context.ReportingStatus
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.ReportingStatus.GetAll()", err);
            }
            return ListReportingStatus;
        }
    }
}
