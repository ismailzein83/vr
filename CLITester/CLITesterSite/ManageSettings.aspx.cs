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
            GetUser();
        }
    }

    #region Events
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int id = 0;
        if (ViewState["SipAccountId"] != null)
            int.TryParse(ViewState["SipAccountId"].ToString(), out id);

        SipAccount user = new SipAccount { Id = id };

        if (id > 0)
            user = SipAccountRepository.Load(3);

        user.Username = txtUserName.Text;
        user.Login = txtLogin.Text;
        user.Password = txtPassword.Text;
        user.Server = txtServer.Text;
        user.DisplayName = txtCallerId.Text;

        SipAccountRepository.Save(user);
        ActionLog action = new ActionLog();
        action.ObjectId = user.Id;
        action.ObjectType = "User";
        action.Description = Utilities.SerializeLINQtoXML<SipAccount>(user);
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
    private void GetUser()
    {
        ViewState["SipAccountId"] = 3;
        SipAccount user = new SipAccount { Id = 3 };
        user = SipAccountRepository.Load(user.Id);

        txtUserName.Text = user.Username;
        txtLogin.Text = user.Login;
        txtPassword.Text = user.Password;
        txtCallerId.Text = user.DisplayName;
        txtServer.Text = user.Server;
    }
    #endregion
}