using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;

public partial class ForgotPassword : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        wucForgotPassword.PortalType = 2;

        if (!IsPostBack)
        {
            SetCaptions();
        }
    }

    public  void ShowError(string error)
    {
        ShowError(error);
    }

    public void ClearError()
    {
        
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.ForgotPassword;
        
    }

}