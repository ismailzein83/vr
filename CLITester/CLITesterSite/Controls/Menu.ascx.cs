using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Controls_Menu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Current.User.IsSuperAdministrator == true)
        {
            liSettings.Visible = true;
            liBalances.Visible = true;
            licalls.Visible = true;
            liPhoneNumbers.Visible = true;
            liOperators.Visible = true;
            liUsers.Visible = false;
            liRequestCalls.Visible = true;
            liManual.Visible = false;
            liDirect.Visible = false;
            liCarriers.Visible = false;
            liSchedule.Visible = false;
        }
        else
        {
            licalls.Visible = false;
            liPhoneNumbers.Visible = false;
            liOperators.Visible = false;
            liSettings.Visible = false;
            liBalances.Visible = false;
            liRequestCalls.Visible = false;
            if (Current.User.User.ParentId == null)
            {
                liUsers.Visible = true;
                liCarriers.Visible = true;
            }
            else
            {
                liUsers.Visible = false;
                liCarriers.Visible = false;
            }
        }
    }
}