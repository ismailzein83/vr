using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using SIPVoipSDK;
using System.Configuration;
using System.Globalization;

namespace CallGeneratorService
{
    public partial class Service1 : ServiceBase
    {
        public ChannelAllocation c = new ChannelAllocation();

        public GetCalls thGetCalls = new GetCalls();

        public static List<ChannelAllocation> LstChanels = new List<ChannelAllocation>();

        public static int NxtSipId = 0;

        public static List<SIP> LstSip = new List<SIP>();

        private CConfig config;

        internal static ServiceHost myServiceHost = null;
        static readonly Random r = new Random();

        public void Configure()
        {
            try
            {
                List<SipAccount> LstSipAccounts = SipAccountRepository.GetSipAccounts();
                int i = 0;
                foreach (SipAccount sp in LstSipAccounts)
                {
                    SIP s = new SIP();

                    s.phone = new CAbtoPhone();
                    config = s.phone.Config;
                    
                    config.ActivePlaybackDevice = "";
                    config.ActiveNetworkInterface = sp.NetworkInterface;

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
                    config.CallerId = sp.User.CallerId;
                    config.RegDomain = sp.Server;

                    var leftPart = r.Next(1, 9);
                    var rightPart = Math.Round(r.NextDouble() * 1e+11, 0);

                    string digits12 = string.Format("{0}{1}", leftPart.ToString(CultureInfo.InvariantCulture), rightPart.ToString(CultureInfo.InvariantCulture).PadLeft(11, '0'));

                    config.RegUser = digits12;
                    config.RegPass = sp.Password;
                    config.RegAuthId = sp.Username;
                    config.RegExpire = 300;
                   
                    //config.ExSipAccount_Add(sp.Server, sp.Login, sp.Password, sp.Username, sp.DisplayName, 300, 1, 0);
                    //System.Threading.Thread.Sleep(1000);

                    s.SipId = sp.Id;
                    s.ConfigId = i;

                    //config.ExSipAccount_Add("sip.telbo.com", "myworld80", "hello2013", "myworld80", "00442074542000", 300, 1, 1);
                    //config.ExSipAccount_Add("149.7.44.141", "hadi", "had1", "hadi", "00442074542000", 300, 1, 1);

                    s.phone.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(phone_OnInitialized);
                    s.phone.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(phone_OnLineSwiched);
                    s.phone.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(phone_OnEstablishedCall);
                    s.phone.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(phone_OnClearedCall);
                    s.phone.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(phone_OnRegistered);
                    s.phone.OnUnRegistered += new _IAbtoPhoneEvents_OnUnRegisteredEventHandler(phone_OnUnRegistered);
                    s.phone.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(phone_OnPlayFinished);
                    s.phone.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(phone_OnEstablishedConnection);
                    s.phone.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(phone_OnClearedConnection);
                    s.phone.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(phone_OnPhoneNotify);

                    s.phone.OnRemoteAlerting2 += new _IAbtoPhoneEvents_OnRemoteAlerting2EventHandler(phone_OnRemoteAlerting2);
                    s.phone.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
                    s.phone.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);

                    s.phone.ApplyConfig();
                    s.phone.Initialize();
                    System.Threading.Thread.Sleep(1000);

                    LstSip.Add(s);
                    i++;
                    NxtSipId = i;
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Configure();
                //int lineCount = phone.Config.ExSipAccount_Count();
                for (int i = 0; i < 64; i++)
                {
                    ChannelAllocation Chanel = new ChannelAllocation();
                    Chanel.Id = i;
                    Chanel.Idle = true;
                    Chanel.DestinationNumber = "";
                    Chanel.StartLastCall = DateTime.MinValue;
                    Chanel.ConnectDateTime = DateTime.MinValue;
                    LstChanels.Add(Chanel);
                }
                thGetCalls.Start();
                c.Start();
            }
            catch (System.Exception ex)
            {
                WriteToEventLog("EXCEPTION S: " + ex.ToString());
            }
        }

