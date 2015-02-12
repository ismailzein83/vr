using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class wucNotificationControl : System.Web.UI.UserControl
{
    public void WriteError(string message)
    {
        dvError.InnerText = message;
        dvError.Visible = true;

        dvSuccess.InnerText = string.Empty;
        dvSuccess.Visible = false;

    }

    public void ClearError()
    {
        dvError.InnerText = string.Empty;
        dvError.Visible = false;

        dvSuccess.InnerText = string.Empty;
        dvSuccess.Visible = false;
    }

    public void WriteSucess(string message)
    {
        dvSuccess.InnerText = message;
        dvSuccess.Visible = true;

        dvError.InnerText = string.Empty;
        dvError.Visible = false;

    }
   
}