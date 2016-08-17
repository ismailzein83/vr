using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;

namespace Vanrise.Invoice.Web.VR_Invoice.Reports
{
    public partial class CustomerInvoiceReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                long invoiceId = Convert.ToInt32(Request.QueryString["invoiceId"]);
                string actionTypeName = Request.QueryString["actionTypeName"];
                InvoiceManager invoiceManager = new InvoiceManager();

                var invoice = invoiceManager.GetInvoice(invoiceId);

                InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
                var invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
                OpenRDLCReportAction openRDLCReportAction = null;
                foreach(var action in invoiceType.Settings.UISettings.InvoiceGridActions)
                {
                    if(action.ActionTypeName == actionTypeName)
                    {
                       openRDLCReportAction = action.Settings as OpenRDLCReportAction;
                        break;
                    }
                }
                if(openRDLCReportAction !=null)
                {
                    ReportViewer1.LocalReport.DataSources.Clear();
                    InvoiceItemManager manager = new InvoiceItemManager();
                    RDLCReportDataSourceSettingsContext context = new RDLCReportDataSourceSettingsContext();
                    context.InvoiceId = invoiceId;
                    foreach(var dataSource in openRDLCReportAction.DataSources)
                    {
                        var items = dataSource.Settings.GetDataSourceItems(context);
                        ReportDataSource ds = new ReportDataSource(dataSource.DataSourceName, items);
                        ReportViewer1.LocalReport.DataSources.Add(ds);
                    }
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath(openRDLCReportAction.ReportURL);
                }
                //List<ReportParameter> customerInvoiceReportParameters = new List<ReportParameter>();
                ////foreach (var p in GetRdlcReportParameters(invoice))
                ////{
                ////    customerInvoiceReportParameters.Add(new ReportParameter(p.Key, p.Value.Value, p.Value.IsVisible));
                ////}
                //ReportViewer1.LocalReport.SetParameters(customerInvoiceReportParameters.ToArray());
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }
    }
}