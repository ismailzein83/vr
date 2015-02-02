using Vanrise.CommonLibrary;
using System;
using Vanrise.Fzero.Bypass;


public partial class ApplicationUserProfile : BasePage
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

    #endregion

    #region Methods

    private void FillCombos()
    {
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.ApplicationUserProfile;
        lblSectionName.Text = Resources.Resources.EditProfile;
        btnSave.Text = Resources.Resources.Save;
    }

    public void FillData(ApplicationUser ApplicationUser)
    {
        hdnId.Value = ApplicationUser.ID.ToString();
        txtAddress.Text = ApplicationUser.User.Address;
        txtMobileNumber.Text = ApplicationUser.User.MobileNumber;
        txtUserName.Text = ApplicationUser.User.UserName;
        txtEmailAddress.Text = ApplicationUser.User.EmailAddress;
        txtFullName.Text = ApplicationUser.User.FullName;
       
        currentObject = ApplicationUser;
    }
    
    public void SetData()
    {
        ApplicationUser ApplicationUser = new ApplicationUser();
      
        ApplicationUser.ID = Manager.GetInteger(hdnId.Value);
        
        ApplicationUser.User = new User();

        if (hdnId.Value != "0")
        {
            int ApplicationUserId = Manager.GetInteger(hdnId.Value);
            ApplicationUser admin = ApplicationUser.Load(ApplicationUserId);
            ApplicationUser.UserID = admin.UserID;
            ApplicationUser.User = admin.User;
        }


        ApplicationUser.User.MobileNumber = txtMobileNumber.Text.Trim();
        ApplicationUser.User.Address = txtAddress.Text.Trim();
        ApplicationUser.User.UserName = txtUserName.Text.Trim();
        ApplicationUser.User.EmailAddress = txtEmailAddress.Text.Trim();
        ApplicationUser.User.FullName = txtFullName.Text.Trim();
        ApplicationUser.User.AppTypeID = 2;//Admin


        currentObject = ApplicationUser;
    }

    private bool Save()
    {
      SetData();
      ApplicationUser.Save(currentObject);

      if (currentObject.UserID == CurrentUser.User.ID)
      {
          CurrentUser.SetUser(ApplicationUser.Load(hdnId.Value.ToInt()).User);
      }

      ShowAlert(Resources.Resources.Profilehasbeenupdated);

      return true;
    }

    #endregion

    #region Events
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (CurrentUser.portalType != 2)//Admin
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            SetCaptions();
            FillCombos();
            hdnUserId.Value = CurrentUser.User.ID.ToString();
            currentObject = ApplicationUser.LoadbyUserId(Manager.GetInteger(hdnUserId.Value));
            FillData(currentObject);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Save();
    }


    #endregion

    
}