using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;
using System.Configuration;

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

        LstCarriersList = CarrierRepository.GetCarriers();
        
        rptCarriers.DataSource = LstCarriersList;
        rptCarriers.DataBind();

        imgSRC.Value = GetURL();
    }

    public string GetURL()
    {
        return ConfigurationSettings.AppSettings["pathImg"];
    }
}