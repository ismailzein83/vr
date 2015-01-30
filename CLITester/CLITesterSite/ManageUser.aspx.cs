using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Helpers;
using CallGeneratorLibrary.Utilities;

public partial class ManageUser : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            FillDropDownList();
            GetUser();
        }
    }
    #region Events
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int id = 0;
        if (ViewState["UserId"] != null)
            int.TryParse(ViewState["UserId"].ToString(), out id);

        User user = new User { Id = id };

        if (id > 0)
            user = UserRepository.Load(user.Id);

        user.Name = txtName.Text;
        user.LastName = txtLastName.Text;

        user.UserName = txtUserName.Text;
        user.Email = txtEmail.Text;
        user.CallerId = Current.User.CallerId;
        user.IpSwitch = Current.User.IpSwitch;
        user.MobileNumber = txtMobile.Text;
        user.IsActive = true;
        user.IsSuperAdmin = false;
        user.Guid = Guid.NewGuid().ToString();
        user.CreationDate = user.Id == 0 ? DateTime.Now : user.CreationDate;
        user.ParentId = Current.User.Id;
        //int UnitId = 0;
        //int.TryParse(ddlCompany.SelectedValue.Trim(), out UnitId);
        //if (UnitId == 0)
        //    user.CompanyId = null;
        //else
        //    user.CompanyId = UnitId;

        if (txtPassword.Text != "")
            user.Password = CommonWebComponents.SecureTextBox.GetHash(txtPassword.Text);

        if (user.Id == 0 && txtPassword.Text.Trim() == "")
        {
            JavaScriptAlert("Password field must be filled");
            return;
        }

        UserRepository.Save(user);
        ActionLog action = new ActionLog();
        action.ObjectId = user.Id;
        action.ObjectType = "User";
        action.Description = Utilities.SerializeLINQtoXML<User>(user);
        if (id == 0) // Add Operation - Action Log
            action.ActionType = (int)Enums.ActionType.Add;
        else // Edit Operation - Action Log
            action.ActionType = (int)Enums.ActionType.Modify;

        AuditRepository.Save(action);
        Response.Redirect("./ManageUsers.aspx", true);
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("./ManageUsers.aspx", true);
    }
    #endregion

    #region Methods
    private void GetUser()
    {
        string encryptedquerystring = Request.QueryString["?qs"].Replace(" ", "+");
        string decryptedquerystring = QueryStringModule.Decrypt(encryptedquerystring);
        string[] QueryStringparameters = decryptedquerystring.Split(new char[] { '&' });
        string loanProductId = QueryStringparameters[0].Substring(QueryStringparameters[0].IndexOf("=") + 1);

        if (!string.IsNullOrEmpty(loanProductId))
        {
            int id = 0;
            int.TryParse(loanProductId, out id);

            if (id > 0)
            {
                ViewState["UserId"] = id;
                User user = new User { Id = id };
                user = UserRepository.Load(user.Id);

                txtName.Text = user.Name;
                txtLastName.Text = user.LastName;
                txtMobile.Text = user.MobileNumber;
                txtEmail.Text = user.Email;
                txtUserName.Text = user.UserName;
                //txtCallerId.Text = user.CallerId;
                //txtIPSwitch.Text = user.IpSwitch;
                //chkIsActive.Checked = user.IsActive.HasValue ? user.IsActive.Value : false;
                //chkIsSuperAdmin.Checked = user.IsSuperAdmin.HasValue ? user.IsSuperAdmin.Value : false;
                //ddlCompany.SelectedValue = user.CompanyId.ToString();
            }
        }
    }
    private void FillDropDownList()
    {
        //List<Company> companies = CompanyRepository.GetCompanies().OrderBy(l => l.Name).ToList();
        //Company company = new Company { Id = 0, Name = "-- Select Company --" };
        //companies.Insert(0, company);
        //ddlCompany.DataTextField = "Name";
        //ddlCompany.DataValueField = "Id";
        //ddlCompany.DataSource = companies;
        //ddlCompany.DataBind();
    }
    #endregion
}