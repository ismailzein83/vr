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

public partial class ManageBalances : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated || Current.User.IsSuperAdministrator == false)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            GetBalanceLogs();
            GetSipAccount();
        }
    }

    #region Events
    protected void btnSave_Click(object sender, EventArgs e)
    {

        //ContractDetails contractDetails = new ContractDetails();
        //contractDetails.NumberOfCalls = 50;
        //contractDetails.NumberOfPeriod = 1;
        //contractDetails.Period = (int)CallGeneratorLibrary.Utilities.Enums.PeriodRecharge.Monthly;

        //Contract contract = new Contract();
        //contract.Name = "Bonus";
        //contract.ChargeType = (int)CallGeneratorLibrary.Utilities.Enums.ContractType.Bonus;


        //contract.Description = Utilities.SerializeLINQtoXML<ContractDetails>(contractDetails);
        //ContractRepository.Save(contract);

        //Response.Redirect("./default.aspx", true);
    }

    protected void btnSaveBonus_Click(object sender, EventArgs e)
    {
        //int balance = 0;
        //int.TryParse(txtBonusValue.Text, out balance);

        //User user = UserRepository.GetUser((int)CallGeneratorLibrary.Utilities.Enums.UserRole.SuperUser);
        //user.Balance = user.Balance + balance;
        //UserRepository.Save(user);
    }
    
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("./default.aspx", true);
    }
    #endregion

    #region Methods
    private void GetBalanceLogs()
    {
        List<BalanceDetail> lstBalanceLogs = new List<BalanceDetail>();
        lstBalanceLogs = BalanceDetailRepository.GetBalanceDetails();
        rptBalanceLogs.DataSource = lstBalanceLogs;
        rptBalanceLogs.DataBind();
    }
    private void GetSipAccount()
    {
        //SipAccount sipAccount = SipAccountRepository.GetTop();
        //txtNumberOfMonths.Text = sipAccount.NumberOfMonths.Value.ToString();
        //txtValue.Text = sipAccount.BalanceValue.Value.ToString();
    }
    #endregion
}