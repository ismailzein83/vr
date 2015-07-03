using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Helpers;
using CallGeneratorLibrary.Utilities;

public partial class ManageSettings : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated || Current.User.IsSuperAdministrator == false)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            GetSipAccount();
        }
    }

    #region Events
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int id = 0;
        if (ViewState["SipAccountId"] != null)
            int.TryParse(ViewState["SipAccountId"].ToString(), out id);

        SipAccount sipAccount = new SipAccount { Id = id };

        if (id > 0)
            sipAccount = SipAccountRepository.GetTop();

        sipAccount.Username = txtUserName.Text;
        sipAccount.Login = txtLogin.Text;
        sipAccount.Password = txtPassword.Text;
        sipAccount.Server = txtServer.Text;
        sipAccount.DisplayName = txtCallerId.Text;

        SipAccountRepository.Save(sipAccount);
        ActionLog action = new ActionLog();
        action.ObjectId = sipAccount.Id;
        action.ObjectType = "SipAccount";
        action.Description = Utilities.SerializeLINQtoXML<SipAccount>(sipAccount);
        if (id == 0) // Add Operation - Action Log
            action.ActionType = (int)Enums.ActionType.Add;
        else // Edit Operation - Action Log
            action.ActionType = (int)Enums.ActionType.Modify;

        AuditRepository.Save(action);
        Response.Redirect("./default.aspx", true);
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("./default.aspx", true);
    }
    #endregion

    #region Methods
    private void GetSipAccount()
    {
        SipAccount sipAccount = SipAccountRepository.GetTop();
        txtUserName.Text = sipAccount.Username;
        txtLogin.Text = sipAccount.Login;
        txtPassword.Text = sipAccount.Password;
        txtCallerId.Text = sipAccount.DisplayName;
        txtServer.Text = sipAccount.Server;
        ViewState["SipAccountId"] = sipAccount.Id;
    }
    #endregion
}