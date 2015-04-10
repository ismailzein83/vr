<%@ WebHandler Language="C#" Class="HandlerGetChartUserCalls" %>

using System;
using System.Web;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary;
using System.Collections.Generic;

public class HandlerGetChartUserCalls : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    
    public void ProcessRequest (HttpContext context) {

        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var jsonString = String.Empty;

        List<List<ChartCall>> LstChartCall = new List<List<ChartCall>>();

        User us = UserRepository.Load(Current.getCurrentUser(context).Id);
        if (us.ParentId != null)
            LstChartCall.Add(TestOperatorRepository.GetChartCallsUser(Current.getCurrentUser(context).Id));
        else
        {
            LstChartCall.Add(TestOperatorRepository.GetChartCallsUser(Current.getCurrentUser(context).Id));
            
            List<User> LstUs = UserRepository.GetSubUsers(Current.getCurrentUser(context).Id);
            foreach(User u in LstUs)
            {
                LstChartCall.Add(TestOperatorRepository.GetChartCallsUser(u.Id));
            }
        }
        
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