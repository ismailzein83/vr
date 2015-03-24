using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


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
        ((Master)Master).PageHeaderTitle = Resources.Resources.ChangePassword;
        ((Master)Master).SignedInUser = CurrentUser.User.FullName;
    }

    #endregion


    protected void wucChangePassword_ShowError(string err)
    {
        ((Master)Master).WriteSucess(err);
    }
    protected void wucChangePassword_Success()
    {

    }
}