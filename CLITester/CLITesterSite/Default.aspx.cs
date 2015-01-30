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
            Response.Redirect("Login.aspx");
        lblTotalCalls.Text = TestOperatorRepository.GetTotalCalls(Current.User.Id).ToString();
        lblCLIDel.Text = TestOperatorRepository.GetCLIDeliv(Current.User.Id).ToString();
        lblCLINonDel.Text = TestOperatorRepository.GetCLINonDeliv(Current.User.Id).ToString();

            //List<ChartCall> lst = new List<ChartCall>();

        //lst = TestOperatorRepository.GetChartCalls();
    }
}