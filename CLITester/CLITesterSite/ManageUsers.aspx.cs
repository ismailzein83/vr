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

public partial class ManageUsers : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            GetData();
        }
    }
    #region Events
    protected void btnNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("./ManageUser.aspx?" + QueryStringModule.Encrypt("mId=0"), true);
    }
    #endregion

    #region Methods
    private void GetData()
    {
        List<User> Users = UserRepository.GetSubUsers(Current.User.Id).OrderByDescending(l => l.Id).ToList();
        Session["Users"] = Users;
        rptUsers.DataSource = Users;
        rptUsers.DataBind();
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        LinkButton lk = (LinkButton)sender;
        int id = 0;
        int.TryParse(lk.CommandArgument.ToString(), out id);
        if (UserRepository.Delete(id))
        {
            ActionLog action = new ActionLog();
            action.ObjectId = id;
            action.ObjectType = "User";
            action.UserId = Current.User.User.Id;
            //action.Description = Utilities.SerializeLINQtoXML<LoanProduct>(contractbidd);
            action.ActionType = (int)Enums.ActionType.Delete;
            AuditRepository.Save(action);
            GetData();
        }
        else
        {
            JavaScriptAlert("We can't delete a record with child rows");
            GetData();
            return;
        }
    }
    #endregion
}
