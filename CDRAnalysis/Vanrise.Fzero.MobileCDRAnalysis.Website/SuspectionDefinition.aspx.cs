using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
using Vanrise.CommonLibrary;


public partial class SuspectionDefinition : BasePage
{
    #region Properties

    List<Strategy_Suspicion_Level> CurrentSuspectionList
    {
        get
        {
            if (Session["SuspectionLevel.CurrentSuspectionList"] == null
                || !(Session["SuspectionLevel.CurrentSuspectionList"] is List<Strategy_Suspicion_Level>))
                GetList();
            return (List<Strategy_Suspicion_Level>)Session["SuspectionLevel.CurrentSuspectionList"];
        }
        set
        {
            Session["SuspectionLevel.CurrentSuspectionList"] = value;
        }
    }

   


    //-----------------
    int id { get { return (int)ViewState["Id"]; } set { ViewState["Id"] = value; } }


    private void FillControls()
    {
       

        List<Strategy> strategies = new List<Strategy>();
        strategies = Strategy.GetAll();
        Manager.BindCombo(ddlSearchStrategy, strategies, "Name", "Id", "", "0");
        Manager.BindCombo(ddlStrategies, strategies, "Name", "Id", "", "0");


        List<Suspicion_Level> suspection_Levels = new List<Suspicion_Level>();
        suspection_Levels = Suspicion_Level.GetAll();
        Manager.BindCombo(ddlSuspectionLevel, suspection_Levels, "Name", "Id", "", "0");


        int StartegyId;
        int.TryParse(Request.QueryString["Strategyid"], out StartegyId);

        if ((StartegyId) != 0)
        {
            ddlSearchStrategy.SelectedValue = StartegyId.ToString();
        }


    }

