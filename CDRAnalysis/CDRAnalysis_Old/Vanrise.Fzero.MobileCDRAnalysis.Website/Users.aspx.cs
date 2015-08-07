using Vanrise.CommonLibrary;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Telerik.Web.UI;


public partial class Users : BasePage
{
    #region Properties
    public User currentObject
    {
        get
        {
            if (Session["ApplicationUsers.currentObject"] is User)
                return (User)Session["ApplicationUsers.currentObject"];
            return new User();
        }
        set
        {
            Session["ApplicationUsers.currentObject"] = value;
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
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.Users))
            RedirectToAuthenticationPage();


        btnAddNew.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageUsers);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 4].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ActivateDeactivateUser);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 3].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ResetUserPassword);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageUsers);
        gvApplicationUsers.Columns[gvApplicationUsers.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageUsers);
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = "Users";



     

        int columnIndex = 0;
        gvApplicationUsers.Columns[columnIndex++].HeaderText = "Full Name";
        gvApplicationUsers.Columns[columnIndex++].HeaderText = "Email";
        gvApplicationUsers.Columns[columnIndex++].HeaderText = "Username";
        gvApplicationUsers.Columns[columnIndex++].HeaderText = "ID";
        gvApplicationUsers.Columns[columnIndex++].HeaderText = "Address";
        gvApplicationUsers.Columns[columnIndex++].HeaderText = "Mobile Number";
        gvApplicationUsers.Columns[columnIndex++].HeaderText = "Is Active";
        gvApplicationUsers.Columns[columnIndex++].HeaderText = "Last Login Time";


    }

    public void FillData(User User)
    {
        hdnId.Value = User.ID.ToString();
        txtAddress.Text = User.Address;
        txtMobileNumber.Text = User.MobileNumber;
        txtUserName.Text = User.UserName;
        txtEmailAddress.Text = User.EmailAddress;
        txtFullName.Text = User.FullName;

        chkIsActive.Enabled = false;

        if (User.IsSuperUser)
        {
            chkIsActive.Checked = true;
            trPermission.Visible = false;
            trPermissionNotes.Visible = true;
        }
        else
        {
            chkIsActive.Checked = User.IsActive;
            trPermission.Visible = true;
            trPermissionNotes.Visible = false;
            FillAdminPermissions(User.UserPermissions);
        }

        currentObject = User;

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

    public void FillResetPasswordData(User User)
    {
        hdnId.Value = User.ID.ToString();
        currentObject = User;
    }

    public User SetData()
    {
        int id = Manager.GetInteger(hdnId.Value);
        if (id == 0)
        {
            currentObject = new User();
        }

        Vanrise.Fzero.MobileCDRAnalysis.User User = currentObject;
        User.MobileNumber = txtMobileNumber.Text.Trim();
        User.UserName = txtUserName.Text.Trim();
        User.EmailAddress = txtEmailAddress.Text.Trim();
        User.FullName = txtFullName.Text.Trim();
        User.Address = txtAddress.Text.Trim();
        User.IsActive = chkIsActive.Checked;

        if (trPassword.Visible)
            User.Password = EncryptionHelper.Encrypt(txtPassword.Text);

        return User;
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
                return false;
            }
        }

        if (currentObject.ID == 0)
        {
            string username = txtUserName.Text.Trim();
            if (Vanrise.Fzero.MobileCDRAnalysis.User.IsUserNameExists(username))
            {
                ShowError("Username already Exists for other user");
                return false;
            }
        }
       

        User User = SetData();

        User.Save(User);
        List<Vanrise.Fzero.MobileCDRAnalysis.UserPermission> userPermissions = SetAdminPermissions(User.ID, CurrentUser.User.ID);
        Vanrise.Fzero.MobileCDRAnalysis.UserPermission.DeleteByUserId(User.ID);
        Vanrise.Fzero.MobileCDRAnalysis.UserPermission.Save(userPermissions);

      


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
            return "Deactivate";
        }
        return "Activate";
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
                if (currentObject.UserPermissions.Where(i => i.PermissionID == p.ID).Count() > 0)
                {
                    CheckBox chk = (CheckBox)e.Item.FindControl("chk");
                    chk.Checked = true;
                }
            }
            else
            {
                HtmlInputCheckBox chkChild = (HtmlInputCheckBox)e.Item.FindControl("chkChild");
                //if (!Userpermissions.Contains(p.ParentId.Value))
                if (currentObject.UserPermissions.Where(i => i.PermissionID == p.ParentID.Value).Count() > 0)
                {
                    chkChild.Disabled = true;
                }
                else
                {
                    //if (Userpermissions.Contains(p.Id))
                    if (currentObject.UserPermissions.Where(i => i.PermissionID == p.ID).Count() > 0)
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

        gvApplicationUsers.DataSource = Vanrise.Fzero.MobileCDRAnalysis.User.GetUsers(UserName, FullName, EmailAddress, Address, MobileNumber, IsActive);
      
    }

    protected void gvApplicationUsers_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        string[] arg = new string[2];
        arg = e.CommandArgument.ToString().Split(';');

        int Id = 0;
        string FullName = string.Empty;

        if (arg.Length == 2)
        {
            Id = Manager.GetInteger(arg[0]);
            FullName = arg[1];
        }


        switch (e.CommandName)
        {
            case "Remove":
                if (Vanrise.Fzero.MobileCDRAnalysis.User.Delete(Id))
                {
                    gvApplicationUsers.Rebind();
                    ClearForm();
                    ClearSearchForm();
                }
                else
                    ShowError("Unable to delete");
                break;

            case "Modify":
                currentObject = Vanrise.Fzero.MobileCDRAnalysis.User.Load(Id);
                FillData(currentObject);
                ShowAddEditSection();
                trPassword.Visible = false;
                trRetypePassword.Visible = false;
                txtUserName.Enabled = false;
                break;

            case "Reset":
                wucChangePassword.LoadUser(Id);
                ShowResetSection();
                break;

            case "ActivateDeactivate":
                if (Vanrise.Fzero.MobileCDRAnalysis. User.ActivateDeactivate(Id))
                {
                    gvApplicationUsers.Rebind();
                    ClearSearchForm();
                }
                else
                    ShowError("Unable to activate deactivate");
                break;


        }
    }

    #endregion
      

}