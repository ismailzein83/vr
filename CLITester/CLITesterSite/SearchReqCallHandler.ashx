<%@ WebHandler Language="C#" Class="SearchReqCallHandler" %>

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CLINumberLibrary;

public class SearchReqCallHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        if (!Current.getCurrentUser(context).IsAuthenticated)
        {
            context.Response.Redirect("Login.aspx");
            return;
        }
        
        String dateFormat = "dd MMMM yyyy";
        int iDisplayLength = int.Parse(context.Request["iDisplayLength"]);
        int iDisplayStart = int.Parse(context.Request["iDisplayStart"]);

        DateTime? startDate = null;
        DateTime? releaseDate = null;
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
            releaseDate = DateTime.ParseExact(context.Request["releaseDate"], dateFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
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

        List<RequestCallHistory> data = RequestCallRepository.GetRequestCallHistory(startDate, releaseDate, operatorId, iDisplayStart, iDisplayLength);

        int RowCount = data.Count > 0 ? data[0].RowCount.Value : 0;

        var result = new
        {
            iTotalRecords = RowCount,
            iTotalDisplayRecords = RowCount,
            aaData = data
            .Select(p => new[] { p.RequestId.Value.ToString(), (p.CreationDate == null || p.CreationDate.Value.ToString() == "")? "" : p.CreationDate.ToString(), 
                (p.ReleaseDate == null || p.ReleaseDate.Value.ToString() == "")? "<span class='label label-important'>NOT RELEASED</span>" : p.ReleaseDate.ToString(), (p.Operator == null || p.Operator == "")? "" : p.Operator,
                (p.Client == null || p.Client == "") ? "" : p.Client, (p.PhoneNumber == null || p.PhoneNumber == "")? "<span class='label label-default'>NO LINE AVAILABLE</span>" : p.PhoneNumber})
        };

        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var json = serializer.Serialize(result);
        context.Response.ContentType = "application/json";
        context.Response.Write(json);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}