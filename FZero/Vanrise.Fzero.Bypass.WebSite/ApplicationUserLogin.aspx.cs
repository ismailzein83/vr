using Vanrise.CommonLibrary;
using System;
using Vanrise.Fzero.Bypass;

public partial class ApplicationUserLogin : BasePage
{
    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        
        

        if (!IsPostBack)
        {
        CurrentUser.Logout();
            SetCaptions();
        }
    }

    protected void btnSignIn_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;
        password = EncryptionHelper.Encrypt(password);
        CurrentUser.User = Vanrise.Fzero.Bypass.User.GetUser(username, password, 2);
        if (CurrentUser.User != null && CurrentUser.User.ID > 0)
        {
            tdError.InnerText = string.Empty;
            CurrentUser.Login();
            RedirectToRerturnedPage();
        }
        else
        {
            tdError.InnerText = Resources.Resources.InvalidUsernamePassword;
        }
    }

    #endregion

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = string.Empty;
    }
    protected void btnForgotPassword_Click(object sender, EventArgs e)
    {
      
        RedirectTo("~/ForgotPassword.aspx");
    }
}