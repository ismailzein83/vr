using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOne.Web.Reports
{
    
    public class ReportModel
    {
        public void TestMethod(ReportModel model)
        {

        }

        public List<ReportModel> GetReportData()
        {
            return null;
        }
        public string Title { get; set; }

        public decimal Value { get; set; }
    }

    public class ReportModelMatirx
    {
        public void TestMethod(ReportModelMatirx model)
        {

        }

        public List<ReportModelMatirx> GetReportMatrix()
        {
            return null;
        }
        public string ZoneName { get; set; }
        public string Month { get; set; }

        public decimal Attemps { get; set; }
    }

    public class ReportSubModelMatirx
    {
        public void TestMethod(ReportModelMatirx model)
        {

        }

        public List<ReportSubModelMatirx> GetSubReportMatrix()
        {
            return null;
        }
        public string Day { get; set; }

        public decimal Attemps { get; set; }

        public string Month { get; set; }
    }
}