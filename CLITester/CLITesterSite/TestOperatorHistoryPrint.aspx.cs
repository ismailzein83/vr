using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CallGeneratorLibrary;
using Microsoft.Reporting.WebForms;
using CallGeneratorLibrary.Repositories;
using System.Globalization;

public partial class TestOperatorHistoryPrint : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            GetQualification();
    }
    #region Methods
    private void GetQualification()
    {
        {
            if (!Current.User.IsAuthenticated)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            //String dateFormat = "dd MMMM yyyy - HH:mm";
            String dateFormat = "dd MMMM yyyy";

            DateTime? startDate = null;
            DateTime? endDate = null;
            int? operatorId = null;
            try
            {
                startDate = DateTime.ParseExact(Request["startDate"], dateFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            }
            catch (System.Exception ex)
            {
            }

            try
            {
                endDate = DateTime.ParseExact(Request["endDate"], dateFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            }
            catch (System.Exception ex)
            {
            }

            try
            {
                operatorId = Int32.Parse(Request["operatorId"]);
                if (operatorId == 0)
                    operatorId = null;
            }
            catch (System.Exception ex)
            {
                operatorId = null;
            }

            List<TestOperatorHistory> data = TestOperatorRepository.GetTestOperatorHistory(startDate, endDate, operatorId,0,0);

            if (Current.TestOperatorHistoryReportData != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
               new DataColumn("Name",typeof(System.String))
             , new DataColumn("Route",typeof(System.String))
             , new DataColumn("CreationDate",typeof(System.String))
             , new DataColumn("EndDate",typeof(System.String))
             , new DataColumn("PDD",typeof(System.String))
             , new DataColumn("Duration",typeof(System.String))
             , new DataColumn("DisplayName",typeof(System.String))
             , new DataColumn("TestCli",typeof(System.String))
             , new DataColumn("ReceivedCLI",typeof(System.String))
             , new DataColumn("Status",typeof(System.String))
             , new DataColumn("ErrorMessage",typeof(System.String))
            });


                foreach (TestOperatorHistory app in data)
                {
                    DataRow row = dt.NewRow();

                    if (app.Name != null)
                        row["Name"] = app.Name;

                    if (app.Route != null)
                        row["Route"] = app.Route;

                    if (app.CreationDate != null)
                        row["CreationDate"] = app.CreationDate.Value.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    if (app.EndDate != null)
                        row["EndDate"] = app.EndDate.Value.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    if (app.PDD != null)
                        row["PDD"] = app.PDD;

                    if (app.Duration != null)
                        row["Duration"] = app.Duration;

                    if (app.DisplayName != null)
                        row["DisplayName"] = app.DisplayName;

                    if (app.TestCli != null)
                        row["TestCli"] = app.TestCli;

                    if (app.ReceivedCli != null)
                        row["ReceivedCLI"] = app.ReceivedCli;

                    if (app.Status != null)
                    {
                        if(app.Status == 0)
                            row["Status"] = "Error";
                        if (app.Status == 1)
                            row["Status"] = "CLI Deliv.";
                        if (app.Status == 2)
                            row["Status"] = "Not Deliv.";
                        if (app.Status == 3)
                            row["Status"] = "Waiting";
                        if (app.Status == 4)
                            row["Status"] = "Expired";
                        if (app.Status == 5)
                            row["Status"] = "Failed";
                        if (app.Status == 6)
                            row["Status"] = "FAS";
                    }

                    if (app.ErrorMessage != null)
                        row["ErrorMessage"] = app.ErrorMessage;

                    dt.Rows.Add(row);
                }

                rptApplication.LocalReport.ReportPath = "Reports\\TestOperatorReport.rdlc";
                rptApplication.LocalReport.DataSources.Clear();

                rptApplication.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", dt));
                this.rptApplication.LocalReport.Refresh();
            }
        }
    }
    #endregion
}