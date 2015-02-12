using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class ForgotPassword : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (PortalType == 0)
        {
            PreviousPageRedirect();
        }
        wucForgotPassword.PortalType = PortalType;

        if (!IsPostBack)
        {
            SetCaptions();
        }
    }

    public  void ShowError(string error)
    {
        ((Master)Master).WriteError(error);
    }

    public void ClearError()
    {
        ((Master)Master).ClearError();
    }

    private void SetCaptions()
    {
        ((Master)Master).PageHeaderTitle = Resources.Resources.ForgotPassword;
        
    }

}