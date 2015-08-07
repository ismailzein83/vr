using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.CommonLibrary;

public partial class Strategies : BasePage
{
    #region Properties
    List<Strategy> CurrentList
    {
        get
        {
            if (Session["Strategy.CurrentList"] == null
                || !(Session["Strategy.CurrentList"] is List<SwitchProfile>))
                GetList();
            return (List<Strategy>)Session["Strategy.CurrentList"];
        }
        set
        {
            Session["Strategy.CurrentList"] = value;
        }
    }



    int id { get { return (int)ViewState["Id"]; } set { ViewState["Id"] = value; } }
    #endregion

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            SetCaptions();
            SetPermissions();
            SetDetailsVisible(false);
            LoadData();
        }
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.Strategies))
            RedirectToAuthenticationPage();

        btnAdd.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageStrategies);
        gvData.Columns[gvData.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageStrategies);
        gvData.Columns[gvData.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageStrategies);
    }

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "Detection Strategies";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadData();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearFiltrationFields();
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
            Strategy strategy = SetData();
            if (Strategy.IsFullNameUsed(strategy))
            {
                ShowError( "The full name is unique and can not be used more than once.");
                return;
            }
            if (!Strategy.Save(strategy))
            {
                id = strategy.Id;
                ShowError("An error occured when trying to save data, kindly try to save later.");
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
        txtName.Text = string.Empty;
        txtDescription.Text = string.Empty;
               
    }

    private void ClearFiltrationFields()
    {
        txtSearchName.Text = string.Empty;
       
        txtSearchDescription.Text = string.Empty;
    }

    private void LoadData()
    {
        GetList();
        gvData.PageIndex = 0;
        FillGrid();
    }

    private void GetList()
    {
        string name = txtSearchName.Text;
      
        string description = txtSearchDescription.Text;

        CurrentList = Vanrise.Fzero.MobileCDRAnalysis.Strategy.GetList(name, description,CurrentUser.User.ID,CurrentUser.User.IsSuperUser);
    }
    private void FillGrid()
    {
        gvData.DataSource = CurrentList;
        gvData.DataBind();
    }

    private Strategy SetData()
    {
        Strategy currentObject = new Strategy() { Id = id };
        currentObject.Name = txtName.Text.Trim();
        currentObject.Description = txtDescription.Text.Trim();
        currentObject.UserId = CurrentUser.User.ID;
        currentObject.CreationDate = DateTime.Now;

        if (currentObject.Id == 0)
        {
            currentObject.IsDefault = false;
        }
            
        return currentObject;
    }

    private void FillDetails(int id)
    {
        Strategy currentObject = Strategy.Load(id);
        FillData(currentObject);
        SetDetailsVisible(true);
    }

    private void FillData(Strategy currentObject)
    {
        txtName.Text = currentObject.Name;
        txtDescription.Text = currentObject.Description;
       

        
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
                    if (Strategy.Delete(id))
                    {
                        LoadData();
                        id = 0;
                    }
                    else
                    {
                        ShowError( "An error occured when trying to delete Strategy.");
                    }

                    break;
                case "Properties":
                    Response.Redirect("StrategyProperties.aspx?Strategyid=" + id);
                    break;

               case "SuspectionLevels":
                    Response.Redirect("SuspectionDefinition.aspx?Strategyid=" + id);
                    break;

               case "MinimumValues":
                    Response.Redirect("MinimumValues.aspx?Strategyid=" + id);
                    break;
               case "RelatedCriteria":
                    Response.Redirect("CriteriaRelations.aspx?Strategyid=" + id);
                    break;
                    
                    
            }
        }
    }
   

   
}