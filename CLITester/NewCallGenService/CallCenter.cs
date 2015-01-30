using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using SIPVoipSDK;
using System.Configuration;
using System.Timers;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using System.Diagnostics;

namespace NewCallGenService
{
    public class CallCenter
    {
        System.Timers.Timer timer = new System.Timers.Timer(10000);

        #region Definitions

        private static CallService _CurrentRunning;
        public static CallService CurrentRunning
        {
            get { return _CurrentRunning; }
            set { _CurrentRunning = value; }
        }


        private SipAccount sip;
        private CallQueue queue = new CallQueue();

        private static CAbtoPhone phone;
        private int lineCount = 0;

        private bool isPhoneRegistered = false;
        
        private static string sessionId = "";
        private string audioFilePath = "";

        private static List<CallService> services = new List<CallService>();

        public delegate void RegistrationCompleteDelegate(CallCenter center);
        public event RegistrationCompleteDelegate RegistrationComplete;

        public delegate void BecomeIdleDelegate(CallCenter center);
        public event BecomeIdleDelegate BecomeIdle;

        private bool idle = false;

        private bool startedWork = false;

        private int frequency = 0;

        public bool StartedWork
        {
            get
            {
                return startedWork;
            }
        }

        public bool Idle
        {
            get
            {
                return idle;
            }
        }

