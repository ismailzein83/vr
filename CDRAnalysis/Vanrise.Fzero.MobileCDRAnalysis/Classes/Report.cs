using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
using System.Data.Entity.Infrastructure;

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class Report
    {

        public static int Count()
        {
            int Count = 0;
            try
            {
                using (Entities context = new Entities())
                {
                    Count = context.Reports
                        .Count();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.report.Count()", err);
            }
            return Count;
        }

        public static Report Load(int id)
        {
            Report report = new Report();
            try
            {
                using (Entities context = new Entities())
                {

                    report = context.Reports
                        .Where(s => s.Id == id)
                        .Include(s => s.ReportDetails).Include(s=>s.ReportingStatu).Include("ReportDetails.Strategy")
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.report.Load(" + id + ")", err);
            }
            return report;
        }
        public static Report Load(string reportnumber)
        {
            Report report = new Report();
            try
            {
                using (Entities context = new Entities())
                {

                    report = context.Reports
                        .Where(s => s.ReportNumber == reportnumber)
                        .Include(s => s.ReportDetails)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.report.Load(" + reportnumber + ")", err);
            }
            return report;
        }



        public static List<Report> GetList(string ReportNumber,string subscriberNumber, DateTime? fromDate, DateTime? toDate)
        {
           
            List<Report> reports = new List<Report>();
            List<ReportDetail> reportDetails = new List<ReportDetail>();
            try
            {
               using (Entities context = new Entities())
                {

                    reports = context.Reports
                                     .Include(s => s.ReportDetails)
                                     .Include(s => s.ReportingStatu)
                                     .Where(s => s.Id > 0
                                      && (ReportNumber == string.Empty || s.ReportNumber == ReportNumber)
                                      && (subscriberNumber == string.Empty || s.ReportDetails.Where(u => u.SubscriberNumber == subscriberNumber).Count() > 0)
                                      && (!fromDate.HasValue || s.ReportDate >= fromDate)
                                      && (!toDate.HasValue || s.ReportDate <= toDate))
                                     .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.ReportDetail.GetList()", err);
            }
            return reports;
        }





        public static List<Report> GetAll()
        {
            List<Report> reports = new List<Report>();
            try
            {
                using (Entities context = new Entities())
                {
                    reports = context.Reports
                       .Include(s => s.ReportDetails)
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.report.GetAll()", err);
            }
            return reports;
        }

        public static List<Report> GetTop(int number)
        {
            List<Report> reports = new List<Report>();
            try
            {
                using (Entities context = new Entities())
                {
                    reports = context.Reports
                       .Include(s => s.ReportDetails)
                       .OrderByDescending(s => s.Id)
                       .Take(number)
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.report.GetAll()", err);
            }
            return reports;
        }







        public static bool Save(Report report)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    if (report.Id == 0)
                    {
                        context.Reports.Add(report);
                    }
                    else
                    {
                        report.ReportingStatu = null;
                        context.Entry(report).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();

                }

                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Report.Save(Id: " + report.Id + ")", err);
            }
            return success;
        }

        public static void SetReportVariables(Report report)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    var result = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<vwReportVariable>("call prGetReportVariables ()").ToList().FirstOrDefault();

                    report.ReportNumber = result.ReportNumber;
                    report.ReportDate = result.ReportDate; 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.report.GetReportNumber()", err);
            }
        }



        //public static List<Report> GetList(string subscriberNumber)
        //{
        //    if (string.IsNullOrWhiteSpace(subscriberNumber))
        //        return GetAll();

        //    List<Report> reports = new List<Report>();
        //    try
        //    {
        //        using (Entities context = new Entities())
        //        {
                    
        //            reports = from rt in context.Reports
        //                       join rd in context.ReportDetails on rt.Id equals rd.ReportId
        //                       where (rd.SubscriberNumber.Contains(subscriberNumber))
        //                   select new Report()
        //                   { 
                               
        //                   };

                    
                    
                    
        //            reports = context.Reports
        //                .Include(s => s.ReportDetails)
        //                .Where(s =>
        //                    (s.ReportDetails. .Name.Contains(name))


        //                )
        //                .ToList();
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        FileLogger.Write("DataLayer.Report.GetList()", err);
        //    }
        //    return reports;
        //}



        public static bool Delete(int ID)
        {
            Report report = new Report();
            report.Id = ID;
            return Delete(report);
        }

        public static bool Delete(Report report)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.Entry(report).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.MobileCDRAnalysis.Report.Delete(" + report.Id.ToString() + ")", err);
            }
            return success;
        }






    }
}
