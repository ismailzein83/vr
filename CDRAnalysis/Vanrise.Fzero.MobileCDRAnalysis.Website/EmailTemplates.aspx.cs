using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Telerik.Web.UI;

public partial class EmailTemplates : BasePage
{
    #region Properties
    public EmailTemplate currentObject
    {
        get
        {
            if (Session["ManageEmailTemplates.currentObject"] is EmailTemplate)
                return (EmailTemplate)Session["ManageEmailTemplates.currentObject"];
            return new EmailTemplate();
        }
        set
        {
            Session["ManageEmailTemplates.currentObject"] = value;
        }
    }

    #endregion

    #region Methods

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = "Email Templates";

        gvEmailTemplates.Columns[0].HeaderText = "Name";
        gvEmailTemplates.Columns[1].HeaderText = "Is Active";

    }

    public void FillData(EmailTemplate EmailTemplate)
    {
        hdnId.Value = EmailTemplate.ID.ToString();
        txtName.Text = EmailTemplate.Name;
        txtMessageBody.Content = EmailTemplate.MessageBody;
        chkIsActive.Checked = EmailTemplate.IsActive;
        txtSubject.Text = EmailTemplate.Subject.ToString();
        currentObject = EmailTemplate;
    }

    public void SetData()
    {
        EmailTemplate EmailTemplate = new EmailTemplate();
        EmailTemplate.LastUpdatedBy = CurrentUser.User.ID;
        EmailTemplate.ID = Manager.GetInteger(hdnId.Value);
        EmailTemplate.Name = txtName.Text.Trim();
        EmailTemplate.MessageBody = txtMessageBody.Content.Trim();
        EmailTemplate.IsActive = chkIsActive.Checked;
        EmailTemplate.Subject = txtSubject.Text;

        currentObject = EmailTemplate;
    }

    private bool Save()
    {
        SetData();
        EmailTemplate.Save(currentObject);
        gvEmailTemplates.Rebind();
        return true;
    }


    private void ClearSearchForm()
    {
        txtSearchName.Text = string.Empty;
        rblSearchStatus.SelectedValue = string.Empty;
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

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.EmailTemplates))
            RedirectToAuthenticationPage();


        gvEmailTemplates.Columns[gvEmailTemplates.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageEmailTemplates);
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
            lvTokens.DataSource = EmailToken.GetAllEmailTokens();
            lvTokens.DataBind();

        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        HideSections();
    }

    protected void btnSearchClear_Click(object sender, EventArgs e)
    {
        ClearSearchForm();
        gvEmailTemplates.Rebind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Save();
        HideSections();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvEmailTemplates.Rebind();
    }

    protected void gvEmailTemplates_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;

        int Id = Manager.GetInteger(e.CommandArgument.ToString());
        switch (e.CommandName)
        {

            case "Modify":
                currentObject = EmailTemplate.Load(Id);
                FillData(currentObject);
                ShowAddEditSection();
                break;
        }
    }

    protected void gvEmailTemplates_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        string Name = txtSearchName.Text.Trim();
        bool? IsActive = Manager.GetNullableBoolean(rblSearchStatus.SelectedValue);
        gvEmailTemplates.DataSource = EmailTemplate.GetEmailTemplates(Name, IsActive);
    }

    #endregion
    
}