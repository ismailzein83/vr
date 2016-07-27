using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;

public partial class ManageTestNumberGroups : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        divSuccess.Visible = false;
        divError.Visible = false;

        if (!IsPostBack)
        {
            GetData();

        }
    }

    #region Methods
    private void GetData()
    {
        List<TestNumberGroup> TestNumberGroups = TestNumberGroupRepository.GetTestNumberGroups().OrderByDescending(l => l.Id).ToList();
        Session["TestNumberGroup"] = TestNumberGroups;
        rptTestNumberGroups.DataSource = TestNumberGroups;
        rptTestNumberGroups.DataBind();
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "disableAddBtn2", "<script type='text/javascript'>disableAddBtn2();</script>", false);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        LinkButton lk = (LinkButton)sender;
        int id = 0;
        int.TryParse(lk.CommandArgument.ToString(), out id);
        if (TestNumberGroupRepository.Delete(id))
        {
            NewActionLog action = new NewActionLog();
            action.ObjectId = id;
            action.ObjectType = "TestNumberGroup";
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

    private void SetError(string Error)
    {
        divSuccess.Visible = false;
        divError.Visible = true;
        lblError.Text = Error;
    }
    private void SetSuccess(string Message)
    {
        divSuccess.Visible = true;
        divError.Visible = false;
        lblError.Text = Message;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(txtName.Text))
        {
            return;
        }

        TestNumberGroup newTestNumberGroup = new TestNumberGroup();

        newTestNumberGroup = TestNumberGroupRepository.Load(txtName.Text);
        if (newTestNumberGroup != null)
        {
            SetError("same group name");
        }
        else
        {
            NewActionLog action = new NewActionLog();
            action.ObjectId = Current.User.Id;
            action.ObjectType = "TestNumberGroup";

            if (String.IsNullOrEmpty(HdnId.Value))
            {
                action.ActionType = (int)Enums.ActionType.Add;
            }
            else
            {
                int id = 0;
                if (Int32.TryParse(HdnId.Value, out id))
                {
                    newTestNumberGroup = TestNumberGroupRepository.Load(id);
                    if (newTestNumberGroup == null) return;
                    action.ActionType = (int)Enums.ActionType.Modify;
                }
                else
                {
                    return;
                }
            }

            newTestNumberGroup.Name = txtName.Text;

            TestNumberGroupRepository.Save(newTestNumberGroup);

            AuditRepository.Save(action);

            GetData();
        }


    }
}