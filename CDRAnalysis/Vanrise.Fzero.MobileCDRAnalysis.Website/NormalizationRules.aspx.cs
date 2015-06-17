using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.CommonLibrary;
using Telerik.Web.UI;

public partial class NormalizationRules : BasePage
{
    #region Properties
    List<NormalizationRule> CurrentList
    {
        get
        {
            if (Session["NormalizationRules.CurrentList"] == null
                || !(Session["NormalizationRules.CurrentList"] is List<NormalizationRule>))
                GetList();
            return (List<NormalizationRule>)Session["NormalizationRules.CurrentList"];
        }
        set
        {
            Session["NormalizationRules.CurrentList"] = value;
        }
    }

    int id { get { return (int)ViewState["Id"]; } set { ViewState["Id"] = value; } }

    protected DataPagination pagination 
    { 
        get 
        {
            if(ViewState["pagination"] == null
                || (!(ViewState["pagination"] is DataPagination)))
                ViewState["pagination"] = new DataPagination();
            return (DataPagination)ViewState["pagination"]; 
        } 
        set { ViewState["pagination"] = value; } }
    #endregion

    #region Methods

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "Normalization Rules";
    }

    private bool IsValidData()
    {
        return true;
    }
    private void SetDetailsVisible(bool flag)
    {
        divFilter.Visible = !flag;
        divData.Visible = !flag;
        divDetails.Visible = flag;
    }

    private void ClearDetailsData()
    {
        id = 0;

        ddlSwitches.SelectedIndex = 0;
        rblstParty.SelectedIndex = -1;
        txtPrefix.Text = string.Empty;
        txtLength.Text = string.Empty;
        txtIgnore.Text = string.Empty;

        txtPrefixToAdd.Text = string.Empty;
        txtSubstringStartIndex.Text = string.Empty;
        txtSubstringLength.Text = string.Empty;
        txtSuffixToAdd.Text = string.Empty;

    }

    private void ClearFiltrationFields()
    {
        ddlSearchSwitches.SelectedIndex = 0;
        rblstSearchParty.SelectedIndex = 0;
        txtSearchLength.Text = string.Empty;
        txtSearchPrefix.Text = string.Empty;
    }

    private void LoadData()
    {
        GetList();
        SetPagingControls();
        gvData.PageIndex = 0;
        FillGrid();
    }

    private void SetPagingControls()
    {
        ddlPages.Items.Clear();
        for (int i = 1; i <= pagination.PagesCount; i++)
        {
            ddlPages.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        ddlPages.SelectedValue = pagination.CurrentPage.ToString();
    }

    private void GetList()
    {
        pagination = new DataPagination();
        int switchId = Manager.GetInteger(ddlSearchSwitches.SelectedValue);
        string party = rblstSearchParty.SelectedValue;
        int? length = txtSearchLength.Text.ToNullableInt();
        string prefix = txtSearchPrefix.Text;
        int pageSize = Manager.GetInteger(ddlPageSizes.SelectedValue);
        int pageNumber = Manager.GetInteger(ddlPages.SelectedValue, 1);
        string orderBy = string.Empty;
        int rowsCount = 0;
        CurrentList = NormalizationRule.GetList(switchId, party, length, prefix, pageSize, pageNumber, orderBy, out rowsCount);
        pagination = new DataPagination(rowsCount, pageSize, pageNumber);
    }
    private void FillGrid()
    {
        gvData.DataSource = CurrentList;
        gvData.DataBind();
    }

    private NormalizationRule SetData()
    {
        NormalizationRule currentObject = new NormalizationRule() { Id = id };

        currentObject.SwitchId = Manager.GetInteger(ddlSwitches.SelectedValue);
        currentObject.SwitchName = ddlSwitches.SelectedItem.Text;
        currentObject.Party = rblstParty.SelectedValue;
        currentObject.Prefix = txtPrefix.Text;
        currentObject.CallLength = txtLength.Text.ToNullableInt();
        currentObject.Ignore = txtIgnore.Text.ToNullableInt();

        currentObject.PrefixToAdd = string.IsNullOrWhiteSpace(txtPrefixToAdd.Text) ? null : txtPrefixToAdd.Text.Trim();
        currentObject.SubstringStartIndex = txtSubstringStartIndex.Text.ToNullableInt();
        currentObject.SubstringLength = txtSubstringLength.Text.ToNullableInt();
        currentObject.SuffixToAdd = string.IsNullOrWhiteSpace(txtSuffixToAdd.Text) ? null : txtSuffixToAdd.Text.Trim();
        return currentObject;
    }

    private void FillDetails(int id)
    {
        NormalizationRule currentObject = NormalizationRule.Load(id);
        FillData(currentObject);
        SetDetailsVisible(true);
    }

    private void FillData(NormalizationRule currentObject)
    {
        id = currentObject.Id;

        ddlSwitches.SelectedValue = currentObject.SwitchId.ToString();
        rblstParty.SelectedValue = currentObject.Party;
        txtPrefix.Text = currentObject.Prefix;
        txtLength.Text = currentObject.CallLength.HasValue ? currentObject.CallLength.ToString() : string.Empty;
        txtIgnore.Text = currentObject.Ignore.HasValue ? currentObject.Ignore.ToString() : string.Empty;

        txtPrefixToAdd.Text = currentObject.PrefixToAdd;
        txtSubstringStartIndex.Text = currentObject.SubstringStartIndex.HasValue ? currentObject.SubstringStartIndex.ToString() : string.Empty;
        txtSubstringLength.Text = currentObject.SubstringLength.HasValue ? currentObject.SubstringLength.ToString() : string.Empty;
        txtSuffixToAdd.Text = currentObject.SuffixToAdd;
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

        rblstSearchParty.Items.Add(new ListItem("All", ""));
        rblstSearchParty.Items.AddRange(items.ToArray());
        rblstSearchParty.SelectedIndex = 0;

        //Pages
        ddlPages.Items.Add(new ListItem ("1", "1"));
    }


    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.NormalizationRules))
            RedirectToAuthenticationPage();

        btnAdd.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageNormalizationRules);
        gvData.Columns[gvData.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageNormalizationRules);
        gvData.Columns[gvData.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageNormalizationRules);
    }

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
            bool isNewMode = Manager.QueryString("mode") == "new";
            if (isNewMode && CurrentUser.Params.Rule != null)
            {
                FillData(CurrentUser.Params.Rule);
                SetDetailsVisible(true);
            }
            else
            {
                LoadData();
                SetDetailsVisible(false);
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadData();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearFiltrationFields();
        LoadData();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ClearDetailsData();
        SetDetailsVisible(true);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (IsValidData())
        {
            NormalizationRule rule = SetData();

            if (NormalizationRule.CheckIfExists(rule))
            {
                if (!NormalizationRule.Save(rule))
                {
                    id = rule.Id;
                    ShowError( "An error occured when trying to save data, kindly try to save later.");
                    return;
                }
                SetDetailsVisible(false);
                LoadData();
            }
            else
            {
                ShowError("This rule is already found.");
            }
          
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        SetDetailsVisible(false);
        LoadData();
    }


    protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        id = Manager.GetInteger(e.CommandArgument.ToString());

        switch (e.CommandName)
        {
            case "Modify":
                FillDetails(id);
                break;
            case "Remove":
                NormalizationRule.Delete(id);
                LoadData();
                id = 0;
                break;
        }
    }
    #endregion
    protected void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadData();
    }
    protected void ddlPageSizes_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadData();
    }
    protected void cvRuleValidation_ServerValidate(object source, ServerValidateEventArgs args)
    {

    }

    protected void rblstParty_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rblstParty.SelectedValue == Constants.MSISDN)
        {
            tdIgnore.Attributes.Add("class", "caption");
            rfvIgnore.Enabled = false;
        }
        else
        {
            tdIgnore.Attributes.Add("class", "caption required");
            rfvIgnore.Enabled = true;
        }
    }
}