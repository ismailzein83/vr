using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class OpenRDLCReportActionManager
    {
        OpenRDLCReportAction openRDLCReportAction;
        IInvoiceActionContext invoiceActionContext;
        Entities.Invoice invoice;
        Entities.InvoiceType invoiceType;
        Dictionary<string, RepeatedReportDetails> repeatedReports;
        public void BuildRdlcReport(ReportViewer reportViewer, ReportInput reportInput)
        {
            try
            {
                repeatedReports = new Dictionary<string, RepeatedReportDetails>();
                invoiceActionContext = reportInput.Context;

                InvoiceManager invoiceManager = new InvoiceManager();

                invoice = reportInput.Context.GetInvoice;
                InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
                invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);

                var invoiceAction = invoiceType.Settings.InvoiceActions.FirstOrDefault(x => x.InvoiceActionId == reportInput.ActionId);
                var gridAction = invoiceType.Settings.InvoiceGridSettings.InvoiceGridActions.FirstOrDefault(x => x.InvoiceGridActionId == reportInput.ActionId);

                InvoiceGridActionFilterConditionContext context = new InvoiceGridActionFilterConditionContext
                {
                    Invoice = invoice,
                    InvoiceType = invoiceType
                };

                if (invoiceAction != null && (gridAction.FilterCondition == null || gridAction.FilterCondition.IsFilterMatch(context)))
                {
                    openRDLCReportAction = invoiceAction.Settings as OpenRDLCReportAction;
                }

                List<ReportParameter> invoiceReportParameters = new List<ReportParameter>();
                if (openRDLCReportAction != null)
                {
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.LocalReport.ReportPath = HttpContext.Current.Server.MapPath(openRDLCReportAction.ReportURL);
                    reportViewer.LocalReport.DisplayName = String.Format("Invoice");
                    SetDataSources(reportViewer.LocalReport.DataSources, openRDLCReportAction.MainReportDataSources, true, null);
                    invoiceReportParameters = GetParameters(reportViewer.LocalReport.GetParameters(), openRDLCReportAction.MainReportParameters);
                }
                reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
                reportViewer.LocalReport.SetParameters(invoiceReportParameters.ToArray());


            }
            catch (Exception error)
            {
                throw error;
            }
        }
        void SubreportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {

            RecordFilterManager manager = new RecordFilterManager();
            DataRecordFilterGenericFieldMatchContext context = new DataRecordFilterGenericFieldMatchContext(invoice.Details, invoiceType.Settings.InvoiceDetailsRecordTypeId);
            if (openRDLCReportAction != null && openRDLCReportAction.SubReports != null && openRDLCReportAction.SubReports.Count > 0)
            {
                foreach (var subReport in openRDLCReportAction.SubReports)
                {
                    if (subReport.SubReportName == e.ReportPath)
                    {

                        bool loadDataSource = false;
                        RepeatedReportDetails repeatedReportDetails = null;
                        if (subReport.SubReportDataSources != null && manager.IsFilterGroupMatch(subReport.FilterGroup, context))
                        {
                            loadDataSource = true;
                            if (subReport.RepeatedSubReport)
                            {
                                if (!repeatedReports.TryGetValue(subReport.SubReportName, out repeatedReportDetails))
                                {
                                    RDLCReportDataSourceSettingsContext reportDataSourceContext = new RDLCReportDataSourceSettingsContext();
                                    reportDataSourceContext.InvoiceActionContext = invoiceActionContext;
                                    repeatedReportDetails = new RepeatedReportDetails
                                    {
                                        Index = 0,
                                        ParentDataSourceItems = subReport.ParentSubreportDataSource.Settings.GetDataSourceItems(reportDataSourceContext),
                                        ItemsByDataSource = new Dictionary<string, IEnumerable<dynamic>>()
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
        private List<ReportParameter> GetParameters(ReportParameterInfoCollection reportParameters, List<RDLCReportParameter> parameters)
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
                        if (reportParameter != null)
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

                                dataSourceItems = dataSource.Settings.GetDataSourceItems(context);
                                repeatedReportDetails.ItemsByDataSource.Add(dataSource.DataSourceName, dataSourceItems);
                            }
                            itemsFilterContext.Items = dataSourceItems;
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
    }
    public class RepeatedReportDetails
    {
        public int Index { get; set; }
        public IEnumerable<dynamic> ParentDataSourceItems { get; set; }
        public Dictionary<string, IEnumerable<dynamic>> ItemsByDataSource { get; set; }
    }
    public class ReportInput
    {
        public IInvoiceActionContext Context { get; set; }
        public Guid ActionId { get; set; }
    }
}
