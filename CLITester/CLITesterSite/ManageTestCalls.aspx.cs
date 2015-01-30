using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;

public partial class ManageTestCalls : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated || Current.User.IsSuperAdministrator == false)
            Response.Redirect("Login.aspx");

        divSuccess.Visible = false;
        divError.Visible = false;

        if (!IsPostBack)
        {
            GetData();
        }
    }

    #region Methods
    private void GetData()
    {
        List<DataCalls> LstTestOp = TestOperatorRepository.GetDataCalls().ToList();
        Session["LstTestDataHistory"] = LstTestOp;
        rptCarriers.DataSource = LstTestOp;
        rptCarriers.DataBind();
    }
   
    #endregion
}