        protected override void OnStop()
        {
            if (myServiceHost != null)
            {
                myServiceHost.Close();
                myServiceHost = null;
            }
        }

        #region phone functions

        void phone_OnTextMessageReceived(string address, string message)
        {
        }

        void phone_OnTextMessageSentStatus(string address, string reason, int bSuccess)
        {
        }

        void phone_OnRemoteAlerting2(int ConnectionId, int LineId, int responseCode, string reasonMsg)
        {
            try
            {
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        void phone_OnPhoneNotify(string Msg)
        {
        }

        void phone_OnClearedConnection(int ConnectionId, int LineId)
        {
            try
            {
            //    WriteToEventLog(" \r\n" + ("phone_OnClearedConnection: ConnectionId" + ConnectionId.ToString() + " LineId: " + LineId.ToString()));
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        void phone_OnEstablishedConnection(string AddrFrom, string AddrTo, int ConnectionId, int LineId)
        {
            try
            {
                WriteToEventLog(" \r\n" + ("phone_OnEstablishedConnection: ConnectionId" + ConnectionId.ToString() + " LineId: " + LineId.ToString() + " AddrFrom: " + AddrFrom.ToString() + " AddrTo: " + AddrTo));

                ChannelAllocation c = ChannelAllocation.GetCallService(LineId);
                WriteToEventLog("phone_OnEstablishedConnection:: " + c.DestinationNumber);
                if (c != null)
                    LstChanels[c.Id].ConnectDateTime = DateTime.Now;
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        void phone_OnPlayFinished(string Msg)
        {
        }

        void phone_OnUnRegistered(string Msg)
        {
            try
            {
               // WriteToEventLog(" \r\n" + ("phone_OnUnRegistered: Msg" + Msg.ToString()));
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        void phone_OnRegistered(string Msg)
        {
            try
            {
            //    WriteToEventLog(" \r\n" + ("phone_OnRegistered: Msg" + Msg.ToString()));
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }

        }

        void phone_OnClearedCall(string Msg, int Status, int LineId)
        {
            try
            {
                ChannelAllocation c = ChannelAllocation.GetCallService(LineId);

                WriteToEventLog(" phone_OnClearedCall " + "c GeneratedCallid : " + c.GeneratedCallid + " ID: " + c.Id);


                GeneratedCall GenCall = GeneratedCallRepository.Load(c.GeneratedCallid);


                //Save into Table CDR
                CDR cdr = new CDR();
                if (Service1.LstChanels[c.Id].AttemptDate == DateTime.MinValue)
                {
                    cdr.AttemptDateTime = DateTime.Now;
                    //cdr.ConnectDateTime = DateTime.Now;
                }
                else
                {
                    cdr.AttemptDateTime = Service1.LstChanels[c.Id].AttemptDate;
                    //cdr.ConnectDateTime = Service1.LstChanels[c.Id].AttemptDate;
                }

                if (Service1.LstChanels[c.Id].ConnectDateTime == DateTime.MinValue)
                {
                    cdr.ConnectDateTime = null;
                    cdr.DisconnectDateTime = null;
                }
                else
                {
                    cdr.ConnectDateTime = Service1.LstChanels[c.Id].ConnectDateTime;
                    cdr.DisconnectDateTime = DateTime.Now;
                }

                //if (Service1.LstChanels[c.Id].EndDate == DateTime.MinValue)
                //    cdr.DisconnectDateTime = DateTime.Now;
                //else
                //    cdr.DisconnectDateTime = Service1.LstChanels[c.Id].EndDate;
                if (GenCall != null)
                    cdr.ClientId = GenCall.ClientId;
                else
                {
                    int group = TestNumberRepository.LoadbyNumber(Service1.LstChanels[c.Id].DestinationNumber).GroupId.Value;
                    if( group == 196 || group == 197 ||group == 198 )
                        cdr.ClientId = 2;

                    else if (group == 199 || group == 200 || group == 201)
                        cdr.ClientId = 1;

                    else if (group == 202 || group == 203 || group == 204)
                        cdr.ClientId = 3;
                    else
                        cdr.ClientId = 0;
                }

                if (cdr.ConnectDateTime != null)
                {
                    TimeSpan? t = (cdr.DisconnectDateTime - cdr.ConnectDateTime);
              //      WriteToEventLog("DisconnectDateTime : " + cdr.DisconnectDateTime.ToString() + " cdr.ConnectDateTime: " + cdr.ConnectDateTime);
                    cdr.DurationInSeconds = t.Value.TotalSeconds;
                }
                else
                {
                    cdr.DurationInSeconds = 0;
                }
                cdr.CDPN = Service1.LstChanels[c.Id].DestinationNumber;

                cdr.CGPN = Service1.LstChanels[c.Id].sip.phone.Config.RegUser;

                cdr.CAUSE_TO_RELEASE_CODE = Status.ToString();
                cdr.CAUSE_FROM_RELEASE_CODE = Status.ToString();
                //cdr.TransactionId = session.TransactionId;
                //cdr.UserID = session.UserId.ToString();
                if (Service1.LstChanels[c.Id].sip != null)
                {
                    if (Service1.LstChanels[c.Id].sip.SipId != null)
                        cdr.SIP = Service1.LstChanels[c.Id].sip.SipId;
                    else
                        cdr.SIP = null;
                }
                else
                    cdr.SIP = null;

                bool tst = CDRRepository.Insert(cdr);



                if (GenCall != null)
                {
                //    WriteToEventLog(" \r\n" + "GenCall : " + GenCall.Id);
                    GenCall.Status = "3";
                    GenCall.EndDate = DateTime.Now;
                    GenCall.ResponseCode = Status.ToString();
                    GeneratedCallRepository.Save(GenCall);
                }
              //  else
             //       WriteToEventLog(" \r\n" + "GenCall NULL: " + GenCall);

                LstChanels[c.Id].Idle = true;
                LstChanels[c.Id].StartDate = DateTime.MinValue;
                LstChanels[c.Id].StartLastCall = DateTime.MinValue;
                LstChanels[c.Id].ConnectDateTime = DateTime.MinValue;
                LstChanels[c.Id].GeneratedCallid = 0;
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" phone_OnClearedCall EXCEPTION \r\n" + (ex.ToString()));

                LstChanels[c.Id].Idle = true;
                LstChanels[c.Id].StartDate = DateTime.MinValue;
                LstChanels[c.Id].StartLastCall = DateTime.MinValue;
                LstChanels[c.Id].ConnectDateTime = DateTime.MinValue;
                LstChanels[c.Id].GeneratedCallid = 0;
            }
        }

        void phone_OnEstablishedCall(string Msg, int LineId)
        {
            try
            {

                //ChannelAllocation c = ChannelAllocation.GetCallService(LineId);
              //  if (c == null)
                 //   WriteToEventLog(" \r\n" + "c is null : ");
               // else
                //    WriteToEventLog(" \r\n" + "c GeneratedCallid : " + c.GeneratedCallid + " ID: " + c.Id);

                //LstChanels[c.Id].ConnectDateTime = DateTime.Now;

               // WriteToEventLog(" \r\n" + ("phone_OnEstablishedCall: Msg" + Msg.ToString() + " LineId: " + LineId.ToString()));
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        void phone_OnLineSwiched(int LineId)
        {
            try
            {
               // WriteToEventLog(" \r\n" + ("phone_OnLineSwiched: LineId" + LineId.ToString()));
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        void phone_OnInitialized(string Msg)
        {
            try
            {
                WriteToEventLog(" \r\n" + ("phone_OnInitialized: Msg" + Msg.ToString()));
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        #endregion

        private void WriteToEventLog(string message)
        {
            string cs = "VanCGenSer";
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
