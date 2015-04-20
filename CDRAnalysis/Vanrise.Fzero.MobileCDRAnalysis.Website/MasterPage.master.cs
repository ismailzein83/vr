using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;

public partial class MasterPage : System.Web.UI.MasterPage
{
    #region Properties
    public string PageHeaderTitle
    {
        set
        {
            lbltitle.InnerText = value;
            Page.Title = value;
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (hdnMenuClosed.Value == "1")
        {
            string functionName = " hideMenu(document.getElementById('iconMenu')); ";
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "CallFunction", functionName, true);
        }


        btnDashboard.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.Dashboard);
        btnSwitches.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.Switches);
        btnNormalizationRules.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.NormalizationRules);
        btnUnNormalizationRules.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.UnNormalizationRules);
        btnStrategies.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.Strategies);
        btnSuspectionAnalysis.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.SuspectionAnalysis);
        btnReportManagement.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.ReportManagement);
        
        btnUsers.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.Users);
        btnEmailReceivers.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.EmailRecievers);
        btnSentEmails.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.SentEmails);
        btnEmailTemplates.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.EmailTemplates);
        btnSourcesMappings.Visible = BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)(int)Enums.SystemPermissions.SourcesMapping);


        if (!btnDashboard.Visible && !btnSwitches.Visible &&  !btnNormalizationRules.Visible && !btnStrategies.Visible && !btnSuspectionAnalysis.Visible && !btnReportManagement.Visible   )
        {
            string functionName = " hideMenu(document.getElementById('iconMenu')); ";
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "CallFunction", functionName, true);
        }

        if (!btnSentEmails.Visible && !btnEmailTemplates.Visible && !btnUsers.Visible && !btnEmailReceivers.Visible && !btnSourcesMappings.Visible )
        {
            liConfigurations.Visible = false;
        }

        if (!BasePage.CurrentUser.IsAuthenticated)
        {
            liLogin.Visible = false;
        }

    }
}
