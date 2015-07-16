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
using System.Timers;
using System.IO;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using SIPVoipSDK;
using System.Configuration;

namespace CallGeneratorServiceCLI
{
    public partial class NewCallGenCLI : ServiceBase
    {
        private static readonly object _syncRoot = new object();

        public ChannelAllocation thChannelAllocation = new ChannelAllocation();

        public GetCalls thGetCalls = new GetCalls();

        public static List<ChannelAllocation> LstChanels = new List<ChannelAllocation>();

        public static int NxtSipId = 0;

        public static List<SIP> LstSip = new List<SIP>();

        public static CAbtoPhone generalPhone = new CAbtoPhone();
        private CConfig config;
        
        internal static ServiceHost myServiceHost = null;

        public static void Reconfigure(int i)
        {
            SipAccount spAccount = SipAccountRepository.GetTop();
            LstChanels[i].sip = new SIP();
            LstChanels[i].sip.phone = generalPhone;
        }

        public void AddnewSIP(User user)
        {
            SipAccount spAccount = SipAccountRepository.GetTop();
            SIP s = new SIP();

            s.phone = new CAbtoPhone();
            CConfig config;

            config = s.phone.Config;

            config.ActivePlaybackDevice = "";
            config.ActiveNetworkInterface = "Ethernet-IPV4-192.168.22.12";

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
            config.CallerId = spAccount.DisplayName;
            config.RegDomain = spAccount.DisplayName;
            config.RegUser = spAccount.DisplayName;
            config.RegPass = spAccount.Password;
            config.RegAuthId = spAccount.DisplayName;
            config.RegExpire = 300;

            s.SipId = spAccount.Id;
            s.ConfigId = CallGeneratorServiceCLI.NewCallGenCLI.NxtSipId;
            CallGeneratorServiceCLI.NewCallGenCLI.NxtSipId++;
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
        }

