﻿<%@ WebHandler Language="C#" Class="HandlerGetChartCalls" %>

using System;
using System.Web;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary;
using System.Collections.Generic;

public class HandlerGetChartCalls : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    
    public void ProcessRequest (HttpContext context) {

        String status = context.Request.QueryString["status"];
        
        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var jsonString = String.Empty;

        List<List<ChartCall>> LstChartCall = new List<List<ChartCall>>();

        int statusId = 0;

        Int32.TryParse(status, out statusId);

        LstChartCall.Add(TestOperatorRepository.GetChartCalls(1, Current.getCurrentUser(context).Id));
        LstChartCall.Add(TestOperatorRepository.GetChartCalls(2, Current.getCurrentUser(context).Id));
        
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