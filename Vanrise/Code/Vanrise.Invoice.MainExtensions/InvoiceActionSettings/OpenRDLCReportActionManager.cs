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
using Vanrise.Common;
using System.Threading;
using System.IO;
using Vanrise.Common.Business;

namespace Vanrise.Invoice.MainExtensions
{
    public class OpenRDLCReportActionManager
    {
        OpenRDLCReportAction openRDLCReportAction;
        IInvoiceActionContext invoiceActionContext;
        Entities.Invoice invoice;
        Entities.InvoiceType invoiceType;
        Dictionary<string, RepeatedReportDetails> repeatedReports;
        RecordFilterManager _manager = new RecordFilterManager();
        Dictionary<string, IEnumerable<dynamic>> mainItemsByDataSourceName;
        Dictionary<string, IEnumerable<dynamic>> nonRepeatedReportItemsByDataSourceName;
        Dictionary<string, IEnumerable<dynamic>> _currentItemsByDataSourceName = new Dictionary<string,IEnumerable<dynamic>>();


        public void BuildRdlcReport(ReportViewer reportViewer, ReportInput reportInput)
        {
                repeatedReports = new Dictionary<string, RepeatedReportDetails>();
                nonRepeatedReportItemsByDataSourceName = new Dictionary<string, IEnumerable<dynamic>>();
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

                if (invoiceAction != null && (gridAction == null || (gridAction.FilterCondition == null || gridAction.FilterCondition.IsFilterMatch(context))))
                {
                    openRDLCReportAction = invoiceAction.Settings as OpenRDLCReportAction;
                }

                List<ReportParameter> invoiceReportParameters = new List<ReportParameter>();
                if (openRDLCReportAction != null)
                {
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    if(HttpContext.Current != null)
                    {
                        reportViewer.LocalReport.ReportPath = HttpContext.Current.Server.MapPath(openRDLCReportAction.ReportURL);
                    }else
                    {
                        string currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(OpenRDLCReportActionManager)).Location);
                        reportViewer.LocalReport.ReportPath = Path.Combine(currentDir, openRDLCReportAction.ReportRuntimeURL);

                    }
                    reportViewer.LocalReport.DisplayName = new PartnerManager().EvaluateInvoiceFileNamePattern(invoice.InvoiceTypeId, invoice.PartnerId, invoice);
                   // reportViewer.LocalReport.DisplayName = String.Format("Invoice");

