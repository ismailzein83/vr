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

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class ReportDetail
    {

        public static ReportDetail Load(int id)
        {
            ReportDetail reportDetail = new ReportDetail();
            try
            {
                using (Entities context = new Entities())
                {

                    reportDetail = context.ReportDetails
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.ReportDetail.Load(" + id + ")", err);
            }
            return reportDetail;
        }



        public static bool CheckNumberExistance(string subscriberNumber,int reportId)
        {
            bool existed = false;
            try
            {
                using (Entities context = new Entities())
                {

                    if (context.ReportDetails.Where(s => s.SubscriberNumber == subscriberNumber && s.ReportId == reportId).ToList().Count() > 0)
                        existed = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.ReportDetail.CheckNumberExistance()", err);
            }
            return existed;
        }






        public static List<ReportDetail> GetList(string subscriberNumber)
        {
            //if (string.IsNullOrWhiteSpace(subscriberNumber))
            //    return GetAll();

            List<ReportDetail> reportDetails = new List<ReportDetail>();
            try
            {
                using (Entities context = new Entities())
                {

                    reportDetails = context.ReportDetails
                               .Where(s => s.SubscriberNumber == subscriberNumber)
                               .Include(s => s.Report)
                               .Include(s => s.Strategy)
                               .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.ReportDetail.GetList()", err);
            }
            return reportDetails;
        }


        public static List<ReportDetail> GetList(int reportId)
        {
            //if (string.IsNullOrWhiteSpace(subscriberNumber))
            //    return GetAll();

            List<ReportDetail> reportDetails = new List<ReportDetail>();
            try
            {
                using (Entities context = new Entities())
                {

                    reportDetails = context.ReportDetails
                               .Where(s => s.ReportId == reportId)
                               .Include(s => s.Report)
                               .Include(s => s.Strategy)
                               .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.ReportDetail.GetList()", err);
            }
            return reportDetails;
        }

        public static int Save(ReportDetail reportDetail)
        {
            int reportId = 0;
            try
            {
                using (Entities context = new Entities())
                {
                    if (reportDetail.Id == 0)
                    {
                        context.ReportDetails.Add(reportDetail);
                    }
                    else
                    {
                        context.Entry(reportDetail).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();

                }

                reportId = reportDetail.Id;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.ReportDetail.Save(Id: " + reportDetail.Id + ")", err);
            }
            return reportId;
        }

        public static bool Save(List<ReportDetail> reportDetails)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    foreach (ReportDetail rd in reportDetails)
                    {
                        if (rd.Id == 0)
                        {
                            context.ReportDetails.Add(rd);
                        }
                        else
                        {
                            context.Entry(rd).State = System.Data.EntityState.Modified;

                        }
                    }


                    context.SaveChanges();

                }

                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.ReportDetail.Save()", err);
            }
            return success;
        }

        public static bool Delete(int ID)
        {
            ReportDetail reportDetail = new ReportDetail();
            reportDetail.Id = ID;
            return Delete(reportDetail);
        }

        public static bool Delete(ReportDetail reportDetail)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.Entry(reportDetail).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.MobileCDRAnalysis.ReportDetail.Delete(" + reportDetail.Id.ToString() + ")", err);
            }
            return success;
        }




    }
}
