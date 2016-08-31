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
        OpenRDLCReportAction openRDLCReportAction;
        IInvoiceActionContext invoiceActionContext;
        Entities.Invoice invoice;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                invoiceActionContext = Vanrise.Common.Serializer.Deserialize<IInvoiceActionContext>(Request.QueryString["invoiceActionContext"]);
              
                string actionTypeName = Request.QueryString["actionTypeName"];
                InvoiceManager invoiceManager = new InvoiceManager();

                 invoice = invoiceActionContext.GetInvoice;
                InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
                var invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
                
                foreach(var action in invoiceType.Settings.UISettings.InvoiceGridActions)
                {
                    if(action.ActionTypeName == actionTypeName)
                    {
                       openRDLCReportAction = action.Settings as OpenRDLCReportAction;
                        break;
                    }
                }
                List<ReportParameter> invoiceReportParameters = new List<ReportParameter>();
                if (openRDLCReportAction != null)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath(openRDLCReportAction.ReportURL);
                    SetDataSources(ReportViewer1.LocalReport.DataSources, openRDLCReportAction.MainReportDataSources);
                    invoiceReportParameters = GetParameters(ReportViewer1.LocalReport.GetParameters(), openRDLCReportAction.MainReportParameters);
                }
                ReportViewer1.LocalReport.SetParameters(invoiceReportParameters.ToArray());
                ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);

            }
        }
        void SubreportProcessingEventHandler(object sender,SubreportProcessingEventArgs e)
        {
            
            if(openRDLCReportAction != null && openRDLCReportAction.SubReports != null && openRDLCReportAction.SubReports.Count>0)
            {
                foreach(var subReport in openRDLCReportAction.SubReports)
                {
                    if(subReport.SubReportName == e.ReportPath)
                    {
                        if(subReport.SubReportDataSources != null)
                        {
                            SetDataSources(e.DataSources, subReport.SubReportDataSources);
                        }
                    }
                }
            }
        }
        private List<ReportParameter> GetParameters(ReportParameterInfoCollection reportParameters,List<RDLCReportParameter> parameters)
        {
            List<ReportParameter> invoiceReportParameters = new List<ReportParameter>();

            if (reportParameters != null)
            {
                if (parameters != null && parameters.Count > 0)
                {
                    RDLCReportParameterValueContext paramterContext = new RDLCReportParameterValueContext
                    {
                        Invoice = invoice
                    };
                    foreach (var parameter in parameters)
                    {
                        var reportParameter = reportParameters.FirstOrDefault(x => x.Name == parameter.ParameterName);
                        if(reportParameter != null)
                        {
                            var parameterValue = parameter.Value.Evaluate(paramterContext);
                            if (parameterValue != null)
                                invoiceReportParameters.Add(new ReportParameter(parameter.ParameterName, parameterValue.ToString(), parameter.IsVisible));
                        }
                    }
                }
                PartnerManager partnerManager = new PartnerManager();
                var partnerInfo = partnerManager.GetPartnerInfo(invoice.InvoiceTypeId, invoice.PartnerId, "InvoiceRDLCReport") as Dictionary<string, VRRdlcReportParameter>;
                if (partnerInfo != null)
                {
                    foreach (var par in partnerInfo)
                    {
                        var reportParameter = reportParameters.FirstOrDefault(x => x.Name == par.Key);
                        if (reportParameter != null)
                        {
                            invoiceReportParameters.Add(new ReportParameter(par.Key, par.Value.Value, par.Value.IsVisible));
                        }
                    };
                }
            }
            
            return invoiceReportParameters;
        }
        private void SetDataSources(ReportDataSourceCollection reportDataSources, List<RDLCReportDataSource> dataSources)
        {
            if (dataSources != null && dataSources.Count > 0)
            {
                RDLCReportDataSourceSettingsContext context = new RDLCReportDataSourceSettingsContext();
                context.InvoiceActionContext = invoiceActionContext;
                foreach (var dataSource in dataSources)
                {
                    var items = dataSource.Settings.GetDataSourceItems(context);
                    ReportDataSource ds = new ReportDataSource(dataSource.DataSourceName, items);
                    reportDataSources.Add(ds);
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