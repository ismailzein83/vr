using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities.BillingReport;
using Microsoft.Reporting.WebForms;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
namespace TOne.WhS.Analytics.Web.Reports.Analytics
{
    public partial class BillingReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Vanrise.Security.Entities.ContextFactory.GetContext().HasPermissionToActions("WhS_Analytics/ReportDefinition/GetAllRDLCReportDefinition"))
            {
                if (!IsPostBack)
                {
                    int reportId = Convert.ToInt32(Request.QueryString["reportId"]);
                    DateTime from = DateTime.ParseExact(Request.QueryString["fromDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime? to = !string.IsNullOrEmpty(Request.QueryString["toDate"]) ? DateTime.ParseExact(Request.QueryString["toDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture) : default(DateTime?);

                    string customers = Request.QueryString["customer"];
                    string suppliers = Request.QueryString["supplier"];
                    int currencyId = Convert.ToInt32(Request.QueryString["currency"]);

                    CurrencyManager currencyManager = new CurrencyManager();
                    Currency currency = new Currency();
                    currency = currencyManager.GetCurrency(currencyId);


                    bool groupByCustomer = (Request.QueryString["groupByCustomer"] != null) ? (Request.QueryString["groupByCustomer"] == "true") : false;
                    bool isCost = (Request.QueryString["isCost"] != null) ? (Request.QueryString["isCost"] == "true") : false;
                    bool service = (Request.QueryString["service"] != null) ? (Request.QueryString["service"] == "true") : false;
                    bool commission = (Request.QueryString["commission"] != null) ? (Request.QueryString["commission"] == "true") : false;
                    bool bySupplier = (Request.QueryString["bySupplier"] != null) ? (Request.QueryString["bySupplier"] == "true") : false;
                    bool isExchange = (Request.QueryString["isExchange"] != null) ? (Request.QueryString["isExchange"] == "true") : false;

                    bool pageBreak = (Request.QueryString["pageBreak"] != null) ? (Request.QueryString["pageBreak"] == "true") : false;
                    bool groupByProfile = (Request.QueryString["groupByProfile"] != null) ? (Request.QueryString["groupByProfile"] == "true") : false;

                    int margin = (Request.QueryString["margin"] != null) ? Convert.ToInt32(Request.QueryString["margin"]) : 10;
                    int top = (Request.QueryString["top"] != null) ? Convert.ToInt32(Request.QueryString["top"]) : 10;
                    string zones = Request.QueryString["zone"];

                    ReportDefinitionManager managerReport = new ReportDefinitionManager();

                    RDLCReportDefinition rdlc = managerReport.GetRDLCReportDefinition(reportId);

                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath(rdlc.ReportURL);

                    ReportParameters parameters = new ReportParameters();

                    parameters.FromTime = from;
                    parameters.ToTime = (from == to) ? to.Value.AddDays(1) : to;

                    parameters.GroupByCustomer = groupByCustomer;
                    parameters.CustomersId = customers;
                    parameters.SuppliersId = suppliers;
                    parameters.IsCost = isCost;
                    parameters.IsService = service;
                    parameters.IsCommission = commission;
                    parameters.GroupBySupplier = bySupplier;
                    parameters.CurrencyId = currencyId;
                    parameters.Margin = margin;
                    parameters.ZonesId = zones;
                    parameters.IsExchange = isExchange;
                    parameters.Top = top;
                    parameters.CurrencyDescription = String.Format("[{0}] {1}", currency.Symbol, currency.Name);
                    parameters.PageBreak = pageBreak;
                    parameters.GroupByProfile = groupByProfile;

                    IReportGenerator r = rdlc.GetReportGenerator();

                    ReportViewer1.LocalReport.DataSources.Clear();
                    foreach (var a in r.GenerateDataSources(parameters))
                    {
                        ReportDataSource ds = new ReportDataSource(a.Key, a.Value);
                        ReportViewer1.LocalReport.DataSources.Add(ds);
                    }
                    List<ReportParameter> BillingRDLCReportParameters = new List<ReportParameter>();
                    foreach (var p in r.GetRdlcReportParameters(parameters))
                    {
                        BillingRDLCReportParameters.Add(new ReportParameter(p.Key, p.Value.Value, p.Value.IsVisible));
                    }
                    ReportViewer1.LocalReport.SetParameters(BillingRDLCReportParameters.ToArray());
                }
            }
            else
                throw new Exception("you are not authorized to perform this request");

        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }
    }
}