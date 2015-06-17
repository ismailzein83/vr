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
        items.Add(new ListItem(Constants.MSISDN, Constants.MSISDN));
        items.Add(new ListItem(Constants.Destination, Constants.Destination));
        rblstParty.Items.AddRange(items.ToArray());


        rblstSearchParty.Items.Add(new ListItem(Constants.MSISDN, Constants.MSISDN));
        rblstSearchParty.Items.Add(new ListItem(Constants.Destination, Constants.Destination));
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

            if (callsStatistic.Party == Constants.MSISDN)
            {
                tdIgnore.Attributes.Add("class", "caption");
                rfvIgnore.Enabled = false;
            }
            else
            {
                tdIgnore.Attributes.Add("class", "caption required");
                rfvIgnore.Enabled = true;
            }

            FillData(currentObject);
            SetDetailsVisible(true);


           


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
        currentObject.Prefix = txtPrefix.Text;
        currentObject.CallLength = txtLength.Text.ToNullableInt();
        currentObject.CallsCount = txtCallsCount.Text.ToNullableInt();
        currentObject.Durations = txtDuration.Text.ToNullableInt();
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
   


    private void FillData(NormalizationRule currentObject)
    {
        id = currentObject.Id;

        ddlSwitches.SelectedValue = currentObject.SwitchId.ToString();
        rblstParty.SelectedValue = currentObject.Party;
        txtPrefix.Text = currentObject.Prefix;
        txtLength.Text = currentObject.CallLength.HasValue ? currentObject.CallLength.ToString() : string.Empty;
        txtCallsCount.Text = currentObject.CallsCount.HasValue ? currentObject.CallsCount.ToString() : string.Empty;
        txtDuration.Text = currentObject.Durations.HasValue ? currentObject.Durations.ToString() : string.Empty;
        txtIgnore.Text = currentObject.Ignore.HasValue ? currentObject.Ignore.ToString() : string.Empty;

        txtPrefixToAdd.Text = currentObject.PrefixToAdd;
        txtSubstringStartIndex.Text = currentObject.SubstringStartIndex.HasValue ? currentObject.SubstringStartIndex.ToString() : string.Empty;
        txtSubstringLength.Text = currentObject.SubstringLength.HasValue ? currentObject.SubstringLength.ToString() : string.Empty;
        txtSuffixToAdd.Text = currentObject.SuffixToAdd;
    }

    

   
    //--------------------------------------------


}