    #endregion

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetCaptions();
            SetDetailsVisible(false);
            FillControls();
            LoadData();

        }
    }

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "Suspicion Levels";
        List<Criteria_Profile> criteria_Profiles = Criteria_Profile.GetAll();
        lblCriteria1.Text = "Filter 1: (" + criteria_Profiles[0].Description + ")";
        lblCriteria2.Text = "Filter 2: (" + criteria_Profiles[1].Description + ")";
        lblCriteria3.Text = "Filter 3: (" + criteria_Profiles[2].Description + ")";
        lblCriteria4.Text = "Filter 4: (" + criteria_Profiles[3].Description + ")";
        lblCriteria5.Text = "Filter 5: (" + criteria_Profiles[4].Description + ")";
        lblCriteria6.Text = "Filter 6: (" + criteria_Profiles[5].Description + ")";
        lblCriteria7.Text = "Filter 7: (" + criteria_Profiles[6].Description + ")";
        lblCriteria8.Text = "Filter 8: (" + criteria_Profiles[7].Description + ")";
        lblCriteria9.Text = "Filter 9: (" + criteria_Profiles[8].Description + ")";
        lblCriteria10.Text = "Filter 10: (" + criteria_Profiles[9].Description + ")";
        lblCriteria11.Text = "Filter 11: (" + criteria_Profiles[10].Description + ")";
        lblCriteria12.Text = "Filter 12: (" + criteria_Profiles[11].Description + ")";
        lblCriteria13.Text = "Filter 13: (" + criteria_Profiles[12].Description + ")";
        lblCriteria14.Text = "Filter 14: (" + criteria_Profiles[13].Description + ")";
        lblCriteria15.Text = "Filter 15: (" + criteria_Profiles[14].Description + ")";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadData();
    }
    
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        id = 0;
        ClearDetailsData();
        SetDetailsVisible(true);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (IsValidData())
        {
            Strategy_Suspicion_Level Strategy_Suspicion_Level = SetData(); ;

            if (!Strategy_Suspicion_Level.Save(Strategy_Suspicion_Level))
            {
                id = Strategy_Suspicion_Level.Id;
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
    #endregion

    #region Methods

    public string Convert_Decimal_Percentage(string x)
    {
        return (100 * (x.ToDouble() - 1)).ToString()+"%"; 
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
        ddlStrategies.SelectedValue = ddlSearchStrategy.SelectedValue;
        ddlSuspectionLevel.SelectedValue = "0";
        chkCriteria1.Checked = false;
        chkCriteria2.Checked = false;
        chkCriteria3.Checked = false;
        chkCriteria4.Checked = false;
        chkCriteria5.Checked = false;
        chkCriteria6.Checked = false;
        chkCriteria7.Checked = false;
        chkCriteria8.Checked = false;
        chkCriteria9.Checked = false;
        chkCriteria10.Checked = false;
        chkCriteria11.Checked = false;
        chkCriteria12.Checked = false;
        chkCriteria13.Checked = false;
        chkCriteria14.Checked = false;
        chkCriteria15.Checked = false;


        RadSlider1.Visible = false;
        RadSlider2.Visible = false;
        RadSlider3.Visible = false;
        RadSlider4.Visible = false;
        RadSlider5.Visible = false;
        RadSlider6.Visible = false;
        RadSlider7.Visible = false;
        RadSlider8.Visible = false;
        RadSlider9.Visible = false;
        RadSlider10.Visible = false;
        RadSlider11.Visible = false;
        RadSlider12.Visible = false;
        RadSlider13.Visible = false;
        RadSlider14.Visible = false;
        RadSlider15.Visible = false;


        RadSlider1.SelectedValue = RadSlider1.Items[3].Value;
        RadSlider2.SelectedValue = RadSlider2.Items[3].Value;
        RadSlider3.SelectedValue = RadSlider3.Items[3].Value;
        RadSlider4.SelectedValue = RadSlider4.Items[3].Value;
        RadSlider5.SelectedValue = RadSlider5.Items[3].Value;
        RadSlider6.SelectedValue = RadSlider6.Items[3].Value;
        RadSlider7.SelectedValue = RadSlider7.Items[3].Value;
        RadSlider8.SelectedValue = RadSlider8.Items[3].Value;
        RadSlider9.SelectedValue = RadSlider9.Items[3].Value;
        RadSlider10.SelectedValue = RadSlider10.Items[3].Value;
        RadSlider11.SelectedValue = RadSlider11.Items[3].Value;
        RadSlider12.SelectedValue = RadSlider12.Items[3].Value;
        RadSlider13.SelectedValue = RadSlider13.Items[3].Value;
        RadSlider14.SelectedValue = RadSlider14.Items[3].Value;
        RadSlider15.SelectedValue = RadSlider15.Items[3].Value;

        

    }

    private void ClearFiltrationFields()
    {
        ddlSearchStrategy.SelectedValue = "0";
    }

    private void LoadData()
    {

        GetList();
        gvData.CurrentPageIndex = 0;
        FillGrid();
    }

    private void GetList()
    {
        int strategyId = 0;
        if (ddlSearchStrategy.SelectedItem != null)
            strategyId = int.Parse(ddlSearchStrategy.SelectedItem.Value);
        CurrentSuspectionList = Vanrise.Fzero.MobileCDRAnalysis.Strategy_Suspicion_Level.GetList(strategyId);
        
    }
    private void FillGrid()

    {

        gvData.DataSource = CurrentSuspectionList;
        gvData.DataBind();
    }

    private Strategy_Suspicion_Level  SetData()
    {
       Strategy_Suspicion_Level currentObject = new Strategy_Suspicion_Level() { Id = id };
       currentObject.StrategyId = int.Parse(ddlStrategies.SelectedItem.Value);
       currentObject.LevelId = int.Parse(ddlSuspectionLevel.SelectedItem.Value);
       currentObject.CriteriaId1 = chkCriteria1.Checked == true ? 1 : 0;
       currentObject.CriteriaId2 = chkCriteria2.Checked == true ? 1 : 0;
       currentObject.CriteriaId3 = chkCriteria3.Checked == true ? 1 : 0;
       currentObject.CriteriaId4 = chkCriteria4.Checked == true ? 1 : 0;
       currentObject.CriteriaId5 = chkCriteria5.Checked == true ? 1 : 0;
       currentObject.CriteriaId6 = chkCriteria6.Checked == true ? 1 : 0;
       currentObject.CriteriaId7 = chkCriteria7.Checked == true ? 1 : 0;
       currentObject.CriteriaId8 = chkCriteria8.Checked == true ? 1 : 0;
       currentObject.CriteriaId9 = chkCriteria9.Checked == true ? 1 : 0;
       currentObject.CriteriaId10 = chkCriteria10.Checked == true ? 1 : 0;
       currentObject.CriteriaId11 = chkCriteria11.Checked == true ? 1 : 0;
       currentObject.CriteriaId12 = chkCriteria12.Checked == true ? 1 : 0;
       currentObject.CriteriaId13 = chkCriteria13.Checked == true ? 1 : 0;
       currentObject.CriteriaId14 = chkCriteria14.Checked == true ? 1 : 0;
       currentObject.CriteriaId15 = chkCriteria15.Checked == true ? 1 : 0;

       currentObject.Cr1Per = RadSlider1.SelectedItem.Value.ToDecimal();
       currentObject.Cr2Per = RadSlider2.SelectedItem.Value.ToDecimal();
       currentObject.Cr3Per = RadSlider3.SelectedItem.Value.ToDecimal();
       currentObject.Cr4Per = RadSlider4.SelectedItem.Value.ToDecimal();
       currentObject.Cr5Per = RadSlider5.SelectedItem.Value.ToDecimal();
       currentObject.Cr6Per = RadSlider6.SelectedItem.Value.ToDecimal();
       currentObject.Cr7Per = RadSlider7.SelectedItem.Value.ToDecimal();
       currentObject.Cr8Per = RadSlider8.SelectedItem.Value.ToDecimal();
       currentObject.Cr9Per = RadSlider9.SelectedItem.Value.ToDecimal();
       currentObject.Cr10Per = RadSlider10.SelectedItem.Value.ToDecimal();
       currentObject.Cr11Per = RadSlider11.SelectedItem.Value.ToDecimal();
       currentObject.Cr12Per = RadSlider12.SelectedItem.Value.ToDecimal();
       currentObject.Cr13Per = RadSlider13.SelectedItem.Value.ToDecimal();
       currentObject.Cr14Per = RadSlider14.SelectedItem.Value.ToDecimal();
       currentObject.Cr15Per = RadSlider15.SelectedItem.Value.ToDecimal();

       return currentObject;
    }

    private void FillDetails(int id)
    {

        Strategy_Suspicion_Level currentObject = Strategy_Suspicion_Level.Load(id);
        FillData(currentObject);
        SetDetailsVisible(true);
    }

    private void FillData(Strategy_Suspicion_Level currentObject)
    {


        if (currentObject.LevelId != null)
            ddlSuspectionLevel.SelectedValue = currentObject.LevelId.ToString();
        else
            ddlSuspectionLevel.SelectedIndex = 0;

        ddlStrategies.Enabled = true;
        if (currentObject.Strategy != null)
        {
            if (currentObject.Strategy.Id != 0)
            {
                ddlStrategies.SelectedValue = currentObject.Strategy.Id.ToString();
                ddlStrategies.Enabled = false;
            }
        }
        else
        {
            ddlStrategies.SelectedIndex = 0;
        }
   



        chkCriteria1.Checked = currentObject.CriteriaId1 == 1 ? true : false;
        chkCriteria2.Checked = currentObject.CriteriaId2 == 1 ? true : false;
        chkCriteria3.Checked = currentObject.CriteriaId3 == 1 ? true : false;
        chkCriteria4.Checked = currentObject.CriteriaId4 == 1 ? true : false;
        chkCriteria5.Checked = currentObject.CriteriaId5 == 1 ? true : false;
        chkCriteria6.Checked = currentObject.CriteriaId6 == 1 ? true : false;
        chkCriteria7.Checked = currentObject.CriteriaId7 == 1 ? true : false;
        chkCriteria8.Checked = currentObject.CriteriaId8 == 1 ? true : false;
        chkCriteria9.Checked = currentObject.CriteriaId9 == 1 ? true : false;
        chkCriteria10.Checked = currentObject.CriteriaId10 == 1 ? true : false;
        chkCriteria11.Checked = currentObject.CriteriaId11 == 1 ? true : false;
        chkCriteria12.Checked = currentObject.CriteriaId12 == 1 ? true : false;
        chkCriteria13.Checked = currentObject.CriteriaId13 == 1 ? true : false;
        chkCriteria14.Checked = currentObject.CriteriaId14 == 1 ? true : false;
        chkCriteria15.Checked = currentObject.CriteriaId15 == 1 ? true : false;
      

       



        RadSlider1.SelectedValue = currentObject.Cr1Per.ToString();
        RadSlider2.SelectedValue = currentObject.Cr2Per.ToString();
        RadSlider3.SelectedValue = currentObject.Cr3Per.ToString();
        RadSlider4.SelectedValue = currentObject.Cr4Per.ToString();
        RadSlider5.SelectedValue = currentObject.Cr5Per.ToString();
        RadSlider6.SelectedValue = currentObject.Cr6Per.ToString();
        RadSlider7.SelectedValue = currentObject.Cr7Per.ToString();
        RadSlider8.SelectedValue = currentObject.Cr8Per.ToString();
        RadSlider9.SelectedValue = currentObject.Cr9Per.ToString();
        RadSlider10.SelectedValue = currentObject.Cr10Per.ToString();
        RadSlider11.SelectedValue = currentObject.Cr11Per.ToString();
        RadSlider12.SelectedValue = currentObject.Cr12Per.ToString();
        RadSlider13.SelectedValue = currentObject.Cr13Per.ToString();
        RadSlider14.SelectedValue = currentObject.Cr14Per.ToString();
        RadSlider15.SelectedValue = currentObject.Cr15Per.ToString();

        RadSlider1.Visible = chkCriteria1.Checked;
        RadSlider2.Visible = chkCriteria2.Checked;
        RadSlider3.Visible = chkCriteria3.Checked;
        RadSlider4.Visible = chkCriteria4.Checked;
        RadSlider5.Visible = chkCriteria5.Checked;
        RadSlider6.Visible = chkCriteria6.Checked;
        RadSlider7.Visible = chkCriteria7.Checked;
        RadSlider8.Visible = chkCriteria8.Checked;
        RadSlider9.Visible = chkCriteria9.Checked;
        RadSlider10.Visible = chkCriteria10.Checked;
        RadSlider11.Visible = chkCriteria11.Checked;
        RadSlider12.Visible = chkCriteria12.Checked;
        RadSlider13.Visible = chkCriteria13.Checked;
        RadSlider14.Visible = chkCriteria14.Checked;
        RadSlider15.Visible = chkCriteria15.Checked;
       



    }

    #endregion

   
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Response.Redirect("Strategies.aspx");
    }
    protected void chkCriteria_CheckedChanged(object sender, EventArgs e)
    {
        RadSlider1.Visible = chkCriteria1.Checked;
        RadSlider2.Visible = chkCriteria2.Checked;
        RadSlider3.Visible = chkCriteria3.Checked;
        RadSlider4.Visible = chkCriteria4.Checked;
        RadSlider5.Visible = chkCriteria5.Checked;
        RadSlider6.Visible = chkCriteria6.Checked;
        RadSlider7.Visible = chkCriteria7.Checked;
        RadSlider8.Visible = chkCriteria8.Checked;
        RadSlider9.Visible = chkCriteria9.Checked;
        RadSlider10.Visible = chkCriteria10.Checked;
        RadSlider11.Visible = chkCriteria11.Checked;
        RadSlider12.Visible = chkCriteria12.Checked;
        RadSlider13.Visible = chkCriteria13.Checked;
        RadSlider14.Visible = chkCriteria14.Checked;
        RadSlider15.Visible = chkCriteria15.Checked;
       
    }
    protected void gvData_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        id = Manager.GetInteger(e.CommandArgument.ToString());
        if (e.CommandArgument != null)
        {
            switch (e.CommandName)
            {
                case "Modify":

                    FillDetails(id);
                    break;
                case "Remove":
                    if (Strategy_Suspicion_Level.Delete(id))
                    {
                        LoadData();
                        id = 0;
                    }
                    else
                    {
                        ShowError("An error occured when trying to delete this record.");
                    }

                    break;
            }
        }
    }
}