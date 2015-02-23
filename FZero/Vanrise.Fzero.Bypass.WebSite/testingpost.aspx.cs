using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.UI.WebControls;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;
using Microsoft.Reporting.WebForms;
using Telerik.Web.UI;




public partial class testingpost : BasePage
{
   

    protected void btnTest_Click(object sender, EventArgs e)
    {
        EmailManager.SendForgetPassword("123", "walid.emailbox@gmail.com", string.Empty);
        Response.Redirect("Redirect.aspx");
    }
}