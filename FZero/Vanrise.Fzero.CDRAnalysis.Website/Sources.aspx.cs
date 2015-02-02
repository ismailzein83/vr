using Vanrise.CommonLibrary;
using System;
using Vanrise.Fzero.CDRAnalysis;


public partial class Sources : BasePage
{
    #region Properties

    public Source currentObject
    {
        get
        {
            if (Session["Sources.currentObject"] is Source)
                return (Source)Session["Sources.currentObject"];
            return new Source();
        }
        set
        {
            Session["Sources.currentObject"] = value;
        }
    }

    #endregion

    #region Methods

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.EditSources))
            PreviousPageRedirect();
    }

    private void ClearForm()
    {
        hdnId.Value = "0";
        txtEmail.Text = string.Empty;
        ddlSources.SelectedIndex = 0;
        
        currentObject = null;
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.Sources;
    }

    public void FillData(Source Source)
    {
        hdnId.Value = Source.ID.ToString();
        txtEmail.Text = Source.Email;
        currentObject = Source;
    }
    
    public void SetData()
    {
        Source Source = new Source();
        Source.ID = Manager.GetInteger(hdnId.Value);
        Source.Name = ddlSources.SelectedItem.Text.Trim();
        Source.LastUpdateDate = DateTime.Now;
        Source.LastUpdatedBy = CurrentUser.User.ID;
        Source.Email = txtEmail.Text;
        currentObject = Source;
    }

    private bool Save()
    {


        if ( Manager.IsValidEmail(txtEmail.Text))
        {
                SetData();
                Source.Save(currentObject);
                ClearForm();
                ShowAlert(Resources.Resources.Sourcehasbeenupdated);
                return true;
        }
        else{
                ShowError( Resources.Resources.Emailisnotavalid);
                return false;
        }
     
    }

    private void FillCombos()
    {
        Manager.BindCombo(ddlSources, Vanrise.Fzero.CDRAnalysis.Source.GetAllSources(), "Name", "Id", null, null);
        Manager.BindCombo(ddlSourceKind, Vanrise.Fzero.CDRAnalysis.SourceKind.GetAll(), "Name", "Id", null, null);
        Manager.BindCombo(ddlSwitch, Vanrise.Fzero.CDRAnalysis.SwitchProfile.GetAll(), "Name", "Id", null, null);
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
            SetPermissions();
            FillCombos();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Save();
    }

    protected void ddlSources_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (ddlSources.SelectedValue != string.Empty)
        {
            currentObject = Source.Load(ddlSources.SelectedValue.ToInt());
            FillData(currentObject);
        }
        else
        {
            ClearForm();
        }
    }

    #endregion


    
}