using System;
using Vanrise.Fzero.Bypass;

public partial class Logout : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUser.Logout();
        RedirectTo("~/Default.aspx");
    }
}