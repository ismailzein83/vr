using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class ReportingStatus
    {
        public static List<ReportingStatus> GetReportingStatuses()
           
        {
            List<ReportingStatus> ReportingStatussList = new List<ReportingStatus>();

            try
            {
                using (Entities context = new Entities())
                {
                    ReportingStatussList = context.ReportingStatuses
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ReportingStatus.GetReportingStatuses()", err);
            }

            return ReportingStatussList;
        }
    }
}
