using Vanrise.CommonLibrary;
using System;
using Vanrise.Fzero.MobileCDRAnalysis;

public partial class Login : BasePage
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
        CurrentUser.User = Vanrise.Fzero.MobileCDRAnalysis.User.GetUser(username, password);
        if (CurrentUser.User != null && CurrentUser.User.ID > 0)
        {
            tdError.InnerText = string.Empty;
            CurrentUser.Login();
            RedirectToRerturnedPage();
        }
        else
        {
            tdError.InnerText = "Invalid Username or Password";
        }
    }

    #endregion

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = string.Empty;
    }
   
}
