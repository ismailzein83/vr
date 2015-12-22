using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.Web;
using Vanrise.Fzero.Bypass;
using Vanrise.Fzero.Bypass.TOne;


namespace Vanrise.Fzero.Services.LevelTwoComparison
{
    public partial class LevelTwoComparisonService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public LevelTwoComparisonService()
	    {
		    InitializeComponent();
	    }

        private void ErrorLog(string message)
        {
            string cs = "Level Two Comparison Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        protected override void OnStart(string[] args)
        {
            base.RequestAdditionalTime(int.Parse(System.Configuration.ConfigurationManager.AppSettings["RequestAdditionalTime"].ToString()));
            Debugger.Launch(); // launch and attach debugger

            // Create a timer with a ten second interval.
            aTimer = new System.Timers.Timer(int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"].ToString()));
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            aTimer.Interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"].ToString());
            aTimer.Enabled = true;

            GC.KeepAlive(aTimer);

        }

 

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                List<GeneratedCall> AnalyzedCallsList = new List<GeneratedCall>();
                foreach (ToneGeneratedCall VGC in ToneGeneratedCall.GetAnalyzed())
                {
                    GeneratedCall vGC = new GeneratedCall();
                    vGC.ID = VGC.ID;
                    vGC.SourceID = VGC.SourceID;
                    vGC.MobileOperatorID = VGC.MobileOperatorID;
                    vGC.StatusID = VGC.StatusID;
                    vGC.PriorityID = VGC.PriorityID;
                    vGC.ReportingStatusID = VGC.ReportingStatusID;
                    vGC.DurationInSeconds = VGC.DurationInSeconds;
                    vGC.MobileOperatorFeedbackID = VGC.MobileOperatorFeedbackID;
                    vGC.a_number = VGC.a_number;
                    vGC.b_number = VGC.b_number;
                    vGC.CLI = VGC.CLI;
                    vGC.OriginationNetwork = VGC.OriginationNetwork;
                    vGC.AssignedTo = VGC.AssignedTo;
                    vGC.AssignedBy = VGC.AssignedBy;
                    vGC.ReportID = VGC.ReportID;
                    vGC.AttemptDateTime = VGC.AttemptDateTime;
                    vGC.LevelOneComparisonDateTime = VGC.LevelOneComparisonDateTime;
                    vGC.LevelTwoComparisonDateTime = VGC.LevelTwoComparisonDateTime;
                    vGC.FeedbackDateTime = VGC.FeedbackDateTime;
                    vGC.AssignmentDateTime = VGC.AssignmentDateTime;
                    vGC.ImportID = VGC.ImportID;
                    vGC.ReportingStatusChangedBy = VGC.ReportingStatusChangedBy;
                    vGC.Level1Comparison = VGC.Level1Comparison;
                    vGC.Level2Comparison = VGC.Level2Comparison;
                    vGC.ToneFeedbackID = VGC.ToneFeedbackID;
                    vGC.FeedbackNotes = VGC.FeedbackNotes;
                    AnalyzedCallsList.Add(vGC);
                }
                GeneratedCall.PerformLevelTwoComparison(AnalyzedCallsList);


                ToneGeneratedCall.TruncateGeneratedCalls();


                List<GeneratedCall> GeneratedCallsList = GeneratedCall.GetCallsDidNotPassLevelTwo(bool.Parse(System.Configuration.ConfigurationManager.AppSettings["LevelTwoComparisonIsObligatory"].ToString())).ToList();
                List<ToneGeneratedCall> listGC = new List<ToneGeneratedCall>();
                foreach (GeneratedCall VGC in GeneratedCallsList)
                {
                    ToneGeneratedCall vGC = new ToneGeneratedCall();

                    vGC.ID = VGC.ID;
                    vGC.SourceID = VGC.SourceID;
                    vGC.MobileOperatorID = VGC.MobileOperatorID;
                    vGC.StatusID = VGC.StatusID;
                    vGC.PriorityID = VGC.PriorityID;
                    vGC.ReportingStatusID = VGC.ReportingStatusID;
                    vGC.DurationInSeconds = VGC.DurationInSeconds;
                    vGC.MobileOperatorFeedbackID = VGC.MobileOperatorFeedbackID;
                    vGC.a_number = VGC.a_number;
                    vGC.b_number = VGC.b_number;
                    vGC.CLI = VGC.CLI;
                    vGC.OriginationNetwork = VGC.OriginationNetwork;
                    vGC.AssignedTo = VGC.AssignedTo;
                    vGC.AssignedBy = VGC.AssignedBy;
                    vGC.ReportID = VGC.ReportID;
                    vGC.AttemptDateTime = VGC.AttemptDateTime;
                    vGC.LevelOneComparisonDateTime = VGC.LevelOneComparisonDateTime;
                    vGC.LevelTwoComparisonDateTime = VGC.LevelTwoComparisonDateTime;
                    vGC.FeedbackDateTime = VGC.FeedbackDateTime;
                    vGC.AssignmentDateTime = VGC.AssignmentDateTime;
                    vGC.ImportID = VGC.ImportID;
                    vGC.ReportingStatusChangedBy = VGC.ReportingStatusChangedBy;
                    vGC.Level1Comparison = VGC.Level1Comparison;
                    vGC.Level2Comparison = VGC.Level2Comparison;
                    vGC.ToneFeedbackID = VGC.ToneFeedbackID;
                    vGC.FeedbackNotes = VGC.FeedbackNotes;
                    listGC.Add(vGC);
                }


                    ToneGeneratedCall.SaveBulk(listGC.ToList());

                    ToneGeneratedCall.FillReceivedCalls();
                    
                    





            }
            catch (Exception ex)
            {
                ErrorLog("OnTimedEvent: " + ex.Message);
                ErrorLog("OnTimedEvent: " + ex.InnerException);
                ErrorLog("OnTimedEvent: " + ex.StackTrace);
                ErrorLog("OnTimedEvent: " + ex.ToString());
                ErrorLog("OnTimedEvent: " + ex.InnerException.Data);
            }


        }
       

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }
}









