using System;


public partial class Logout : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUser.Logout();
        PortalType = 0;//None
        RedirectTo("~/Login.aspx");
    }
}