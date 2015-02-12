using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;


public partial class MobileOperators : BasePage
{

    #region Properties

    public MobileOperator currentObject
    {
        get
        {
            if (Session["ManageMobileOperators.currentObject"] is MobileOperator)
                return (MobileOperator)Session["ManageMobileOperators.currentObject"];
            return new MobileOperator();
        }
        set
        {
            Session["ManageMobileOperators.currentObject"] = value;
        }
    }
    
    #endregion

    #region Methods

    private void FillCombos()
    {
        List<GMT> listGMTs = Vanrise.Fzero.Bypass.GMT.GetGMTs();
        Manager.BindCombo(ddlSearchGMT, listGMTs, "Name", "Id", null, null);
    }

    private void SetCaptions()
    {
      
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.MobileOperators;


        int columnIndex = 0;
        gvMobileOperators.Columns[columnIndex++].HeaderText = Resources.Resources.FullName;
        gvMobileOperators.Columns[columnIndex++].HeaderText = Resources.Resources.Email;
        gvMobileOperators.Columns[columnIndex++].HeaderText = Resources.Resources.UserName;
        gvMobileOperators.Columns[columnIndex++].HeaderText = Resources.Resources.LastLoginTime;
        gvMobileOperators.Columns[columnIndex++].HeaderText = Resources.Resources.NumberPrefixes;
        gvMobileOperators.Columns[columnIndex++].HeaderText = Resources.Resources.View;
        gvMobileOperators.Columns[columnIndex++].HeaderText = Resources.Resources.Edit;



      
        
    }

    private void ClearSearchForm()
    {

        txtSearchName.Text = string.Empty;
        txtSearchEmailAddress.Text = string.Empty;
        txtSearchWebsite.Text = string.Empty;
        txtSearchNumberPrefixes.Text = string.Empty;
    }

    public string ProcessMyDataItemTextActivation(bool IsActive)
    {
        if (IsActive)
        {
            return  Resources.Resources.Deactivate; 
        }
        return Resources.Resources.Activate; 
    }

    private bool Save()
    {
        string Validate1 = wucMobileOperatorInformation.IsValidData();

        if (Validate1 == string.Empty )
        {
            


            

            MobileOperator.Save(wucMobileOperatorInformation.SetData());
            
            HideSections();
            gvMobileOperators.Rebind();

          


            return true;
        }

        else
        {
            if (Validate1 != string.Empty)
            {
                ShowError(Validate1);
            }

          
            return false;
        }
    }

    public string ProcessMyDataItemTextApproval(bool IsActive)
    {
        if (IsActive)
        {
            return Resources.Resources.Disapprove;
        }
        return Resources.Resources.Approve;
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ViewMobileOperators))
            PreviousPageRedirect();

        gvMobileOperators.Columns[gvMobileOperators.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.EditMobileOperator);
        gvMobileOperators.Columns[gvMobileOperators.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ViewMobileOperators);
    }

    private void ShowAddEditSection()
    {
        trAddEdit.Visible = true;
        trData.Visible = false;
    }

    private void ShowSearchSection()
    {
        trAddEdit.Visible = false;
        trData.Visible = true;
    }

    private void HideSections()
    {
        trAddEdit.Visible = false;
        trData.Visible = true;
    }

    #endregion

    #region Events
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (CurrentUser.portalType != 2)//Admin
            PreviousPageRedirect();


        if (!IsPostBack)
        {
            SetCaptions();
            FillCombos();
            SetPermissions();
            gvMobileOperators.Rebind();
        }
    }

    protected void btnSearchClear_Click(object sender, EventArgs e)
    {
        ClearSearchForm();
        gvMobileOperators.Rebind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvMobileOperators.Rebind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Save();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        
        HideSections();
    }

    protected void gvMobileOperators_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        string website = txtSearchWebsite.Text.Trim();
        string email = txtSearchEmailAddress.Text.Trim();
        string name = txtSearchName.Text.Trim();
        string numberPrefixes = txtSearchNumberPrefixes.Text.Trim();
        int GMT = ddlSearchGMT.SelectedValue.ToInt();

        gvMobileOperators.DataSource = MobileOperator.GetMobileOperators(name, email, numberPrefixes, website, GMT);
    }

    protected void gvMobileOperators_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        string[] arg = new string[3];
        arg = e.CommandArgument.ToString().Split(';');

        int Id = 0;
        int UserId = 0;
        string FullName = string.Empty;

        if (arg.Length == 3)
        {
            Id = Manager.GetInteger(arg[0]);
            UserId = Manager.GetInteger(arg[1]);
            FullName = arg[2];
        }

        switch (e.CommandName)
        {

            case "Modify":
                currentObject = MobileOperator.Load(Id);
                wucMobileOperatorInformation.MobileOperatorId = Id.ToString();
                wucMobileOperatorInformation.ControlsEnabled(true);
                wucMobileOperatorInformation.FillData(currentObject);
                ShowAddEditSection();
                btnSave.Visible = true;
                break;

            case "View":
                currentObject = MobileOperator.Load(Id);
                wucMobileOperatorInformation.MobileOperatorId = Id.ToString();
                wucMobileOperatorInformation.ControlsEnabled(false);
                wucMobileOperatorInformation.FillData(currentObject);
                ShowAddEditSection();
                btnSave.Visible = false;
                CallJavaScriptFunction("RemoveRequired();");
                break;


        }
    }

    protected void gvMobileOperators_ItemDataBound(object sender, GridItemEventArgs e)
    {

        if (e.Item is GridDataItem)
        {

            GridDataItem item = (GridDataItem)e.Item;

            TextBox lblPrefixes = (TextBox)item.FindControl("lblPrefixes");//accessing Label

            string Prefixes = lblPrefixes.Text;
            if (Prefixes == ";;")
            {
                lblPrefixes.Text = "All Others";
            }

        }

    }
    #endregion
   
}