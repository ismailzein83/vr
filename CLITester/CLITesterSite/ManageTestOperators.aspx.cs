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

public partial class ManageTestOperators : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            GetData();
        }
    }

    #region Methods
    private void GetData()
    {
        List<Operator> lstOperators = new List<Operator>();
        lstOperators = OperatorRepository.GetOperators();
        rptOperators.DataSource = lstOperators;
        rptOperators.DataBind();
    }
   
    #endregion
}