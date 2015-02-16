using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
using Vanrise.CommonLibrary;


public partial class SuspectionDefinition : BasePage
{
    #region Properties

    List<Strategy_Suspection_Level> CurrentSuspectionList
    {
        get
        {
            if (Session["SuspectionLevel.CurrentSuspectionList"] == null
                || !(Session["SuspectionLevel.CurrentSuspectionList"] is List<Strategy_Suspection_Level>))
                GetList();
            return (List<Strategy_Suspection_Level>)Session["SuspectionLevel.CurrentSuspectionList"];
        }
        set
        {
            Session["SuspectionLevel.CurrentSuspectionList"] = value;
        }
    }

   


    //-----------------
    int id { get { return (int)ViewState["Id"]; } set { ViewState["Id"] = value; } }


    private void FillControls()
    {
       

        List<Strategy> strategies = new List<Strategy>();
        strategies = Strategy.GetAll();
        Manager.BindCombo(ddlSearchStrategy, strategies, "Name", "Id", "", "0");
        Manager.BindCombo(ddlStrategies, strategies, "Name", "Id", "", "0");


        List<Suspection_Level> suspection_Levels = new List<Suspection_Level>();
        suspection_Levels = Suspection_Level.GetAll();
        Manager.BindCombo(ddlSuspectionLevel, suspection_Levels, "Name", "Id", "", "0");


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
        ((MasterPage)this.Master).PageHeaderTitle = "Suspicion Levels";
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
            Strategy_Suspection_Level strategy_Suspection_Level = SetData(); ;

            if (!Strategy_Suspection_Level.Save(strategy_Suspection_Level))
            {
                id = strategy_Suspection_Level.Id;
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
        ddlStrategies.SelectedValue = ddlSearchStrategy.SelectedValue;
        ddlSuspectionLevel.SelectedValue = "0";
        chkCriteria1.Checked = false;
        chkCriteria2.Checked = false;
        chkCriteria3.Checked = false;
        chkCriteria4.Checked = false;
        chkCriteria5.Checked = false;
        chkCriteria6.Checked = false;
    }

    private void ClearFiltrationFields()
    {
        ddlSearchStrategy.SelectedValue = "0";
    }

    private void LoadData()
    {

        GetList();
        gvData.PageIndex = 0;
        FillGrid();
    }

    private void GetList()
    {
        int strategyId = 0;
        if (ddlSearchStrategy.SelectedItem != null)
            strategyId = int.Parse(ddlSearchStrategy.SelectedItem.Value);
        CurrentSuspectionList = Vanrise.Fzero.MobileCDRAnalysis.Strategy_Suspection_Level.GetList(strategyId);
        
    }
    private void FillGrid()

    {

        gvData.DataSource = CurrentSuspectionList;
        gvData.DataBind();
    }

    private Strategy_Suspection_Level  SetData()
    {
       Strategy_Suspection_Level currentObject = new Strategy_Suspection_Level() { Id = id };
       currentObject.StrategyId = int.Parse(ddlStrategies.SelectedItem.Value);
       currentObject.LevelId = int.Parse(ddlSuspectionLevel.SelectedItem.Value);
       currentObject.CriteriaId1 = chkCriteria1.Checked == true ? 1 : 0;
       currentObject.CriteriaId2 = chkCriteria2.Checked == true ? 1 : 0;
       currentObject.CriteriaId3 = chkCriteria3.Checked == true ? 1 : 0;
       currentObject.CriteriaId4 = chkCriteria4.Checked == true ? 1 : 0;
       currentObject.CriteriaId5 = chkCriteria5.Checked == true ? 1 : 0;
       currentObject.CriteriaId6 = chkCriteria6.Checked == true ? 1 : 0;

        return currentObject;
    }

    private void FillDetails(int id)
    {

        Strategy_Suspection_Level currentObject = Strategy_Suspection_Level.Load(id);
        FillData(currentObject);
        SetDetailsVisible(true);
    }

    private void FillData(Strategy_Suspection_Level currentObject)
    {


        if (currentObject.LevelId != null)
            ddlSuspectionLevel.SelectedValue = currentObject.LevelId.ToString();
        else
            ddlSuspectionLevel.SelectedIndex = 0;

        ddlStrategies.Enabled = true;
        if (currentObject.Strategy != null)
        {
            if (currentObject.Strategy.Id != 0)
            {
                ddlStrategies.SelectedValue = currentObject.Strategy.Id.ToString();
                ddlStrategies.Enabled = false;
            }
        }
        else
        {
            ddlStrategies.SelectedIndex = 0;
        }
   



        chkCriteria1.Checked = currentObject.CriteriaId1 == 1 ? true : false;
        chkCriteria2.Checked = currentObject.CriteriaId2 == 1 ? true : false;
        chkCriteria3.Checked = currentObject.CriteriaId3 == 1 ? true : false;
        chkCriteria4.Checked = currentObject.CriteriaId4 == 1 ? true : false;
        chkCriteria5.Checked = currentObject.CriteriaId5 == 1 ? true : false;
        chkCriteria6.Checked = currentObject.CriteriaId6 == 1 ? true : false;

        List<Criteria_Profile> criteria_Profiles = Criteria_Profile.GetAll();
        lblCriteria1.Text = criteria_Profiles[0].Description;
        lblCriteria2.Text = criteria_Profiles[1].Description;
        lblCriteria3.Text = criteria_Profiles[2].Description;
        lblCriteria4.Text = criteria_Profiles[3].Description;
        lblCriteria5.Text = criteria_Profiles[4].Description;
        lblCriteria6.Text = criteria_Profiles[5].Description;
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
                    if (Strategy_Suspection_Level.Delete(id))
                    {
                        LoadData();
                        id = 0;
                    }
                    else
                    {
                        ShowError( "An error occured when trying to delete this record.");
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