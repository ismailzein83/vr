<%@ WebHandler Language="C#" Class="HandlerDeleteNumber" %>

using System;
using System.Web;
using System.Collections.Generic;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
public class HandlerDeleteNumber : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    private class RequestNumber
    {
        public string Id { get; set; }
        public string Number { get; set; }
    }
    
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


        var groupList = jsonSerializer.Deserialize<System.Collections.Generic.List<RequestNumber>>(jsonString);

        RequestNumber resp = groupList[0];

        int numberId = 0;
        int.TryParse(resp.Id, out numberId);

        TestNumberRepository.Delete(numberId);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}