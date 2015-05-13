using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TOne.Web.Reports
{
    public partial class ReportPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/TestReport.rdlc");
                List<ReportModel> dataSource = new List<ReportModel>();
                dataSource.Add(new ReportModel { Title = "Item 1", Value = 4 });
                dataSource.Add(new ReportModel { Title = "Item 2", Value = 6 });
                for(int i=0;i<10000;i++)
                {
                    dataSource.Add(new ReportModel { Title = "Item " + i.ToString(), Value = i * 3 });
                }

                ReportDataSource datasource = new ReportDataSource("ReportModel", dataSource);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(datasource);
                
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }
    }
}