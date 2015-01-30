<%@ WebHandler Language="C#" Class="HandlerGetNumbers" %>

using System;
using System.Web;
using System.Collections.Generic;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
public class HandlerGetNumbers : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    private class RequestGroup
    {
        public string id { get; set; }
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
        int.TryParse(resp.id, out groupId);

        List<TestNumber> Lstnumber = new List<TestNumber>();
        List<ResponseNumber> number = new List<ResponseNumber>();
        Lstnumber = TestNumberRepository.GetTestNumber(groupId);

        foreach (TestNumber tstnb in Lstnumber)
        {
            ResponseNumber num = new ResponseNumber();
            num.Number = tstnb.Number;
            num.Id = tstnb.Id.ToString();
            number.Add(num);
        }       
           
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        context.Response.Write(jsonSerializer.Serialize(number));
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}