using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using System.Web.UI.HtmlControls;

public partial class UserProfile : BasePage
{
    private void SetError(string Error)
    {
        divSuccess.Visible = false;
        divError.Visible = true;
        lblError.Text = Error;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        if (!Page.IsPostBack)
        {
            if (Current.User.User.Role ==  (int)CallGeneratorLibrary.Utilities.Enums.UserRole.SuperUser)
                liSettings.Visible = true;
            else
                liSettings.Visible = false;

            CallGeneratorLibrary.User current = UserRepository.Load(Current.User.Id);
            SipAccount sipAccount = SipAccountRepository.GetTop();

            txtEmail.Text = current.Email;
            txtName.Text = current.Name;
            txtLastName.Text = current.LastName;
            txtMobileNumber.Text = current.MobileNumber;
            txtWebsiteUrl.Text = current.WebsiteURL;
            txtCallerId.Text = sipAccount.DisplayName;

            lblDate.Text = Current.User.CreationDateF;
            lblLoginDate.Text = Current.User.LastLoginDateF;
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        CallGeneratorLibrary.User current = UserRepository.Load(Current.User.Id);
        SipAccount sipAccount = SipAccountRepository.GetTop();

        string pass = "";
        if (txtPassword.Text != "")
            pass = CommonWebComponents.SecureTextBox.GetHash(txtPassword.Text);
        if (pass != current.Password)
        {
            SetError("Old Password Incorrect");
        }
        else
        {
            if (txtNewPassword.Text == "" && txtRetypePassword.Text == "")
            {
                if (sipAccount.DisplayName != txtCallerId.Text)
                    sipAccount.IsChangedCallerId = true;
                sipAccount.DisplayName = txtCallerId.Text;
                SipAccountRepository.Save(sipAccount);

                current.Name = txtName.Text;
                current.LastName = txtLastName.Text;
                current.MobileNumber = txtMobileNumber.Text;
                current.WebsiteURL = txtWebsiteUrl.Text;
                current.Email = txtEmail.Text;
                UserRepository.Save(current);

                Response.Redirect("./Default.aspx");
            }
            else if ((txtNewPassword.Text != txtRetypePassword.Text) || txtNewPassword.Text == "" || txtNewPassword.Text == null)
            {
                SetError("Re-type New Password doesn't matchs");
            }
            else
            {
                if (sipAccount.DisplayName != txtCallerId.Text)
                    sipAccount.IsChangedCallerId = true;
                sipAccount.DisplayName = txtCallerId.Text;
                SipAccountRepository.Save(sipAccount);

                current.Name = txtName.Text;
                current.LastName = txtLastName.Text;
                current.MobileNumber = txtMobileNumber.Text;
                current.WebsiteURL = txtWebsiteUrl.Text;
                current.Email = txtEmail.Text;
                current.Password = CommonWebComponents.SecureTextBox.GetHash(txtNewPassword.Text);
                UserRepository.Save(current);

                Response.Redirect("./Default.aspx");
            }
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("./Default.aspx");
    }
}
