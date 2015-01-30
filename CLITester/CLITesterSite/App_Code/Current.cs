using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Current
/// </summary>
public class Current
{
    public static OfficeUser User
    {
        get
        {
            if ((OfficeUser)HttpContext.Current.Session["OfficeUser"] == null)
                HttpContext.Current.Session["OfficeUser"] = new OfficeUser();
            return (OfficeUser)HttpContext.Current.Session["OfficeUser"];
        }
        set
        {
            HttpContext.Current.Session["OfficeUser"] = value;
        }
    }

    public static OfficeUser getCurrentUser(HttpContext context)
    {
        if ((OfficeUser)context.Session["OfficeUser"] == null)
            context.Session["OfficeUser"] = new OfficeUser();
        return (OfficeUser)context.Session["OfficeUser"];
    }

    //public static List<ProjectsLibrary.ProjectTask> ProjectTasksReportData
    //{
    //    get
    //    {
    //        if ((List<ProjectsLibrary.ProjectTask>)HttpContext.Current.Session["TasksReportData"] == null)
    //            HttpContext.Current.Session["TasksReportData"] = new List<ProjectsLibrary.ProjectTask>();
    //        return (List<ProjectsLibrary.ProjectTask>)HttpContext.Current.Session["TasksReportData"];
    //    }
    //    set
    //    {
    //        HttpContext.Current.Session["TasksReportData"] = value;
    //    }
    //}

}
