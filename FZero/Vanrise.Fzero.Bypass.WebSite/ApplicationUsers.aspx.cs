using Vanrise.CommonLibrary;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;


public partial class ApplicationUsers : BasePage
{
    #region Properties
    public ApplicationUser currentObject
    {
        get
        {
            if (Session["ManageApplicationUsers.currentObject"] is ApplicationUser)
                return (ApplicationUser)Session["ManageApplicationUsers.currentObject"];
            return new ApplicationUser();
        }
        set
        {
            Session["ManageApplicationUsers.currentObject"] = value;
        }
    }

    public bool PasswordVisible
    {
        get
        {
            return txtPassword.Visible;
        }
        set
        {
            txtPassword.Visible = value;
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

        btnAddNew.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageApplicationUsers);

        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 4].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ActivateDeactivateApplicationUser);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 3].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ResetApplicationUserPassword);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageApplicationUsers);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageApplicationUsers);
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.ApplicationUsers;



        btnSave.Text = Resources.Resources.Save;
        btnCancel.Text = Resources.Resources.Cancel;
       

        int columnIndex = 0;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.FullName;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.Email;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.UserName;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.Id;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.UserId;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.Address;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.MobileNumber;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.MaxDailyCases;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.IsActive;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = Resources.Resources.LastLoginTime;


    }

    public void FillData(ApplicationUser ApplicationUser)
    {
        hdnId.Value = ApplicationUser.ID.ToString();
        txtAddress.Text = ApplicationUser.User.Address;
        txtDailyMaxCases.Text = ApplicationUser.User.MaxDailyCases.ToString();
        txtMobileNumber.Text = ApplicationUser.User.MobileNumber;
        txtUserName.Text = ApplicationUser.User.UserName;
        txtEmailAddress.Text = ApplicationUser.User.EmailAddress;
        txtFullName.Text = ApplicationUser.User.FullName;

        chkIsActive.Enabled = false;

        if (ApplicationUser.User.IsSuperUser)
        {
            chkIsActive.Checked = true;
            trPermission.Visible = false;
            trPermissionNotes.Visible = true;
        }
        else
        {
            chkIsActive.Checked = ApplicationUser.User.IsActive;
            trPermission.Visible = true;
            trPermissionNotes.Visible = false;
            FillAdminPermissions(ApplicationUser.User.UserPermissions);
        }

        currentObject = ApplicationUser;

    }

    private void FillAdminPermissions(ICollection<UserPermission> userPermissions)
    {
        foreach (RadTreeNode node in rtvPermissions.Nodes)
        {
            bool hasPermission = userPermissions.Where(p => p.PermissionID.ToString() == node.Value.ToString()).Count() > 0;
            if (hasPermission)
            {
                node.Checked = true;

                foreach (RadTreeNode ChildNode in node.Nodes)
                {
                    bool hasChildPermission = userPermissions.Where(p => p.PermissionID.ToString() == ChildNode.Value.ToString()).Count() > 0;
                    if (hasChildPermission)
                    {
                        ChildNode.Checked = true;
                    }
                }


            }
        }
              
    }

    public void FillResetPasswordData(ApplicationUser ApplicationUser)
    {
        hdnId.Value = ApplicationUser.ID.ToString();
        hdnUserId.Value = ApplicationUser.UserID.ToString();
        currentObject = ApplicationUser;
    }

    public ApplicationUser SetData()
    {
        int id = Manager.GetInteger(hdnId.Value);
        if (id == 0)
        {
            currentObject = new ApplicationUser();
            currentObject.User = new User();
        }

        ApplicationUser ApplicationUser = currentObject;
        ApplicationUser.User.MaxDailyCases = txtDailyMaxCases.Text.ToInt();
        ApplicationUser.User.MobileNumber = txtMobileNumber.Text.Trim();
        ApplicationUser.User.AppTypeID = 2;//Admin
        ApplicationUser.User.UserName = txtUserName.Text.Trim();
        ApplicationUser.User.EmailAddress = txtEmailAddress.Text.Trim();
        ApplicationUser.User.FullName = txtFullName.Text.Trim();
        ApplicationUser.User.Address = txtAddress.Text.Trim();
        ApplicationUser.User.IsActive = chkIsActive.Checked;

            foreach (UploadedFile file in ruSignature.UploadedFiles)
            {
              byte[] bytes = new byte[file.ContentLength];
              file.InputStream.Read(bytes, 0, file.ContentLength);
              ApplicationUser.User.Signature = bytes;

            }

        if (trPassword.Visible)
            ApplicationUser.User.Password = EncryptionHelper.Encrypt(txtPassword.Text);

        return ApplicationUser;
    }

    private List<UserPermission> SetAdminPermissions(int userId, int createdBy)
    {
        List<UserPermission> userPermissions = new List<UserPermission>();
        string permissionsIds = string.Empty;
              
        foreach (RadTreeNode ParentItem in rtvPermissions.Nodes)
        {
            if (ParentItem.Checked)
            {
                UserPermission permission = new UserPermission() { CreatedBy = createdBy, CreationDate = DateTime.Now, UserID = userId};
                permission.PermissionID = Manager.GetInteger(ParentItem.Value);
                userPermissions.Add(permission);
                permissionsIds += ParentItem.Value + ",";
            }
            foreach (RadTreeNode ChildNode in ParentItem.Nodes)
            {
                if (ChildNode.Checked)
                {
                    UserPermission permission = new UserPermission() { CreatedBy = createdBy, CreationDate = DateTime.Now, UserID = userId };
                    permission.PermissionID = Manager.GetInteger(ChildNode.Value);
                    userPermissions.Add(permission);
                    permissionsIds += ChildNode.Value + ",";
                }

            }
        }
        return userPermissions;
    }

    public void FillPermissions() 
    {
        rtvPermissions.DataSource = Permission.GetAll();
        rtvPermissions.DataBind();
    }

    private bool Save()
    {
        if (PasswordVisible)
        {
            string valid = Manager.IsValidPassword(txtPassword.Text, txtRetypePassword.Text);

            if (valid != "")
            {
                ShowError(valid);
                return false;
            }
        }

        if (currentObject.ID == 0)
        {
            string username = txtUserName.Text.Trim();
            if (Vanrise.Fzero.Bypass.User.IsUserNameExists(username))
            {
                ShowError(Resources.Resources.UserNameExists);
                return false;
            }
        }
       

        
        ApplicationUser ApplicationUser = SetData();

        ApplicationUser.Save(ApplicationUser);
        List<UserPermission> userPermissions = SetAdminPermissions(ApplicationUser.UserID, CurrentUser.User.ID);
        UserPermission.DeleteByUserId(ApplicationUser.UserID);
        UserPermission.Save(userPermissions);

      


        gvApplicationUsers.Rebind();
        ClearForm();

        return true;


    }

    private void ClearSearchForm()
    {
        txtSearchUserName.Text = string.Empty;
        txtSearchFullName.Text = string.Empty;
        txtSearchEmailAddress.Text = string.Empty;
        txtSearchMobileNumber.Text = string.Empty;
        txtSearchAddress.Text = string.Empty;
    }

    private void ClearForm()
    {
        hdnId.Value = "0";

        txtUserName.Text = string.Empty;
        txtFullName.Text = string.Empty;
        txtEmailAddress.Text = string.Empty;
        txtMobileNumber.Text = string.Empty;
        txtDailyMaxCases.Text = string.Empty;
        txtAddress.Text = string.Empty;
        txtPassword.Text = string.Empty;
        txtRetypePassword.Text = string.Empty;
        chkIsActive.Checked = false;
        txtUserName.Enabled = true;
        chkIsActive.Enabled = true;
        trPassword.Visible = true;
        trRetypePassword.Visible = true;
        chkIsActive.Enabled = true;
        rtvPermissions.UncheckAllNodes();
        trPermission.Visible = true;
        trPermissionNotes.Visible = false;
        

        currentObject = null;
    }

    private void ShowResetSection()
    {
        trAddEdit.Visible = false;
        trResetPassword.Visible = true;
        trData.Visible = false;
    }

    private void ShowAddEditSection()
    {
        trAddEdit.Visible = true;
        trResetPassword.Visible = false;
        trData.Visible = false;
    }


    private void HideSections()
    {
        trAddEdit.Visible = false;
        trResetPassword.Visible = false;
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
            FillPermissions();
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
        gvApplicationUsers.Rebind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Save())
            HideSections();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvApplicationUsers.Rebind();
    }

    protected void btnResetPassword_Click(object sender, EventArgs e)
    {
        //Reset();
        HideSections();
    }

    protected void btnResetPasswordCancel_Click(object sender, EventArgs e)
    {
        //ClearResetForm();
        HideSections();
    }

    protected void wucChangePassword_ShowError(string err)
    {
        ShowError(err);
    }

    protected void wucChangePassword_Success()
    {
        HideSections();
    }

    protected void repPermissions_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item != null && e.Item.DataItem != null)
        {
            Permission p = (Permission)e.Item.DataItem;
            if (!p.ParentID.HasValue)
            {
                if (currentObject.User.UserPermissions.Where(i => i.PermissionID == p.ID).Count() > 0)
                {
                    CheckBox chk = (CheckBox)e.Item.FindControl("chk");
                    chk.Checked = true;
                }
            }
            else
            {
                HtmlInputCheckBox chkChild = (HtmlInputCheckBox)e.Item.FindControl("chkChild");
                //if (!Userpermissions.Contains(p.ParentId.Value))
                if (currentObject.User.UserPermissions.Where(i => i.PermissionID == p.ParentID.Value).Count() > 0)
                {
                    chkChild.Disabled = true;
                }
                else
                {
                    //if (Userpermissions.Contains(p.Id))
                    if (currentObject.User.UserPermissions.Where(i => i.PermissionID == p.ID).Count() > 0)
                    {
                        chkChild.Checked = true;
                    }
                }
            }
        }
    }

    protected void gvApplicationUsers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        string UserName = txtSearchUserName.Text.Trim();
        string FullName = txtSearchFullName.Text.Trim();
        string EmailAddress = txtSearchEmailAddress.Text.Trim();
        string MobileNumber = txtSearchMobileNumber.Text.Trim();
        string Address = txtSearchAddress.Text.Trim();
        bool? IsActive = Manager.GetNullableBoolean(rblSearchStatus.SelectedValue.ToString());

        gvApplicationUsers.DataSource = ApplicationUser.GetApplicationUsers(UserName, FullName, EmailAddress, Address, MobileNumber, IsActive);
      
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

        if (arg.Length == 3)
        {
            Id = Manager.GetInteger(arg[0]);
            UserId = Manager.GetInteger(arg[1]);
            FullName = arg[2];
        }


        switch (e.CommandName)
        {
            case "Remove":
                if (Vanrise.Fzero.Bypass.User.Delete(UserId))
                {
                    gvApplicationUsers.Rebind();
                    ClearForm();
                    ClearSearchForm();
                    
                }
                else
                    ShowError(Resources.Resources.UnabletoDelete);
                break;

            case "Modify":
                currentObject = ApplicationUser.Load(Id);
                FillData(currentObject);
                ShowAddEditSection();
                trPassword.Visible = false;
                trRetypePassword.Visible = false;
                txtUserName.Enabled = false;
                break;

            case "Reset":
                wucChangePassword.Load(UserId);
                ShowResetSection();
                break;

            case "ActivateDeactivate":
                if (ApplicationUser.ActivateDeactivate(Id))
                {
                    LoggedAction.AddLoggedAction((int) Enums.ActionTypes.ActivationDeactivationofanApplicationUser, CurrentUser.User.ID);
                    gvApplicationUsers.Rebind();
                    ClearSearchForm();
                    
                    if (UserId == CurrentUser.User.ID)
                    {
                        CurrentUser.Logout();
                        RedirectToAuthenticationPage();
                    }
                }
                else
                    ShowError(Resources.Resources.UnabletoActivateDeactivate);
                break;


        }
    }

    #endregion
      

}