﻿using System;
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

    public MobileOperator currentObject
    {
        get
        {
            if (Session["Controls_MobileOperatorInformation.currentObject"] is MobileOperator)
                return (MobileOperator)Session["Controls_MobileOperatorInformation.currentObject"];
            return new MobileOperator();
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

    public string AutoBlockEmail
    {
        get
        {
            return txtAutoBlockEmail.Text;
        }
        set
        {
            txtAutoBlockEmail.Text = value;
        }
    }

    public string SecurityEmail
    {
        get
        {
            return txtSecurityEmail.Text;
        }
        set
        {
            txtSecurityEmail.Text = value;
        }
    }

    public bool EnableSecurity
    {
        get
        {
            return chkEnableSecurity.Checked;
        }
        set
        {
            chkEnableSecurity.Checked = value;
        }
    }

    public bool EnableAutoBlock
    {
        get
        {
            return chkEnableAutoBlock.Checked;
        }
        set
        {
            chkEnableAutoBlock.Checked = value;
        }
    }

    public bool EnableAutoReport
    {
        get
        {
            return chkAutoReporting.Checked;
        }
        set
        {
            chkAutoReporting.Checked = value;
        }
    }

    public bool EnableNonFruadReport
    {
        get
        {
            return enableNonFruadReport.Checked;
        }
        set
        {
            enableNonFruadReport.Checked = value;
        }
    }

    public string NonFruadReportEmail
    {
        get
        {
            return nonFruadReportEmail.Text;
        }
        set
        {
            nonFruadReportEmail.Text = value;
        }
    }
    public bool EnableDailyReportsSummary
    {
        get
        {
            return enableDailyReportsSummary.Checked;
        }
        set
        {
            enableDailyReportsSummary.Checked = value;
        }
    }

    public string DailyReportsSummaryEmail
    {
        get
        {
            return dailyReportsSummaryEmail.Text;
        }
        set
        {
            dailyReportsSummaryEmail.Text = value;
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

    public string GMT
    {
        get
        {
            return ddlGMT.SelectedValue;
        }
        set
        {
            ddlGMT.SelectedValue = value;
        }
    }

    public string Prefix
    {
        get
        {
            return txtPrefix.Text;
        }
        set
        {
            txtPrefix.Text = value;
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
    public bool EnableFTP
    {
        get
        {
           return enableFTP.Checked;
        }
        set
        {
            enableFTP.Checked = value;
        }
    }

    public string FTPAddress
    {
        get
        {
            return ftpAddress.Text;
        }
        set
        {
            ftpAddress.Text = value;
        }
    }
    public string FTPPassword
    {
        get
        {
            return ftpPassword.Text;
        }
        set
        {
            ftpPassword.Text = value;
        }
    }
    public string FTPUserName
    {
        get
        {
            return ftpUserName.Text;
        }
        set
        {
            ftpUserName.Text = value;
        }
    }
    public int FTPType
    {
        get
        {
            return ftpType.SelectedValue.ToInt();
        }
        set
        {
            ftpType.SelectedValue = value.ToString();
        }
    }
    public bool IncludeCSVFile
    {
        get
        {
            return includeCSVFile.Checked;
        }
        set
        {
            includeCSVFile.Checked = value;
        }
    }
    public string FTPPort
    {
        get
        {
            return ftpPort.Text;
        }
        set
        {
            ftpPort.Text = value;
        }
    }

    public int? Compression
    {
        get
        {
            return compression.SelectedValue != null ? compression.SelectedValue.ToInt() : default(int?);
        }
        set
        {
            compression.Text = value.HasValue ? value.Value.ToString() : null;
        }
    }
    public int? SshEncryptionAlgorithm
    {
        get
        {
            return sshEncryptionAlgorithm.SelectedValue != null ? sshEncryptionAlgorithm.SelectedValue.ToInt(): default(int?);
        }
        set
        {
            sshEncryptionAlgorithm.Text = value.HasValue? value.Value.ToString() : null;
        }
    }
    public int? SshHostKeyAlgorithm
    {
        get
        {
            return sshHostKeyAlgorithm.SelectedValue != null ? sshHostKeyAlgorithm.SelectedValue.ToInt() : default(int?);
        }
        set
        {
            sshHostKeyAlgorithm.Text = value.HasValue ? value.Value.ToString() : null;
        }
    }
    public int? SshKeyExchangeAlgorithm
    {
        get
        {
            return sshKeyExchangeAlgorithm.SelectedValue != null ? sshKeyExchangeAlgorithm.SelectedValue.ToInt() : default(int?);
        }
        set
        {
            sshKeyExchangeAlgorithm.Text = value.HasValue ? value.Value.ToString() : null;
        }
    }
    public int? SshMacAlgorithm
    {
        get
        {
            return sshMacAlgorithm.SelectedValue != null ? sshMacAlgorithm.SelectedValue.ToInt() : default(int?);
        }
        set
        {
            sshMacAlgorithm.Text = value.HasValue ? value.Value.ToString() : null;
        }
    }
    public int? SshOptions
    {
        get
        {
            return sshOptions.SelectedValue != null ? sshOptions.SelectedValue.ToInt() : default(int?);
        }
        set
        {
            sshOptions.Text = value.HasValue ? value.Value.ToString() : null;
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
        ddlGMT.Enabled = Enabled;
        txtAutoBlockEmail.ReadOnly = !Enabled;
        chkEnableAutoBlock.Enabled = Enabled;
        chkAutoReporting.Enabled = Enabled;
    }

    public string IsValidData()
    {

        List<MobileOperator> listMobileOperator = MobileOperator.GetMobileOperators();

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
                    foreach (MobileOperator mobileOperator in listMobileOperator)
                    {
                        if (mobileOperator.User.Prefix != null && mobileOperator.User.Prefix.Contains(p) && mobileOperator.ID != MobileOperatorId.ToInt())
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


        if (string.IsNullOrWhiteSpace(AutoBlockEmail) && EnableAutoBlock)
        {
            return "AutoBlock email required";
        }
        else if (!Manager.IsValidEmail(AutoBlockEmail) && EnableAutoBlock)
        {
            return "AutoBlock email not valid";
        }


        if (string.IsNullOrWhiteSpace(NonFruadReportEmail) && EnableNonFruadReport)
        {
            return "Non-Fruad report email required";
        }
        else if (!Manager.IsValidEmail(NonFruadReportEmail) && EnableNonFruadReport)
        {
            return "Non-Fruad report email not valid";
        }



        if (string.IsNullOrWhiteSpace(DailyReportsSummaryEmail) && EnableDailyReportsSummary)
        {
            return "Daily reports summary email required";
        }
        //else if (!Manager.IsValidEmail(DailyReportsSummaryEmail) && EnableDailyReportsSummary)
        //{
        //    return "Daily reports summary email required";
        //}


        if (!txtUserNameReadOnly)
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

        if (string.IsNullOrWhiteSpace(GMT))
        {
            return Resources.Resources.GMTisrequired;
        }

        return string.Empty;
    }

    public void FillData(MobileOperator MobileOperator)
    {

        Manager.BindCombo(ddlGMT, Vanrise.Fzero.Bypass.GMT.GetGMTs(), "Name", "Id", null, null);

        hdnId.Value = MobileOperator.ID.ToString();
        ContactName = MobileOperator.User.ContactName.ToText().Trim();
        Mobile = MobileOperator.User.Mobile.ToText().Trim();
        MobileOperatorUserName = MobileOperator.User.UserName.Trim();
        ProfileName = MobileOperator.User.FullName.ToString().Trim();
        GMT = MobileOperator.User.GMT.ToString().Trim();
        EnableAutoBlock = MobileOperator.EnableAutoBlock;
        EnableAutoReport = MobileOperator.AutoReport;
        AutoBlockEmail = MobileOperator.AutoBlockEmail;
        SecurityEmail = MobileOperator.AutoReportSecurityEmail;

        DailyReportsSummaryEmail = MobileOperator.DailyReportsSummaryEmail;
        if (MobileOperator.EnableDailyReportsSummary.HasValue)
            EnableDailyReportsSummary = MobileOperator.EnableDailyReportsSummary.Value;
        else
        {
            EnableDailyReportsSummary = false;
        }

        NonFruadReportEmail = MobileOperator.NonFruadReportEmail;
        if (MobileOperator.EnableNonFruadReport.HasValue)
          EnableNonFruadReport = MobileOperator.EnableNonFruadReport.Value;
        else
        {
            EnableNonFruadReport = false;
        }
        EnableSecurity = MobileOperator.AutoReportSecurity;
        if(MobileOperator.IncludeCSVFile.HasValue)
          IncludeCSVFile = MobileOperator.IncludeCSVFile.Value;
        else
        {
            IncludeCSVFile = false;
        }
        if (MobileOperator.EnableFTP.HasValue)
         EnableFTP = MobileOperator.EnableFTP.Value;
        else
        {
            EnableFTP = false;
        }
        FTPAddress = MobileOperator.FTPAddress;
        FTPPassword = MobileOperator.FTPPassword;
        FTPUserName = MobileOperator.FTPUserName;
        FTPPort = MobileOperator.FTPPort;
        if (MobileOperator.FTPType.HasValue)
             FTPType = MobileOperator.FTPType.Value;

        if (SysParameter.Global_DefaultMobileOperator != MobileOperator.User.FullName)
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

        Compression = MobileOperator.Compression;
        SshEncryptionAlgorithm = MobileOperator.SshEncryptionAlgorithm;
        SshHostKeyAlgorithm = MobileOperator.SshHostKeyAlgorithm;
        SshKeyExchangeAlgorithm = MobileOperator.SshKeyExchangeAlgorithm;
        SshMacAlgorithm = MobileOperator.SshMacAlgorithm;
        SshOptions = MobileOperator.SshOptions;



        Website = MobileOperator.User.Website.ToText().Trim();
        PasswordVisible = false;
        txtUserNameReadOnly = true;
        Email = MobileOperator.User.EmailAddress.ToString().Trim();

        currentObject = MobileOperator;
    }

    public MobileOperator SetData()
    {

        int id = Manager.GetInteger(hdnId.Value);
        if (id == 0)
        {
            currentObject = new MobileOperator();

        }
        MobileOperator MobileOperator = currentObject;


        if (currentObject.User == null)
            currentObject.User = new User();



        MobileOperator.User.Prefix = Prefix;
        MobileOperator.User.Mobile = Mobile;
        MobileOperator.User.ContactName = ContactName;
        MobileOperator.User.Website = Website;
        MobileOperator.User.AppTypeID = 1; //Public
        MobileOperator.User.FullName = ProfileName;
        MobileOperator.User.EmailAddress = Email;
        MobileOperator.User.GMT = GMT.ToInt();
        MobileOperator.EnableAutoBlock = EnableAutoBlock;
        MobileOperator.AutoReport = EnableAutoReport;
        MobileOperator.AutoBlockEmail = AutoBlockEmail;

        MobileOperator.EnableNonFruadReport = EnableNonFruadReport;
        MobileOperator.NonFruadReportEmail = NonFruadReportEmail;

        MobileOperator.EnableDailyReportsSummary = EnableDailyReportsSummary;
        MobileOperator.DailyReportsSummaryEmail = DailyReportsSummaryEmail;

        MobileOperator.AutoReportSecurityEmail = SecurityEmail;
        MobileOperator.AutoReportSecurity = EnableSecurity;

        MobileOperator.EnableFTP = EnableFTP;
        MobileOperator.FTPAddress = FTPAddress;
        MobileOperator.FTPUserName = FTPUserName;
        MobileOperator.FTPPassword = FTPPassword;
        MobileOperator.FTPPort = FTPPort;
        MobileOperator.FTPType = FTPType;
        MobileOperator.IncludeCSVFile = IncludeCSVFile;
        if (!txtUserNameReadOnly)
        {
            MobileOperator.User.UserName = MobileOperatorUserName;
        }


        MobileOperator.Compression = Compression;
        MobileOperator.SshEncryptionAlgorithm = SshEncryptionAlgorithm;
        MobileOperator.SshHostKeyAlgorithm = SshHostKeyAlgorithm;
        MobileOperator.SshKeyExchangeAlgorithm = SshKeyExchangeAlgorithm;
        MobileOperator.SshMacAlgorithm = SshMacAlgorithm;
        MobileOperator.SshOptions =  SshOptions;


        if (PasswordVisible)
        {
            MobileOperator.User.Password = EncryptionHelper.Encrypt(MobileOperatorPassword);
            MobileOperator.User.IsActive = false;
        }


        return MobileOperator;
    }


    #endregion
}

