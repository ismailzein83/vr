using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass
{
    public partial class CasesLog
    {
        public static bool SaveBulk(string tableName, List<CasesLog> listCasesLogs)
        {
            bool success = false;
            try
            {
                Manager.InsertData(listCasesLogs.ToList(), tableName, "FMSConnectionString");
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.CasesLog.SaveBulk(" + listCasesLogs.Count.ToString() + ")", err);
            }
            return success;
        }

        public static dynamic GetCaseLog(int ID)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    var CasesLogsList_ReportingStatus = context.CasesLogs
                                          .Where(u => u.GeneratedCallID == ID)
                                            .OrderByDescending(u => u.ID)
                                            .Join(context.ReportingStatuses, x => x.ReportingStatusID, s => s.ID, ((x, s) => new { ChangeName = (x.ReportingStatus != null ? s.Name : string.Empty), x.UpdatedOn })).ToList();

                    var CasesLogsList_Status = context.CasesLogs
                                         .Where(u => u.GeneratedCallID == ID)
                                           .OrderByDescending(u => u.ID)
                                           .Join(context.Statuses, x => x.StatusID, s => s.ID, ((x, s) => new { ChangeName = (x.Status != null ? s.Name : string.Empty), x.UpdatedOn })).ToList();

                    var CasesLogsList_MobileOperatorFeedback = context.CasesLogs
                                         .Where(u => u.GeneratedCallID == ID)
                                           .OrderByDescending(u => u.ID)
                                           .Join(context.MobileOperatorFeedbacks, x => x.MobileOperatorFeedbackID, s => s.ID, ((x, s) => new { ChangeName = (x.MobileOperatorFeedback != null ? s.Name : string.Empty), x.UpdatedOn })).ToList();


                    var CasesLogsList = CasesLogsList_ReportingStatus.Union(CasesLogsList_Status).Union(CasesLogsList_MobileOperatorFeedback).OrderByDescending(u => u.UpdatedOn);
                    return CasesLogsList;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.CasesLog.GetCaseLog()", err);
            }

            return null;
            
        }
    }
}
