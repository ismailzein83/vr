using System;
using System.Collections.Generic;
using System.Web.UI;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;

public partial class MasterPage : System.Web.UI.MasterPage
{
    #region Properties

    public string PageHeaderTitle
    {
        set
        {
            lbltitle.InnerText = value ;
            Page.Title = value;
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (hdnMenuClosed.Value == "1")
        {
            string functionName = " hideMenu(document.getElementById('iconMenu')); ";
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "CallFunction", functionName, true);
        }

        btnMonitoringnTracking.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.MonitoringnTracking);
        btnResultedCases.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ViewResultedCalls);
        btnReporttoOperator.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ReporttoOperator);
        btnReportedCases.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.MonitorReportedCases);
        btnClients.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.Dashboard);
        btnMobileOperators.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.Dashboard);
        btnUsers.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ManageApplicationUsers);
        btnEmailReceivers.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ReporttoOperator);
        btnMobileOperatorLists.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ViewMobileOperators);
        btnManualImport.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ManualImport);
        btnRelatedNumbers.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.RelatedNumbers);

        
        btnEditSources.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.EditSources);
        btnSourcesMappings.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.SourcesMapping);
        btnRelatedNumbersMapping.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.RelatedNumberMappings);
        btnLoggedActions.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ViewLoggedActions);
        btnSystemParameters.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ViewSystemParameters);
        btnSentEmails.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ViewEmails);
        btnEmailTemplates.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ViewEmailTemplates);


        if (!btnMonitoringnTracking.Visible && !btnResultedCases.Visible && !btnReporttoOperator.Visible && !btnReportedCases.Visible && !btnClients.Visible && !btnMobileOperators.Visible && !btnUsers.Visible && !btnEmailReceivers.Visible && !btnMobileOperatorLists.Visible && !btnManualImport.Visible && !btnRelatedNumbers.Visible)
        {
            string functionName = " hideMenu(document.getElementById('iconMenu')); ";
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "CallFunction", functionName, true);
        }

        if (!btnEditSources.Visible && !btnSourcesMappings.Visible && !btnRelatedNumbersMapping.Visible && !btnLoggedActions.Visible && !btnSystemParameters.Visible && !btnSentEmails.Visible && !btnEmailTemplates.Visible)
        {
            liConfigurations.Visible = false;
        }

        if (!BasePage.CurrentUser.IsAuthenticated)
        {
            liLogin.Visible = false;
        }

    }
    
    #endregion

   

      
}