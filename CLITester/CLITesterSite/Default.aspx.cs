using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;

public class CarrierStat
{
    public string Prefix { get; set; }
    public int Total { get; set; }
    public int Delivered { get; set; }
    public int NotDelivered { get; set; }
    public decimal percentage { get; set; }
    public decimal percentageW { get; set; }
}

public partial class Default : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Current.User.IsAuthenticated)
            Response.Redirect("Login.aspx");

        if (!IsPostBack)
        {
            DateTime d = new DateTime();
            DateTime maxd = new DateTime();
            int i = 0;
            List<Schedule> LstShc = new List<Schedule>();
            if(Current.User.User.ParentId == null)
                LstShc = ScheduleRepository.GetSchedules(Current.User.Id);
            else
                LstShc = ScheduleRepository.GetSchedules(Current.User.User.ParentId.Value);
            foreach (Schedule s in LstShc)
            {
                ScheduleLog sl = ScheduleLogRepository.GetLastLog(s.Id);
                if (sl != null)
                    if (sl.StartDate.HasValue)
                        d = sl.StartDate.Value;

                if (i == 0)
                    maxd = d;
                if (d <= maxd)
                    maxd = d;
                i++;
            }
            if (maxd == DateTime.MinValue)
                lblNxtSch.Text = "No Schedule is running";
            else
                lblNxtSch.Text = maxd.ToString("dd MMM yyyy - HH:mm");
            
            if (Current.User.User.ParentId != null)
                RegionalFeeds.Visible = false;
            else
                RegionalFeeds.Visible = true;

            GetData();

            int CliDeliv = 0;
            int CliNonDeliv = 0;
            int total = 0;
            int balance = 0;
            int failed = 0;
            CliDeliv = TestOperatorRepository.GetCLIDeliv(Current.User.Id);
            CliNonDeliv = TestOperatorRepository.GetCLINonDeliv(Current.User.Id);
            total = TestOperatorRepository.GetTotalCalls(Current.User.Id);
            if (Current.User.User.Balance.HasValue)
                balance = Current.User.User.Balance.Value;

            int totCalls = CliDeliv + CliNonDeliv;
            failed = total - totCalls;
            lblTotalCalls.Text = totCalls.ToString();
            lblRemaining.Text = balance.ToString();
            lblFailed.Text = failed.ToString();
            lblCLIDel.Text = TestOperatorRepository.GetCLIDeliv(Current.User.Id).ToString();
            lblCLINonDel.Text = TestOperatorRepository.GetCLINonDeliv(Current.User.Id).ToString();

            User us = UserRepository.Load(Current.User.User.Id);
            List<Carrier> LstCarriers = new List<Carrier>();
            if(us.ParentId == null)
                LstCarriers = CarrierRepository.LoadbyUserID(Current.User.User.Id);
            else
                LstCarriers = CarrierRepository.LoadbyUserID(us.ParentId.Value);

            List<CarrierStat> LstStat = new List<CarrierStat>();
            
            foreach (Carrier c in LstCarriers)
            {
                CarrierStat cstat = new CarrierStat();
                cstat.Prefix = c.Prefix;

                cstat.Delivered = TestOperatorRepository.GetPercentage(c.Prefix, 1, Current.User.User.Id);
                cstat.NotDelivered = TestOperatorRepository.GetPercentage(c.Prefix, 2, Current.User.User.Id);

                cstat.Total = TestOperatorRepository.GetPercentage(c.Prefix, null, Current.User.User.Id);
                if (cstat.Delivered != 0 && cstat.Delivered != null)
                    cstat.percentage = decimal.Round((((decimal)(cstat.Delivered) / (decimal)(cstat.Total)) * 100), 2);
                else
                    cstat.percentage = 0;

                if (cstat.NotDelivered != 0 && cstat.NotDelivered != null)
                    cstat.percentageW = decimal.Round((((decimal)(cstat.NotDelivered) / (decimal)(cstat.Total)) * 100), 2);
                else
                    cstat.percentageW = 0;

                LstStat.Add(cstat);
            }
            LstStat.OrderBy(l => l.percentage);
            if (LstStat.Count >= 1)
            {
                lblBest1.Text = "[" + LstStat[0].Prefix  + "]" + " " + LstStat[0].percentage.ToString("#00.00");
                //lblWorst1.Text = LstStat[0].Prefix + " " + LstStat[0].percentageW.ToString();
            }
            if (LstStat.Count > 1)
            {
                lblBest2.Text = "[" + LstStat[1].Prefix + "]" + " " + LstStat[1].percentage.ToString("#00.00");
                //lblWorst2.Text = LstStat[1].Prefix + " " + LstStat[1].percentageW.ToString();
            }

            LstStat.OrderBy(l => l.percentageW);
            if (LstStat.Count >= 1)
            {
                //lblBest1.Text = LstStat[0].Prefix + " " + LstStat[0].percentage.ToString();
                lblWorst1.Text = "[" + LstStat[0].Prefix + "]" + " " + LstStat[0].percentageW.ToString("#00.00");
            }
            if (LstStat.Count > 1)
            {
                //lblBest2.Text = LstStat[1].Prefix + " " + LstStat[1].percentage.ToString();
                lblWorst2.Text = "[" + LstStat[1].Prefix + "]" + " " + LstStat[1].percentageW.ToString("#00.00");
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }

    private void GetData()
    {
        List<ActionLogFeed> Schedules = AuditRepository.GetActionLogs(Current.User.User.Id, "Schedule").ToList();
        rptSchedules.DataSource = Schedules;
        rptSchedules.DataBind();

        List<ActionLogFeed> lstUsers = AuditRepository.GetActionLogs(Current.User.User.Id, "User").ToList();
        rptUsers.DataSource = lstUsers;
        rptUsers.DataBind();
        
    }
}