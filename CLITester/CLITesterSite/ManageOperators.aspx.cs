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
using System.Configuration;

public partial class ManageOperators : BasePage
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
        List<CLINumberLibrary.Operator> lstOperators = new List<CLINumberLibrary.Operator>();
        lstOperators = CLINumberLibrary.OperatorRepository.GetOperators();
        rptOperators.DataSource = lstOperators;
        rptOperators.DataBind();
    }
    public string GetURL()
    {
        return ConfigurationSettings.AppSettings["pathImg"];
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        LinkButton lk = (LinkButton)sender;
        int id = 0;
        int.TryParse(lk.CommandArgument.ToString(), out id);
        if (CLINumberLibrary.OperatorRepository.Delete(id))
        {
            ActionLog action = new ActionLog();
            action.ObjectId = id;
            action.ObjectType = "CLINumberLibrary.Operator";
            action.ActionType = (int)Enums.ActionType.Delete;
            AuditRepository.Save(action);
            GetData();
        }
        else
        {
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
        if(CLINumberLibrary.OperatorRepository.Load(txtMCC.Text,txtMNC.Text) != null)
        {
            SetError("Combination of MCC and MNC exist");
            return;
        }

        CLINumberLibrary.Operator newOperator = new CLINumberLibrary.Operator();

        ActionLog action = new ActionLog();
        action.ObjectType = "CLINumberLibrary.Operator";

        if (String.IsNullOrEmpty(HdnId.Value))
        {
            action.ActionType = (int)Enums.ActionType.Add;
        }
        else
        {
            int id = 0;
            if (Int32.TryParse(HdnId.Value, out id))
            {
                newOperator = CLINumberLibrary.OperatorRepository.Load(id);
                if (newOperator == null) return;
                action.ActionType = (int)Enums.ActionType.Modify;
            }
            else
            {
                return;
            }
        }

        newOperator.Name = txtName.Text;
        newOperator.mcc = txtMCC.Text;
        newOperator.mnc = txtMNC.Text;
        //newOperator.ServiceAndroid = true;
        //newOperator.ServiceMonty = null;
        newOperator.CountryPicture = hdnOperatorId.Value;
        newOperator.Country = hdnCountry.Value;
        CLINumberLibrary.OperatorRepository.Save(newOperator);

        action.ObjectId = newOperator.Id;
        action.Description = Utilities.SerializeLINQtoXML<CLINumberLibrary.Operator>(newOperator);
        action.UserId = Current.User.User.Id;
        AuditRepository.Save(action);

        GetData();
    }
}