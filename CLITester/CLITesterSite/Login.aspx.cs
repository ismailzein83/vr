using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Net;

public partial class Login : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    #region Events
    protected void btnLogin_Click(object sender, System.EventArgs e)
    {
        OfficeUser loggedUser = new OfficeUser();
        string username = txtUsername.Text.Trim();
        CallGeneratorLibrary.User user = UserRepository.GetUser(username);

        if (user != null && user.Id > 0 && txtPassword.Text!= "")
        {
            if (CommonWebComponents.SecureTextBox.GetHash(txtPassword.Text) == user.Password)
            {
                loggedUser.Login(user, false);
                //loggedUser.ManipulateRoles();

                if (loggedUser.IsAuthenticated)
                {
                    Current.User = loggedUser;

                    ActionLog action = new ActionLog();
                    action.ActionType = (int)Enums.ActionType.Login;
                    action.Username = Current.User.Username;
                    action.LogDate = DateTime.Now;
                    action.ObjectId = user.Id;
                    action.ObjectType = "User";
                    action.UserId = user.Id;
                    AuditRepository.Save(action);

                    if (backto.Value != "")
                        Response.Redirect(CallGeneratorLibrary.Utilities.Config.SiteUrl + backto.Value);
                    else
                        Response.Redirect(CallGeneratorLibrary.Utilities.Config.SiteUrl + "Default.aspx");
                }
            }
        }
    }



    protected void btnNewPassword_Click(object sender, EventArgs e)
    {
        User user = new User();
        string email = txtReset.Text.Trim();

        if (email != string.Empty)
        {
            user = UserRepository.GetUser(txtReset.Text);

            if (user.Id > 0)
            {
                user.Password = CommonWebComponents.SecureTextBox.GetHash(user.UserName);
                UserRepository.Save(user);
                SendEmail(user);
            }
            else
                JavaScriptAlert("The username doesn't exist in our system!");
        }
    }
    #endregion
    
    #region Methods
    private void SendEmail(User member)
    {
        try
        {
            StringBuilder EmailBody = new StringBuilder();
            EmailBody.Append("<table cellspacing='0' cellpadding='0'>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 13pt; font-weight: bold'>CLITester Website</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Dear&nbsp;" + member.Name + ",</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Please check out your account details:</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Username = " + member.UserName + "</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Password = " + member.UserName + "</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>We suggest to change your password now.</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>For more information, don't hesitate to contact us.</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Thanks,</td></tr>");
            EmailBody.Append("<tr><td><a style='font-family: Arial; font-size: 11pt' href='http://www.vanrise.com'>www.vanrise.com</a> Team</td></tr>");
            EmailBody.Append("</table>");

            MailMessage objMail = new MailMessage();

            objMail.To.Add(member.Email);

            string strEmailFrom = ConfigurationSettings.AppSettings["SendingEmail"];

            objMail.From = new MailAddress(strEmailFrom, "CLI Tester");
            objMail.Subject = "CLITester - Reset Password";
            objMail.Body = EmailBody.ToString();
            objMail.IsBodyHtml = true;
            objMail.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient(ConfigurationSettings.AppSettings["SmtpServer"], 587);
            smtp.Host = ConfigurationSettings.AppSettings["SmtpServer"];
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(strEmailFrom, "passwordQ1");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            smtp.Send(objMail);
        }
        catch { }
    }
    #endregion
}
