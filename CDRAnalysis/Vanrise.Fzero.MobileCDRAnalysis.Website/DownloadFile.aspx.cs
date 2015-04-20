using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DownloadFile : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            string ReportID = string.Empty;
            if (Request.QueryString["ReportID"] != null)
            {
                ReportID = Request.QueryString["ReportID"];
                string path = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], ReportID + ".xls");


                WebClient req = new WebClient();
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=\"" + path + "\"");
                byte[] data = req.DownloadData(path);
                response.BinaryWrite(data);
                response.End();
               
            }
            else
            {
                RedirectToAuthenticationPage();
            }

            
        }
    }
}