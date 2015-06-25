using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;

namespace NewCallGenService
{
    public class CallService
    {
        #region Definitions

        private int lineNumber;
        private string currentNumber;
        private CallCenter center;
        private bool isAvailable = true;
        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MinValue;
        private DateTime attemptDate = DateTime.MinValue;
        private int connectionId = 0;
        private int frequency = 0;
        //private CDR _cdr = new CDR();
        public delegate void EndingCallDelegate(CallService service);
        public event EndingCallDelegate EndingCall;

        //public CDR CDR
        //{
        //    get
        //    {
        //        return _cdr;
        //    }
        //    set
        //    {
        //        _cdr = value;
        //    }
        //}

        public int ConnectionId
        {
            get
            {
                return connectionId;
            }
            set
            {
                connectionId = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                endDate = value;
            }
        }

        public DateTime AttemptDate
        {
            get
            {
                return attemptDate;
            }
            set
            {
                endDate = value;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return isAvailable;
            }
            set
            {
                isAvailable = value;
            }
        }

        public int LineNumber
        {
            get
            {
                return lineNumber;
            }
            set
            {
                lineNumber = value;
            }
        }

        public string CurrentNumber
        {
            get
            {
                return currentNumber;
            }
            set
            {
                currentNumber = value;
            }
        }

        public CallCenter CallCenter
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
            }
        }
        #endregion

        public void StartWork()
        {
            try
            {
                isAvailable = false;
                center.Phone.SetCurrentLine(lineNumber);
                WriteToEventLog("Call Service=> " + "lineNumber:" + lineNumber + " currentNumber:" + currentNumber);
                if (currentNumber != null)
                    center.Phone.StartCall(currentNumber);
                attemptDate = DateTime.Now;
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CSS StartWork EXCEPTION" + ex.ToString());

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
                
                CallDistributor c = new CallDistributor();
                c.Start();
            }
        }

        public void EndCall()
        {
            try
            {
                WriteToEventLogEx("CSS EndCall 1");
                startDate = DateTime.MinValue;
                endDate = DateTime.MinValue;
                isAvailable = true;
                currentNumber = "";
                WriteToEventLogEx("CSS EndCall 2");
                if (EndingCall != null)
                    EndingCall(this);
                WriteToEventLogEx("CSS EndCall finish");
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CSS EndCall EXCEPTION111" + ex.ToString());
                startDate = DateTime.MinValue;
                endDate = DateTime.MinValue;
                isAvailable = true;
                currentNumber = "";

                WriteToEventLogEx("CSS EndCall EXCEPTION" + ex.ToString());
            }
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
