<%@ WebHandler Language="C#" Class="HandlerAddNumber" %>

using System;
using System.Web;
using System.Collections.Generic;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
public class HandlerAddNumber : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{


    private class RequestGroup
    {
        public string Id { get; set; }
        public string Number { get; set; }
    }

    private class ResponseNumber
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


        var groupList = jsonSerializer.Deserialize<System.Collections.Generic.List<RequestGroup>>(jsonString);

        RequestGroup resp = groupList[0];

        int groupId = 0;
        int.TryParse(resp.Id, out groupId);
        
        //Add Group ID , Number
        TestNumber t = new TestNumber();
        t.Number = resp.Number;
        t.GroupId = groupId;
        //t.CreatedBy = Current.User.Id;
        TestNumberRepository.Save(t);
        ResponseNumber num = new ResponseNumber();
        num.Number = resp.Number;
        num.Id = t.Id.ToString();
        
        
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        context.Response.Write(jsonSerializer.Serialize(num));
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}