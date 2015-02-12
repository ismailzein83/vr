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
            string ReportRealID=string.Empty;
            if (Request.QueryString["ReportRealID"] != null)
            {
                ReportRealID = Request.QueryString["ReportRealID"];
                string path = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], ReportRealID + ".pdf");

                WebClient client = new WebClient();
                Byte[] buffer = client.DownloadData(path);
                if (buffer != null)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", buffer.Length.ToString());
                    Response.BinaryWrite(buffer);
                }
            }
            else
            {
                RedirectToAuthenticationPage();
            }

            
        }
    }
}