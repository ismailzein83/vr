using System;
using System.Collections.Generic;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;

public partial class Master : System.Web.UI.MasterPage
{
    #region Properties
    public void WriteError(string msg)
    {
        wucNotificationControlMain.WriteError(msg);
    }
    public void ShowMenu(bool Visible)
    {
        rtbMain.Visible=true;
        RadToolBarButton rtbiTemp = new RadToolBarButton();
        rtbiTemp.NavigateUrl = "Login.aspx";
        rtbiTemp.Text = "Logout";
        rtbMain.Items.Clear();
        rtbMain.Items.Add(rtbiTemp); 
    }
    public void ShowTitle(bool Visible)
    {
        //divTitle.Visible=Visible;
    }
    public void WriteSucess(string msg)
    {
        wucNotificationControlMain.WriteSucess(msg);
    }
    public void ClearError()
    {
        wucNotificationControlMain.ClearError();
    }

    public string PageHeaderTitle
    {
        set
        {
            lbltitle.InnerText = "-| " + value + " |-";
            Page.Title = value;
        }
    }

    public string SignedInUser
    {
        set
        {
            lblSignedInUser.Text= "Logged In: " + value;
        }
    }

    #endregion

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadMenus();
        }
                             
    }
    #endregion

    #region Methods
    public void LoadMenus()
    {
        if (BasePage.CurrentUser.User == null
            || !BasePage.CurrentUser.User.AppTypeID.HasValue
            || BasePage.CurrentUser.User.AppTypeID.Value == 0)//None
        {
            SetData(Vanrise.Fzero.Bypass.WebsiteMenu.GetMenus(0));//None
        }
        else
        {
            if (BasePage.CurrentUser.User.AppTypeID == 2)//Admin
            {
                SetData(Vanrise.Fzero.Bypass.WebsiteMenu.GetMenus(2));//Admin
            }
            else if (BasePage.CurrentUser.User.AppTypeID == 1)//Public
            {
                SetData(Vanrise.Fzero.Bypass.WebsiteMenu.GetMenus(1));//Public
            }
        }
    }


    private void SetData(List<WebsiteMenu> menus)
    {
        foreach (var i in menus)
        {
            if (i.WebsiteMenus1.Count > 0)
            {

                RadToolBarDropDown rtbiTemp = new RadToolBarDropDown();
                rtbiTemp.Text = i.Name;


                foreach (var j in i.WebsiteMenus1)
                {
                    if ((!j.PermissionID.HasValue) || (j.PermissionID.HasValue && BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)j.PermissionID)))
                    {
                        RadToolBarButton rtbiSubTemp = new RadToolBarButton();
                        rtbiSubTemp.NavigateUrl = j.URL;
                        rtbiSubTemp.Text = j.Name;
                        rtbiSubTemp.Value = j.PermissionID.ToString();
                        rtbiSubTemp.ForeColor = System.Drawing.Color.Black;
                        rtbiSubTemp.Width = 200;
                        rtbiTemp.Buttons.Add(rtbiSubTemp);
                    }
                }

                rtbMain.Items.Add(rtbiTemp);

            }
            else
            {
                if ((!i.PermissionID.HasValue) || (i.PermissionID.HasValue && BasePage.CurrentUser.HasPermission((Enums.SystemPermissions)i.PermissionID)))
                {
                    RadToolBarButton rtbiTemp = new RadToolBarButton();
                    rtbiTemp.NavigateUrl = i.URL;
                    rtbiTemp.Text = i.Name;
                    rtbiTemp.Value = i.PermissionID.ToString();
                    rtbMain.Items.Add(rtbiTemp);
                }


            }



        }

        rtbMain.DataBind();
    }
    #endregion

      
}