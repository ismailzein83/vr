using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using TOne.Entities;
using System.Web.UI.WebControls;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;
using TOne.Business;

namespace TOne.Web.Reports.Analytics
{
    public partial class ZoneProfite : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
               

                DateTime from = DateTime.ParseExact(Request.QueryString["fromDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(Request.QueryString["toDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);

                ReportDefinitionManager managerReport = new ReportDefinitionManager();

                RDLCReportDefinition rdlc = managerReport.GetRDLCReportDefinition(1);

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath(rdlc.ReportURL);

                ReportParameters parameters = new ReportParameters();

                parameters.FromTime = from;
                parameters.ToTime = to;

                IReportGenerator r =  rdlc.GetReportGenerator();

                ReportViewer1.LocalReport.DataSources.Clear();
                foreach (var a in r.GenerateDataSources(parameters))
                {
                    ReportDataSource ds = new ReportDataSource(a.Key, a.Value);               
                    ReportViewer1.LocalReport.DataSources.Add(ds);
                }
               

            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }

    }
}