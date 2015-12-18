using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;

public partial class Default : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx",false);
        lblTotalCalls.Text = CDRRepository.GetTotalCallsITPC().ToString();
        lblCLIDel.Text = CDRRepository.GetTotalCallsZain().ToString();
        lblCLINonDel.Text = CDRRepository.GetTotalCallsST().ToString();
    }
}