using Vanrise.CommonLibrary;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;


public partial class EmailCCs : BasePage
{
    #region Properties
    public EmailCC currentObject
    {
        get
        {
            if (Session["ManageEmailCCs.currentObject"] is EmailCC)
                return (EmailCC)Session["ManageEmailCCs.currentObject"];
            return new EmailCC();
        }
        set
        {
            Session["ManageEmailCCs.currentObject"] = value;
        }
    }

    

    #endregion

    #region Methods

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ReporttoOperator))
            PreviousPageRedirect();

        btnAddNew.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ReporttoOperator);

        gvEmailCCs.Columns[gvEmailCCs.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ReporttoOperator);
        gvEmailCCs.Columns[gvEmailCCs.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ReporttoOperator);
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.EmailCCs;



        //btnSave.Text = Resources.Resources.Save;
        //btnAddNew.Text = Resources.Resources.AddNew;
        //btnCancel.Text = Resources.Resources.Cancel;
        //btnSearch.Text = Resources.Resources.Search;
        //btnSearchClear.Text = Resources.Resources.Clear;

        int columnIndex = 0;
        gvEmailCCs.Columns[columnIndex++].HeaderText = Resources.Resources.Client;
        gvEmailCCs.Columns[columnIndex++].HeaderText = Resources.Resources.Email;
        gvEmailCCs.Columns[columnIndex++].HeaderText = Resources.Resources.MobileOperator;
        gvEmailCCs.Columns[columnIndex++].HeaderText = Resources.Resources.Id;

    }

    public void FillData(EmailCC EmailCC)
    {
        hdnId.Value = EmailCC.Id.ToString();
        txtEmailAddress.Text = EmailCC.Email;
        ddlMobileOperator.SelectedValue = EmailCC.MobileOperatorID.ToString();
        ddlClients.SelectedValue = EmailCC.ClientID.ToString();
        currentObject = EmailCC;

    }

    private void FillCombos()
    {

        List<Client> Clients = Vanrise.Fzero.Bypass.Client.GetAllClients();

        Manager.BindCombo(ddlSearchClients, Clients, "Name", "Id", "All Clients ...", "0");

        Manager.BindCombo(ddlClients, Clients, "Name", "Id", "Choose Client ...", "0");

        List<MobileOperator> listMobileOperator = Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperatorsList();


        ddlSearchMobileOperator.Items.Clear();
        ddlSearchMobileOperator.Items.Add(new RadComboBoxItem("All Operators ...", "0"));
        foreach (MobileOperator i in listMobileOperator)
        {
            ddlSearchMobileOperator.Items.Add(new RadComboBoxItem(i.User.FullName, i.ID.ToString()));
        }


        ddlMobileOperator.Items.Clear();
        ddlMobileOperator.Items.Add(new RadComboBoxItem("All Operators", "0"));
        foreach (MobileOperator i in listMobileOperator)
        {
            ddlMobileOperator.Items.Add(new RadComboBoxItem(i.User.FullName, i.ID.ToString()));
        }

    }

    public EmailCC SetData()
    {
        int id = Manager.GetInteger(hdnId.Value);
        if (id == 0)
        {
            currentObject = new EmailCC();
        }

        EmailCC EmailCC = currentObject;
        EmailCC.Email = txtEmailAddress.Text;

        if (ddlMobileOperator.SelectedValue.ToInt() != 0)
            EmailCC.MobileOperatorID = ddlMobileOperator.SelectedValue.ToInt();
        else
            EmailCC.MobileOperatorID = null;

        if (ddlClients.SelectedValue.ToInt() != 0)
            EmailCC.ClientID = ddlClients.SelectedValue.ToInt();
       

        return EmailCC;
    }



    private bool Save()
    {
        
        EmailCC EmailCC = SetData();
        EmailCC.Save(EmailCC);
        gvEmailCCs.Rebind();
        ClearForm();
        return true;
    }

    private void ClearSearchForm()
    {
        txtSearchEmailAddress.Text = string.Empty;
        ddlSearchMobileOperator.SelectedValue = string.Empty;
        ddlSearchClients.SelectedValue = string.Empty ;
    }

    private void ClearForm()
    {
        hdnId.Value = "0";
        txtEmailAddress.Text = string.Empty;
        ddlMobileOperator.SelectedValue = string.Empty;
        ddlClients.SelectedValue = string.Empty;
        
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

        if (CurrentUser.portalType != 2)
            PreviousPageRedirect();


        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            FillCombos();
            gvEmailCCs.Rebind();
        }
    }

    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        ClearForm();
        ShowAddEditSection();
        lblSectionName.Text = Resources.Resources.AddEmailCC;

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearForm();
        HideSections();
    }

    protected void btnSearchClear_Click(object sender, EventArgs e)
    {
        ClearSearchForm();
        gvEmailCCs.Rebind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Save())
            HideSections();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvEmailCCs.Rebind();
    }



    protected void gvEmailCCs_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        string EmailAddress = txtSearchEmailAddress.Text.Trim();
        int MobileOperatorID = ddlSearchMobileOperator.SelectedValue.ToInt();
        gvEmailCCs.DataSource = EmailCC.GetEmailCCs(MobileOperatorID, EmailAddress,ddlSearchClients.SelectedValue.ToInt());



      
    }


    protected void gvEmailCCs_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = (GridDataItem)e.Item;
            if (item["Name"].Text == "&nbsp;")
            {
                item["Name"].Text = "All Operators";
                item["Name"].ForeColor = System.Drawing.Color.Gray;
            }
        }
    }


    protected void gvEmailCCs_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        string arg = e.CommandArgument.ToString();

        int Id = Manager.GetInteger(arg);


        switch (e.CommandName)
        {
            case "Remove":
                if (Vanrise.Fzero.Bypass.EmailCC.Delete(Id))
                {
                    gvEmailCCs.Rebind();
                    ClearForm();
                    ClearSearchForm();
                    
                }
                else
                    ShowError(Resources.Resources.UnabletoDelete);
                break;

            case "Modify":
                currentObject = EmailCC.Load(Id);
                FillData(currentObject);
                ShowAddEditSection();
                lblSectionName.Text = Resources.Resources.EditEmailCC;
                break;

          

          


        }
    }

    #endregion
      

}