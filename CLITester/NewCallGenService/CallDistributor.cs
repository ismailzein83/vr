using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Timers;
using System.Threading;
using SIPVoipSDK;
using System.Configuration;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;
using System.Windows;
using System.Diagnostics;

namespace NewCallGenService
{
    public class CallDistributor
    {
        System.Timers.Timer timer = new System.Timers.Timer();

        // Create a timer with a two second interval.
        System.Timers.Timer aTimer = new System.Timers.Timer(2000);

        public bool FinishStart = false;

        #region Definitions

        private static List<CallCenter> centers = new List<CallCenter>();
        CAbtoPhone phone = new CAbtoPhone();

        public static List<CallCenter> Centers
        {
            get
            {
                return centers;
            }
        }

        private List<string> messages = new List<string>();

        public List<string> Messages
        {
            get
            {
                return messages;
            }
        }
        #endregion

        #region phone functions

        void phone_OnTextMessageReceived(string address, string message)
        {
        }

        void phone_OnTextMessageSentStatus(string address, string reason, int bSuccess)
        {
        }

        void phone_OnUnRegistered(string Msg)
        {
            //CallCenter.Log(Msg);
        }

        void phone_OnRegistered(string Msg)
        {
            //txtNotes.Text += "\r\n" + Msg;
        }

        void phone_OnRemoteAlerting(int ConnectionId, int responseCode, string reasonMsg)
        {
            //txtNotes.Text += "\r\n" + reasonMsg + " Code: " + responseCode;
        }

        void phone_OnPhoneNotify(string Msg)
        {
            // txtNotes.Text += "\r\n" + Msg;
        }

        void phone_OnClearedConnection(int ConnectionId, int LineId)
        {
            // txtNotes.Text += "\r\nConnection Cleared " + " on line " + LineId;
        }

        void phone_OnEstablishedConnection(string AddrFrom, string AddrTo, int ConnectionId, int LineId)
        {
            //   txtNotes.Text += "\r\nEstablished Connection: " + AddrTo;
        }

        void phone_OnPlayFinished(string Msg)
        {
            //txtNotes.Text += "\r\n" + Msg;
        }

        void phone_OnClearedCall(string Msg, int Status, int LineId)
        {
            //txtNotes.Text += "\r\n" + Msg + " Status: " + Status;
        }

        void phone_OnEstablishedCall(string Msg, int LineId)
        {
            //play sound file if exists
            //if (!String.IsNullOrEmpty(audioFilePath))
            //{
            //    int succeded = phone.PlayFile(audioFilePath);

            //    if (succeded > 0)
            //        txtNotes.Text += "\r\nStarting to playing audio file";
            //    else
            //        txtNotes.Text += "\r\nFile playing failed";
            //}
        }

        void phone_OnLineSwiched(int LineId)
        {
            //txtNotes.Text += "\r\nLine Switched " + LineId;
        }

        void phone_OnInitialized(string Msg)
        {
            // txtNotes.Text += "\r\n" + Msg;
        }

        #endregion

        private void btnHangUp_Click(object sender, EventArgs e)
        {
            phone.HangUpLastCall();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                foreach (CallCenter center in centers)
                {
                    if (!center.StartedWork && center.IsPhoneRegistered)
                    {
                        center.StartWork();
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CD OnTimedEvent EXCEPTIONN=> " + ex.ToString());
            }
        }

        public void Start()
        {
            try
            {
                FinishStart = false;
                timer.Enabled = true;
                timer.Interval = 3000;

                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEvent;
                aTimer.Enabled = true;

                centers = new List<CallCenter>();
                //Read If there exist a schedule to run
                //--------------------------------------
                ScheduleLog sl = new ScheduleLog();
                //sl = ScheduleManager.CurrentActiveSchedule;
                sl = ScheduleManager.CurrentSchedule;
                if (sl != null)
                    WriteToEventLog("CD=> sl " + sl.ScheduleId);

                if (sl != null)
                    ScheduleRepository.create_session(sl.Schedule);


                //start call generation
                //----------------------
                centers = new List<CallCenter>();

                List<CallSession> sessions = CallSessionRepository.GetCallSessions();
                WriteToEventLog("CD=> Sessions Found: " + sessions.Count);
                foreach (CallSession session in sessions)
                {
                    int count = 0;// centers.Count(x => x.Account.Id == session.SipAccount.Id);
                    messages.Add("Sessions Found: " + session.SessionId);
                    WriteToEventLog("CD=> Sessions SessionId: " + session.SessionId);
                    if (count == 0)
                    {
                        CallCenter center = new CallCenter();
                        center.Account = session.SipAccount;
                        center.CurrentSession = session.SessionId;

                        //fill the center queue
                        List<CallEntry> entries = CallEntryRepository.GetCallEntries(session.SessionId);
                        center.Messages.Add("Adding " + entries.Count + " in the queue.");
                        WriteToEventLog("CD=> Adding " + entries.Count + " in the queue.");
                        if (entries.Count > 0)
                        {
                            center.RegistrationComplete += new CallCenter.RegistrationCompleteDelegate(center_RegistrationComplete);
                            center.BecomeIdle += new CallCenter.BecomeIdleDelegate(center_BecomeIdle);
                            centers.Add(center);

                            foreach (CallEntry entry in entries)
                            {
                                center.Queue.Enqueue(entry.Number);
                            }

                            string s = "Entries: " + entries.Count();

                            //configure and initialize call center
                            center.Configure();
                            center.Initialize();
                        }
                    }
                }
                FinishStart = true;
            }
            catch(System.Exception ex)
            {
                WriteToEventLogEx("CD start EXCEPTIONN=> " + ex.ToString());

                //End all Sessions and Entries
                List<CallSession> LstCs = new List<CallSession>();
                LstCs = CallSessionRepository.GetCallSessions();
                for (int i = 0; i < LstCs.Count(); i++)
                {
                    LstCs[i].EndDate = DateTime.Now;
                    CallSessionRepository.Update(LstCs[i]);
                }

                List<CallEntry> LstEntriess = new List<CallEntry>();
                LstEntriess = CallEntryRepository.GetCallEntries();

                for (int i = 0; i < LstEntriess.Count(); i++)
                {
                    LstEntriess[i].EndDate = DateTime.Now;
                    LstEntriess[i].IsProcessed = 0;
                    CallEntryRepository.Save(LstEntriess[i]);
                }

                centers.Clear();
                Start();
            }
        }

        void center_BecomeIdle(CallCenter center)
        {
            try
            {
                int idleCount = centers.Count(x => x.Idle);
                WriteToEventLog("CD=> idleCount: " + idleCount);
                WriteToEventLog("CD=> centers.Count: " + centers.Count);
                if (idleCount == centers.Count)
                {
                    WriteToEventLog("CD=> Center is idle");
                    //Application.Exit();
                    FinishStart = true;
                    Start();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CD center_BecomeIdle EXCEPTIONN=> " + ex.ToString());
            }
        }

        void center_RegistrationComplete(CallCenter center)
        {
        }

        private void WriteToEventLog(string message)
        {
            string cs = "Call Generator Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        private void WriteToEventLogEx(string message)
        {
            string cs = "Call Generator Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

    }
}
