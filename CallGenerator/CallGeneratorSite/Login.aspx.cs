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

        if (user != null && user.Id > 0 && txtPassword.Text != "")
        {
            if (CommonWebComponents.SecureTextBox.GetHash(txtPassword.Text) == user.Password)
            {
                loggedUser.Login(user, false);
                //loggedUser.ManipulateRoles();

                if (loggedUser.IsAuthenticated)
                {
                    Current.User = loggedUser;

                    //ActionLog action = new ActionLog();
                    //action.ActionType = (int)Enums.ActionType.Login;
                    //action.Username = Current.User.Username;
                    //action.LogDate = DateTime.Now;
                    //action.ObjectId = user.Id;
                    //action.ObjectType = "User";
                    ////System.Net.IPHostEntry ipHostEntries = System.Net.Dns.GetHostEntry(Environment.MachineName);
                    ////System.Net.IPAddress[] arrIpAddress = ipHostEntries.AddressList;
                    ////action.RemoteAddress = Request.ServerVariables["REMOTE_ADDR"];
                    ////action.IPAddress = arrIpAddress[arrIpAddress.Length - 1].ToString();
                    ////action.ComputerName = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName;

                    //action.IPAddress = ActionClass.GetIPAddress();
                    //action.RemoteAddress = ActionClass.GetRemoteAddress();
                    //action.ComputerName = ActionClass.GetComputerName();
                    //action.Description = Utilities.SerializeLINQtoXML<User>(user);
                    //AuditRepository.Save(action);

                    if (backto.Value != "")
                        Response.Redirect(CallGeneratorLibrary.Utilities.Config.SiteUrl + backto.Value);
                    else
                        Response.Redirect(CallGeneratorLibrary.Utilities.Config.SiteUrl + "Default.aspx");
                }
            }
            else
                JavaScriptAlert("Please check your entries");
        }
        else
        {
            JavaScriptAlert("Check the username entered.");
        }
    }
    protected void btnNewPassword_Click(object sender, EventArgs e)
    {
        User user = new User();
        string email = txtUsername.Text.Trim();

        if (email != string.Empty)
        {
            user = UserRepository.GetUserByEmail(email);

            if (user.Id > 0)
                SendEmail(user);
            else
                JavaScriptAlert("The email doesn't exist in our system!");
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
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 13pt; font-weight: bold'>MOA Website</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Dear&nbsp;" + member.Name + ",</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Please check out your account details:</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Username = " + member.UserName + "</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Password = " + member.Password + "</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>For more information, don't hesitate to contact us.</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Thanks,</td></tr>");
            EmailBody.Append("<tr><td><a style='font-family: Arial; font-size: 11pt' href='http://www.kuna.net.kw'>www.kuna.net.kw</a> Team</td></tr>");
            EmailBody.Append("</table>");

            MailMessage objMail = new MailMessage();
            objMail.To.Add(member.Email);
            string strEmailFrom = WebConfigurationManager.AppSettings["SendingEmail"];
            objMail.From = new MailAddress(strEmailFrom, "Ministry of Agriculture");

            objMail.Subject = "MOA - Forget Password";

            objMail.Body = EmailBody.ToString();
            objMail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(objMail);
            JavaScriptAlert("Please check your mail, thank you!");
        }
        catch { }
    }
    #endregion
}
