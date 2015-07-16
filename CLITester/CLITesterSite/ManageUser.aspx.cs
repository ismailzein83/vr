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
            GetUser();
        }
    }
    #region Events
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int id = 0;
        if (ViewState["UserId"] != null)
            int.TryParse(ViewState["UserId"].ToString(), out id);

        int balance = 0;
        int.TryParse(txtBalance.Text, out balance);

        User user = new User { Id = id };

        if (id > 0)
            user = UserRepository.Load(user.Id);

        user.Name = txtName.Text;
        user.LastName = txtLastName.Text;

        user.UserName = txtUserName.Text;
        user.Email = txtEmail.Text;
        user.MobileNumber = txtMobile.Text;
        user.IsActive = true;
        user.Guid = Guid.NewGuid().ToString();
        user.CreationDate = user.Id == 0 ? DateTime.Now : user.CreationDate;
        user.Role = (int)CallGeneratorLibrary.Utilities.Enums.UserRole.User;
        user.Balance = balance;
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
        action.UserId = Current.User.User.Id;
        //action.Description = Utilities.SerializeLINQtoXML<User>(user);
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
                txtBalance.Text = user.Balance.ToString();
            }
        }
    }
    #endregion
}