                    SetMainReportDataSources(reportViewer.LocalReport.DataSources, openRDLCReportAction.MainReportDataSources);
                    invoiceReportParameters = GetParameters(reportViewer.LocalReport.GetParameters(), openRDLCReportAction.MainReportParameters);
                }
                reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);
                reportViewer.LocalReport.SetParameters(invoiceReportParameters.ToArray());
        }
        void SubreportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            //Thread.Sleep(100);
            DataRecordFilterGenericFieldMatchContext context = new DataRecordFilterGenericFieldMatchContext(invoice.Details, invoiceType.Settings.InvoiceDetailsRecordTypeId);
            if (openRDLCReportAction != null)
            {
                LoadSubReport(context, e, openRDLCReportAction.SubReports,null,false);
            }
        }

        private bool LoadSubReport(DataRecordFilterGenericFieldMatchContext context, SubreportProcessingEventArgs e, List<RDLCSubReport> subReports,RDLCSubReport parentReport, bool isParentRepeated)
        {
            if(openRDLCReportAction.SubReports != null && openRDLCReportAction.SubReports.Count > 0)
            {
                foreach (var subReport in subReports)
                {
                    if (subReport.SubReportName == e.ReportPath)
                    {

                        bool loadDataSource = false;
                        RepeatedReportDetails repeatedReportDetails = null;
                        if (subReport.SubReportDataSources != null && _manager.IsFilterGroupMatch(subReport.FilterGroup, context))
                        {
                            loadDataSource = true;
                            if (subReport.RepeatedSubReport)
                            {
                                if (!repeatedReports.TryGetValue(subReport.SubReportName, out repeatedReportDetails))
                                {
                                    RDLCReportDataSourceSettingsContext reportDataSourceContext = new RDLCReportDataSourceSettingsContext();
                                    reportDataSourceContext.DataSourceItemsFunc = GetDataSourceItems;
                                    reportDataSourceContext.InvoiceActionContext = invoiceActionContext;
                                    repeatedReportDetails = new RepeatedReportDetails
                                    {
                                        Index = 0,
                                        ParentDataSourceName = subReport.ParentDataSourceName,
                                        ItemsByDataSource = new Dictionary<string, IEnumerable<dynamic>>()
                                    };
                                  
                                    repeatedReports.Add(subReport.SubReportName, repeatedReportDetails);
                                }
                                var subReportName = parentReport != null ? parentReport.SubReportName : null;
                                repeatedReportDetails.ParentDataSourceItems = GetDataSourceItems(subReport.ParentDataSourceName, subReportName);
                            }
                        }
                        SetSubReportDataSources(e.DataSources, subReport.SubReportDataSources, loadDataSource, repeatedReportDetails);
                        return true;
                    }
                    else
                    {
                        var result = LoadSubReport(context, e, subReport.SubReports, subReport, subReport.RepeatedSubReport);
                      if (result)
                          return true;
                    }
                }
            }
            return false;
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
                        Invoice = invoice,
                        InvoiceType = invoiceType
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
                    }
                }
                if (reportParameters.Any(x => x.Name == "NormalPrecisionValue"))
                {
                    invoiceReportParameters.Add(new ReportParameter("NormalPrecisionValue", new GeneralSettingsManager().GetNormalPrecisionValue().ToString(), true));
                }
            }
            return invoiceReportParameters;
        }

        private void SetMainReportDataSources(ReportDataSourceCollection reportDataSources, List<InvoiceDataSource> dataSources)
        {
            if (dataSources != null && dataSources.Count > 0)
            {
                mainItemsByDataSourceName = new Dictionary<string, IEnumerable<dynamic>>();
                RDLCReportDataSourceSettingsContext context = new RDLCReportDataSourceSettingsContext();
                context.DataSourceItemsFunc = GetDataSourceItems;
                context.InvoiceActionContext = invoiceActionContext;
                foreach (var dataSource in dataSources)
                {
                    IEnumerable<dynamic> items =  dataSource.Settings.GetDataSourceItems(context);
                    mainItemsByDataSourceName.Add(dataSource.DataSourceName, items);
                    ReportDataSource ds = new ReportDataSource(dataSource.DataSourceName, items);
                    reportDataSources.Add(ds);
                    SetCurrentDataSourceItems(dataSource.DataSourceName, items);
                }
            }
        }

        private void SetCurrentDataSourceItems(string datasourceName, IEnumerable<dynamic> items)
        {
            if (_currentItemsByDataSourceName.ContainsKey(datasourceName))
                _currentItemsByDataSourceName[datasourceName] = items;
            else
                _currentItemsByDataSourceName.Add(datasourceName, items);
            foreach(var repeatedReport in repeatedReports.Values)
            {
                if (repeatedReport.ParentDataSourceName == datasourceName)
                    repeatedReport.Index = 0;
            }
        }
        private void SetSubReportDataSources(ReportDataSourceCollection reportDataSources, List<InvoiceDataSource> dataSources, bool loadDataSource, RepeatedReportDetails repeatedReportDetails)
        {
            if (dataSources != null && dataSources.Count > 0)
            {
                RDLCReportDataSourceSettingsContext context = new RDLCReportDataSourceSettingsContext();
                context.InvoiceActionContext = invoiceActionContext;
                context.DataSourceItemsFunc = GetDataSourceItems;
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
                            nonRepeatedReportItemsByDataSourceName.Add(dataSource.DataSourceName, items);
                        }
                    }
                    SetCurrentDataSourceItems(dataSource.DataSourceName, items);
                    ReportDataSource ds = new ReportDataSource(dataSource.DataSourceName, items);
                    reportDataSources.Add(ds);
                }
            }
        }
        private IEnumerable<dynamic> GetDataSourceItems(string dataSourceName, string reportName)
        {
            IEnumerable<dynamic> dataSourceItems;
            if (!_currentItemsByDataSourceName.TryGetValue(dataSourceName, out dataSourceItems))
                throw new Exception(String.Format("Data Source '{0}' is not available in _currentItemsByDataSourceName", dataSourceName));
            return dataSourceItems;
        }
    }


    public class RepeatedReportDetails
    {
        public int Index { get; set; }
        public IEnumerable<dynamic> ParentDataSourceItems { get; set; }
        public Dictionary<string, IEnumerable<dynamic>> ItemsByDataSource { get; set; }

        public string ParentDataSourceName { get; set; }
    }
    public class ReportInput
    {
        public IInvoiceActionContext Context { get; set; }
        public Guid ActionId { get; set; }
    }
}
