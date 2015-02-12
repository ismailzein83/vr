<%@ WebHandler Language="C#" Class="SearchTestOpHandler" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;

public class SearchTestOpHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        if (!Current.getCurrentUser(context).IsAuthenticated)
        {
            context.Response.Redirect("Login.aspx");
            return;
        }

        String dateFormat = "dd MMMM yyyy - HH:mm";
        int iDisplayLength = int.Parse(context.Request["iDisplayLength"]);
        int iDisplayStart = int.Parse(context.Request["iDisplayStart"]);
        string sdff = context.Request["startDate"];
        DateTime? startDate = null;
        DateTime? endDate = null;
        int? operatorId = null;
        try
        {
            startDate = DateTime.ParseExact(context.Request["startDate"], dateFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        }
        catch (System.Exception ex)
        {
        }

        try
        {
            endDate = DateTime.ParseExact(context.Request["endDate"], dateFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        }
        catch (System.Exception ex)
        {
        }

        try
        {
            operatorId = Int32.Parse(context.Request["operatorId"]);
            if (operatorId == 0)
                operatorId = null;
        }
        catch (System.Exception ex)
        {
        }

        List<TestOperatorHistory> data = TestOperatorRepository.GetTestOperatorHistory(startDate, endDate, operatorId, iDisplayStart, iDisplayLength);
        int RowCount = data.Count > 0 ? data[0].RowCount.Value : 0;

        var result = new
        {
            iTotalRecords = RowCount,
            iTotalDisplayRecords = RowCount,
            aaData = data
            .Select(p => new[] { p.Name, p.CreationDate.ToString(), p.EndDate == null ? "" : p.EndDate.ToString(), p.DisplayName == null ? "" : p.DisplayName, p.TestCli == null ? "" : p.TestCli, p.ReceivedCli == null ? "" : p.ReceivedCli, p.Status == null ? "" : p.Status.ToString() })
        };

        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var json = serializer.Serialize(result);
        context.Response.ContentType = "application/json";
        context.Response.Write(json);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}