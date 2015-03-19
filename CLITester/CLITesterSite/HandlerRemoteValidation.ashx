<%@ WebHandler Language="C#" Class="HandlerRemoteValidation" %>

using System;
using System.Web;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
public class HandlerRemoteValidation : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest (HttpContext context) {
        if (!Current.getCurrentUser(context).IsAuthenticated)
        {
            context.Response.Redirect("Login.aspx");
            return;
        }
        
        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        
        String text = context.Request.Form["ctl00$MainContent$txtShortName"].ToString();
        int id = 0;
        String ID = context.Request.Form["id"].ToString();
        Int32.TryParse(ID, out id);
        
        bool b = !CarrierRepository.ExistShortName(text,id);

        string msg = b.ToString();
        if (b)
            msg = "true";
        else
            msg = "false";
        
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        context.Response.Write(jsonSerializer.Serialize(msg));
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}