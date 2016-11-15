using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
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
        Entities.InvoiceType invoiceType;
        Dictionary<string, RepeatedReportDetails> repeatedReports;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                try
                {
                    repeatedReports = new Dictionary<string, RepeatedReportDetails>();
                    invoiceActionContext = Vanrise.Common.Serializer.Deserialize<IInvoiceActionContext>(Request.QueryString["invoiceActionContext"]);

                    string actionTypeName = Request.QueryString["actionTypeName"];
                    InvoiceManager invoiceManager = new InvoiceManager();
                    string actionIdString = Request.QueryString["actionId"];
                    Guid actionId = Guid.Empty;
                    if (actionIdString != null)
                        actionId = new Guid(actionIdString);

                    invoice = invoiceActionContext.GetInvoice;
                    InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
                    invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);

                    foreach (var action in invoiceType.Settings.UISettings.InvoiceGridActions)
                    {
                        InvoiceFilterConditionContext context = new InvoiceFilterConditionContext
                        {
                            Invoice = invoice,
                            InvoiceType = invoiceType
                        };
                        if (action.InvoiceFilterCondition.IsFilterMatch(context) && action.Settings.ActionTypeName == actionTypeName)
                        {
                            openRDLCReportAction = action.Settings as OpenRDLCReportAction;
                            if (actionId == Guid.Empty || actionId == openRDLCReportAction.ActionId)
                              break;
                        }
                    }
                    List<ReportParameter> invoiceReportParameters = new List<ReportParameter>();
                    if (openRDLCReportAction != null)
                    {
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath(openRDLCReportAction.ReportURL);
                        this.ReportViewer1.LocalReport.DisplayName = String.Format("Invoice");
                        SetDataSources(ReportViewer1.LocalReport.DataSources, openRDLCReportAction.MainReportDataSources, true, null);
                        invoiceReportParameters = GetParameters(ReportViewer1.LocalReport.GetParameters(), openRDLCReportAction.MainReportParameters);
                    }
                    ReportViewer1.LocalReport.SetParameters(invoiceReportParameters.ToArray());
                    ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
                }
                catch(Exception error)
                {
                    labelError.Text = error.Message;
                }

            }
        }
        void SubreportProcessingEventHandler(object sender,SubreportProcessingEventArgs e)
        {
           
            RecordFilterManager manager = new RecordFilterManager();
            DataRecordFilterGenericFieldMatchContext context = new DataRecordFilterGenericFieldMatchContext(invoice.Details, invoiceType.Settings.InvoiceDetailsRecordTypeId);
            if(openRDLCReportAction != null && openRDLCReportAction.SubReports != null && openRDLCReportAction.SubReports.Count>0)
            {
                foreach(var subReport in openRDLCReportAction.SubReports)
                {
                    if(subReport.SubReportName == e.ReportPath)
                    {

                        bool loadDataSource = false;
                        RepeatedReportDetails repeatedReportDetails = null;
                        if(subReport.SubReportDataSources != null &&  manager.IsFilterGroupMatch(subReport.FilterGroup, context))
                        {
                            loadDataSource = true;
                            if(subReport.RepeatedSubReport)
                            {
                                if (!repeatedReports.TryGetValue(subReport.SubReportName, out repeatedReportDetails))
                                {
                                    RDLCReportDataSourceSettingsContext reportDataSourceContext = new RDLCReportDataSourceSettingsContext();
                                    reportDataSourceContext.InvoiceActionContext = invoiceActionContext;
                                    repeatedReportDetails = new RepeatedReportDetails
                                    {
                                        Index = 0,
                                        ParentDataSourceItems = subReport.ParentSubreportDataSource.Settings.GetDataSourceItems(reportDataSourceContext).ToList(),
                                        ItemsByDataSource = new Dictionary<string,IEnumerable<dynamic>>()
                                    };
                                    repeatedReports.Add(subReport.SubReportName, repeatedReportDetails);
                                }
                            }
                        }
                        SetDataSources(e.DataSources, subReport.SubReportDataSources, loadDataSource, repeatedReportDetails);
                        break;
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
        private void SetDataSources(ReportDataSourceCollection reportDataSources, List<InvoiceDataSource> dataSources, bool loadDataSource, RepeatedReportDetails repeatedReportDetails)
        {
            if (dataSources != null && dataSources.Count > 0)
            {
                RDLCReportDataSourceSettingsContext context = new RDLCReportDataSourceSettingsContext();
                context.InvoiceActionContext = invoiceActionContext;
                foreach (var dataSource in dataSources)
                {
                    IEnumerable<dynamic> items = new List<dynamic>();
                    if (loadDataSource)
                    {
                        if (repeatedReportDetails != null)
                        {
                            ItemsFilterContext itemsFilterContext = new ItemsFilterContext
                            {
                                ParentItem = repeatedReportDetails.ParentDataSourceItems.ElementAt(repeatedReportDetails.Index)
                            };
                            IEnumerable<dynamic> dataSourceItems = null;
                            if (!repeatedReportDetails.ItemsByDataSource.TryGetValue(dataSource.DataSourceName, out dataSourceItems))
                            {
                                
                                dataSourceItems = dataSource.Settings.GetDataSourceItems(context).ToList();
                                repeatedReportDetails.ItemsByDataSource.Add(dataSource.DataSourceName, dataSourceItems);
                            }
                            itemsFilterContext.Items = dataSourceItems.ToList();
                            repeatedReportDetails.Index++;
                            items = dataSource.ItemsFilter.GetFilteredItems(itemsFilterContext);
                        }
                        else
                        {
                            items = dataSource.Settings.GetDataSourceItems(context);
                        }
                    }
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
    public class RepeatedReportDetails
    {
        public int Index { get; set; }
        public List<dynamic> ParentDataSourceItems { get; set; }
        public Dictionary<string,IEnumerable<dynamic>> ItemsByDataSource { get; set; }
    }
}