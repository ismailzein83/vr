using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.MobileCDRAnalysis;

public partial class Dashboard : BasePage
{
    public DataStatistics staticData
    {
        get 
        {
            if (ViewState["Dashboard.DataStatistics"] == null || !(ViewState["Dashboard.DataStatistics"] is DataStatistics))
            {
                ViewState["Dashboard.DataStatistics"] = DataStatistics.GetStatistics();
            }
            return (DataStatistics)ViewState["Dashboard.DataStatistics"];
        }
        set { ViewState["Dashboard.DataStatistics"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            SetCaptions();
            SetPermissions();
            FillControls();
            FillStaticData();
        }
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.Dashboard))
            RedirectToAuthenticationPage();

        aSwitchesCount.HRef = (BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.Switches) ? aSwitchesCount.HRef : "javascript:void(0);");
        aRulesCount.HRef = (BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.NormalizationRules) ? aRulesCount.HRef : "javascript:void(0);");
        aUnNormalizedCDPN.HRef = (BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.UnNormalizationRules) ? aUnNormalizedCDPN.HRef : "javascript:void(0);");
        aUnNormalizedCGPN.HRef = (BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.UnNormalizationRules) ? aUnNormalizedCGPN.HRef : "javascript:void(0);");
        aStrategies.HRef = (BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.Strategies) ? aStrategies.HRef : "javascript:void(0);");
        aReportedCases.HRef = (BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ReportManagement) ? aReportedCases.HRef : "javascript:void(0);");
        aReportManagement.HRef = (BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ReportManagement) ? aReportManagement.HRef : "javascript:void(0);");


       

    }

    private void SetCaptions()
    {

        ((MasterPage)this.Master).PageHeaderTitle = "Dashboard";
    }


    private void FillControls()
    {
        Manager.BindCombo(ddlSwitches, SwitchProfile.GetAll(), "Name", "DatabaseConnection", "Select Source ...", "");
    }

    private void FillStaticData()
    {
        divRulesCount.InnerText = staticData.RulesCount.ToString(Formatter.AmountFormat);
        divSwitchesCount.InnerText = staticData.SwitchesCount.ToString(Formatter.AmountFormat);
        divStrategyCount.InnerText = staticData.StrategyCount.ToString(Formatter.AmountFormat);
        divReports.InnerText = staticData.ReportsCount.ToString(Formatter.AmountFormat);
        divReportedCases.InnerText = staticData.ReportDetails.ToString(Formatter.AmountFormat);

    }
   
    protected void ddlSwitches_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (ddlSwitches.SelectedIndex > 0)
        {
            string databaseName = Switch_DatabaseConnections.GetDatabaseName(SwitchProfile.Load(ddlSwitches.SelectedItem.Text).Id);
            DataStatistics switchData = DataStatistics.GetCDRsStatistics(databaseName);
            divCDRsDate.InnerText = switchData.CDRsLastDate.HasValue ? switchData.CDRsLastDate.Value.ToString(Formatter.DateFormat) : string.Empty;
            divCDRsCount.InnerText = switchData.NewCDRsCount.ToString(Formatter.AmountFormat);
            divUnNormalizedCDPN.InnerText = switchData.CDPNCount.ToString(Formatter.AmountFormat);
            divUnNormalizedCGPN.InnerText = switchData.CGPNCount.ToString(Formatter.AmountFormat);
        }
        else
        {
            divCDRsCount.InnerText = string.Empty;
            divCDRsCount.InnerText = string.Empty;
        }
    }
}