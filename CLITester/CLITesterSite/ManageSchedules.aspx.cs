﻿using System;
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
using System.Net.Mail;
using System.Web.Configuration;

public partial class ManageSchedules : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        divSuccess.Visible = false;
        divError.Visible = false;
        viewDiv.Visible = false;

        if (!IsPostBack)
        {
            List<Operator> lstOperators = new List<Operator>();
            lstOperators = OperatorRepository.GetOperators();
            rptOperators.DataSource = lstOperators;
            rptOperators.DataBind();

            List<Carrier> LstCarriersList = new List<Carrier>();
            LstCarriersList = CarrierRepository.LoadbyUserID(Current.User.Id);
            rptCarriers.DataSource = LstCarriersList;
            rptCarriers.DataBind();
            GetData();

        }
    }

    #region Methods
    private void GetData()
    {
        List<Schedule> Schedules = ScheduleRepository.GetSchedules(Current.User.Id).OrderByDescending(l => l.Id).ToList();
        Session["Schedules"] = Schedules;
        rptSchedules.DataSource = Schedules;
        rptSchedules.DataBind();
    }

    private void GetDataHistory(int ScheduleId)
    {
        List<TestOperator> SchedulesHist = TestOperatorRepository.GetTestOperatorsByScheduleId(ScheduleId).OrderByDescending(l => l.Id).ToList();
        Session["TestOperatorsHistory"] = SchedulesHist;
        rptHistory.DataSource = SchedulesHist;
        rptHistory.DataBind();
    }

    private string getCode(string jsCodetoRun)
    {
        StringBuilder sb1 = new StringBuilder();
        sb1.AppendLine("function pageLoad() {");
        sb1.AppendLine(jsCodetoRun);
        sb1.AppendLine(" };");
        return sb1.ToString();

    }
    private void runjQuery(string jsCodeforRun)
    {
        ScriptManager requestSM = ScriptManager.GetCurrent(Page);
        if (requestSM != null && requestSM.IsInAsyncPostBack)
        {
            ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), Guid.NewGuid().ToString(), getCode(jsCodeforRun), true);
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), Guid.NewGuid().ToString(), getCode(jsCodeforRun), true);
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
       // txtName.Text = "Asdasdsad";
        //runjQuery("ShowPopUpDialog();");

        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tmp", "<script type='text/javascript'>disableAddBtn2();</script>", false);
        //System.Web.UI.ScriptManager.RegisterStartupScript(this, this.GetType(), "disableAddBtn2", "<script type='text/javascript'>$('#addDiv').show();</script>", false);
        //ClientScript.RegisterStartupScript(this.GetType(), "disableAddBtn2", "disableAddBtn2();", true);
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
            ActionLog action = new ActionLog();
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
    protected void btnView_Click(object sender, EventArgs e)
    {
        LinkButton lk = (LinkButton)sender;
        int id = 0;
        int.TryParse(lk.CommandArgument.ToString(), out id);

        viewDiv.Visible = true;
        GetDataHistory(id);
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

    #region Methods
    private void SendEmail(User member)
    {
        try
        {
            StringBuilder EmailBody = new StringBuilder();
            EmailBody.Append("<table cellspacing='0' cellpadding='0'>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 13pt; font-weight: bold'>CLITester Website</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Dear&nbsp;" + member.Name + ",</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Please check out your account details:</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Username = " + member.UserName + "</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Password = " + member.Password + "</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>For more information, don't hesitate to contact us.</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Thanks,</td></tr>");
            EmailBody.Append("<tr><td><a style='font-family: Arial; font-size: 11pt' href='http://www.vanrise.com'>www.vanrise.com</a> Team</td></tr>");
            EmailBody.Append("</table>");

            MailMessage objMail = new MailMessage();
            objMail.To.Add(member.Email);
            string strEmailFrom = WebConfigurationManager.AppSettings["SendingEmail"];
            objMail.From = new MailAddress(strEmailFrom, "CLI Tester");

            objMail.Subject = "CLITester - Schedule done";

            objMail.Body = EmailBody.ToString();
            objMail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = Config.SmtpServer;
            smtp.Send(objMail);
        }
        catch { }
    }
    #endregion

    private static List<RolePermission> pagePermission;

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(txtOccurs.Text) || String.IsNullOrEmpty(txtName.Text) || String.IsNullOrEmpty(txtTime.Value) || String.IsNullOrEmpty(txtDate.Value))
        {
            return;
        }

        Schedule newSchedule = new Schedule();

        ActionLog action = new ActionLog();
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
        List<ScheduleOperator> lstOperators = new List<ScheduleOperator>();
        int ind = txtTime.Value.IndexOf(':');

        int hour = 0;
        Int32.TryParse(txtTime.Value.Substring(0, ind).ToString(), out hour);
        //int len = txtTime.Value.Length;
        int minutes = 0;
        Int32.TryParse(txtTime.Value.Substring(ind + 1, 2).ToString(), out minutes);

        //int len = txtTime.Value.Length;

        //string day = txtTime.Value.Substring(len - 2, 2);
        //if (day == "PM")
        //    if (hour != 12)
        //        hour += 12;
        //    else
        //        hour = 11;
        //else
        //    if (hour == 12)
        //        hour = 0;

        DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minutes, 0);

        int i = txtDate.Value.IndexOf("t");
        string endDate = txtDate.Value.Substring(i + 3);
        string fromDate = txtDate.Value.Substring(0, i - 1);

        string[] wordsDateF = fromDate.Split('-');

        string fromdayS = wordsDateF[0];
        string frommonthS = wordsDateF[1];
        string fromyearS = wordsDateF[2];
        
        int fromday = 0;
        Int32.TryParse(fromdayS, out fromday);

        int frommonth = 0;
        Int32.TryParse(frommonthS, out frommonth);

        int fromyear = 0;
        Int32.TryParse(fromyearS, out fromyear);

        DateTime fromdateD = new DateTime(fromyear, frommonth, fromday, 0, 0, 0);

        string[] wordsDateE = endDate.Split('-');

        string todayS = wordsDateE[0];
        string tomonthS = wordsDateE[1];
        string toyearS = wordsDateE[2];

        int today = 0;
        Int32.TryParse(todayS, out today);

        int tomonth = 0;
        Int32.TryParse(tomonthS, out tomonth);

        int toyear = 0;
        Int32.TryParse(toyearS, out toyear);

        DateTime todateD = new DateTime(toyear, tomonth, today, 0, 0, 0);


        int ratio = 0;
        Int32.TryParse(txtOccurs.Text, out ratio);

        newSchedule.DisplayName = txtName.Text;
        newSchedule.OccursEvery = ratio;
        newSchedule.SpecificTime = d;
        newSchedule.StartDate = fromdateD;
        newSchedule.EndDate = todateD;
        ScheduleRepository.Save(newSchedule);
        //action.Description = Utilities.SerializeLINQtoXML<Carrier>(newCarrier);
        ScheduleLog log = ScheduleLogRepository.GetLastLog(newSchedule.Id);
        if(log != null)
            ScheduleLogRepository.Delete(log.Id);

        AuditRepository.Save(action);

        string words = HdTable.Value;

        string[] split = words.Split(new Char[] { ' ', ';' });

        foreach (string s in split)
        {

            if (s.Trim() != "")
            {
                string[] split2 = s.Split(new Char[] { '~' });
                int OpId = 0;
                Int32.TryParse(split2[0], out OpId);

                int PreId = 0;
                Int32.TryParse(split2[1], out PreId);
               
                ScheduleOperator op = new ScheduleOperator();
                op.ScheduleId = newSchedule.Id;
                op.OperatorId = OpId;
                op.CarrierId = PreId;
                lstOperators.Add(op);
            }
        }

        ScheduleOperatorRepository.DeleteAll(newSchedule.Id);
        foreach (ScheduleOperator schop in lstOperators)
        {
            ScheduleOperatorRepository.Save(schop);
        }




        SendEmail(Current.User.User);
        GetData();
    }

    private void SetActionPermission()
    {
        if (!Current.User.IsSuperAdministrator)
        {
            string currentPageName = Request.Path.Substring(Request.Path.LastIndexOf("/") + 1);
            pagePermission = Current.User.GetRolePermissions(currentPageName);

            if (pagePermission != null)
            {
                int canAddCount = 0;
                canAddCount = pagePermission.Where(pp => pp.CanAdd.HasValue && pp.CanAdd.Value).Count();

                int canDeleteCount = 0;
                canDeleteCount = pagePermission.Where(pp => pp.CanDelete.HasValue && pp.CanDelete.Value).Count();

                //btnNew.Visible = canAddCount > 0;
                //dgForestCenters.MasterTableView.GetColumn("Delete").Display = canDeleteCount > 0;

                ////btnNew.Visible = pagePermission.CanAdd.HasValue ? pagePermission.CanAdd.Value : false;
                ////dgForestCenters.MasterTableView.GetColumn("Delete").Display = pagePermission.CanDelete.HasValue ? pagePermission.CanDelete.Value : false;
            }
        }
    }

}