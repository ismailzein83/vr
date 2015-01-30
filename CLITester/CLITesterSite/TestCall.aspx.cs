using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;

public partial class TestCall : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        List<Operator> lstOperators = new List<Operator>();
        lstOperators = OperatorRepository.GetOperators();
        rptOperators.DataSource = lstOperators;
        rptOperators.DataBind();

        List<Carrier> LstCarriersList = new List<Carrier>();
        
        if(Current.User.User.ParentId == null)
            LstCarriersList = CarrierRepository.LoadbyUserID(Current.User.Id);
        else
            LstCarriersList = CarrierRepository.LoadbyUserID(Current.User.User.ParentId.Value);
        
        rptCarriers.DataSource = LstCarriersList;
        rptCarriers.DataBind();
    }
}