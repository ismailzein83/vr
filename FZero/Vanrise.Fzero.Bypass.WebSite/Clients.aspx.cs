using Vanrise.CommonLibrary;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;

public partial class Clients : BasePage
{
    #region Properties
    public Client currentObject
    {
        get
        {
            if (Session["ManageClients.currentObject"] is Client)
                return (Client)Session["ManageClients.currentObject"];
            return new Client();
        }
        set
        {
            Session["ManageClients.currentObject"] = value;
        }
    }


    #endregion

    #region Methods

    private void FillCombos()
    {

    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ManageApplicationUsers))
            PreviousPageRedirect();
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 4].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ActivateDeactivateApplicationUser);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 3].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ResetApplicationUserPassword);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageApplicationUsers);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageApplicationUsers);
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.Clients;



        btnSave.Text = Resources.Resources.Save;
        btnCancel.Text = Resources.Resources.Cancel;


        int columnIndex = 0;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.Id;

        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.Name;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.Email;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.IsClientReport;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.EmailSecurity;        
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.IsClientReportSecurity;        
    }

    public void FillData(Client Client)
    {
        hdnId.Value = Client.ID.ToString();
        txtName.Text = Client.Name;
        chkIsClientReport.Checked = Client.ClientReport.Value;
        chkIsClientReportSecurity.Checked = Client.ClientReportSecurity.Value;

        currentObject = Client;

    }

    public void FillResetPasswordData(Client client)
    {
        hdnId.Value = client.ID.ToString();
        currentObject = client;
    }

    public Client SetData()
    {
        int id = Manager.GetInteger(hdnId.Value);
        if (id == 0)
        {
            currentObject = new Client();
        }

        Client client = currentObject;
        client.ClientReport = chkIsClientReport.Checked;
        client.ClientReportSecurity = chkIsClientReportSecurity.Checked;
        return client;
    }



    private bool Save()
    {
        Client client = SetData();

        Client.Save(client);

        gvApplicationUsers.Rebind();
        ClearForm();
        return true;
    }

    private void ClearForm()
    {
        hdnId.Value = "0";

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

    public string ProcessMyDataItemText(bool IsActive)
    {
        if (IsActive)
        {
            return Resources.Resources.Deactivate;
        }
        return Resources.Resources.Activate;
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
            gvApplicationUsers.Rebind();
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearForm();
        HideSections();
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Save())
            HideSections();
    }

    protected void gvApplicationUsers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvApplicationUsers.DataSource = Client.GetAllClients();
    }

    protected void gvApplicationUsers_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        string[] arg = new string[3];
        arg = e.CommandArgument.ToString().Split(';');

        int Id = 0;
        int UserId = 0;
        string FullName = string.Empty;

        if (arg.Length == 2)
        {
            Id = Manager.GetInteger(arg[0]);
            FullName = arg[1];
        }


        switch (e.CommandName)
        {
            case "Modify":
                currentObject = Client.Load(Id);
                FillData(currentObject);
                ShowAddEditSection();
                txtName.Enabled = false;
                break;
        }
    }

    #endregion


}