using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Business;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Invoice.Web.WhS_Invoice.Reports
{
    public partial class CompareInvoiceReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Vanrise.Security.Entities.ContextFactory.GetContext().HasPermissionToActions("WhS_Invoice/WhSInvoice/CompareVoiceInvoices"))
                {
                    if (!IsPostBack)
                    {
                        InvoiceManager invoiceManager = new InvoiceManager();
                        InvoiceCompareManager invoiceCompareManager = new InvoiceCompareManager();

                        WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
                        VRTempPayloadManager vrTempPayloadManager = new VRTempPayloadManager();

                        var reportInvoiceVoiceComparisonResults = new List<ReportInvoiceVoiceComparisonResult>();
                        var reportInvoiceSMSComparisonResults = new List<ReportInvoiceSMSComparisonResult>();

                        List<InvoiceComparisonVoiceResult> comparisonVoiceResults = new List<InvoiceComparisonVoiceResult>();
                        List<InvoiceComparisonSMSResult> comparisonSMSResults = new List<InvoiceComparisonSMSResult>();

                        reportInvoiceVoiceComparisonResults.Add(new ReportInvoiceVoiceComparisonResult()
                        {
                            ProviderDuration = 0,
                            SystemDuration = 0
                        });
                        var reportInvoiceComparisonVoiceResult = reportInvoiceVoiceComparisonResults[0];

                        reportInvoiceSMSComparisonResults.Add(new ReportInvoiceSMSComparisonResult()
                        {
                            ProviderSMSs = 0,
                            SystemSMSs = 0
                        });
                        var reportInvoiceSMSComparisonResult = reportInvoiceSMSComparisonResults[0];


                        Guid tempPayloadId;
                        if (!Guid.TryParse(Request.QueryString["tempPayloadId"], out tempPayloadId))
                        {
                            throw new Exception("error while parsing tempPayloadId");
                        }
                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        var payload = vrTempPayloadManager.GetVRTempPayload(tempPayloadId);
                        var invoiceInput = payload.Settings as InvoiceComparisonVRTempPayload;
                        if (invoiceInput == null)
                            throw new Exception("invoice comparison input is null");

                        bool isCustomer = invoiceInput.IsCustomer;
                        if (isCustomer)
                        {
                            ReportViewer1.LocalReport.ReportPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/WhS_Invoice/Reports/CustomerCompareInvoiceReport.rdlc");
                        }
                        else
                        {
                            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Client/Modules/WhS_Invoice/Reports/SupplierCompareInvoiceReport.rdlc");
                        }

                        ComparisonInvoiceDetail comparisonInvoiceDetail = invoiceManager.GetInvoiceDetails(invoiceInput.InvoiceId, isCustomer ? InvoiceCarrierType.Customer : InvoiceCarrierType.Supplier);


                        List<ReportParameter> RDLCReportParameters = new List<ReportParameter>();

                        RDLCReportParameters.Add(new ReportParameter("IssueDate", comparisonInvoiceDetail.IssuedDate.ToString(), true));
                        RDLCReportParameters.Add(new ReportParameter("FromDate", comparisonInvoiceDetail.FromDate.ToString(), true));
                        RDLCReportParameters.Add(new ReportParameter("ToDate", comparisonInvoiceDetail.ToDate.ToString(), true));

                        if (invoiceInput.VoiceInput != null)
                        {
                            comparisonVoiceResults = invoiceCompareManager.CompareVoiceInvoices(invoiceInput.VoiceInput);
                            RDLCReportParameters.Add(new ReportParameter("IsVoiceCompare", "true", true));
                            RDLCReportParameters.Add(new ReportParameter("VoiceThreshold", invoiceInput.VoiceInput.Threshold.ToString(), true));


                            if (comparisonVoiceResults != null && comparisonVoiceResults.Count > 0)
                            {
                                foreach (var result in comparisonVoiceResults)
                                {
                                    if (result.SystemDuration.HasValue)
                                        reportInvoiceComparisonVoiceResult.SystemDuration += result.SystemDuration.Value;

                                    if (result.ProviderDuration.HasValue)
                                        reportInvoiceComparisonVoiceResult.ProviderDuration += result.ProviderDuration.Value;
                                }
                            }
                            reportInvoiceComparisonVoiceResult.Difference = Math.Abs(reportInvoiceComparisonVoiceResult.SystemDuration - reportInvoiceComparisonVoiceResult.ProviderDuration);

                            reportInvoiceComparisonVoiceResult.DiffPercentage = reportInvoiceComparisonVoiceResult.SystemDuration > 0 ?
                                reportInvoiceComparisonVoiceResult.Difference * 100 / reportInvoiceComparisonVoiceResult.SystemDuration : 100;

                            reportInvoiceComparisonVoiceResult.Average = reportInvoiceComparisonVoiceResult.DiffPercentage < invoiceInput.VoiceInput.Threshold ?
                                    (reportInvoiceComparisonVoiceResult.SystemDuration + reportInvoiceComparisonVoiceResult.ProviderDuration) / 2
                                    : Math.Min(reportInvoiceComparisonVoiceResult.SystemDuration, reportInvoiceComparisonVoiceResult.ProviderDuration);
                        }
                        else
                        {
                            RDLCReportParameters.Add(new ReportParameter("IsVoiceCompare", "false", true));
                            RDLCReportParameters.Add(new ReportParameter("VoiceThreshold", "0", true));
                        }

                        if (invoiceInput.SMSInput != null)
                        {
                            comparisonSMSResults = invoiceCompareManager.CompareSMSInvoices(invoiceInput.SMSInput);
                            RDLCReportParameters.Add(new ReportParameter("IsSMSCompare", "true", true));
                            RDLCReportParameters.Add(new ReportParameter("SMSThreshold", invoiceInput.SMSInput.Threshold.ToString(), true));


                            if (comparisonSMSResults != null && comparisonSMSResults.Count > 0)
                            {
                                foreach (var result in comparisonSMSResults)
                                {
                                    if (result.SystemSMSs.HasValue)
                                        reportInvoiceSMSComparisonResult.SystemSMSs += result.SystemSMSs.Value;

                                    if (result.ProviderSMSs.HasValue)
                                        reportInvoiceSMSComparisonResult.ProviderSMSs += result.ProviderSMSs.Value;
                                }
                            }
                            reportInvoiceSMSComparisonResult.Difference = reportInvoiceSMSComparisonResult.ProviderSMSs - reportInvoiceSMSComparisonResult.SystemSMSs;


                            reportInvoiceSMSComparisonResult.DiffPercentage = reportInvoiceSMSComparisonResult.SystemSMSs > 0 ?
                                reportInvoiceSMSComparisonResult.Difference * 100 / reportInvoiceSMSComparisonResult.SystemSMSs : 100;

                            reportInvoiceSMSComparisonResult.Average = reportInvoiceSMSComparisonResult.DiffPercentage < invoiceInput.SMSInput.Threshold ?
                                    (reportInvoiceSMSComparisonResult.SystemSMSs + reportInvoiceSMSComparisonResult.ProviderSMSs) / 2
                                    : Math.Min(reportInvoiceSMSComparisonResult.SystemSMSs, reportInvoiceSMSComparisonResult.ProviderSMSs);
                        }
                        else
                        {
                            RDLCReportParameters.Add(new ReportParameter("IsSMSCompare", "false", true));
                            RDLCReportParameters.Add(new ReportParameter("SMSThreshold", "0", true));
                        }


                        var companySettings = financialAccountManager.GetCompanySettings(invoiceInput.FinancialAccountId);
                        CompanyContact companyContact;

                        RDLCReportParameters.Add(new ReportParameter("FromCompany", companySettings.CompanyName, true));
                        RDLCReportParameters.Add(new ReportParameter("FromContact", companySettings.Contacts.TryGetValue("Billing", out companyContact) ? companyContact.ContactName : String.Empty, true));


                        var carrierProfile = financialAccountManager.GetCarrierProfile(invoiceInput.FinancialAccountId);

                        RDLCReportParameters.Add(new ReportParameter("ToCompany", (carrierProfile.Settings != null) ? carrierProfile.Settings.Company : String.Empty, true));
                        RDLCReportParameters.Add(new ReportParameter("ToContact", carrierProfile.Name, true));


                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.SetParameters(RDLCReportParameters.ToArray());

                        ReportDataSource reportInvoiceComparisonVoiceResultDataSource = new ReportDataSource("ReportInvoiceComparisonVoiceResult", reportInvoiceVoiceComparisonResults);
                        ReportViewer1.LocalReport.DataSources.Add(reportInvoiceComparisonVoiceResultDataSource);

                        ReportDataSource reportInvoiceComparisonSMSResultDataSource = new ReportDataSource("ReportInvoiceComparisonSMSResult", reportInvoiceSMSComparisonResults);
                        ReportViewer1.LocalReport.DataSources.Add(reportInvoiceComparisonSMSResultDataSource);
                    }
                }
            }

            catch (Exception ex)
            {
                Vanrise.Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }

        }
        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                base.Render(writer);
                GC.Collect();
            }
            catch (Exception ex)
            {
                Vanrise.Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }
        }
    }
}