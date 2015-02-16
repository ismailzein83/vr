using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.CommonLibrary;
using Telerik.Web.UI;

public partial class UnNormalizedCalls : BasePage
{
    #region Properties

    List<CallsStatistics> CurrentList
    {
        get
        {
            if (Session["UnNormalizedCalls.CurrentList"] == null
                || !(Session["UnNormalizedCalls.CurrentList"] is List<CallsStatistics>))
                GetList();
            return (List<CallsStatistics>)Session["UnNormalizedCalls.CurrentList"];
        }
        set
        {
            Session["UnNormalizedCalls.CurrentList"] = value;
        }
    }
    int id { get { return (int)ViewState["Id"]; } set { ViewState["Id"] = value; } }
    #endregion


    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            SetCaptions();
            SetPermissions();
            FillControls();
            SetDetailsVisible(false);
        }
    }

    private void FillControls()
    {
        // Switches
        List<SwitchProfile> switches = SwitchProfile.GetAll();
        Manager.BindCombo(ddlSearchSwitches, switches, "Name", "Id", "", "0");
        Manager.BindCombo(ddlSwitches, switches, "Name", "Id", "", "0");

        // Party (CGPN, CDPN)
        List<ListItem> items = new List<ListItem>();
        items.Add(new ListItem(Constants.CGPN, Constants.CGPN));
        items.Add(new ListItem(Constants.CDPN, Constants.CDPN));
        rblstParty.Items.AddRange(items.ToArray());
       

        rblstSearchParty.Items.Add(new ListItem(Constants.CGPN, Constants.CGPN));
        rblstSearchParty.Items.Add(new ListItem(Constants.CDPN, Constants.CDPN));
        rblstSearchParty.SelectedIndex = 0;
    }

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "UnNormalized Calls";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadData();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearFiltrationFields();
        gvData.DataSource = null;
        gvData.DataBind();
    }
   
    #endregion

    #region Methods
    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.UnNormalizationRules))
            RedirectToAuthenticationPage();

        gvData.Columns[gvData.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.AddRule);
    }

    private void ClearFiltrationFields()
    {
        dtpFromDate.Clear();
        dtpToDate.Clear();
        ddlSearchSwitches.SelectedIndex = 0;
        rblstSearchParty.SelectedValue = string.Empty;
    }

    private void LoadData()
    {
        FillGrid();
    }

    private List<CallsStatistics> GetList()
    {
        DateTime? fromDate = dtpFromDate.SelectedDate;
        DateTime? toDate = dtpToDate.SelectedDate;
        int switchId = Manager.GetInteger(ddlSearchSwitches.SelectedValue);
        string connectionString = ddlSearchSwitches.SelectedValue;
        string party = rblstSearchParty.SelectedValue;

        CurrentList = CallsStatistics.GetUnNormalizedCalls(switchId, party, fromDate, toDate);
        return CurrentList;
    }
    private void FillGrid()
    {
        gvData.DataSource = GetList();//CurrentList
        gvData.DataBind();
    }

    #endregion
    protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;

        int index = Manager.GetInteger(e.CommandArgument.ToString());
        if (index >= 0)
        {
            
            
            CallsStatistics callsStatistic = CurrentList[index];
            int switchId = Manager.GetInteger(ddlSearchSwitches.SelectedValue);
            NormalizationRule currentObject = new NormalizationRule() { SwitchId = switchId, Party = callsStatistic.Party, Prefix = callsStatistic.Prefix, CallLength = callsStatistic.Length };
           
            tdIgnore.Attributes.Remove("class");

            if (callsStatistic.Party == Constants.CGPN)
            {
                tdIgnore.Attributes.Add("class", "caption");
                rfvIgnore.Enabled = false;
                currentObject.In_TrunckName = callsStatistic.TrunckName;
            }
            else
            {
                tdIgnore.Attributes.Add("class", "caption required");
                rfvIgnore.Enabled = true;
                currentObject.Out_TrunckName = callsStatistic.TrunckName;
            }

            FillData(currentObject);
            SetDetailsVisible(true);


            //CallsStatistics callsStatistic = CurrentList[index];
            //int switchId = Manager.GetInteger(ddlSearchSwitches.SelectedValue);
            //CurrentUser.Params.Rule = new NormalizationRule() { SwitchId=switchId, Party= callsStatistic.Party,  Prefix = callsStatistic.Prefix, CallLength=callsStatistic.Length};
            //if (callsStatistic.Party == Constants.CGPN)
            //{
            //    CurrentUser.Params.Rule.In_TrunckName = callsStatistic.TrunckName;
            //}
            //else
            //{
            //    CurrentUser.Params.Rule.Out_TrunckName = callsStatistic.TrunckName;
            //}
            //Response.Redirect("NormalizationRules.aspx?mode=new");


        }
    }

    //-------------------------------------------

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        SetDetailsVisible(false);
        LoadData();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (IsValidData())
        {
            NormalizationRule rule = SetData();
            if (!NormalizationRule.Save(rule))
            {
                id = rule.Id;
                ShowError( "An error occured when trying to save data, kindly try to save later.");
                return;
            }
            SetDetailsVisible(false);
            LoadData();
        }
    }

    private void SetDetailsVisible(bool flag)
    {
        divFilter.Visible = !flag;
        divData.Visible = !flag;
        divDetails.Visible = flag;
    }


    private NormalizationRule SetData()
    {
        NormalizationRule currentObject = new NormalizationRule() { Id = id };

        currentObject.SwitchId = Manager.GetInteger(ddlSwitches.SelectedValue);
        currentObject.SwitchName = ddlSwitches.SelectedItem.Text;
        currentObject.Party = rblstParty.SelectedValue;
        if (currentObject.Party == Constants.CGPN)
        {
            currentObject.In_TrunckName = ddlTruncks.SelectedItem.Text;//txtTrunckName.Text.Trim();
            currentObject.In_TrunckId = Manager.GetInteger(ddlTruncks.SelectedValue);
        }
        else
        {
            currentObject.Out_TrunckName = ddlTruncks.SelectedItem.Text;//txtTrunckName.Text.Trim();
            currentObject.Out_TrunckId = Manager.GetInteger(ddlTruncks.SelectedValue);
        }
        currentObject.Prefix = txtPrefix.Text;
        currentObject.CallLength = txtLength.Text.ToNullableInt();
        currentObject.CallsCount = txtCallsCount.Text.ToNullableInt();
        currentObject.Durations = txtDuration.Text.ToNullableInt();
        currentObject.Supplement = string.IsNullOrWhiteSpace(txtSupplement.Text) ? null : txtSupplement.Text.Trim();
        currentObject.Ignore = txtIgnore.Text.ToNullableInt();

        currentObject.PrefixToAdd = string.IsNullOrWhiteSpace(txtPrefixToAdd.Text) ? null : txtPrefixToAdd.Text.Trim();
        currentObject.SubstringStartIndex = txtSubstringStartIndex.Text.ToNullableInt();
        currentObject.SubstringLength = txtSubstringLength.Text.ToNullableInt();
        currentObject.SuffixToAdd = string.IsNullOrWhiteSpace(txtSuffixToAdd.Text) ? null : txtSuffixToAdd.Text.Trim();
        return currentObject;
    }

    private bool IsValidData()
    {
        return true;
    }
    protected void cvRuleValidation_ServerValidate(object source, ServerValidateEventArgs args)
    {

    }
    protected void ddlSwitches_SelectedIndexChanged(object sender, EventArgs e)
    {
        int switchId = Manager.GetInteger(ddlSwitches.SelectedValue);
        FillTruncks(switchId);
    }

    private void FillTruncks(int switchId)
    {
        List<SwitchTrunck> truncks = new List<SwitchTrunck>();
        if (switchId > 0)
        {
            truncks = SwitchTrunck.GetList(switchId);
            Manager.BindCombo(ddlTruncks, truncks, "Name", "Id", "", "0");
        }
        else
        {
            ddlTruncks.DataSource = truncks;
            ddlTruncks.DataBind();
        }
    }

    private void FillData(NormalizationRule currentObject)
    {
        id = currentObject.Id;

        ddlSwitches.SelectedValue = currentObject.SwitchId.ToString();
        FillTruncks(currentObject.SwitchId.Value);
        rblstParty.SelectedValue = currentObject.Party;
        //txtTrunckName.Text = currentObject.Party == Constants.CGPN ? currentObject.In_Trunck : currentObject.Out_Trunck;
        if (currentObject.Party == Constants.CGPN)
        {
            if (currentObject.In_TrunckId.HasValue)
                ddlTruncks.SelectedValue = currentObject.In_TrunckId.Value.ToString();
            else
            {
                if (!string.IsNullOrWhiteSpace(currentObject.In_TrunckName))
                {
                    RadComboBoxItem item = ddlTruncks.Items.FindItemByText(currentObject.In_TrunckName);
                    if (item != null)
                        ddlTruncks.SelectedValue = item.Value;
                    else
                        ddlTruncks.SelectedIndex = -1;
                }
                else
                    ddlTruncks.SelectedIndex = -1;
            }
        }
        else
        {
            if (currentObject.Out_TrunckId.HasValue)
                ddlTruncks.SelectedValue = currentObject.Out_TrunckId.Value.ToString();
            else
            {
                if (!string.IsNullOrWhiteSpace(currentObject.Out_TrunckName))
                {
                    RadComboBoxItem item = ddlTruncks.Items.FindItemByText(currentObject.Out_TrunckName);
                    if (item != null)
                        ddlTruncks.SelectedValue = item.Value;
                    else
                        ddlTruncks.SelectedIndex = -1;
                }
                else
                    ddlTruncks.SelectedIndex = -1;
            }
        }
        txtPrefix.Text = currentObject.Prefix;
        txtLength.Text = currentObject.CallLength.HasValue ? currentObject.CallLength.ToString() : string.Empty;
        txtCallsCount.Text = currentObject.CallsCount.HasValue ? currentObject.CallsCount.ToString() : string.Empty;
        txtDuration.Text = currentObject.Durations.HasValue ? currentObject.Durations.ToString() : string.Empty;
        txtSupplement.Text = currentObject.Supplement;
        txtIgnore.Text = currentObject.Ignore.HasValue ? currentObject.Ignore.ToString() : string.Empty;

        txtPrefixToAdd.Text = currentObject.PrefixToAdd;
        txtSubstringStartIndex.Text = currentObject.SubstringStartIndex.HasValue ? currentObject.SubstringStartIndex.ToString() : string.Empty;
        txtSubstringLength.Text = currentObject.SubstringLength.HasValue ? currentObject.SubstringLength.ToString() : string.Empty;
        txtSuffixToAdd.Text = currentObject.SuffixToAdd;
    }

    

   
    //--------------------------------------------


}
