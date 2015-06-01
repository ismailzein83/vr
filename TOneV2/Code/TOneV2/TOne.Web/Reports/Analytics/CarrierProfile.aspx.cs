using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;

namespace TOne.Web.Reports.Analytics
{
    public partial class CarrierProfie : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Analytics/CarrierProfile.rdlc");

                BillingStatisticManager manager = new BillingStatisticManager();
          


                List<MonthTraffic> monthTraffic = manager.GetMonthTraffic(DateTime.Parse("2012-05-01 00:00:00"), DateTime.Parse("2015-05-01 00:00:00"), "C060", true);


                ReportDataSource ds = new ReportDataSource("MonthTraffic", monthTraffic);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(ds);

                


            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }

    }
}