using Microsoft.Reporting.WebForms;
using Retail.Interconnect.Business;
using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using System.Web.UI;
using Vanrise.Common.Business;
using Retail.BusinessEntity.Business;
using Vanrise.Invoice.Business;
using Vanrise.Entities;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;

namespace Retail.Interconnect.Web.Retail_Interconnect.Reports
{
    public partial class CompareInvoiceReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Vanrise.Security.Entities.ContextFactory.GetContext().HasPermissionToActions("Retail_Interconnect/InterconnectInvoiceController/CompareInvoices"))
                {
                    if (!IsPostBack)
                    {
                        InterconnectInvoiceManager invoiceManager = new InterconnectInvoiceManager();
                        InvoiceCompareManager invoiceCompareManager = new InvoiceCompareManager();
                        InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                        AccountBEManager accountBEManager = new AccountBEManager();

                        var vanriseInvoiceManager = new Vanrise.Invoice.Business.InvoiceManager();


                        FinancialAccountManager financialAccountManager = new FinancialAccountManager();
                        VRTempPayloadManager vrTempPayloadManager = new VRTempPayloadManager();

                        var reportInvoiceVoiceComparisonResults = new List<ReportInvoiceVoiceComparisonResult>();
                        var reportInvoiceSMSComparisonResults = new List<ReportInvoiceSMSComparisonResult>();

                        List<InvoiceComparisonVoiceResult> comparisonVoiceResults = new List<InvoiceComparisonVoiceResult>();
                        List<InvoiceComparisonSMSResult> comparisonSMSResults = new List<InvoiceComparisonSMSResult>();
                        List<ReportParameter> RDLCReportParameters = new List<ReportParameter>();

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
                            ReportViewer1.LocalReport.ReportPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/Retail_Interconnect/Reports/CustomerCompareInvoiceReport.rdlc");
                        }
                        else
                        {
                            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Client/Modules/Retail_Interconnect/Reports/SupplierCompareInvoiceReport.rdlc");
                        }

                        var invoiceTypeId = vanriseInvoiceManager.GetInvoiceTypeId(invoiceInput.InvoiceId);
                        var invoiceTypeExtendedSettings = invoiceTypeManager.GetInvoiceTypeExtendedSettings(invoiceTypeId);
                        var interconnectInvoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<InterconnectInvoiceSettings>("invoiceTypeExtendedSettings", invoiceTypeId);

                        var financialAccountMananger = new FinancialAccountManager();
                        var financialAccountData = financialAccountMananger.GetFinancialAccountData(interconnectInvoiceSettings.AccountBEDefinitionId, invoiceInput.FinancialAccountId);


                        var companyName = accountBEManager.GetAccountName(financialAccountData.Account, true);
                        RDLCReportParameters.Add(new ReportParameter("ToCompany", companyName, true));


                        var companySettings = accountBEManager.GetCompanySetting(interconnectInvoiceSettings.AccountBEDefinitionId, financialAccountData.Account.AccountId);
                        CompanyContact companyContact;
                        AccountPartCompanyProfile companyProfile;
                        RDLCReportParameters.Add(new ReportParameter("FromCompany", companySettings.CompanyName, true));
                        RDLCReportParameters.Add(new ReportParameter("FromContact", companySettings.Contacts.TryGetValue("Billing", out companyContact) ? companyContact.ContactName : String.Empty, true));

                        IAccountProfile accountProfile;
                        if (accountBEManager.HasAccountProfile(interconnectInvoiceSettings.AccountBEDefinitionId, financialAccountData.Account.AccountId, true, out accountProfile))
                        {
                            companyProfile = accountProfile.CastWithValidate<AccountPartCompanyProfile>("accountProfile", financialAccountData.Account.AccountId);

                            if (companyProfile != null)
                            {
                                if (companyProfile.Contacts != null)
                                {
                                    AccountCompanyContact accountCompanyContact;
                                    if (companyProfile.Contacts.TryGetValue("Billing", out accountCompanyContact))
                                    {

                                        if (accountCompanyContact.ContactName != null)
                                        {
                                            string contactName = "";
                                            if (accountCompanyContact.Salutation.HasValue)
                                            {
                                                contactName = string.Format("{0} ", Utilities.GetEnumDescription(accountCompanyContact.Salutation.Value));
                                            }
                                            contactName += accountCompanyContact.ContactName;
                                            RDLCReportParameters.Add(new ReportParameter("ToContact", contactName, true));
                                        }
                                    }
                                }
                            }
                        }

                        ComparisonInvoiceDetail comparisonInvoiceDetail = invoiceManager.GetInvoiceDetails(invoiceInput.InvoiceId, isCustomer ? InvoiceCarrierType.Customer : InvoiceCarrierType.Supplier);

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