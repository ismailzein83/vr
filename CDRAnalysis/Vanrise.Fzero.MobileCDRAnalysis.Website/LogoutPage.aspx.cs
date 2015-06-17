using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class LogoutPage : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUser.Logout();
        Response.Redirect("Login.aspx");
    }
}