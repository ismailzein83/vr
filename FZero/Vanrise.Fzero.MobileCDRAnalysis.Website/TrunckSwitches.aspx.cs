using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.CommonLibrary;

public partial class TrunckSwitches : BasePage
{
    #region Properties
    List<SwitchTrunck> CurrentList
    {
        get
        {
            if (Session["TrunckSwitches.CurrentList"] == null
                || !(Session["TrunckSwitches.CurrentList"] is List<SwitchTrunck>))
                GetList();
            return (List<SwitchTrunck>)Session["TrunckSwitches.CurrentList"];
        }
        set
        {
            Session["TrunckSwitches.CurrentList"] = value;
        }
    }
    int trunckId { get { return (int)ViewState["TrunckId"]; } set { ViewState["TrunckId"] = value; } }
    int id1 { get { return (int)ViewState["Id1"]; } set { ViewState["Id1"] = value; } }
    int id2 { get { return (int)ViewState["Id2"]; } set { ViewState["Id2"] = value; } }
    #endregion

    #region Methods

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "Trunk";
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
        txtTrunckId.Text = string.Empty;

        ddlSwitches1.SelectedIndex = 0;
        txtTrunckName1.Text = string.Empty;
        txtTrunckFullName1.Text = string.Empty;
        rdlstDirection1.SelectedIndex = -1;

        ddlSwitches2.SelectedIndex = 0;
        txtTrunckName2.Text = string.Empty;
        txtTrunckFullName2.Text = string.Empty;
        rdlstDirection2.SelectedIndex = -1;
    }

    private void ClearFiltrationFields()
    {
        txtSearchSwitchName.Text = string.Empty;
        txtSearchTrunckName.Text = string.Empty;
    }

    private void LoadData()
    {
        GetList();
        gvData.PageIndex = 0;
        FillGrid();
    }

    private void GetList()
    {
        string switchName = txtSearchSwitchName.Text;
        string trunckName = txtSearchTrunckName.Text;

        CurrentList = SwitchTrunck.GetList(trunckName, switchName);
    }
    private void FillGrid()
    {
        gvData.DataSource = CurrentList;
        gvData.DataBind();
    }

    private Trunck SetData()
    {
        Trunck currentObject = new Trunck() { Id = trunckId };
        SwitchTrunck switch1 = new SwitchTrunck() { Id = id1 };
        SwitchTrunck switch2 = new SwitchTrunck() { Id = id2 };

        switch1.SwitchId = Manager.GetInteger(ddlSwitches1.SelectedValue);
        switch1.Name = txtTrunckName1.Text.Trim();
        switch1.FullName = txtTrunckFullName1.Text.Trim();
        switch1.DirectionId = rdlstDirection1.SelectedValue.ToInt();
        switch1.TrunckId = trunckId;

        switch2.SwitchId = Manager.GetInteger(ddlSwitches2.SelectedValue);
        switch2.Name = txtTrunckName2.Text.Trim();
        switch2.FullName = txtTrunckFullName2.Text.Trim();
        switch2.DirectionId = rdlstDirection2.SelectedValue.ToInt();
        switch2.TrunckId = trunckId;

        currentObject.SwitchTruncks.Add(switch1);
        currentObject.SwitchTruncks.Add(switch2);
        return currentObject;
    }

    private void FillDetails(int id)
    {
        Trunck currentObject = Trunck.Load(id);
        FillData(currentObject);
        SetDetailsVisible(true);
    }

    private void FillData(Trunck currentObject)
    {
        trunckId = currentObject.Id;
        id1 = currentObject.FirstSwitch.Id;
        id2 = currentObject.SecondSwitch.Id;

        txtTrunckId.Text = trunckId.ToString();

        ddlSwitches1.SelectedValue = currentObject.FirstSwitch.SwitchId.ToString();
        txtTrunckName1.Text = currentObject.FirstSwitch.Name;
        txtTrunckFullName1.Text = currentObject.FirstSwitch.FullName;
        rdlstDirection1.SelectedValue = currentObject.FirstSwitch.DirectionId.ToString();

        ddlSwitches2.SelectedValue = currentObject.SecondSwitch.SwitchId.ToString();
        txtTrunckName2.Text = currentObject.SecondSwitch.Name;
        txtTrunckFullName2.Text = currentObject.SecondSwitch.FullName;
        rdlstDirection2.SelectedValue = currentObject.SecondSwitch.DirectionId.ToString();
    }

    private void FillControls()
    {
        // Switches
        List<SwitchProfile> switches = SwitchProfile.GetAll();
        Manager.BindCombo(ddlSwitches1, switches, "Name", "Id", "", "0");
        Manager.BindCombo(ddlSwitches2, switches, "Name", "Id", "", "0");

        // Directions
        List<Direction> directions = Direction.GetAll();
        rdlstDirection1.DataSource = directions;
        rdlstDirection1.DataTextField = "Name";
        rdlstDirection1.DataValueField = "Id";
        rdlstDirection1.DataBind();
        rdlstDirection1.SelectedIndex = -1;

        rdlstDirection2.DataSource = directions;
        rdlstDirection2.DataTextField = "Name";
        rdlstDirection2.DataValueField = "Id";
        rdlstDirection2.DataBind();
        rdlstDirection2.SelectedIndex = -1;
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.Trunks))
            RedirectToAuthenticationPage();

        btnAdd.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageTrunks);
        gvData.Columns[gvData.Columns.Count - 2].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageTrunks);
        gvData.Columns[gvData.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageTrunks);
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
            SetDetailsVisible(false);
            FillControls();
            LoadData();
        }
        foreach (ListItem item in rdlstDirection1.Items)
        {
            item.Attributes.Add("onclick", "CheckDirections(this)");
        }
        foreach (ListItem item in rdlstDirection2.Items)
        {
            item.Attributes.Add("onclick", "CheckDirections(this)");
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
        trunckId = id1 = id2 = 0;
        ClearDetailsData();
        SetDetailsVisible(true);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (IsValidData())
        {
            Trunck trunck = SetData();
            if (SwitchTrunck.IsSwitchTrunckUsed(trunck.FirstSwitch))
            {
                ShowError("The trunk name (First Switch) can not be used more than once for the same switch.");
                return;
            }
            if (SwitchTrunck.IsSwitchTrunckUsed(trunck.SecondSwitch))
            {
                ShowError("The trunk name (Second Switch) can not be used more than once for the same switch");
                return;
            }
            if (!Trunck.Save(trunck))
            {
                trunckId = trunck.Id;
                id1 = trunck.FirstSwitch.Id;
                id2 = trunck.SecondSwitch.Id;
                ShowError( "An error occured when trying to save data, kindly try to save later.");
                return;
            }
            SetDetailsVisible(false);
            LoadData();
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        SetDetailsVisible(false);
    }


    protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        int rowIndex = Manager.GetInteger(e.CommandArgument.ToString());

        id1 = Manager.GetInteger(gvData.DataKeys[rowIndex]["Id"].ToString());
        trunckId = Manager.GetInteger(gvData.DataKeys[rowIndex]["TrunckId"].ToString());
        id2 = 0;

        if (e.CommandArgument != null)
        {
            switch (e.CommandName)
            {
                case "Modify":
                    FillDetails(trunckId);
                    break;
                case "Remove":
                    Trunck.Delete(trunckId);
                    LoadData();
                    trunckId = id1 = id2 = 0;
                    break;
            }
        }
    }
    #endregion
}