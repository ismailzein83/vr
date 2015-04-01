<%@ WebHandler Language="C#" Class="HandlerGetChartUserCalls" %>

using System;
using System.Web;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary;
using System.Collections.Generic;

public class HandlerGetChartCalls : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    
    public void ProcessRequest (HttpContext context) {

        String status = context.Request.QueryString["userId"];
        
        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var jsonString = String.Empty;
        
        List<ChartCall> LstChartCall = new List<ChartCall>();

        int statusId = 0;

        Int32.TryParse(status, out statusId);

        LstChartCall = TestOperatorRepository.GetChartCallsUser(statusId, Current.getCurrentUser(context).Id);
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        context.Response.Write(jsonSerializer.Serialize(LstChartCall));
        return;
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}