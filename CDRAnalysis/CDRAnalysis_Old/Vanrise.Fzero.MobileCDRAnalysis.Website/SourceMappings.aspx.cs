using System;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.MobileCDRAnalysis;
using Telerik.Web.UI;

public partial class SourceMappings : BasePage
{
    #region Properties

    public SourceMapping currentObject
    {
        get
        {
            if (Session["SourceMapping.currentObject"] is SourceMapping)
                return (SourceMapping)Session["SourceMapping.currentObject"];
            return new SourceMapping();
        }
        set
        {
            Session["SourceMapping.currentObject"] = value;
        }
    }



    #endregion

    #region Methods

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.SourcesMapping))
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
        Manager.BindCombo(ddlSwitch, Vanrise.Fzero.MobileCDRAnalysis.SwitchProfile.GetAll(), "Name", "Id", Resources.Resources.PleaseSelect, "0");
        Manager.BindCombo(ddlMappedtoColumnNumber, Vanrise.Fzero.MobileCDRAnalysis.PredefinedColumn.GetPredefinedColumns(), "Name", "Id", null, null);
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.SourceMappingTemplates;
    }

    public void FillData(SourceMapping SourceMapping)
    {
        hdnId.Value=SourceMapping.ID.ToString();
        txtColumnName.Text=SourceMapping.ColumnName;
        ddlMappedtoColumnNumber.SelectedValue = SourceMapping.MappedtoColumnNumber.ToString();
        currentObject = SourceMapping;
    }

    public void SetData()
    {
        SourceMapping SourceMapping = new SourceMapping();
        SourceMapping.ID = Manager.GetInteger(hdnId.Value);
        SourceMapping.ColumnName = txtColumnName.Text;
        SourceMapping.MappedtoColumnNumber = ddlMappedtoColumnNumber.SelectedValue.ToInt();
        SourceMapping.SwitchID = ddlSwitch.SelectedValue.ToInt();

        if (SourceMapping.ID != 0)
        {
            SourceMapping.LastUpdatedBy = CurrentUser.User.ID;
            SourceMapping.LastUpdateDate = DateTime.Now;
        }
        else
        {
            SourceMapping.CreatedBy = CurrentUser.User.ID;
            SourceMapping.CreationDate = DateTime.Now;
        }
        

        currentObject = SourceMapping;
    }

    private bool Save()
    {
        SetData();
        if (!SourceMapping.CheckIfExists(currentObject))
        {
            SourceMapping.Save(currentObject);
            RadMultiPageMain.SelectedIndex = 0;
            lvColumns.Rebind();
            ClearForm();
            return true;
        }
        ShowError( Resources.Resources.AlreadyExists);
       return false;
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
        if (ddlSwitch.SelectedValue != string.Empty)
            lvColumns.DataSource = SourceMapping.GetSwitchMappings(ddlSwitch.SelectedValue.ToInt());
    }

    protected void ddlSource_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
       
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
                if (SourceMapping.Delete(ID))
                {
                    lvColumns.Rebind();
                }
                else
                    ShowError(Resources.Resources.UnabletoDelete);
                break;

            case "Modify":
                currentObject = SourceMapping.Load(ID);
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

    protected void ddlSwitch_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        lvColumns.Enabled = ddlSwitch.SelectedValue != "0" ? true : false;
        lvColumns.Rebind();
    }

    #endregion

}