using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Entities;
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
                IInvoiceActionContext invoiceActionContext = Vanrise.Common.Serializer.Deserialize<IInvoiceActionContext>(Request.QueryString["invoiceActionContext"]);
              
                string actionTypeName = Request.QueryString["actionTypeName"];
                InvoiceManager invoiceManager = new InvoiceManager();

                var invoice = invoiceActionContext.GetInvoice;
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
                List<ReportParameter> invoiceReportParameters = new List<ReportParameter>();
                if (openRDLCReportAction != null )
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath(openRDLCReportAction.ReportURL);
                    if(openRDLCReportAction.DataSources != null)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        InvoiceItemManager manager = new InvoiceItemManager();
                        RDLCReportDataSourceSettingsContext context = new RDLCReportDataSourceSettingsContext();
                        context.InvoiceActionContext = invoiceActionContext;
                        foreach (var dataSource in openRDLCReportAction.DataSources)
                        {
                            var items = dataSource.Settings.GetDataSourceItems(context);
                            ReportDataSource ds = new ReportDataSource(dataSource.DataSourceName, items);
                            ReportViewer1.LocalReport.DataSources.Add(ds);
                        }
                    }
                    if (openRDLCReportAction.Parameters != null)
                    {
                       
                        RDLCReportParameterValueContext paramterContext = new RDLCReportParameterValueContext{
                            Invoice = invoice
                        };
                        foreach(var parameter in openRDLCReportAction.Parameters)
                        {
                            var parameterValue = parameter.Value.Evaluate(paramterContext);
                            if(parameterValue != null)
                            invoiceReportParameters.Add(new ReportParameter(parameter.ParameterName,parameterValue.ToString(), parameter.IsVisible));
                        }
                    }
                }

                PartnerManager partnerManager = new PartnerManager();
                var partnerInfo = partnerManager.GetPartnerInfo(invoice.InvoiceTypeId, invoice.PartnerId, "InvoiceRDLCReport") as Dictionary<string, VRRdlcReportParameter>;
                if (partnerInfo != null)
                {
                    foreach (var par in partnerInfo)
                    {
                        invoiceReportParameters.Add(new ReportParameter(par.Key, par.Value.Value, par.Value.IsVisible));
                    };
                }
                ReportViewer1.LocalReport.SetParameters(invoiceReportParameters.ToArray());
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }
    }
}