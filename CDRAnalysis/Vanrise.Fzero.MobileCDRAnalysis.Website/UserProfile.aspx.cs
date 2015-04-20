using Vanrise.CommonLibrary;
using System;
using Vanrise.Fzero.MobileCDRAnalysis;


public partial class UserProfile : BasePage
{
    #region Properties

    public User currentObject
    {
        get
        {
            if (Session["ApplicationUserProfile.currentObject"] is User)
                return (User)Session["ApplicationUserProfile.currentObject"];
            return new User();
        }
        set
        {
            Session["ApplicationUserProfile.currentObject"] = value;
        }
    }

    #endregion

    #region Methods

    private void FillCombos()
    {
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = "User Profile";
        btnSave.Text = "Save";
    }

    public void FillData(User User)
    {
        hdnId.Value = User.ID.ToString();
        txtAddress.Text = User.Address;
        txtMobileNumber.Text = User.MobileNumber;
        txtUserName.Text = User.UserName;
        txtEmailAddress.Text = User.EmailAddress;
        txtFullName.Text = User.FullName;

        currentObject = User;
    }
    
    public void SetData()
    {
        User User = new User();
        User.ID = hdnId.Value.ToInt();
        

        if (hdnId.Value != "0")
        {
            User = User.Load(User.ID);
        }


        User.MobileNumber = txtMobileNumber.Text.Trim();
        User.Address = txtAddress.Text.Trim();
        User.UserName = txtUserName.Text.Trim();
        User.EmailAddress = txtEmailAddress.Text.Trim();
        User.FullName = txtFullName.Text.Trim();


        currentObject = User;
    }

    private bool Save()
    {
      SetData();
      Vanrise.Fzero.MobileCDRAnalysis.User.Save(currentObject);
      ShowAlert("Profile has been updated successfully");
      return true;
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
            FillCombos();
            hdnUserId.Value = CurrentUser.User.ID.ToString();
            currentObject = Vanrise.Fzero.MobileCDRAnalysis.User.Load(Manager.GetInteger(hdnUserId.Value));
            FillData(currentObject);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Save();
    }


    #endregion

    
}