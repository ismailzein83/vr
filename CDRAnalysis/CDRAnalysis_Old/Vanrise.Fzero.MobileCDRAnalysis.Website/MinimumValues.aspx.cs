using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
using Vanrise.CommonLibrary;


public partial class MinimumValues : BasePage
{
    #region Properties


    List<Strategy_Min_Values> CurrentList
    {
        get
        {
            if (Session["MinimumValues.Strategy_Min_Values"] == null
                || !(Session["MinimumValues.Strategy_Min_Values"] is List<Strategy_Min_Values>))
                GetList();
            return (List<Strategy_Min_Values>)Session["MinimumValues.Strategy_Min_Values"];
        }
        set
        {
            Session["MinimumValues.Strategy_Min_Values"] = value;
        }
    }

    //-----------------
  
    int id { get { return (int)ViewState["Id"]; } set { ViewState["Id"] = value; } }


    private void FillControls()
    {

        

        List<Criteria_Profile> criteria = new List<Criteria_Profile>();
        criteria = Criteria_Profile.GetAll();
        Manager.BindCombo(ddlSearchCriteria, criteria, "Description", "Id", "", "0");
        Manager.BindCombo(ddlCriteria, criteria, "Description", "Id", "", "0");

        List<Strategy> strategies = new List<Strategy>();
        strategies = Strategy.GetAll();
        Manager.BindCombo(ddlSearchStrategy, strategies, "Name", "Id", "", "0");
        Manager.BindCombo(ddlStrategy, strategies, "Name", "Id", "", "0");

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
        ((MasterPage)this.Master).PageHeaderTitle = "Minimum Values";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadData();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        id = 0;
        ClearDetailsData();
        SetDetailsVisible(true);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (IsValidData())
        {

            Strategy_Min_Values strategy_Min_Values = SetData();
            if (!Strategy_Min_Values.Save(strategy_Min_Values))
            {
                id = strategy_Min_Values.Id;
                ShowError( "An error occured when trying to save data, kindly try to save later.");
                return;
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
        ddlStrategy.SelectedValue = "0";
        ddlStrategy.Enabled = true;
        ddlCriteria.SelectedValue = "0";
        txtMin_Count_Value.Text = string.Empty;
        txtMin_Aggreg_Volume.Text = string.Empty;
       
    }

    private void ClearFiltrationFields()
    {
        
        ddlSearchStrategy.SelectedValue="0";
        ddlSearchCriteria.SelectedValue = "0";
        
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

        CurrentList = Strategy_Min_Values.GetList(strategyId, criteriaId);

      
    }
    private void FillGrid()
    {
        gvData.DataSource = CurrentList;
        gvData.DataBind();
    }

    private Strategy_Min_Values SetData()
    {
        int minCountValue = 0;
        int.TryParse(txtMin_Count_Value.Text.Trim(), out minCountValue);
        decimal MinAggregVolume = 0;
        decimal.TryParse(txtMin_Aggreg_Volume.Text.Trim(), out MinAggregVolume);
        Strategy_Min_Values currentObject = new Strategy_Min_Values() { Id = id };
        currentObject.StrategyId = int.Parse(ddlStrategy.SelectedItem.Value);
        currentObject.CriteriaId = int.Parse(ddlCriteria.SelectedItem.Value);

        currentObject.Min_Count_Value = minCountValue;
        currentObject.Min_Aggreg_Volume = MinAggregVolume;
        return currentObject;
    }

    private void FillDetails(int MinValueId)
    {

        Strategy_Min_Values strategy_Min_Values = Strategy_Min_Values.Load(MinValueId);
        //CurrentMinValues = strategy_Min_Values;
        FillData(strategy_Min_Values);
        SetDetailsVisible(true);
    }

    private void FillData(Strategy_Min_Values strategy_Min_Values)
    {

        ddlStrategy.SelectedValue = strategy_Min_Values.StrategyId.ToString();
        ddlCriteria.SelectedValue = strategy_Min_Values.CriteriaId.ToString();
        ddlStrategy.Enabled = false;
       //ddlCriteria.Enabled = false;
        txtMin_Count_Value.Text = strategy_Min_Values.Min_Count_Value.ToString();
        txtMin_Aggreg_Volume.Text = strategy_Min_Values.Min_Aggreg_Volume.ToString();
               
    }

    #endregion

    protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;

        id = Manager.GetInteger(e.CommandArgument.ToString());
       
        if (e.CommandArgument != null)
        {
            switch (e.CommandName)
            {
                case "Modify":

                    FillDetails(id);
                    break;
                case "Remove":
                    if (Strategy_Min_Values.Delete(id))
                    {
                        LoadData();
                        id = 0;
                    }
                    else
                    {
                        ShowError("An error occured when trying to delete Min_Value.");
                    }

                    break;
            }
        }
    }
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Response.Redirect("Strategies.aspx");
    }
}