using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;

public partial class Controls_wucForgotPassword : System.Web.UI.UserControl
{
    public int PortalType { get; set; }


    private static Random random = new Random((int)DateTime.Now.Ticks);

    string code = "";
    string Email = "";
    int ID;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CheckMode("ForgotPassword");
        }
    }

    private void CheckMode(string Mode)
    {
        switch (Mode)
        {
            case "ForgotPassword":
                lblSectionName.Text = "Enter Email Address";
                Label1.Text = "Email Address";
                Label2.Text = "";
                Label3.Text = "";
                tr2.Visible = false;
                tr3.Visible = false;
                txtEmail.Visible = true;
                btnNext.Visible = true;
                txtCode.Visible = false;
                btnNext1.Visible = false;
                txtUserName.Visible = false;
                txtNewPassword.Visible = false;
                txtConfirmNewPassword.Visible = false;
                btnSave.Visible = false;
                break;

            case "VerificationCode":
                lblSectionName.Text = "Enter the verification code exising in the message sent to your email.";
                Label1.Text = "Verification Code";
                Label2.Text = "";
                Label3.Text = "";
                tr2.Visible = false;
                tr3.Visible = false;
                txtEmail.Visible = false;
                btnNext.Visible = false;
                txtCode.Visible = true;
                btnNext1.Visible = true;
                txtUserName.Visible = false;
                txtNewPassword.Visible = false;
                txtConfirmNewPassword.Visible = false;
                btnSave.Visible = false;
                break;

            case "ResetPassword":
                lblSectionName.Text = "Enter New Password";
                Label1.Text = "User Name";
                Label2.Text = "Password";
                Label3.Text = "Confirm Password";
                tr2.Visible = true;
                tr3.Visible = true;
                txtEmail.Visible = false;
                btnNext.Visible = false;
                txtCode.Visible = false;
                btnNext1.Visible = false;
                txtUserName.Visible = true;
                txtNewPassword.Visible = true;
                txtConfirmNewPassword.Visible = true;
                btnSave.Visible = true;
                break;
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        code = RandomString(6);
        Email = txtEmail.Text;
        ID = Vanrise.Fzero.Bypass.User.UpdateVerificationCode(code, Email, PortalType);

        hfCode.Value = code.ToString();
        hfID.Value = ID.ToString();

        if (ID != 0)
        {
            EmailManager.SendForgetPassword(code, Email, string.Empty);
            CheckMode("VerificationCode");
            this.Page.GetType().InvokeMember("ClearError", System.Reflection.BindingFlags.InvokeMethod, null, this.Page, null);
        }
        else
        {
            this.Page.GetType().InvokeMember("ShowError", System.Reflection.BindingFlags.InvokeMethod, null, this.Page, new object[] { "You did not registered using this Email." });
        }
    }

    protected void btnNext1_Click(object sender, EventArgs e)
    {
        if (hfCode.Value == txtCode.Text)
        {
            this.Page.GetType().InvokeMember("ClearError", System.Reflection.BindingFlags.InvokeMethod, null, this.Page, null);
            CheckMode("ResetPassword");
            txtUserName.Text = Vanrise.Fzero.Bypass.User.GetUserName(Convert.ToInt32(hfID.Value));
            txtUserName.Enabled = false;
        }
        else
        {
            this.Page.GetType().InvokeMember("ShowError", System.Reflection.BindingFlags.InvokeMethod, null, this.Page, new object[] { "This verification code is wrong" });
        }
    }

    private string RandomString(int size)
    {
        StringBuilder builder = new StringBuilder();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }

        return builder.ToString();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string valid = Manager.IsValidPassword(txtNewPassword.Text, txtConfirmNewPassword.Text);

        if (valid == "")
        {
            User.ResetPassword(Convert.ToInt32(hfID.Value), EncryptionHelper.Encrypt(txtNewPassword.Text));
            Response.Redirect("ApplicationUserLogin.aspx");
            return;
        }
        else 
        {
            this.Page.GetType().InvokeMember("ShowError", System.Reflection.BindingFlags.InvokeMethod, null, this.Page, new object[] { valid });
        }     
    }
}