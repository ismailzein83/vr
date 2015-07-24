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

                List<MonthTraffic> monthTrafficSale = manager.GetMonthTraffic(DateTime.Parse("2012-05-01 00:00:00"), DateTime.Parse("2013-05-01 00:00:00"), "C060", true);

                List<CarrierProfileReport> carrierProfile1 = manager.GetCarrierProfileMTDAndMTA(DateTime.Parse("2012-06-01 00:00:00"), DateTime.Parse("2015-06-01 00:00:00"), "C060", false);

                ReportDataSource dsMonthTrafficSale = new ReportDataSource("MonthTrafficSale", monthTrafficSale);
                ReportDataSource cp1 = new ReportDataSource("CarrierProfile1", carrierProfile1);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(dsMonthTrafficSale);
                ReportViewer1.LocalReport.DataSources.Add(cp1);
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }

    }
}