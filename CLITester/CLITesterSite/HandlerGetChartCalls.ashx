<%@ WebHandler Language="C#" Class="HandlerGetChartCalls" %>

using System;
using System.Web;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary;
using System.Collections.Generic;

public class HandlerGetChartCalls : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

        String status = context.Request.QueryString["status"];
        
        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var jsonString = String.Empty;
        
        List<ChartCall> LstChartCall = new List<ChartCall>();

        int statusId = 0;

        Int32.TryParse(status, out statusId);

        LstChartCall = TestOperatorRepository.GetChartCalls(statusId);
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