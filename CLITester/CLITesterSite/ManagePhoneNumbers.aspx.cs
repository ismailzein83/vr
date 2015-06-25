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
using CLINumberLibrary;
using System.Configuration;

public partial class ManagePhoneNumbers : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");
        
        List<CLINumberLibrary.Operator> lstOperators = new List<CLINumberLibrary.Operator>();
        lstOperators = CLINumberLibrary.OperatorRepository.GetOperators();
        rptOperators.DataSource = lstOperators;
        rptOperators.DataBind();

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
        List<CLINumberLibrary.PhoneNumber> lstPhoneNumbers = CLINumberLibrary.PhoneNumberRepository.GetPhoneNumbers().OrderByDescending(l => l.Id).ToList();
        Session["PhoneNumbers"] = lstPhoneNumbers;

        rptPhones.DataSource = lstPhoneNumbers;
        rptPhones.DataBind();
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
        if (CLINumberLibrary.PhoneNumberRepository.Delete(id))
        {
            ActionLog action = new ActionLog();
            action.ObjectId = id;
            action.ObjectType = "CLINumberLibrary.PhoneNumber";
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
        if (String.IsNullOrEmpty(txtNumber.Text))
        {
            return;
        }
        string phoneNumber = txtNumber.Text;
        if(txtNumber.Text.Length > 2)
        {
            string prefixNumber = txtNumber.Text.Substring(0, 2);
        
            if(prefixNumber == "00")
                phoneNumber = txtNumber.Text.Substring(2);
        }
        CLINumberLibrary.PhoneNumber newPhoneNumber = new CLINumberLibrary.PhoneNumber();

        ActionLog action = new ActionLog();
        action.ObjectType = "CLINumberLibrary.PhoneNumber";

        if (String.IsNullOrEmpty(HdnId.Value))
        {
            action.ActionType = (int)Enums.ActionType.Add;
        }
        else
        {
            int id = 0;
            if (Int32.TryParse(HdnId.Value, out id))
            {
                newPhoneNumber = CLINumberLibrary.PhoneNumberRepository.Load(id);
                if (newPhoneNumber == null) return;
                action.ActionType = (int)Enums.ActionType.Modify;
            }
            else
            {
                return;
            }
        }

        int operatorId = 0;
        if (Int32.TryParse(hdnOperatorId.Value, out operatorId))
        {
            newPhoneNumber.Number = phoneNumber;
            newPhoneNumber.OperatorId = operatorId;
            newPhoneNumber.Prefix = txtPrefix.Text;
            newPhoneNumber.CreationDate = DateTime.Now;
            newPhoneNumber.LastCallDate = DateTime.Now;
            newPhoneNumber.Status = 0;
            CLINumberLibrary.PhoneNumberRepository.Save(newPhoneNumber);

            action.ObjectId = newPhoneNumber.Id;
            action.Description = Utilities.SerializeLINQtoXML<CLINumberLibrary.PhoneNumber>(newPhoneNumber);
            action.UserId = Current.User.User.Id;
            AuditRepository.Save(action);
        }
        GetData();
    }
}