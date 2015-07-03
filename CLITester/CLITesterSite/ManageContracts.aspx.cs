using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Helpers;
using CallGeneratorLibrary.Utilities;

public partial class ManageContracts : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated || Current.User.IsSuperAdministrator == false)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            GetContracts();
        }
    }

    #region Methods
    private void GetContracts()
    {
        List<Contract> lstContracts = new List<Contract>();
        List<ContractFields> lstContractFields = new List<ContractFields>();
        lstContracts = ContractRepository.GetAllContracts();
        foreach(Contract c in lstContracts)
        {
            ContractDetails contractDetails = new ContractDetails();
            contractDetails = Utilities.DeserializeLINQfromXML<ContractDetails>(c.Description);

            ContractFields cd = new ContractFields();
            cd.Name = c.Name;
            cd.CreationDate = c.CreationDate.Value;
            
            if(contractDetails.Period.Value == (int)CallGeneratorLibrary.Utilities.Enums.PeriodRecharge.Daily)
                cd.Period = "Daily";
            if(contractDetails.Period.Value == (int)CallGeneratorLibrary.Utilities.Enums.PeriodRecharge.Monthly)
              cd.Period = "Monthly";
        
            if(contractDetails.Period.Value == (int)CallGeneratorLibrary.Utilities.Enums.PeriodRecharge.yearly)
                cd.Period = "yearly";

            if(c.ChargeType.Value == (int)CallGeneratorLibrary.Utilities.Enums.ContractType.Bonus)
                cd.ChargeType = "Bonus";
            if(c.ChargeType.Value == (int)CallGeneratorLibrary.Utilities.Enums.ContractType.Regular)
              cd.ChargeType = "Regular";
        
            cd.NumberOfCalls = contractDetails.NumberOfCalls;
            cd.NumberOfPeriod = contractDetails.NumberOfPeriod;
            lstContractFields.Add(cd);
        }
        rptContracts.DataSource = lstContractFields;
        rptContracts.DataBind();
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("./ManageContract.aspx", true);
    }
    #endregion
}