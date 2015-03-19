using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;
using System.Net.Mail;
using System.Web.Configuration;
using System.Configuration;

public partial class ListOperators : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated || Current.User.IsSuperAdministrator == false)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            GetData();
        }
    }


    private void GetData()
    {
        List<Operator> Operators = OperatorRepository.GetOperators().OrderByDescending(l => l.Id).ToList();
        rptSchedules.DataSource = Operators;
        rptSchedules.DataBind();
    }

    public string GetURL()
    {
        return ConfigurationSettings.AppSettings["pathImg"];
    }
}