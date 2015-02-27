using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;
using System.Web.UI.HtmlControls;

public partial class ManageOperators : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        divError.Visible = false;

        if (!IsPostBack)
        {
            GetData();
        }
    }
    #region Methods
    private void GetData()
    {
        List<Operator> Carriers = OperatorRepository.GetOperators().ToList();
        rptCarriers.DataSource = Carriers;
        rptCarriers.DataBind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        foreach (RepeaterItem aItem in rptCarriers.Items)
        {
            Label lblId = (Label)aItem.FindControl("Label0");
            Label lblIsd = (Label)aItem.FindControl("Label6");

            int id = 0;
            int.TryParse(lblId.Text, out id);

            CheckBox chkAndroid = (CheckBox)aItem.FindControl("chkAndroid");
            CheckBox chkMonty = (CheckBox)aItem.FindControl("chkMonty");
            //HtmlInputCheckBox chkMonty = (HtmlInputCheckBox)aItem.FindControl("chkMonty");
            if (chkAndroid.Checked == false && chkMonty.Checked == false)
            {
                divError.Visible = true;
                divError.InnerHtml = "Please check at least one service";
                return;
            }
            Operator op = OperatorRepository.Load(id);
            if (chkMonty.Checked == true)
                op.ServiceMonty = true;
            else
                op.ServiceMonty = false;

            if (chkAndroid.Checked == true)
                op.ServiceAndroid = true;
            else
                op.ServiceAndroid = false;
            OperatorRepository.Save(op);
        }
    }

    #endregion
}