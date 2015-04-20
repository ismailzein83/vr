using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
using Vanrise.CommonLibrary;

public partial class CriteriaRelations : BasePage
{
    #region Properties


    List<Related_Criteria> CurrentList
    {
        get
        {
            if (Session["CriteriaRelations.Related_Criteria"] == null
                || !(Session["CriteriaRelations.Related_Criteria"] is List<Related_Criteria>))
                GetList();
            return (List<Related_Criteria>)Session["CriteriaRelations.Related_Criteria"];
        }
        set
        {
            Session["CriteriaRelations.Related_Criteria"] = value;
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
        Manager.BindCombo(ddlRelatedCriteria, criteria, "Description", "Id", "", "0");
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
        ((MasterPage)this.Master).PageHeaderTitle = "Criteria Relations";
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

            Related_Criteria related_Criteria = SetData();

            if (!Related_Criteria.Save(related_Criteria))
            {
                id = related_Criteria.Id;
                ShowError   ( "An error occured when trying to save data, kindly try to save later.");
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
        ddlRelatedCriteria.SelectedValue = "0";

    }

    private void ClearFiltrationFields()
    {

        ddlSearchStrategy.SelectedValue = "0";
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

        CurrentList = Related_Criteria.GetList(strategyId, criteriaId);


    }
    private void FillGrid()
    {
        gvData.DataSource = CurrentList;
        gvData.DataBind();
    }

    private Related_Criteria SetData()
    {

        Related_Criteria currentObject = new Related_Criteria() { Id = id };
        currentObject.StrategyId = int.Parse(ddlStrategy.SelectedItem.Value);
        currentObject.CriteriaId = int.Parse(ddlCriteria.SelectedItem.Value);
        currentObject.Related_CriteriaId = int.Parse(ddlRelatedCriteria.SelectedItem.Value);
        return currentObject;
    }

    private void FillDetails(int id)
    {

        Related_Criteria related_Criteria = Related_Criteria.Load(id);
        FillData(related_Criteria);
        SetDetailsVisible(true);
    }

    private void FillData(Related_Criteria related_Criteria)
    {

        ddlStrategy.SelectedValue = related_Criteria.StrategyId.ToString();
        ddlCriteria.SelectedValue = related_Criteria.CriteriaId.ToString();
        ddlRelatedCriteria.SelectedValue = related_Criteria.Related_CriteriaId.ToString();
        ddlStrategy.Enabled = false;
       

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
                    if (Related_Criteria.Delete(id))
                    {
                        LoadData();
                        id = 0;
                    }
                    else
                    {
                        ShowError( "An error occured when trying to delete Related Criteria.");
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