        public void Configure()
        {
            try
            {
                SipAccount sipAccount = SipAccountRepository.GetTop();
                WriteToEventLog(sipAccount.DisplayName);
                //List<SipAccount> LstSipAccounts = SipAccountRepository.GetSipAccounts();
                int i = 0;
                //foreach (SipAccount sp in LstSipAccounts)
                {
                    SIP s = new SIP();

                    s.phone = generalPhone;
                    config = s.phone.Config;

                config.ActivePlaybackDevice = "";
                config.ActiveNetworkInterface = "91.236.236.53";

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
                config.CallerId = sipAccount.DisplayName;
                config.RegDomain = "91.236.236.53";
                config.RegUser = sipAccount.DisplayName;
                config.RegPass = sipAccount.DisplayName;
                config.RegAuthId = sipAccount.DisplayName;
                config.RegExpire = 3000;

                    //s.SipId = sp.Id;
                s.SipId = 1;
                s.ConfigId = i;
                    
                ///Testing Sip Accounts
                //config.ExSipAccount_Add("sip.telbo.com", "myworld80", "hello2013", "myworld80", "00442074542000", 300, 1, 0);
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
                WriteToEventLog(ex.ToString());
                Logger.LogException(ex);
            }
        }

        public NewCallGenCLI()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Configure();
                for (int i = 0; i < 64; i++)
                {
                    ChannelAllocation Chanel = new ChannelAllocation();
                    Chanel.Id = i;
                    Chanel.Idle = true;
                    Chanel.DestinationNumber = "";
                    Chanel.StartLastCall = DateTime.MinValue;

                    Chanel.sip = new SIP();
                    Chanel.sip.ConfigId = 1;
                    Chanel.sip.SipId = 1;
                    Chanel.sip.phone = LstSip[0].phone;

                    LstChanels.Add(Chanel);
                }
                thGetCalls.Start();
                thChannelAllocation.Start();
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
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

        public static void phone_OnTextMessageReceived(string address, string message)
        {
        }

        public static void phone_OnTextMessageSentStatus(string address, string reason, int bSuccess)
        {
        }

        public static void phone_OnRemoteAlerting2(int ConnectionId, int LineId, int responseCode, string reasonMsg)
        {
            //lock (_syncRoot)
            {
                try
                {
                    WriteToEventLog(" \r\n" + ("phone_OnRemoteAlerting: ConnectionId " + ConnectionId.ToString() + " LineId " + LineId + " responseCode: " + responseCode.ToString() + " reasonMsg: " + reasonMsg.ToString()));

                    //180 = Ringing
                    if (responseCode == 180)
                    {
                        ChannelAllocation channel = ChannelAllocation.GetCallServiceConnection(ConnectionId);
                        GeneratedCall generatedCall = GeneratedCallRepository.Load(channel.GeneratedCallid);

                        if (generatedCall != null)
                        {
                            generatedCall.Status = "4";
                            generatedCall.AlertDate = DateTime.Now;
                            generatedCall.ResponseCode = responseCode.ToString();
                            GeneratedCallRepository.Save(generatedCall);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.LogException(ex);
                    WriteToEventLog(" \r\n" + (ex.ToString()));

                    ChannelAllocation channelAllocation = ChannelAllocation.GetCallServiceConnection(ConnectionId);
                    GeneratedCall generatedCall = GeneratedCallRepository.Load(channelAllocation.GeneratedCallid);

                    if (generatedCall != null)
                    {
                        generatedCall.EndDate = DateTime.Now;
                        generatedCall.ResponseCode = responseCode.ToString();
                        GeneratedCallRepository.Save(generatedCall);
                    }

                    LstChanels[channelAllocation.Id].Idle = true;
                    LstChanels[channelAllocation.Id].StartDate = DateTime.MinValue;
                    LstChanels[channelAllocation.Id].StartLastCall = DateTime.MinValue;
                    LstChanels[channelAllocation.Id].GeneratedCallid = 0;
                    LstChanels[channelAllocation.Id].ConnectionId = 0;
                }
            }
        }

        public static void phone_OnPhoneNotify(string Msg)
        {
            try
            {
                WriteToEventLog(" \r\n" + ("phone_OnPhoneNotify: Msg" + Msg.ToString()));
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        public static void phone_OnClearedConnection(int ConnectionId, int LineId)
        {
            try
            {
                WriteToEventLog(" \r\n" + ("phone_OnClearedConnection: ConnectionId" + ConnectionId.ToString() + " LineId: " + LineId.ToString()));

                ChannelAllocation channel = ChannelAllocation.GetCallServiceConnection(ConnectionId);
                if (channel == null)
                    WriteToEventLog(" \r\n" + "channel is null : ");
                else
                {
                    WriteToEventLog(" \r\n" + "channel.GeneratedCallid : " + channel.GeneratedCallid + " channelID: " + channel.Id);
                    GeneratedCall generatedCall = GeneratedCallRepository.Load(channel.GeneratedCallid);

                    if (generatedCall != null && channel.GeneratedCallid != 0)
                    {
                        //generatedCall.Status = "6";
                        generatedCall.EndDate = DateTime.Now;
                        generatedCall.DisconnectDate = DateTime.Now;
                        //generatedCall.ResponseCode = Status.ToString();
                        GeneratedCallRepository.Save(generatedCall);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLog( " \r\n" + (ex.ToString()));
            }
        }

        public static void phone_OnEstablishedConnection(string AddrFrom, string AddrTo, int ConnectionId, int LineId)
        {
            try
            {
                WriteToEventLog(" \r\n" + ("phone_OnEstablishedConnection: ConnectionId" + ConnectionId.ToString() + " LineId: " + LineId.ToString() + " AddrFrom: " + AddrFrom.ToString() + " AddrTo: " + AddrTo));
                
                ChannelAllocation channel = ChannelAllocation.GetCallServiceConnection(ConnectionId);
                if (channel == null)
                    WriteToEventLog(" \r\n" + "c is null : ");
                else
                {
                    WriteToEventLog(" \r\n" + "c GeneratedCallid : " + channel.GeneratedCallid + " ID: " + channel.Id);
                    GeneratedCall generatedCall = GeneratedCallRepository.Load(channel.GeneratedCallid);

                    if (generatedCall != null && channel.GeneratedCallid != 0)
                    {
                        generatedCall.Status = "6";
                        generatedCall.EndDate = DateTime.Now;
                        generatedCall.ConnectDate = DateTime.Now;
                        GeneratedCallRepository.Save(generatedCall);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLog( " \r\n" + (ex.ToString()));
            }
        }

        public static void phone_OnEstablishedCall(string Msg, int LineId)
        {
            try
            {
                WriteToEventLog(" \r\n" + ("phone_OnEstablishedCall: Msg" + Msg.ToString() + " LineId: " + LineId.ToString()));
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        public static void phone_OnPlayFinished(string Msg)
        {
        }

        public static void phone_OnUnRegistered(string Msg)
        {
            try
            {
                WriteToEventLog(" \r\n" + ("phone_OnUnRegistered: Msg" + Msg.ToString()));
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLog( " \r\n" + (ex.ToString()));
            }
        }

        public static void phone_OnRegistered(string Msg)
        {
            try
            {
                WriteToEventLog( " \r\n" + ("phone_OnRegistered: Msg" + Msg.ToString()));
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        public static void phone_OnClearedCall(string Msg, int Status, int LineId)
        {
            lock (_syncRoot)
            {
                try
                {
                    ChannelAllocation channel = ChannelAllocation.GetCallService(LineId);
                    GeneratedCall generatedCall = GeneratedCallRepository.Load(channel.GeneratedCallid);

                    if (generatedCall != null && channel.GeneratedCallid != 0)
                    {
                        LstChanels[channel.Id].Idle = true;
                        LstChanels[channel.Id].StartDate = DateTime.MinValue;
                        LstChanels[channel.Id].StartLastCall = DateTime.MinValue;
                        LstChanels[channel.Id].GeneratedCallid = 0;
                        LstChanels[channel.Id].ConnectionId = 0;

                        //generatedCall.Status = "3";
                        //generatedCall.EndDate = DateTime.Now;
                        generatedCall.ResponseCode = Status.ToString();
                        GeneratedCallRepository.Save(generatedCall);
                    }
                    WriteToEventLog(" \r\n" + ("phone_OnClearedCall: Msg" + Msg.ToString() + " LineId: " + LineId.ToString() + " Status: " + Status.ToString()));
                }
                catch (System.Exception ex)
                {
                    Logger.LogException(ex);
                    WriteToEventLog(" \r\n" + (ex.ToString()));
                }
            }
        }

        public static void phone_OnLineSwiched(int LineId)
        {
            try
            {
                WriteToEventLog( " \r\n" + ("phone_OnLineSwiched: LineId" + LineId.ToString()));
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLog(" \r\n" + (ex.ToString()));
            }
        }

        public static void phone_OnInitialized(string Msg)
        {
            try
            {
                WriteToEventLog( " \r\n" + ("phone_OnInitialized: Msg" + Msg.ToString()));
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                WriteToEventLog( " \r\n" + (ex.ToString()));
            }
        }

        #endregion

        private static void WriteToEventLog(string message)
        {
            string cs = "Service CallGen";
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
