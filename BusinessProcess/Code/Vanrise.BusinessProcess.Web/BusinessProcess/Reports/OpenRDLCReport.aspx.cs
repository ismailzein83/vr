using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;

namespace Vanrise.BusinessProcess.Web.BusinessProcess.Reports
{
	public partial class OpenRDLCReport : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Guid printPaymentDataRecordTypeId = new Guid("f93995c3-0027-47ca-8746-6a5b1e1986ce");
				DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
				var paymentReceiptDataRecordType=dataRecordTypeManager.GetDataRecordTypeFields(printPaymentDataRecordTypeId);

				List<DefaultDataSource> defaultDataSources = new List<DefaultDataSource>();
				defaultDataSources.Add(new DefaultDataSource { Name = "Test" });

				VRTempPayloadManager vrTempPayloadManager = new VRTempPayloadManager();
				Guid tempPayloadId;
				if (!Guid.TryParse(Request.QueryString["tempPayloadId"], out tempPayloadId))
				{
					throw new Exception("error while parsing tempPayloadId");
				}
				var payload = vrTempPayloadManager.GetVRTempPayload(tempPayloadId);
				var openRDLCInput = payload.Settings as OpenRDLCReportVRTempPayload;

				ReportViewer1.ProcessingMode = ProcessingMode.Local;
				ReportViewer1.LocalReport.ReportPath = VRWebContext.MapVirtualToPhysicalPath("~/Client/Modules/BusinessProcess/Reports/OpenRDLCReport.rdlc");

				List<ReportParameter> RDLCReportParameters = new List<ReportParameter>();
				RDLCReportParameters.Add(new ReportParameter("Date", openRDLCInput.ReceivedTime.ToString(), true));

				var customerField= paymentReceiptDataRecordType.GetRecord("Customer");
				var customerDescription = customerField.Type.GetDescription(openRDLCInput.Customerid);

				var ReceivedByField = paymentReceiptDataRecordType.GetRecord("ReceivedBy");
				var ReceivedByDescription = customerField.Type.GetDescription(openRDLCInput.ReceivedBy);

				var currencyField = paymentReceiptDataRecordType.GetRecord("Currency");
				var currencyFieldDescription = currencyField.Type.GetDescription(openRDLCInput.CurrencyId);

				if (openRDLCInput.PaymentType == 1)
				{
					RDLCReportParameters.Add(new ReportParameter("IsCash", "true", true));
					RDLCReportParameters.Add(new ReportParameter("IsCheck", "false", true));
					RDLCReportParameters.Add(new ReportParameter("IsMoney", "false", true));
				}
				if (openRDLCInput.PaymentType == 2)
				{
					RDLCReportParameters.Add(new ReportParameter("IsCash", "false", true));
					RDLCReportParameters.Add(new ReportParameter("IsCheck", "true", true));
					RDLCReportParameters.Add(new ReportParameter("IsMoney", "false", true));
				}

				if (openRDLCInput.PaymentType == 3)
				{
					RDLCReportParameters.Add(new ReportParameter("IsCash", "false", true));
					RDLCReportParameters.Add(new ReportParameter("IsCheck", "false", true));
					RDLCReportParameters.Add(new ReportParameter("IsMoney", "true", true));
				}
				RDLCReportParameters.Add(new ReportParameter("CurrencySymbol", currencyFieldDescription, true));
				RDLCReportParameters.Add(new ReportParameter("Amount", openRDLCInput.Amount.ToString(), true));
				RDLCReportParameters.Add(new ReportParameter("CustomerName", customerDescription, true));
				RDLCReportParameters.Add(new ReportParameter("ReceivedBy", ReceivedByDescription, true));
				RDLCReportParameters.Add(new ReportParameter("CheckNumber", openRDLCInput.CheckNumber, true));




				ReportViewer1.LocalReport.DataSources.Clear();
				ReportViewer1.LocalReport.SetParameters(RDLCReportParameters.ToArray());
				ReportDataSource reportInvoiceComparisonSMSResultDataSource = new ReportDataSource("DefaultDataSet", defaultDataSources);
				ReportViewer1.LocalReport.DataSources.Add(reportInvoiceComparisonSMSResultDataSource);
			}
		}
	}
	public class DefaultDataSource
	{
		public string Name { get; set; }
	}
}