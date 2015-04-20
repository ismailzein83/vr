using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.CommonLibrary;

public partial class SwitchProfiles : BasePage
{
    #region Properties
    List<SwitchProfile> CurrentList
    {
        get
        {
            if (Session["SwitchProfiles.CurrentList"] == null
                || !(Session["SwitchProfiles.CurrentList"] is List<SwitchProfile>))
                GetList();
            return (List<SwitchProfile>)Session["SwitchProfiles.CurrentList"];
        }
        set
        {
            Session["SwitchProfiles.CurrentList"] = value;
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
            SetPermissions();
            SetCaptions();
            SetDetailsVisible(false);
            LoadData();
        }
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.Switches))
            RedirectToAuthenticationPage();

        btnAdd.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageSwitches);
        gvData.Columns[gvData.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageSwitches);
        gvData.Columns[gvData.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageSwitches);
    }

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "Sources Profiles";
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
            SwitchProfile switchProfile = SetData();
            if (SwitchProfile.IsNameUsed(switchProfile))
            {
                ShowError("The name is unique and can not be used more than once.");
                return;
            }
            if (!SwitchProfile.Save(switchProfile))
            {
                id = switchProfile.Id;
                ShowError( "An error occured when trying to save data, kindly try to save later.");
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
        txtServerName.Text = string.Empty;
        txtUserId.Text = string.Empty;
        txtUserPassword.Text = string.Empty;
        txtDatabaseName.Text = string.Empty;
    }

    private void ClearFiltrationFields()
    {
        txtSearchName.Text = string.Empty;
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
        CurrentList = SwitchProfile.GetList(name);
    }
    private void FillGrid()
    {
        gvData.DataSource = CurrentList;
        gvData.DataBind();
    }

    private SwitchProfile SetData()
    {
        SwitchProfile currentObject = new SwitchProfile() { Id = id };
        currentObject.Name = txtName.Text.Trim();
        currentObject.Switch_DatabaseConnections = new Switch_DatabaseConnections() { Id = id };
        currentObject.Switch_DatabaseConnections.ServerName = txtServerName.Text.Trim();
        currentObject.Switch_DatabaseConnections.UserId = txtUserId.Text.Trim();
        currentObject.Switch_DatabaseConnections.UserPassword = txtUserPassword.Text.Trim();
        currentObject.Switch_DatabaseConnections.DatabaseName = txtDatabaseName.Text.Trim();
        currentObject.AllowAutoImport = chkAutoImport.Checked;
        return currentObject;
    }

    private void FillDetails(int id)
    {
        SwitchProfile currentObject = SwitchProfile.Load(id);
        FillData(currentObject);
        SetDetailsVisible(true);
    }

    private void FillData(SwitchProfile currentObject)
    {
 	    txtName.Text = currentObject.Name;
        chkAutoImport.Checked = currentObject.AllowAutoImport;
        txtServerName.Text = currentObject.Switch_DatabaseConnections.ServerName;
        txtUserId.Text = currentObject.Switch_DatabaseConnections.UserId;
        txtUserPassword.Text = currentObject.Switch_DatabaseConnections.UserPassword;
        txtDatabaseName.Text = currentObject.Switch_DatabaseConnections.DatabaseName;
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
                    if (SwitchProfile.Delete(id))
                    {
                        LoadData();
                        id = 0;
                    }
                    else
                    {
                        ShowError( "An error occured when trying to delete switch profile, this switch is either related to a trunk or to a normalization rule.");
                    }
                   
                    break;
            }
        }
    }
    protected void btnTestConnection_Click(object sender, EventArgs e)
    {
        string connectionString = BuildConnectionString();
        if (Vanrise.CommonLibrary.DbConnection.IsAvailable(connectionString))
        {
            ShowAlert( "The connection was successfully tested");
        }
        else
        {
            ShowError( "An error occured when trying to connect <br/> Check the values you entered");
        }

    }

    private string BuildConnectionString()
    {
        string connectionString = Constants.ConnectionStringPattern
                    .Replace("@ServerName", txtServerName.Text)
                    .Replace("@DatabaseName", txtDatabaseName.Text)
                    .Replace("@UserId", txtUserId.Text)
                    .Replace("@UserPassword", txtUserPassword.Text)
                    ;
        return connectionString;
    }
}