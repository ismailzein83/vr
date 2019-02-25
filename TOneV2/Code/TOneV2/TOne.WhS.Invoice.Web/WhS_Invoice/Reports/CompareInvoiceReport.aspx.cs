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
				if (Vanrise.Security.Entities.ContextFactory.GetContext().HasPermissionToActions("WhS_Invoice/WhSInvoice/CompareInvoices"))
				{
					if (!IsPostBack)
					{
						//InvoiceManager invoiceManager = new InvoiceManager();
						//WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
						//VRTempPayloadManager vrTempPayloadManager = new VRTempPayloadManager();

						//List<ReportInvoiceComparisonResult> reportInvoiceComparisonResults = new List<ReportInvoiceComparisonResult>();
						//List<ReportInvoiceComparisonInput> reportInvoiceComparisonInputs = new List<ReportInvoiceComparisonInput>();
						//List<ReportComparisonInvoiceDetail> reportComparisonInvoiceDetails = new List<ReportComparisonInvoiceDetail>();
						//List<ReportInvoiceComparisonCarrierProfile> reportInvoiceComparisonCarrierProfiles = new List<ReportInvoiceComparisonCarrierProfile>();
						//List<ReportInvoiceCamparisonCompanySettings> reportInvoiceCamparisonCompanySettings = new List<ReportInvoiceCamparisonCompanySettings>();


						//Guid tempPayloadId;
						//if (!Guid.TryParse(Request.QueryString["tempPayloadId"], out tempPayloadId))
						//{
						//	throw new Exception("error while parsing tempPayloadId");
						//}
						//ReportViewer1.ProcessingMode = ProcessingMode.Local;
						//var payload = vrTempPayloadManager.GetVRTempPayload(tempPayloadId);
						//var invoiceInput = payload.Settings as InvoiceComparisonInput;
						//if (invoiceInput == null)
						//	throw new Exception("invoice comparison input is null");

						//bool isCustomer = invoiceInput.IsCustomer;
						//if (isCustomer)
						//{
						//	ReportViewer1.LocalReport.ReportPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/WhS_Invoice/Reports/CustomerCompareInvoiceReport.rdlc");
						//}
						//else
						//{
						//	ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Client/Modules/WhS_Invoice/Reports/SupplierCompareInvoiceReport.rdlc");
						//}

						//List<InvoiceComparisonResult> comparisonResults = invoiceManager.CompareInvoices(invoiceInput);
						//if (comparisonResults != null)
						//{
						//	foreach (var result in comparisonResults)
						//	{
						//		reportInvoiceComparisonResults.Add(new ReportInvoiceComparisonResult
						//		{
						//			Destination = result.Destination,
						//			From = result.From,
						//			To = result.To,
						//			Currency = result.Currency,
						//			SystemRate = result.SystemRate,
						//			ProviderRate = result.ProviderRate,
						//			SystemDuration = result.SystemDuration,
						//			ProviderDuration = result.ProviderDuration,
						//			SystemAmount = result.ProviderAmount,
						//			ProviderAmount = result.ProviderAmount,
						//			SystemCalls = result.ProviderCalls,
						//			ProviderCalls = result.ProviderCalls,
						//			DiffCallsPercentage = result.DiffCallsPercentage,
						//			DiffCalls = result.DiffCalls,
						//			DiffCallsColor = result.DiffCallsColor,
						//			DiffDuration = result.DiffDuration,
						//			DiffDurationPercentage = result.DiffDurationPercentage,
						//			DiffDurationColor = result.DiffDurationColor,
						//			DiffAmount = result.DiffAmount,
						//			DiffAmountPercentage = result.DiffAmountPercentage,
						//			DiffAmountColor = result.DiffCallsColor,
						//			Result = result.Result,
						//			ResultColor = result.ResultColor,
						//		});
						//	}
						//}

						//reportInvoiceComparisonInputs.Add(new ReportInvoiceComparisonInput
						//{
						//	Threshold = invoiceInput.Threshold
						//});

						//ComparisonInvoiceDetail comparisonInvoiceDetail = invoiceManager.GetInvoiceDetails(invoiceInput.InvoiceId, isCustomer ? InvoiceCarrierType.Customer : InvoiceCarrierType.Supplier);
						//reportComparisonInvoiceDetails.Add(new ReportComparisonInvoiceDetail
						//{
						//	IssuedDate = comparisonInvoiceDetail.IssuedDate
						//});

						//var companySettings = financialAccountManager.GetCompanySettings(invoiceInput.FinancialAccountId);
						//CompanyContact companyContact;
						//reportInvoiceCamparisonCompanySettings.Add(new ReportInvoiceCamparisonCompanySettings
						//{
						//	CompanyName = companySettings.CompanyName,
						//	ContactName = companySettings.Contacts.TryGetValue("Billing", out companyContact) ? companyContact.ContactName : String.Empty
						//});


						//var carrierProfile = financialAccountManager.GetCarrierProfile(invoiceInput.FinancialAccountId);
						//reportInvoiceComparisonCarrierProfiles.Add(new ReportInvoiceComparisonCarrierProfile
						//{
						//	Company = (carrierProfile.Settings != null) ? carrierProfile.Settings.Company : String.Empty,
						//	ProfileName = carrierProfile.Name
						//});


						//ReportViewer1.LocalReport.DataSources.Clear();


						//ReportDataSource reportInvoiceComparisonResultDataSource = new ReportDataSource("ReportInvoiceComparisonResult", reportInvoiceComparisonResults);
						//ReportViewer1.LocalReport.DataSources.Add(reportInvoiceComparisonResultDataSource);

						//ReportDataSource reportInvoiceComparisonInputsDataSource = new ReportDataSource("ReportInvoiceComparisonInputs", reportInvoiceComparisonInputs);
						//ReportViewer1.LocalReport.DataSources.Add(reportInvoiceComparisonInputsDataSource);

						//ReportDataSource reportComparisonInvoiceDetailsDataSource = new ReportDataSource("ReportComparisonInvoiceDetails", reportComparisonInvoiceDetails);
						//ReportViewer1.LocalReport.DataSources.Add(reportComparisonInvoiceDetailsDataSource);

						//ReportDataSource reportInvoiceCamparisonCompanySettingsDataSource = new ReportDataSource("ReportInvoiceCamparisonCompanySettings", reportInvoiceCamparisonCompanySettings);
						//ReportViewer1.LocalReport.DataSources.Add(reportInvoiceCamparisonCompanySettingsDataSource);

						//ReportDataSource reportInvoiceComparisonCarrierProfilesDataSource = new ReportDataSource("ReportInvoiceComparisonCarrierProfiles", reportInvoiceComparisonCarrierProfiles);
						//ReportViewer1.LocalReport.DataSources.Add(reportInvoiceComparisonCarrierProfilesDataSource);
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