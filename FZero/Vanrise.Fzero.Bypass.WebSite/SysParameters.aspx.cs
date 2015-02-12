using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;


public partial class SysParameters : BasePage
{
    #region Properties
    public SysParameter currentObject
    {
        get
        {
            if (Session["SysParameters.currentObject"] is SysParameter)
                return (SysParameter)Session["SysParameters.currentObject"];
            return new SysParameter();
        }
        set
        {
            Session["SysParameters.currentObject"] = value;
        }
    }
    #endregion

    #region Methods

    private void FillCombos()
    {
        List<Vanrise.Fzero.Bypass.ValueType> valueType = Vanrise.Fzero.Bypass.ValueType.GetValueTypes();
        Manager.BindCombo(ddlType, valueType, "Name", "Id", null, null);


        List<Vanrise.Fzero.Bypass.MobileOperator> listMobileOperators = Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperatorsList();
        ddlMobileOperators.Items.Add(new RadComboBoxItem(null, null));
        foreach( MobileOperator i in listMobileOperators)
            ddlMobileOperators.Items.Add(new RadComboBoxItem(i.User.FullName, i.User.FullName));

    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.SysParameters;
        btnCancel.Text = Resources.Resources.Cancel;
        btnSave.Text = Resources.Resources.Save;
        btnSearch.Text = Resources.Resources.Search;
        btnSearchClear.Text = Resources.Resources.Clear;

        int columnIndex = 0;
        gvSysParameters.Columns[columnIndex++].HeaderText = Resources.Resources.Name;
        gvSysParameters.Columns[columnIndex++].HeaderText = Resources.Resources.Description;
        gvSysParameters.Columns[columnIndex++].HeaderText = Resources.Resources.ValueType;
        gvSysParameters.Columns[columnIndex++].HeaderText = Resources.Resources.Value;

    }

    public void FillData(SysParameter SysParameter)
    {
        hdnId.Value = SysParameter.ID.ToString();
        txtName.Text = SysParameter.Name;
        txtDescription.Text = SysParameter.Description;
        ddlType.SelectedValue = SysParameter.ValueTypeID.ToString();

        switch (SysParameter.ValueTypeID.ToString())
        {
            case "1"://Text
                txtValue.Text = SysParameter.Value.ToString();
                break;

            case "2"://True / False
                chkBooleanValue.Checked = (SysParameter.Value == null ? false : Manager.GetNullableBoolean( SysParameter.Value ).Value);
                break;

            case "3"://Positive Integer
                txtValue.Text = SysParameter.Value.ToString();
                break;

            case "4"://Positive Decimal
                txtValue.Text = SysParameter.Value.ToString();
                break;

            case "5"://Email
                txtValue.Text = SysParameter.Value.ToString();
                break;

            case "6"://Integer
                txtValue.Text = SysParameter.Value.ToString();
                break;

            case "7"://Mobile Operator
                ddlMobileOperators.SelectedValue = SysParameter.Value.ToString();
                break;
        }



        
        
        
        currentObject = SysParameter;
    }

    public void SetData()
    {
        SysParameter SysParameter = new SysParameter();
        SysParameter.LastUpdatedBy = CurrentUser.User.ID;
        SysParameter.ID = Manager.GetInteger(hdnId.Value);
        SysParameter.Name = txtName.Text.Trim();
        SysParameter.Description = txtDescription.Text.Trim();
        SysParameter.ValueTypeID= Manager.GetInteger(ddlType.SelectedValue);

        switch (ddlType.SelectedValue)
        {
            case "1"://Text
                SysParameter.Value= txtValue.Text ;
                break;

            case "2"://True / False
                SysParameter.Value = chkBooleanValue.Checked.ToString();
                break;

            case "3"://Positive Integer
                SysParameter.Value = txtValue.Text;
                break;

            case "4"://Positive Decimal
                SysParameter.Value = txtValue.Text;
                break;

            case "5"://Email
                SysParameter.Value = txtValue.Text;
                break;

            case "6"://Integer
                SysParameter.Value = txtValue.Text;
                break;

            case "7"://Mobile Operator
                 SysParameter.Value = ddlMobileOperators.SelectedValue;
                break;
        }


        currentObject = SysParameter;
    }

