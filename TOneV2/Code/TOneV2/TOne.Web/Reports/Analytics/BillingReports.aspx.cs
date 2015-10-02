using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TOne.Business;
using TOne.Entities;

namespace TOne.Web.Reports.Analytics
{
    public partial class BillingReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                int reportId = Convert.ToInt32(Request.QueryString["reportId"]);
                DateTime from = DateTime.ParseExact(Request.QueryString["fromDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(Request.QueryString["toDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
              
                string customer = Request.QueryString["customer"];
                string supplier = Request.QueryString["supplier"];
                string currency = Request.QueryString["currency"];

                bool groupByCustomer = (Request.QueryString["groupByCustomer"] != null) ? (Request.QueryString["groupByCustomer"] == "true") : false;
                bool isCost = (Request.QueryString["isCost"] != null) ? (Request.QueryString["isCost"] == "true") : false;
                bool service = (Request.QueryString["service"] != null) ? (Request.QueryString["service"] == "true") : false;
                bool commission = (Request.QueryString["commission"] != null) ? (Request.QueryString["commission"] == "true") : false;
                bool bySupplier = (Request.QueryString["bySupplier"] != null) ? (Request.QueryString["bySupplier"] == "true") : false;
                bool isExchange = (Request.QueryString["isExchange"] != null) ? (Request.QueryString["isExchange"] == "true") : false;                
                
                int margin = (Request.QueryString["margin"] != null) ? Convert.ToInt32(Request.QueryString["margin"]) : 10;
                int top = (Request.QueryString["top"] != null) ? Convert.ToInt32(Request.QueryString["top"]) : 10;
                int zoneId = (Request.QueryString["zone"] != null) ? Convert.ToInt32(Request.QueryString["zone"]) : 0;
                string currencyDesc = Request.QueryString["currencyDesc"];

                ReportDefinitionManager managerReport = new ReportDefinitionManager();

                RDLCReportDefinition rdlc = managerReport.GetRDLCReportDefinition(reportId);

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath(rdlc.ReportURL);

                ReportParameters parameters = new ReportParameters();

                parameters.FromTime = from;
                parameters.ToTime = to;
                parameters.GroupByCustomer = groupByCustomer;
                parameters.CustomerId = customer;
                parameters.SupplierId = supplier;
                parameters.IsCost = isCost;
                parameters.IsService = service;
                parameters.IsCommission = commission;
                parameters.GroupBySupplier = bySupplier;
                parameters.CurrencyId = currency;
                parameters.Margin = margin;
                parameters.ZoneId = zoneId;
                parameters.IsExchange = isExchange;
                parameters.Top = top;
                parameters.CurrencyDescription = String.Format("[{0}] {1}", currency, currencyDesc);
                         

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
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            GC.Collect();
        }
    }
}