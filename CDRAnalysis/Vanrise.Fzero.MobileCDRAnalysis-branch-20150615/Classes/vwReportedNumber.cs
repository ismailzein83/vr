using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;




namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class vwReportedNumber
    {
        public static List<vwReportedNumber> GetList(int ReportID)
        {

            List<vwReportedNumber> list_ReportedNumbers = new List<vwReportedNumber>();
            try
            {
                using (Entities context = new Entities())
                {
                    list_ReportedNumbers = context.vwReportedNumbers.Where(x=>x.ReportID==ReportID).ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.vw_ReportedNumber.GetList("+ReportID.ToString()+")", err);
            }
            return list_ReportedNumbers;
        }
    }
}