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

public partial class ManageSchedules : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        divSuccess.Visible = false;
        divError.Visible = false;

        if (!IsPostBack)
        {
            
            List<TestNumberGroup> lstGroups = new List<TestNumberGroup>();
            lstGroups = TestNumberGroupRepository.GetTestNumberGroups();
            rptGroups.DataSource = lstGroups;
            rptGroups.DataBind();

            List<SipAccount> LstSipAccounts = new List<SipAccount>();
            LstSipAccounts = SipAccountRepository.GetSipAccounts();
            rptSipAccounts.DataSource = LstSipAccounts;
            rptSipAccounts.DataBind();
            GetData();

        }
    }

    #region Methods
    private void GetData()
    {
        List<Schedule> Schedules = ScheduleRepository.GetSchedules2().OrderByDescending(l => l.Id).ToList();
        Session["Schedules"] = Schedules;
        rptSchedules.DataSource = Schedules;
        rptSchedules.DataBind();
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tmp", "<script type='text/javascript'>disableAddBtn2();</script>", false);
        System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "disableAddBtn2", "<script type='text/javascript'>disableAddBtn2();</script>", false);

        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "disableAddBtn2", "disableAddBtn2();", true);
        // Page.ClientScript.RegisterStartupScript(this.GetType(), "disableAddBtn2", "disableAddBtn2();", true);

        //ScriptManager.RegisterStartupScript(this, GetType(), "disableAddBtn2", "disableAddBtn2();", true);

        //ClientScript.RegisterStartupScript(GetType(), "hwa", "disableAddBtn2();", true);

        // Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "disableAddBtn()", true);

        //LinkButton lk = (LinkButton)sender;
        //int id = 0;
        //int.TryParse(lk.CommandArgument.ToString(), out id);
        //Schedule s = ScheduleRepository.Load(id);
        //txtName.Text = s.DisplayName;
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        LinkButton lk = (LinkButton)sender;
        int id = 0;
        int.TryParse(lk.CommandArgument.ToString(), out id);
        if (ScheduleRepository.Delete(id))
        {
            NewActionLog action = new NewActionLog();
            action.ObjectId = id;
            action.ObjectType = "Schedule";
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
        if (String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtTime.Text) || String.IsNullOrEmpty(txtStartDate.Value) || String.IsNullOrEmpty(txtEndDate.Value))
        {
            return;
        }

        Schedule newSchedule = new Schedule();

        NewActionLog action = new NewActionLog();
        action.ObjectId = Current.User.Id;
        action.ObjectType = "Schedule";

        if (String.IsNullOrEmpty(HdnId.Value))
        {
            newSchedule.UserId = Current.User.Id;
            action.ActionType = (int)Enums.ActionType.Add;
        }
        else
        {
            int id = 0;
            if (Int32.TryParse(HdnId.Value, out id))
            {
                newSchedule = ScheduleRepository.Load(id);
                if (newSchedule == null) return;
                action.ActionType = (int)Enums.ActionType.Modify;
            }
            else
            {
                return;
            }
        }
        
        //int hour = 0;
        //Int32.TryParse(txtTime.Text.Substring(0, 2).ToString(), out hour);

        //int minutes = 0;
        //Int32.TryParse(txtTime.Text.Substring(3, 2).ToString(), out minutes);

        //string day = txtTime.Text.Substring(6, 2);
        //if (day == "PM")
        //    hour += 12;

        //DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minutes, 0);

        int fromday = 0;
        Int32.TryParse(txtStartDate.Value.Substring(0, 2).ToString(), out fromday);

        int frommonth = 0;
        Int32.TryParse(txtStartDate.Value.Substring(3, 2).ToString(), out frommonth);

        int fromyear = 0;
        Int32.TryParse(txtStartDate.Value.Substring(6, 4).ToString(), out fromyear);

        int fromHour = 0;
        Int32.TryParse(txtStartDate.Value.Substring(11, 2).ToString(), out fromHour);

        int fromMinutes = 0;
        Int32.TryParse(txtStartDate.Value.Substring(14, 2).ToString(), out fromMinutes);

        DateTime fromdate = new DateTime(fromyear, frommonth, fromday, fromHour, fromMinutes, 0);

        int today = 0;
        Int32.TryParse(txtEndDate.Value.Substring(0, 2).ToString(), out today);

        int tomonth = 0;
        Int32.TryParse(txtEndDate.Value.Substring(3, 2).ToString(), out tomonth);

        int toyear = 0;
        Int32.TryParse(txtEndDate.Value.Substring(6, 4).ToString(), out toyear);

        int toHour = 0;
        Int32.TryParse(txtEndDate.Value.Substring(11, 2).ToString(), out toHour);

        int toMinutes = 0;
        Int32.TryParse(txtEndDate.Value.Substring(14, 2).ToString(), out toMinutes);

        DateTime todate = new DateTime(toyear, tomonth, today, toHour, toMinutes, 0);


        int ratio = 0;
        //Int32.TryParse(txtOccurs.Text, out ratio);

        newSchedule.DisplayName = txtName.Text;
        
        int TimeFrequency = 0;
        Int32.TryParse(txtTime.Text, out TimeFrequency);
        
        newSchedule.Frequency = 1;
        newSchedule.TimeFrequency = TimeFrequency;
        newSchedule.MonthDay = null;
        newSchedule.OccursEvery = TimeFrequency;
        
        DateTime d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        newSchedule.SpecificTime = d2;
        newSchedule.StartDate = fromdate;
        newSchedule.EndDate = todate;
        int sipId = 0;
        Int32.TryParse(HiddenFieldSelectSip.Value.ToString(), out sipId);
        newSchedule.SipAccountId = sipId;
        //newSchedule.CreatedBy = Current.User.Id;
        newSchedule.CreationDate = DateTime.Now;
        newSchedule.TotalNumbers = null;
        newSchedule.RatioOfCalls = null;
        newSchedule.UserId = Current.User.Id;
        newSchedule.TotalNumbers = 64;
        newSchedule.RatioOfCalls = 99;
        
        newSchedule.IsEnabled = chkEnabled.Checked;
        ScheduleRepository.Save(newSchedule);
        //action.Description = Utilities.SerializeLINQtoXML<Carrier>(newCarrier);

        int GId = 0;
        Int32.TryParse(HiddenFieldSelectGroup.Value.ToString(), out GId);


        AuditRepository.Save(action);

        ScheduleGroup s = ScheduleGroupRepository.LoadGroup(newSchedule.Id);
        if (s != null)
        {
            s.GroupId = GId;
        }
        else
        {
            s = new ScheduleGroup();
            s.GroupId = GId;
            s.ScheduleId = newSchedule.Id;
        }
        ScheduleGroupRepository.Save(s);


        GetData();
    }
}