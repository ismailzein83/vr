using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;

public partial class ChangePassword : BasePage
{

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            DefaultPageRedirect();

        if (!IsPostBack)
        {
            SetCaptions();
        }

        wucChangePassword.UserId = CurrentUser.User.ID;
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.ChangePassword;
    }

    #endregion


    protected void wucChangePassword_ShowError(string err)
    {
        ShowAlert(err);
    }
    protected void wucChangePassword_Success()
    {

    }
}