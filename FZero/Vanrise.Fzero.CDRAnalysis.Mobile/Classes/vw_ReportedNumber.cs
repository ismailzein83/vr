using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;




namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class vw_ReportedNumber
    {
        public static List<vw_ReportedNumber> GetList(int ReportID)
        {

            List<vw_ReportedNumber> list_ReportedNumbers = new List<vw_ReportedNumber>();
            try
            {
                using (Entities context = new Entities())
                {
                    list_ReportedNumbers = context.vw_ReportedNumber.ToList();
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