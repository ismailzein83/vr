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
        var jsonString = String.Empty;

        context.Request.InputStream.Position = 0;
        using (var inputStream = new System.IO.StreamReader(context.Request.InputStream))
        {
            jsonString = inputStream.ReadToEnd();
        }
        
        String text = jsonString.Split('=')[1].ToString();
        bool b = true;
        //bool b = !CarrierRepository.ExistShortName(text);

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