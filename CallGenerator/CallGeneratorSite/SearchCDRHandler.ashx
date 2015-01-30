<%@ WebHandler Language="C#" Class="SearchCDRHandler" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
public class SearchCDRHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest (HttpContext context) {

        if (!Current.getCurrentUser(context).IsAuthenticated)
        {
            context.Response.Redirect("Login.aspx");
            return;
        }

        String dateFormat = "dd MMMM yyyy - HH:mm";
        int iDisplayLength = int.Parse(context.Request["iDisplayLength"]);
        int iDisplayStart = int.Parse(context.Request["iDisplayStart"]);

        DateTime? startDate = null;
        DateTime? endDate = null;
        String number = null;
        int? clientId = null;
        try
        {
            startDate = DateTime.ParseExact(context.Request["startDate"], dateFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        }
        catch (Exception ex)
        { 
        }

        try
        {
            endDate = DateTime.ParseExact(context.Request["endDate"], dateFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        }
        catch (Exception ex)
        {
        }

        try
        {
            clientId = Int32.Parse(context.Request["clientId"]);
            if (clientId == 0)
                clientId = null;
        }
        catch (Exception ex)
        {
        }
        try
        {
            number = context.Request["number"];
            if (String.IsNullOrEmpty(number))
                number = null;
        }
        catch (Exception ex)
        {
        }
        
        List<CDRHistory> data = CDRRepository.GetCDRHistory(startDate, endDate, number, clientId, iDisplayStart , iDisplayLength);
        int RowCount = data.Count > 0 ? data[0].RowCount.Value : 0;
        
        var result = new
        {
            iTotalRecords = RowCount ,
            iTotalDisplayRecords = RowCount,
            aaData = data
            .Select(p => new[] { p.CDPN, p.ConnectDateTime.ToString(), p.DisconnectDateTime.ToString(), p.CAUSE_FROM_RELEASE_CODE, GetClientName(p.ClientId.Value) })
        };
        
        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var json = serializer.Serialize(result);
        context.Response.ContentType = "application/json";
        context.Response.Write(json);
        
    }

    public string GetClientName(int ClientId)
    {
        if (ClientId == 1)
        {
            return "ITPC";
        }
        if (ClientId == 2)
        {
            return "Zain";
        }
        if (ClientId == 3)
        {
            return "Syria";
        }
        return "";
    }
    public bool IsReusable {
        get {
            return false;
        }
    }

}