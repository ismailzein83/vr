using Vanrise.CommonLibrary;
using System;

using Vanrise.Fzero.Bypass;
using System.Configuration;

public partial class Login : MobileOperatorPage
{
    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CurrentUser.Logout();
            txtUsername.Focus();
            SetCaptions();
        }
    }

    protected void btnSignIn_Click(object sender, EventArgs e)
    {
        PortalType = 1;

        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (username.ToLower() == ConfigurationManager.AppSettings["ClientName"].ToLower() && password == ConfigurationManager.AppSettings["ClientPassword"])
        {
            CurrentUser.IsAuthenticated = true;
            User u = new User();
            u.UserName = username;
            u.Password = password;
            u.FullName = username;
            u.GMT = ConfigurationManager.AppSettings["GMT"].ToInt();
            CurrentUser.User = u;
            RedirectTo("~/DefaultClient.aspx");
        }
        else
        {
            password = EncryptionHelper.Encrypt(password);
            CurrentUser.User = Vanrise.Fzero.Bypass.User.GetUser(username, password, PortalType);
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

            

     
    }

    #endregion

    private void SetCaptions()
    {
        ((Master)Master).PageHeaderTitle = Resources.Resources.LoginPage;
        btnSignIn.Text = Resources.Resources.Login;
    }
    protected void btnForgotPassword_Click(object sender, EventArgs e)
    {
        PortalType = 1;//Public
        RedirectTo("~/ForgotPassword.aspx");
    }
   
}