using System;
using System.Collections.Generic;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;

public partial class RelatedNumberMappings : BasePage
{
    #region Properties

    public RelatedNumberMapping currentObject
    {
        get
        {
            if (Session["RelatedNumberMapping.currentObject"] is RelatedNumberMapping)
                return (RelatedNumberMapping)Session["RelatedNumberMapping.currentObject"];
            return new RelatedNumberMapping();
        }
        set
        {
            Session["RelatedNumberMapping.currentObject"] = value;
        }
    }



    #endregion

    #region Methods

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.RelatedNumberMappings))
            PreviousPageRedirect();
    }

    private void ClearForm()
    {
        hdnId.Value = "0";
        txtColumnName.Text = string.Empty;
        ddlMappedtoColumnNumber.SelectedIndex = 0;
        
        currentObject = null;
    }

    private void FillCombos()
    {
        List<MobileOperator> listMobileOperator = Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperatorsList();

        ddlMobileOperator.Items.Clear();
        ddlMobileOperator.Items.Add(new RadComboBoxItem(Resources.Resources.PleaseSelect, "0"));
        foreach (MobileOperator i in listMobileOperator)
        {
            ddlMobileOperator.Items.Add(new RadComboBoxItem(i.User.FullName, i.ID.ToString()));
        }

        Manager.BindCombo(ddlMappedtoColumnNumber, Vanrise.Fzero.Bypass.PredefinedColumnsforRelatedNumber.GetPredefinedColumnsforRelatedNumbers(), "Name", "Id", null, null);
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.RelatedNumberMappingTemplates;
        lblSectionName.Text = Resources.Resources.EditMappings;
    }

    public void FillData(RelatedNumberMapping RelatedNumberMapping)
    {
        hdnId.Value=RelatedNumberMapping.ID.ToString();
        txtColumnName.Text=RelatedNumberMapping.ColumnName;
        ddlMappedtoColumnNumber.SelectedValue = RelatedNumberMapping.MappedtoColumnNumber.ToString();
        currentObject = RelatedNumberMapping;
    }

    public void SetData()
    {
        RelatedNumberMapping RelatedNumberMapping = new RelatedNumberMapping();
        RelatedNumberMapping.ID = Manager.GetInteger(hdnId.Value);
        RelatedNumberMapping.ColumnName = txtColumnName.Text;
        RelatedNumberMapping.MappedtoColumnNumber = ddlMappedtoColumnNumber.SelectedValue.ToInt();
        RelatedNumberMapping.MobileOperatorID = ddlMobileOperator.SelectedValue.ToInt();
        currentObject = RelatedNumberMapping;
    }

    private bool Save()
    {
        SetData();
        if (!RelatedNumberMapping.CheckIfExists(currentObject))
        {
            RelatedNumberMapping.Save(currentObject);
            RadMultiPageMain.SelectedIndex = 0;
            lvColumns.Rebind();
            ClearForm();
            LoggedAction.AddLoggedAction((int)Enums.ActionTypes.UpdatedRelatedNumberMapping, CurrentUser.User.ID);
            return true;
        }
        ShowError(Resources.Resources.AlreadyExists);
       return false;
    }

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (CurrentUser.portalType != 2)//Admin
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            SetCaptions();
            FillCombos();
            SetPermissions();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Save();
    }

    protected void lvColumns_NeedDataSource(object sender, RadListViewNeedDataSourceEventArgs e)
    {
        if (ddlMobileOperator.SelectedValue !=string.Empty)
            lvColumns.DataSource = RelatedNumberMapping.GetRelatedNumberMappings(ddlMobileOperator.SelectedValue.ToInt());
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearForm();
        RadMultiPageMain.SelectedIndex = 0;
    }

    protected void lvColumns_ItemCommand(object sender, RadListViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;

        int ID = 0;
        ID = e.CommandArgument.ToString().ToInt();


        switch (e.CommandName)
        {
            case "Delete":
                if (RelatedNumberMapping.Delete(ID))
                {
                    lvColumns.Rebind();
                }
                else
                    ShowError(Resources.Resources.UnabletoDelete);
                break;

            case "Modify":
                currentObject = RelatedNumberMapping.Load(ID);
                FillData(currentObject);
                RadMultiPageMain.SelectedIndex = 1;
                break;

            case "Insert":
                ClearForm();
                RadMultiPageMain.SelectedIndex = 1;
                break;


            case "Cancel":
                RadMultiPageMain.SelectedIndex = 0;
                break;



        }
    }

    #endregion

    protected void ddlMobileOperator_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        lvColumns.Enabled = ddlMobileOperator.SelectedValue != "0" ? true : false;
        lvColumns.Rebind();
    }
}