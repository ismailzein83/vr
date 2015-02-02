using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Vanrise.CommonLibrary;

using Vanrise.Fzero.Bypass;

public partial class wucMobileOperatorInformation : System.Web.UI.UserControl
{

    #region Properties
   

    public string MobileOperatorId
    {
        get
        {
            return currentObject.ID.ToString();
        }
        set
        {
            currentObject.ID = value.ToInt();
        }
    }

    public Vanrise.Fzero.Bypass.MobileOperator currentObject
    {
        get
        {
            if (Session["Controls_MobileOperatorInformation.currentObject"] is Vanrise.Fzero.Bypass.MobileOperator)
                return (Vanrise.Fzero.Bypass.MobileOperator)Session["Controls_MobileOperatorInformation.currentObject"];
            return new Vanrise.Fzero.Bypass.MobileOperator();
        }
        set
        {
            Session["Controls_MobileOperatorInformation.currentObject"] = value;
        }
    }

    public void BindGrid()
    {
    }

    public string Email
    {
        get
        {
            return txtEmail.Text;
        }
        set
        {
            txtEmail.Text = value;
        }
    }

    public string Mobile
    {
        get
        {
            return txtMobile.Text;
        }
        set
        {
            txtMobile.Text = value;
        }
    }

    public string MobileOperatorRetypePassword
    {
        get
        {
            return txtMobileOperatorConfirmPassword.Text;
        }
        set
        {
            txtMobileOperatorConfirmPassword.Text = value;
        }
    }

    public string MobileOperatorPassword
    {
        get
        {
            return txtMobileOperatorPassword.Text;
        }
        set
        {
            txtMobileOperatorPassword.Text = value;
        }
    }

    public string MobileOperatorUserName
    {
        get
        {
            return txtMobileOperatorUserName.Text;
        }
        set
        {
            txtMobileOperatorUserName.Text = value;
        }
    }

    public string ContactName
    {
        get
        {
            return txtContactName.Text;
        }
        set
        {
            txtContactName.Text = value;
        }
    }

    public string ProfileName
    {
        get
        {
            return txtProfileName.Text;
        }
        set
        {
            txtProfileName.Text = value;
        }
    }
   
    public string Prefix
    {
        get
        {
            return txtPrefix.Text ;
        }
        set
        {
            txtPrefix.Text= value;
        }
    }

    public string Website
    {
        get
        {
            return txtWebsite.Text;
        }
        set
        {
            txtWebsite.Text = value;
        }
    }

    public bool PasswordVisible
    {
        get
        {
            return trPassword1.Visible;
        }
        set
        {
            trPassword1.Visible = value;
            trPassword2.Visible = value;
        }
    }

    public bool txtUserNameReadOnly
    {
        get
        {
            return txtMobileOperatorUserName.ReadOnly;
        }
        set
        {
            txtMobileOperatorUserName.ReadOnly = value;
        }
    }

    #endregion

    #region Methods

    public void ControlsEnabled(bool Enabled)
    {
       
        txtMobile.ReadOnly = !Enabled;
        txtMobileOperatorConfirmPassword.ReadOnly = !Enabled;
        txtEmail.ReadOnly = !Enabled;
        txtContactName.ReadOnly = !Enabled;
        txtPrefix.ReadOnly = !Enabled;
        txtWebsite.ReadOnly = !Enabled;
    }

    public string IsValidData()
    {

        List<Vanrise.Fzero.Bypass.MobileOperator> listMobileOperator = Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperators();

        List<string> Prefixes = Prefix.Split(';').ToList<string>();

        foreach (string p in Prefixes)
        {
            if (p != string.Empty)
            {
                int result = 0;
                if (!int.TryParse(p, out result))
                {
                    return Resources.Resources.InvalidPrefixes;
                }
                else
                {
                    foreach (Vanrise.Fzero.Bypass.MobileOperator mobileOperator in listMobileOperator)
                    {
                        if (mobileOperator.User.Prefix.Contains(p) && mobileOperator.ID != MobileOperatorId.ToInt())
                        {
                            return Resources.Resources.ThisPrefixAlreadyUsedBy + " " + mobileOperator.User.FullName;
                        }
                    }

                }


            }
        }


           

        if (string.IsNullOrWhiteSpace(ProfileName))
        {
            return Resources.Resources.FullNameisrequired;
        }

        if (!string.IsNullOrWhiteSpace(Website) && !Manager.IsValidUrl(Website))
        {
            return Resources.Resources.Websiteisnotavalid;
        }

      

        if (string.IsNullOrWhiteSpace(Email))
        {
            return Resources.Resources.Emailisrequired;
        }
        else if (!Manager.IsValidEmail(Email))
        {
            return Resources.Resources.Emailisnotavalid;
        }


        if (! txtUserNameReadOnly)
        {
            if (string.IsNullOrWhiteSpace(MobileOperatorUserName))
            {
                return Resources.Resources.UserNameisrequired;
            }
        }

        if (PasswordVisible)
        {
          string errorMsg = Manager.IsValidPassword(MobileOperatorPassword, MobileOperatorRetypePassword);
          if (!string.IsNullOrEmpty(errorMsg))
              return errorMsg;
        }


        return string.Empty;
    }

    public void FillData(Vanrise.Fzero.Bypass.MobileOperator MobileOperator)
    {
       


        hdnId.Value = MobileOperator.ID.ToString();
        ContactName = MobileOperator.User.ContactName.ToText().Trim();
        Mobile = MobileOperator.User.Mobile.ToText().Trim();
        MobileOperatorUserName = MobileOperator.User.UserName.Trim();
        ProfileName = MobileOperator.User.FullName.ToString().Trim();

        if (Vanrise.Fzero.Bypass.SysParameter.Global_DefaultMobileOperator != MobileOperator.User.FullName)
        {
            Prefix = MobileOperator.User.Prefix.ToText().Trim();
            txtPrefix.Visible = true;
            lblPrefixes.Visible = false;
        }
        else
        {
            Prefix = ";;";
            txtPrefix.Visible = false;
            lblPrefixes.Visible = true;
        }

        
        Website = MobileOperator.User.Website.ToText().Trim();
        PasswordVisible = false;
        txtUserNameReadOnly = true;
        Email = MobileOperator.User.EmailAddress.ToString().Trim();

        currentObject = MobileOperator;
    }

    public Vanrise.Fzero.Bypass.MobileOperator SetData()
    {

        int id = Manager.GetInteger(hdnId.Value);
        if (id == 0)
        {
            currentObject = new Vanrise.Fzero.Bypass.MobileOperator();
            
        }
        Vanrise.Fzero.Bypass.MobileOperator MobileOperator = currentObject;
     

        if ( currentObject.User == null)
            currentObject.User = new Vanrise.Fzero.Bypass.User();



        MobileOperator.User.Prefix = Prefix;
        MobileOperator.User.Mobile = Mobile;
        MobileOperator.User.ContactName = ContactName;
        MobileOperator.User.Website = Website;
        MobileOperator.User.AppTypeID = 1; //Public
        MobileOperator.User.FullName = ProfileName;
        MobileOperator.User.EmailAddress = Email;


        if (!txtUserNameReadOnly)
        {
            MobileOperator.User.UserName = MobileOperatorUserName;
        }


        if (PasswordVisible)
        {
            MobileOperator.User.Password = EncryptionHelper.Encrypt(MobileOperatorPassword);
            MobileOperator.User.IsActive = false;
        }


        return MobileOperator;
    }

    #endregion
}