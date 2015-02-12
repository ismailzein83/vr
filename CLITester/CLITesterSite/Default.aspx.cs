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
        
        int CliDeliv = 0;
        int CliNonDeliv = 0;
        int total = 0;
        int balance = 0;
        int failed = 0;
        CliDeliv = TestOperatorRepository.GetCLIDeliv(Current.User.Id);
        CliNonDeliv = TestOperatorRepository.GetCLINonDeliv(Current.User.Id);
        total = TestOperatorRepository.GetTotalCalls(Current.User.Id);
        balance = Current.User.User.Balance.Value;
        
        int totCalls = CliDeliv + CliNonDeliv;
        failed = total - totCalls;
        lblTotalCalls.Text = totCalls.ToString();
        lblRemaining.Text = balance.ToString();
        lblFailed.Text = failed.ToString();
        lblCLIDel.Text = TestOperatorRepository.GetCLIDeliv(Current.User.Id).ToString();
        lblCLINonDel.Text = TestOperatorRepository.GetCLINonDeliv(Current.User.Id).ToString();
    }
}