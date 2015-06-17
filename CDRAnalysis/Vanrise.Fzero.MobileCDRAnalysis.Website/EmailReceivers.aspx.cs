using Vanrise.CommonLibrary;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Telerik.Web.UI;


public partial class EmailReceivers : BasePage
{
    #region Properties
    public EmailReceiver currentObject
    {
        get
        {
            if (Session["EmailReceivers.currentObject"] is EmailReceiver)
                return (EmailReceiver)Session["EmailReceivers.currentObject"];
            return new EmailReceiver();
        }
        set
        {
            Session["EmailReceivers.currentObject"] = value;
        }
    }

    

    #endregion

    #region Methods

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.EmailRecievers))
            PreviousPageRedirect();

        btnAddNew.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageEmailRecievers);

        gvEmailReceivers.Columns[gvEmailReceivers.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageEmailRecievers);
        gvEmailReceivers.Columns[gvEmailReceivers.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageEmailRecievers);
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = "Email Recievers";

        int columnIndex = 0;
        gvEmailReceivers.Columns[columnIndex++].HeaderText = "Email Template";
        gvEmailReceivers.Columns[columnIndex++].HeaderText = "Receiver Type";
        gvEmailReceivers.Columns[columnIndex++].HeaderText = "Email";
        gvEmailReceivers.Columns[columnIndex++].HeaderText = "Id";

    }

    public void FillData(EmailReceiver EmailReceiver)
    {
        hdnId.Value = EmailReceiver.Id.ToString();
        txtEmailAddress.Text = EmailReceiver.Email;
        ddlReceiverType.SelectedValue = EmailReceiver.EmailReceiverTypeID.ToString();
        ddlEmailTemplate.SelectedValue = EmailReceiver.EmailTemplateID.ToString();
        currentObject = EmailReceiver;
    }

    private void FillCombos()
    {
        List<EmailTemplate> listEmailTemplates = Vanrise.Fzero.MobileCDRAnalysis.EmailTemplate.GetEmailTemplates();
        Manager.BindCombo(ddlSearchEmailTemplate, listEmailTemplates, "Name", "Id", null, null);
        Manager.BindCombo(ddlEmailTemplate, listEmailTemplates, "Name", "Id", null, null);


        List<EmailReceiverType> listEmailReceiverTypes = Vanrise.Fzero.MobileCDRAnalysis.EmailReceiverType.GetEmailReceiverTypes();
        Manager.BindCombo(ddlSearchReceiverType, listEmailReceiverTypes, "Name", "Id", null, null);
        Manager.BindCombo(ddlReceiverType, listEmailReceiverTypes, "Name", "Id", null, null);
    }

    public EmailReceiver SetData()
    {
        int id = Manager.GetInteger(hdnId.Value);
        if (id == 0)
        {
            currentObject = new EmailReceiver();
        }

        EmailReceiver EmailReceiver = currentObject;
        EmailReceiver.Email = txtEmailAddress.Text;
        EmailReceiver.EmailTemplateID = ddlEmailTemplate.SelectedValue.ToInt();
        EmailReceiver.EmailReceiverTypeID = ddlReceiverType.SelectedValue.ToInt();

        return EmailReceiver;
    }



    private bool Save()
    {
        EmailReceiver EmailReceiver = SetData();
        EmailReceiver.Save(EmailReceiver);
        gvEmailReceivers.Rebind();
        ClearForm();
        return true;
    }

    private void ClearSearchForm()
    {
        txtSearchEmailAddress.Text = string.Empty;
    }

    private void ClearForm()
    {
        hdnId.Value = "0";
        txtEmailAddress.Text = string.Empty;
        ddlReceiverType.SelectedIndex = -1;
        ddlEmailTemplate.SelectedIndex = -1;
        currentObject = null;
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

  

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            FillCombos();
            gvEmailReceivers.Rebind();
        }
    }

    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        ClearForm();
        ShowAddEditSection();

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearForm();
        HideSections();
    }

    protected void btnSearchClear_Click(object sender, EventArgs e)
    {
        ClearSearchForm();
        gvEmailReceivers.Rebind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Save())
            HideSections();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvEmailReceivers.Rebind();
    }

    protected void gvEmailReceivers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        string EmailAddress = txtSearchEmailAddress.Text.Trim();
        int EmailTemplateID = ddlSearchEmailTemplate.SelectedValue.ToInt();
        int EmailReceiverTypeID = ddlSearchReceiverType.SelectedValue.ToInt();

        gvEmailReceivers.DataSource = EmailReceiver.GetEmailReceivers(EmailAddress, EmailTemplateID, EmailReceiverTypeID);
    }

    protected void gvEmailReceivers_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        string arg = e.CommandArgument.ToString();

        int Id = Manager.GetInteger(arg);


        switch (e.CommandName)
        {
            case "Remove":
                if (Vanrise.Fzero.MobileCDRAnalysis.EmailReceiver.Delete(Id))
                {
                    gvEmailReceivers.Rebind();
                    ClearForm();
                    ClearSearchForm();
                }
                else
                    ShowError("Unable to Delete!");
                break;

            case "Modify":
                currentObject = EmailReceiver.Load(Id);
                FillData(currentObject);
                ShowAddEditSection();
                break;

          

          


        }
    }

    #endregion

}