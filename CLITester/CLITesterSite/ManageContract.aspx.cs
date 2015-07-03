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

public partial class ManageContract : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
     protected void btnSave_Click(object sender, EventArgs e)
    {
        int numberOfCalls = 0;
        int.TryParse(txtValue.Text, out numberOfCalls);

        int period = 0;
        int.TryParse(txtPeriod.Text, out period);


         
        ContractDetails contractDetails = new ContractDetails();
        contractDetails.NumberOfCalls = numberOfCalls;
        contractDetails.NumberOfPeriod = period;

        int periodRecharge = 0;
        int.TryParse(drpPeriod.SelectedValue, out periodRecharge);

        contractDetails.Period = periodRecharge;

        Contract contract = new Contract();
        contract.Name = txtName.Text;

        int ChargeType = 0;
        int.TryParse(drpCharge.SelectedValue, out ChargeType);

        contract.ChargeType = ChargeType;

        String dateFormat = "dd MMMM yyyy";
        DateTime? creationDate = DateTime.ParseExact(txtStartDate.Value, dateFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

        contract.CreationDate = creationDate;

        contract.Description = Utilities.SerializeLINQtoXML<ContractDetails>(contractDetails);
        ContractRepository.Save(contract);

        Response.Redirect("./ManageContracts.aspx", true);
    }

     protected void btnCancel_Click(object sender, EventArgs e)
     {
         Response.Redirect("./ManageContracts.aspx", true);
     }
}