using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class Report
    {
        public static Report Save(Report Report)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    context.Reports.Add(Report);
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Report.Save(" + Report.ID.ToString() + ")", err);
            }
            return Report;
        }

        public static Report Load(string PartofReportCode)
        {
            Report Report = new Report();
            try
            {
                using (Entities context = new Entities())
                {
                    Report = context.Reports
                       .Where(u => u.ReportID.Contains(PartofReportCode)).OrderByDescending(x=>x.ID)
                       .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Report.Load(" + PartofReportCode + ")", err);
            }
            return Report;
        }

        public static List<Report> GetAllReports()
        {
            List<Report> ReportsList = new List<Report>();

            try
            {
                using (Entities context = new Entities())
                {
                    ReportsList = context.Reports
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Report.GetReports()", err);
            }

            return ReportsList;
        }
    }
}