        public int Frequency
        {
            get
            {
                return frequency;
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

        public string CurrentSession
        {
            get
            {
                return sessionId;
            }
            set
            {
                sessionId = value;
            }
        }

        public bool IsPhoneRegistered
        {
            get
            {
                return isPhoneRegistered;
            }
        }

        public SipAccount Account
        {
            get
            {
                return sip;
            }
            set
            {
                sip = value;
            }
        }

        public CallQueue Queue
        {
            get
            {
                return queue;
            }
        }

        public CAbtoPhone Phone
        {
            get
            {
                return phone;
            }
        }

        public void Configure()
        {
            try
            {
                services = new List<CallService>();
                timer.Enabled = true;
                timer.Elapsed += OnTimedEvent;

                isPhoneRegistered = false;

                phone = new CAbtoPhone();

                CConfig config = phone.Config;

                config.ActivePlaybackDevice = "";
                config.ActiveNetworkInterface = sip.NetworkInterface;

                if (config.RecordDeviceCount > 0)
                    config.ActiveRecordDevice = config.get_RecordDevice(0);

                config.LicenseUserId = ConfigurationManager.AppSettings["LicenseUserId"];
                config.LicenseKey = ConfigurationManager.AppSettings["LicenseKey"];

                config.StunServer = "";
                config.ListenPort = 5060;

                config.EchoCancelationEnabled = 1;
                config.NoiseReductionEnabled = 1;
                config.VolumeUpdateSubscribed = 1;
                config.DialToneEnabled = 1;
                config.MP3RecordingEnabled = 0;
                config.EncryptedCallEnabled = 0;
                config.AutoAnswerEnabled = 0;
                config.RingToneEnabled = 0;
                config.LocalAudioEnabled = 0;
                config.LocalTonesEnabled = 0;
                config.MixerFilePlayerEnabled = 1;
                config.SamplesPerSecond = 32000;
                config.AutoGainControlEnabled = 1;

                config.LogLevel = LogLevelType.eLogInfo;
                config.CallInviteTimeout = 60;

                config.UserAgent = "ABTO Video SIP SDK";
                config.CallerId = sip.DisplayName;
                config.RegDomain = sip.Server;
                config.RegUser = sip.Username;
                config.RegPass = sip.Password;
                config.RegAuthId = sip.Login;
                config.RegExpire = 300;

                if (sip.UseProxy.Value)
                {
                    config.ProxyDomain = sip.ProxyServer;
                    config.ProxyUser = sip.ProxyUser;
                    config.ProxyPass = sip.ProxyPass;
                }
                //config.SetCodecOrder(sip.Codecs, 0);
                lineCount = sip.TotalLines.Value;

                for (int i = 0; i < lineCount; i++)
                {
                    CallService service = new CallService();
                    service.LineNumber = i + 1;

                    services.Add(service);
                }


                phone.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(phone_OnInitialized);
                phone.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(phone_OnLineSwiched);
                phone.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(phone_OnEstablishedCall);
                phone.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(phone_OnClearedCall);
                phone.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(phone_OnRegistered);
                phone.OnUnRegistered += new _IAbtoPhoneEvents_OnUnRegisteredEventHandler(phone_OnUnRegistered);
                phone.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(phone_OnPlayFinished);
                phone.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(phone_OnEstablishedConnection);
                phone.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(phone_OnClearedConnection);
                phone.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(phone_OnPhoneNotify);
                phone.OnRemoteAlerting += new _IAbtoPhoneEvents_OnRemoteAlertingEventHandler(phone_OnRemoteAlerting);
                phone.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
                phone.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);

                phone.ApplyConfig();
                System.Threading.Thread.Sleep(5000);
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC Configure EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }
        }

        #endregion

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                foreach (CallService service in services)
                {
                    if (service.StartDate != DateTime.MinValue &&
                        DateTime.Now.Subtract(service.StartDate).Seconds > 10 &&
                        service.EndDate == DateTime.MinValue)
                    {
                        phone.HangUp(service.ConnectionId);

                        service.EndDate = DateTime.Now;

                        LogCallEntry(service.CurrentNumber, service.AttemptDate, service.StartDate, service.EndDate, "0");
                        service.EndCall();
                    }
                }
            }
            catch(System.Exception ex)
            {
                WriteToEventLogEx("CC OnTimedEvent EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (CallService service in services)
                {
                    if (service.StartDate != DateTime.MinValue &&
                        DateTime.Now.Subtract(service.StartDate).Seconds > 10 &&
                        service.EndDate == DateTime.MinValue)
                    {
                        phone.HangUp(service.ConnectionId);

                        service.EndDate = DateTime.Now;

                        LogCallEntry(service.CurrentNumber, service.AttemptDate, service.StartDate, service.EndDate, "0");
                        service.EndCall();
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC timer_Tick EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }
        }

        public void Initialize()
        {
            try
            {
                Log("Start Initializing Process");

                phone.Initialize();
                System.Threading.Thread.Sleep(5000);
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC Initialize EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);

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

        public void Unregister()
        {
            Marshal.ReleaseComObject(phone);
        }

        public void TestCall(string number)
        {
            CallService service = (CallService)services[0];

            service.CurrentNumber = number;
            service.CallCenter = this;
            service.StartWork();
        }

        private static void WriteToEventLog(string message)
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

        private static void WriteToEventLogEx(string message)
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

        public void StartWork()
        {
            try
            {
                startedWork = true;
                bool h = AvailableLineExists();
                h = AllLinesAvailable();
                WriteToEventLog("Call Center=> queue.IsEmpty: " + queue.IsEmpty.ToString());

                while (!queue.IsEmpty)
                //if (!queue.IsEmpty)
                {
                    //assign a number to an available work and start calling
                    for (int i = 0; i < lineCount; i++)
                    {
                        CallService service = (CallService)services[i];
                        if (service.IsAvailable && !queue.IsEmpty)
                        {
                            string number = queue.Dequeue();

                            Log("Number to Call Service: " + number);
                            WriteToEventLog("Call Center=> Number: " + number);
                            service.EndingCall += new CallService.EndingCallDelegate(service_EndingCall);
                            service.CurrentNumber = number;
                            service.CallCenter = this;
                            service.StartWork();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC StartWork EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }  
        }

        void service_EndingCall(CallService service)
        {
            try
            {
                WriteToEventLogEx("CC service_EndingCall => " + "EMPTY IS: " + queue.IsEmpty + " " + AllLinesAvailable());

                if (queue.IsEmpty && AllLinesAvailable())
                {
                    CallSession session = CallSessionRepository.GetCallSession(sessionId);

                    WriteToEventLogEx("CC service_EndingCall => session.SessionId " + session.SessionId);

                    if (session != null)
                    {
                        session.EndDate = DateTime.Now;
                        bool s = CallSessionRepository.Update(session);
                    }

                    idle = true;
                    if (BecomeIdle != null)
                        BecomeIdle(this);
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC service_EndingCall EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }
        }

        private bool AllLinesAvailable()
        {
            try
            {
                bool available = false;
                int count = services.Count(x => x.IsAvailable);
                available = count == lineCount;

                return available;
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC AllLinesAvailable EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
                return false;
            }  
        }

        private bool AvailableLineExists()
        {
            try
            {
                bool available = false;
                int count = services.Count(x => x.IsAvailable);
                available = count > 0;
                return available;
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC AvailableLineExists EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
                return false;
            }  
        }

        public void Log(string message)
        {
            messages.Add(message);
        }

        private CallService GetCallService(int lineId)
        {
            try
            {
                WriteToEventLogEx("CC GetCallService 1=> ");
                CallService service = services.Where(x => x.LineNumber == lineId).SingleOrDefault<CallService>();
                WriteToEventLogEx("CC GetCallService 2=> ");
                return service;
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC GetCallService EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);

                return null;
            }
        }

        private void LogGeneratorNotifications(string message, string status, int line, int connectionId, string number)
        {
            Log(message);
            try
            {
                //log every event of the phone
                CallGeneratorLogEntry entry = new CallGeneratorLogEntry();

                entry.SessionId = sessionId;
                entry.Message = message;
                entry.Status = status;
                entry.LineId = line;
                entry.Date = DateTime.Now;
                entry.ConnectionId = connectionId;
                entry.Number = number;
                CallGeneratorLogEntryRepository.Save(entry);
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLogEx("CC LogGeneratorNotifications EXCEPTIONN=> " + ex.ToString());
            }
        }

        private static void LogCallEntry(string number, DateTime attemptDate, DateTime startDate, DateTime endDate, string status)
        {
            try
            {
                WriteToEventLog("CC LogCallEntry CDR22 => " + startDate + " " + endDate);
                if (startDate == DateTime.MinValue)
                {
                    startDate = endDate;
                }

                if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                {
                    //Log("Logging CDR");

                    CallEntry entry = CallEntryRepository.GetCallEntryByNumberSession(number, sessionId);
                    //CallEntry entry = db.CallEntries.Where(x => x.Number == number && x.SessionId == sessionId).SingleOrDefault<CallEntry>();

                    if (entry != null)
                    {
                        entry.StartDate = startDate;
                        entry.EndDate = endDate;
                        entry.Date = DateTime.Now.Date;
                        entry.Number = number;
                        entry.SessionId = sessionId;
                        //entry.Cgpn = Account.Username;
                        CallEntryRepository.Save(entry);

                        CallSession session = CallSessionRepository.GetCallSession(sessionId);
                        WriteToEventLog("CC LogCallEntry CDR => " + attemptDate + " " + startDate + " " + endDate);
                        CDR cdr = new CDR();
                        if(attemptDate == DateTime.MinValue)
                            cdr.AttemptDateTime = DateTime.Now;
                        else
                            cdr.AttemptDateTime = attemptDate;
                        cdr.ClientId = entry.ClientId;
                        cdr.ConnectDateTime = startDate;
                        cdr.DisconnectDateTime = endDate;
                        if (cdr.ConnectDateTime != null)
                        {
                            TimeSpan? t = (endDate - cdr.ConnectDateTime);
                            cdr.DurationInSeconds = Convert.ToInt32(t.Value.TotalSeconds);
                            WriteToEventLog("CC LogCallEntry CDR => " + cdr.DurationInSeconds);
                        }
                        else
                        {
                            cdr.DurationInSeconds = 0;
                        }
                        WriteToEventLog("CC LogCallEntry CDR => " + number + " " + status + " " + session.TransactionId + " " + session.UserId.ToString() + " " + session.SipAccount.Id.ToString());
                        cdr.CDPN = number;
                        cdr.CAUSE_TO_RELEASE_CODE = status;
                        cdr.CAUSE_FROM_RELEASE_CODE = status;
                        //cdr.TransactionId = session.TransactionId;
                        //cdr.UserID = session.UserId.ToString();
                        if (session.SipAccount != null)
                        {
                            if (session.SipAccount.Id != null)
                                cdr.SIP = session.SipAccount.Id;
                            else
                                cdr.SIP = null;
                        }
                        else
                            cdr.SIP = null;

                        bool tst = CDRRepository.Insert(cdr);
                        WriteToEventLog("CC LogCallEntry CDR SAVE => " + tst.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC LogCallEntry EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }
        }
        #region phone definitions
        void phone_OnUnRegistered(string Msg)
        {
            LogGeneratorNotifications(Msg, "", 0, 0, "");
            isPhoneRegistered = false;
        }

        void phone_OnRegistered(string Msg)
        {
            LogGeneratorNotifications(Msg, "", 0, 0, "");
            isPhoneRegistered = true;

            if (RegistrationComplete != null)
                RegistrationComplete(this);
        }

        void phone_OnRemoteAlerting(int ConnectionId, int responseCode, string reasonMsg)
        {
            LogGeneratorNotifications(reasonMsg, responseCode.ToString(), 0, ConnectionId, "");

            if (reasonMsg == "Trying")
                CurrentRunning.CDR.AlertDateTime = DateTime.Now;
        }

        void phone_OnPhoneNotify(string Msg)
        {
            LogGeneratorNotifications(Msg, "", 0, 0, "");
        }

        void phone_OnClearedConnection(int ConnectionId, int LineId)
        {
            LogGeneratorNotifications("Connection Cleared - Line: " + LineId, "", LineId, ConnectionId, "");
        }

        void phone_OnEstablishedConnection(string AddrFrom, string AddrTo, int ConnectionId, int LineId)
        {
            CallService service = GetCallService(LineId);
            service.ConnectionId = ConnectionId;

            LogGeneratorNotifications("Established Connection: " + AddrTo, "", LineId, ConnectionId, AddrFrom);
        }

        void phone_OnPlayFinished(string Msg)
        {
            LogGeneratorNotifications(Msg, "", 0, 0, "");
        }

        void phone_OnTextMessageReceived(string address, string message)
        {
        }

        void phone_OnTextMessageSentStatus(string address, string reason, int bSuccess)
        {
        }

        void phone_OnLineSwiched(int LineId)
        {
            try
            {
                LogGeneratorNotifications("Line Switched " + LineId, "", LineId, 0, "");
                CurrentRunning = GetCallService(LineId);
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC phone_OnLineSwiched EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }
        }

        void phone_OnInitialized(string Msg)
        {
            LogGeneratorNotifications(Msg, "", 0, 0, "");
        }
        #endregion


        void phone_OnClearedCall(string Msg, int Status, int LineId)
        {
            try
            {
                WriteToEventLog("CallCenter=> STATUSS " + Status.ToString());

                //Display status
                LogGeneratorNotifications(Msg + " - Status: " + Status.ToString() + " - Line: " + LineId, Status.ToString(), LineId, 0, "");
                WriteToEventLog("CallCenter =>  " + Msg + " - Status: " + Status.ToString() + " - Line: " + LineId + " services.Count: " + services.Count());

                CallService service = GetCallService(LineId);
                WriteToEventLog("CallCenter=> service.CurrentNumber " + service.CurrentNumber);
                if (service.CurrentNumber == "")
                {
                    service.EndDate = DateTime.Now;
                    service.CDR.DisconnectDateTime = DateTime.Now;
                    service.EndCall();
                }
                else
                {
                    WriteToEventLog("CC CallCenter=> CurrentNumber: " + service.CurrentNumber);
                    WriteToEventLog("CC service CallCenter: " + service.CallCenter);
                    WriteToEventLog("CC CurrentSession: " + service.CallCenter.CurrentSession);
                    CallEntry c = CallEntryRepository.GetCallEntryByNumberSession(service.CurrentNumber, service.CallCenter.CurrentSession);

                    if (c != null)
                    {
                        c.ResponseCode = Status.ToString();
                        c.EndDate = DateTime.Now;
                        WriteToEventLog("CallCenter=> Status: " + Status);
                        bool tr = CallEntryRepository.Save(c);
                    }
                    else
                        WriteToEventLog("CallCenter=> the entry is null");

                    //if status = 0 then call is successfully answered and cleared
                    //if (Status == 0)
                    {
                        //Update line state
                        service.EndDate = DateTime.Now;
                        service.CDR.DisconnectDateTime = DateTime.Now;

                        LogCallEntry(service.CurrentNumber, service.AttemptDate, service.StartDate, service.EndDate, Status.ToString());

                    }
                    service.EndCall();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC phone_OnClearedCall EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }
        }

        void phone_OnEstablishedCall(string Msg, int LineId)
        {
            try
            {
                LogGeneratorNotifications(Msg, "", LineId, 0, "");

                CallService service = GetCallService(LineId);
                service.StartDate = DateTime.Now;
                service.CDR.ConnectDateTime = DateTime.Now;

                //play sound file if exists
                if (!String.IsNullOrEmpty(audioFilePath))
                {
                    int succeded = phone.PlayFile(audioFilePath);

                    if (succeded > 0)
                        LogGeneratorNotifications("Starting to playing audio file", "", 0, 0, "");
                    else
                        LogGeneratorNotifications("File playing failed", "", 0, 0, "");
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CC phone_OnEstablishedCall EXCEPTIONN=> " + ex.ToString());
                Logger.LogException(ex);
            }
        }

    }
}
