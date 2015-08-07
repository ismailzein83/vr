using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
using Vanrise.CommonLibrary;


public partial class StrategyProperties : BasePage
{
    #region Properties
   
    List<StrategyProperty> CurrentPropertyList
    {
        get
        {
            if (Session["StrategyProperties.CurrentPropertyList"] == null
                || !(Session["StrategyProperties.CurrentPropertyList"] is List<StrategyProperty>))
                GetList();
            return (List<StrategyProperty>)Session["StrategyProperties.CurrentPropertyList"];
        }
        set
        {
            Session["StrategyProperties.CurrentPropertyList"] = value;
        }
    }
    //-----------------
    StrategyThreshold CurrentThreshold
    {
        get
        {
            if (Session["StrategyProperties.CurrentThreshold"] == null
                || !(Session["StrategyProperties.CurrentThreshold"] is List<StrategyThreshold>))
                GetList();
            return (StrategyThreshold)Session["StrategyProperties.CurrentThreshold"];
        }
        set
        {
            Session["StrategyProperties.CurrentThreshold"] = value;
        }
    }
    //-----------------
    StrategyPeriod CurrentPeriod
    {
        get
        {
            if (Session["StrategyProperties.CurrentPeriod"] == null
                || !(Session["StrategyProperties.CurrentPeriod"] is List<StrategyProperty>))
                GetList();
            return (StrategyPeriod)Session["StrategyProperties.CurrentPeriod"];
        }
        set
        {
            Session["StrategyProperties.CurrentPeriod"] = value;
        }
    }
    


    private void FillControls()
    {


        List<Criteria_Profile> criteria = new List<Criteria_Profile>();
        criteria = Criteria_Profile.GetAll();
        Manager.BindCombo(ddlSearchCriteria, criteria, "Description", "Id", "", "0");

        List<Period> Periods = new List<Period>();
        Periods = Period.GetAll();
        Manager.BindCombo(ddlPeriod, Periods, "Description", "Id", "", "");

        List<Strategy> strategies = new List<Strategy>();
        strategies = Strategy.GetAll();
        Manager.BindCombo(ddlSearchStrategy, strategies, "Name", "Id", "", "0");

        int StartegyId;
        int.TryParse(Request.QueryString["Strategyid"], out StartegyId);

        if ((StartegyId) != 0)
        {
            ddlSearchStrategy.SelectedValue = StartegyId.ToString();
        }
    }

    #endregion

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            SetCaptions();
            SetDetailsVisible(false);
            FillControls();
            LoadData();

        }
    }

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "Strategy Properties";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadData();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //id = 0;
        ClearDetailsData();
        SetDetailsVisible(true);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (IsValidData())
        {
            decimal maxValue;
            int periodValue;
            decimal.TryParse(txtMaxValue.Text, out maxValue);
            int.TryParse(txtPeriodValue.Text, out periodValue);
            int periodId;
            int.TryParse(ddlPeriod.SelectedItem.Value, out periodId);

            bool flag = true;

            CurrentThreshold.MaxValue = maxValue;
            if (!StrategyThreshold.Save(CurrentThreshold))
            {
                flag = false;

            }
            if (periodValue != 0 )
            {
                if (periodValue != 0)
                    CurrentPeriod.Value = periodValue;

                CurrentPeriod.PeriodId = periodId;
                CurrentPeriod.Period = null;// Period.Load(periodId);

                if (!StrategyPeriod.Save(CurrentPeriod))
                {
                    flag = false;

                }
            }
            if (flag == false)
            {
                ShowError( "An error occured when trying to save data, kindly try to save later.");
            }


            SetDetailsVisible(false);
            LoadData();
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        SetDetailsVisible(false);
    }
    #endregion

    #region Methods
    private bool IsValidData()
    {
        return true;
    }
    private void SetDetailsVisible(bool flag)
    {
        divFilter.Visible = !flag;
        divData.Visible = !flag;
        divDetails.Visible = flag;
    }

    private void ClearDetailsData()
    {
        txtStrategy.Text = string.Empty;
        txtCriteria.Text = string.Empty;
        txtMaxValue.Text = string.Empty;
    }

    private void ClearFiltrationFields()
    {
        
    }

    private void LoadData()
    {

        GetList();
        gvData.PageIndex = 0;
        FillGrid();
    }

    private void GetList()
    {

        int criteriaId = 0;
        int strategyId = 0;

        if (ddlSearchCriteria.SelectedItem != null)
            criteriaId = int.Parse(ddlSearchCriteria.SelectedItem.Value);
        if (ddlSearchStrategy.SelectedItem != null)
            strategyId = int.Parse(ddlSearchStrategy.SelectedItem.Value);

        CurrentPropertyList = StrategyProperty.GetstrategyProperty(strategyId, criteriaId);



    }
    private void FillGrid()
    {
        gvData.DataSource = CurrentPropertyList;
        gvData.DataBind();
    }

   

    private void FillDetails(int strategyThresholId, int strategyPeriodId)
    {

        StrategyThreshold currentthreshold = StrategyThreshold.Load(strategyThresholId);
        StrategyPeriod currentPeriod = StrategyPeriod.Load(strategyPeriodId);
        FillData(currentthreshold, currentPeriod);
        SetDetailsVisible(true);
    }

    private void FillData(StrategyThreshold currentthreshold, StrategyPeriod currentPeriod)
    {

        txtPeriodValue.Text = currentPeriod.Value.ToString();
        if (currentPeriod.PeriodId != null)
            ddlPeriod.SelectedValue = currentPeriod.PeriodId.ToString();
        else
            ddlPeriod.SelectedIndex = 0;


        txtStrategy.Text = currentthreshold.Strategy.Name;
        txtCriteria.Text = currentthreshold.Criteria_Profile.Description;
        txtMaxValue.Text = currentthreshold.MaxValue.ToString();

        CurrentPeriod = currentPeriod;
        CurrentThreshold = currentthreshold;

    }

    #endregion

    protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
      
        string thId = e.CommandArgument.ToString().Substring(0, e.CommandArgument.ToString().IndexOf("-"));
        string prid = e.CommandArgument.ToString().Substring(e.CommandArgument.ToString().IndexOf("-") + 1, e.CommandArgument.ToString().Length - e.CommandArgument.ToString().IndexOf("-") - 1);
        if (e.CommandArgument != null)
        {
            switch (e.CommandName)
            {
                case "Modify":

                    FillDetails(int.Parse(thId), int.Parse(prid));
                    break;
                case "Remove":
                  

                    break;
            }
        }
    }
    protected void btnReturn_Click(object sender, EventArgs e)
    {

        Response.Redirect("Strategies.aspx");


    }
}