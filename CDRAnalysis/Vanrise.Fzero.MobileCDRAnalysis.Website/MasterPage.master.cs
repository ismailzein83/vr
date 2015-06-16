using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;

public partial class MasterPage : System.Web.UI.MasterPage
{
    #region Properties
    public string PageHeaderTitle
    {
        set
        {
            lbltitle.InnerText = value;
            Page.Title = value;
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (hdnMenuClosed.Value == "1")
        {
            string functionName = " hideMenu(document.getElementById('iconMenu')); ";
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "CallFunction", functionName, true);
        }

    
        btnSuspectionAnalysis.Visible = true;
      
       

       

    }
}
