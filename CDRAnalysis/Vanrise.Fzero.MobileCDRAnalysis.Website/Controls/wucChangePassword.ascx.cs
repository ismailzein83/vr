using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;

public partial class Controls_wucChangePassword : System.Web.UI.UserControl
{
    public int UserID { get; set; }
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
            if (Session["Controls_wucChangePassword.currentObject"] is User)
                return (User)Session["Controls_wucChangePassword.currentObject"];
            return new User();
        }
        set
        {
            Session["Controls_wucChangePassword.currentObject"] = value;
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
            if (UserID > 0)
            {
                LoadUser(UserID);
            }
        }
    }
    public void LoadUser(int Id)
    {
        this.UserID = Id;
        hfId.Value = Id.ToString();
        Vanrise.Fzero.MobileCDRAnalysis.User user = Vanrise.Fzero.MobileCDRAnalysis.User.Load(Id);
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
        btnResetPassword.Text = "Reset ";
        btnCancel.Text = "Cancel";
    }

    public void FillResetPasswordData(User user)
    {
        hfId.Value = user.ID.ToString();
        txtRPUserName.Text = user.UserName;
    }


    private bool Reset()
    {
        string error = Manager.IsValidPassword(txtRPPassword.Text, txtRPRetypePassword.Text);

        if(string.IsNullOrWhiteSpace(error))
        {
            if (User.TruePassword(Manager.GetInteger(hfId.Value), EncryptionHelper.Encrypt(txtOldPassword.Text)) || txtOldPassword.Visible == false)
            {
                User.ResetPassword(Manager.GetInteger(hfId.Value), EncryptionHelper.Encrypt(txtRPPassword.Text));
                ShowError("Password has been changed");
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