using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Helpers;
using CallGeneratorLibrary.Utilities;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        User u = new User();

        u.Id = Current.User.User.Id;
        u.UserName = Current.User.User.UserName;
        u.Name = Current.User.User.Name;
        u.Password = Current.User.User.Password;
        u.Email = Current.User.User.Email;
        u.IsActive = Current.User.User.IsActive;
        u.IsSuperAdmin = Current.User.User.IsSuperAdmin;
        u.Guid = Current.User.User.Guid;
        u.LastLoginDate = Current.User.User.LastLoginDate;
        u.CreationDate = Current.User.User.CreationDate;

        //string cacheName = "GeographicLookups";
        //System.Web.HttpContext.Current.Cache[cacheName] = null;

        NewActionLog action = new NewActionLog();
        action.ActionType = (int)Enums.ActionType.Logout;
        action.Username = u.UserName;
        action.LogDate = DateTime.Now;
        action.ObjectId = u.Id;
        action.ObjectType = "User";
        //System.Net.IPHostEntry ipHostEntries = System.Net.Dns.GetHostEntry(Environment.MachineName);
        //System.Net.IPAddress[] arrIpAddress = ipHostEntries.AddressList;
        //action.RemoteAddress = Request.ServerVariables["REMOTE_ADDR"];
        //action.IPAddress = arrIpAddress[arrIpAddress.Length - 1].ToString();
        //action.ComputerName = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName;

        action.IPAddress = ActionClass.GetIPAddress();
        action.RemoteAddress = ActionClass.GetRemoteAddress();
        action.ComputerName = ActionClass.GetComputerName();
        action.Description = Utilities.SerializeLINQtoXML<User>(u);
        AuditRepository.Save(action);

        Current.User.Logout();
        Current.User = null;

        Response.Redirect(CallGeneratorLibrary.Utilities.Config.SiteUrl + "Login.aspx");
    }
}
