using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class MobileOperatorPage : BasePage
{

    public MobileOperatorPage()
    {
        BasePage.authenticationPage = "~/Login.aspx";
    }
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
    }

   
}