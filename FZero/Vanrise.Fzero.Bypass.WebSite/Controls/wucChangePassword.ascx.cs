using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;

public partial class Controls_wucChangePassword : System.Web.UI.UserControl
{
    public int UserId { get; set; }
    public bool ShowOldPassword { get; set; }

    public delegate void ShowErrorEvent(string err);
    public event ShowErrorEvent ShowError;

    public delegate void Succeed();
    public event Succeed Success;

    #region Properties

    public User currentObject
    {
        get
        {
            if (Session["ManageApplicationUsers.currentObject"] is ApplicationUser)
                return (User)Session["ManageApplicationUsers.currentObject"];
            return new User();
        }
        set
        {
            Session["ManageApplicationUsers.currentObject"] = value;
        }
    }

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            if (UserId > 0)
            {
               Load(UserId);
            }
        }
    }
    public void Load(int userId)
    {
        this.UserId = userId;
        hdnUserId.Value = UserId.ToString();
        User user = User.Load(UserId);
        FillResetPasswordData(user);
    }

    private void SetPermissions()
    {
        TrOldPass.Visible = ShowOldPassword;
        btnCancel.Visible = !ShowOldPassword;
    }

    protected void btnResetPassword_Click(object sender, EventArgs e)
    {
        if (Reset()) 
        {
            Success();
        }
    }


    #endregion

    #region Methods

    private void SetCaptions()
    {
        
    }

    public void FillResetPasswordData(User user)
    {
        hdnUserId.Value = user.ID.ToString();
        txtRPUserName.Text = user.UserName;
        //currentObject = ApplicationUser;
    }


    private bool Reset()
    {
        string error = Manager.IsValidPassword(txtRPPassword.Text, txtRPRetypePassword.Text);

        if(string.IsNullOrWhiteSpace(error))
        {
            if (User.TruePassword(Manager.GetInteger(hdnUserId.Value), EncryptionHelper.Encrypt(txtOldPassword.Text)) || txtOldPassword.Visible == false)
            {
                User.ResetPassword(Manager.GetInteger(hdnUserId.Value), EncryptionHelper.Encrypt(txtRPPassword.Text));
                ShowError(Resources.Resources.Passwordhasbeenchanged);
                return true;
            }
            else{
                 if (ShowError != null)
                    ShowError("The Old Password is wrong");
                 return false;
            }
              
        }
        else
        {
            if (ShowError != null)
                ShowError(error);
            return  false;
        }

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Success();
    }


    #endregion

}