    private bool Save()
    {
        if (IsValidData())
        {
            SetData();
            SysParameter.Save(currentObject);
            gvSysParameters.Rebind();
            LoggedAction.AddLoggedAction((int)  Enums.ActionTypes.UpdateaSystemParameter , CurrentUser.User.ID);
            
            return true;

        } return false;
    }

    private bool IsValidData()
    {

        switch( Manager.GetInteger( ddlType.SelectedValue ) )
        {

            case 6://Integer
                int integer = 0;
                if (!int.TryParse(txtValue.Text.Trim(), out integer))
                {
                    ShowError("Value should be Integer.");
                    return false;
                }
                break;
            
            
            case 4:// Positive Decimal
                decimal decimalNumber = 0;
                if (!decimal.TryParse(txtValue.Text.Trim(), out decimalNumber))
                   {
                       ShowError("Value should be Decimal.");
                       return false;
                   }
                else
                {
                    if (decimalNumber <= 0)
                    {
                        ShowError("Value should be positive Decimal.");
                        return false;
                    }
                }


                break;

          
            case 3://Positive Integer
               int number = 0;
               if (!int.TryParse(txtValue.Text.Trim(), out number))
               {
                   ShowError("Value should be Integer.");
                   return false;
               }
               else
               {
                   if (number <= 0)
                   {
                       ShowError("Value should be positive Integer.");
                       return false;
                   }
               }
                break;

            case 5://Email
                if (! Manager.IsValidEmail(txtValue.Text))
                    {
                        ShowError("Not Valid Email.");
                        return false;
                    }
                break;

        }

        

      
        return true;
    }

    private void ClearSearchForm()
    {
        txtSearchText.Text = string.Empty;
        
    }

    private void ShowAddEditSection()
    {
        trAddEdit.Visible = true;
        trData.Visible = false;
    }

    private void HideSections()
    {
        trAddEdit.Visible = false;
        trData.Visible = true;
        
    }

    private void SetPermissions()
    {
            if (!CurrentUser.HasPermission(Enums.SystemPermissions.ViewSystemParameters))
                PreviousPageRedirect();

            gvSysParameters.Columns[gvSysParameters.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.EditSystemParameters);
    }

    #endregion

    #region Events
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (CurrentUser.portalType != 2)
            PreviousPageRedirect();

        
        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            FillCombos();
            gvSysParameters.Rebind();
        }
    }

    protected void gvSysParameters_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;

        int Id = Manager.GetInteger(e.CommandArgument.ToString());
        switch (e.CommandName)
        {

            case "Modify":
                lblSectionName.Text = Resources.Resources.EditSystemParameter;
                currentObject = SysParameter.Load(Id);
                FillData(currentObject);
                ShowAddEditSection();
                switch (ddlType.SelectedValue)
                {
                    case "1"://Text
                        chkBooleanValue.Visible = false;
                        txtValue.Visible = true;
                        ddlMobileOperators.Visible = false;
                        break;

                    case "2"://True / False
                        chkBooleanValue.Visible = true;
                        txtValue.Visible = false;
                        ddlMobileOperators.Visible = false;
                        break;


                    case "3"://Positive Integer
                        chkBooleanValue.Visible = false;
                        txtValue.Visible = true;
                        ddlMobileOperators.Visible = false;
                        break;

                    case "4"://Positive Decimal
                        chkBooleanValue.Visible = false;
                        txtValue.Visible = true;
                        ddlMobileOperators.Visible = false;
                        break;

                    case "5"://Email
                        chkBooleanValue.Visible = false;
                        txtValue.Visible = true;
                        ddlMobileOperators.Visible = false;
                        break;

                    case "6"://Integer
                        chkBooleanValue.Visible = false;
                        txtValue.Visible = true;
                        ddlMobileOperators.Visible = false;
                        break;

                    case "7"://Mobile Operator
                        chkBooleanValue.Visible = false;
                        txtValue.Visible = false;
                        ddlMobileOperators.Visible = true;
                        break;
                }
                break;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        HideSections();
    }

    protected void btnSearchClear_Click(object sender, EventArgs e)
    {
        ClearSearchForm();
        gvSysParameters.Rebind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if ( Save())
        HideSections();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvSysParameters.Rebind();
    }

    protected void gvSysParameters_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        string Text = txtSearchText.Text.Trim();
        gvSysParameters.DataSource = SysParameter.GetSysParameters(Text);
    }

    #endregion

    
}