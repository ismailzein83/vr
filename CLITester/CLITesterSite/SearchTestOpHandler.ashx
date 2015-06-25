<%@ WebHandler Language="C#" Class="SearchTestOpHandler" %>

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        String dateFormat = "dd MMMM yyyy";
        int iDisplayLength = int.Parse(context.Request["iDisplayLength"]);
        int iDisplayStart = int.Parse(context.Request["iDisplayStart"]);
        
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
            .Select(p => new[] { p.Name , p.Route, p.CreationDate.ToString(), (p.EndDate.ToString() == "")? "" : p.EndDate.ToString(), p.PDD, p.Duration, (p.DisplayName == null || p.DisplayName == "")? "" : p.DisplayName, (p.TestCli == null || p.TestCli == "") ? "" : p.TestCli, (p.ReceivedCli == null || p.ReceivedCli == "")? "" : p.ReceivedCli,
                p.Status == (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid ? "<span class='label label-success'>CLI DELIVERED</span>" :
                p.Status == (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid ? "<span class='label label-important'>CLI NOT DELIVERED</span>" :
                
                p.Status == (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Expired ? "<span class='label label-warning'>EXPIRED</span>" :
                p.Status == (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Waiting ? "<span class='label label-warning'>WAITING</span>" :
                p.Status == (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Failed ? "<span class='label label-warning'>FAILED</span>" :
                
                p.Status == (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.ErrorMessage ? "<span class='label label-ERROR'>ERROR</span>" :
                p.Status == (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Phase ? "<span class='label label-inverse'>ERROR</span>" :
                 "<span class='label label-default'>NO STATUS</span>", p.ErrorMessage })
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