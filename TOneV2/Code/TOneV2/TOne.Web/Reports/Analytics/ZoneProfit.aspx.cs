using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;

namespace TOne.Web.Reports.Analytics
{
    public partial class ZoneProfite : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Analytics/rdlZoneProfits.rdlc");

                BillingStatisticManager manager = new BillingStatisticManager();
                //  string showCustomer = Request.QueryString["showCustomer"];
                //  DateTime fromDate = DateTime.Parse(Request.QueryString["from"]);

                List<ZoneProfitFormatted> zoneProfit = manager.GetZoneProfit(DateTime.Parse("2012-05-01 00:00:00"), DateTime.Parse("2015-05-01 00:00:00"), "Y");

              

                ReportDataSource ds = new ReportDataSource("ZoneProfit", zoneProfit);               
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(ds);

                //ReportParameter[] parameters = new ReportParameter[1];
                //parameters[0] = new ReportParameter("GroupByCustomer", "true", false);
                //ReportViewer1.LocalReport.SetParameters(parameters);


            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }

    }
}