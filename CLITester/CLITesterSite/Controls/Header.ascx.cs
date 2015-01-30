using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;

public partial class Controls_Header : BaseUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Current.User.IsAuthenticated)
                spanUser.InnerHtml = Current.User.Name + " " + Current.User.User.LastName;
        }
    }
}