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

        public ChannelAllocation c = new ChannelAllocation();

        public GetCalls thGetCalls = new GetCalls();

        public static List<ChannelAllocation> LstChanels = new List<ChannelAllocation>();

        public static int NxtSipId = 0;

        public static List<SIP> LstSip = new List<SIP>();

        public static CAbtoPhone Generalphone = new CAbtoPhone();
        private CConfig config;
        
        internal static ServiceHost myServiceHost = null;

        public static void Reconfigure(int i, string CallerId)
        {
            SipAccount spAccount = SipAccountRepository.Load(1);
            //SIP s = new SIP();
            WriteToEventLog("Reconfigure 1");
            LstChanels[i].sip = new SIP();

            WriteToEventLog("Reconfigure 11");

            LstChanels[i].sip.phone = Generalphone;

            //////////////////////////////////////////////////////////////////////////
            //WriteToEventLog("Reconfigure 2 " +  " Port: " + LstChanels[i].sip.phone.Config.ListenPort.ToString());
            //CConfig config;

            //config = LstChanels[i].sip.phone.Config;

            //config.ActivePlaybackDevice = "";
            //config.ActiveNetworkInterface = "Ethernet-IPV4-192.168.22.12";

            //if (config.RecordDeviceCount > 0)
            //    config.ActiveRecordDevice = config.get_RecordDevice(0);

            //config.LicenseUserId = ConfigurationManager.AppSettings["LicenseUserId"];
            //config.LicenseKey = ConfigurationManager.AppSettings["LicenseKey"];

            //config.StunServer = "";
            //config.ListenPort = 5060;
            //config.EchoCancelationEnabled = 1;
            //config.NoiseReductionEnabled = 1;
            //config.VolumeUpdateSubscribed = 1;
            //config.DialToneEnabled = 1;
            //config.MP3RecordingEnabled = 0;
            //config.EncryptedCallEnabled = 0;
            //config.AutoAnswerEnabled = 0;
            //config.RingToneEnabled = 0;
            //config.LocalAudioEnabled = 0;
            //config.LocalTonesEnabled = 0;
            //config.MixerFilePlayerEnabled = 1;
            //config.SamplesPerSecond = 32000;
            //config.AutoGainControlEnabled = 1;
            //config.LogLevel = LogLevelType.eLogInfo;
            //config.CallInviteTimeout = 60;
            //config.UserAgent = "ABTO Video SIP SDK";
            //config.CallerId = CallerId;
            //config.RegDomain = CallerId;
            //config.RegUser = CallerId;
            //config.RegPass = spAccount.Password;
            //config.RegAuthId = CallerId;
            //config.RegExpire = 3000;

            ////config.ExSipAccount_Add(sp.Server, sp.Login, sp.Password, sp.Username, sp.DisplayName, 300, 1, 0);
            ////System.Threading.Thread.Sleep(1000);

            //LstChanels[i].sip.SipId = spAccount.Id;
            //LstChanels[i].sip.ConfigId = CallGeneratorServiceCLI.NewCallGenCLI.NxtSipId;
            //CallGeneratorServiceCLI.NewCallGenCLI.NxtSipId++;
            ////config.ExSipAccount_Add("sip.telbo.com", "myworld80", "hello2013", "myworld80", "00442074542000", 300, 1, 1);
            ////config.ExSipAccount_Add("149.7.44.141", "hadi", "had1", "hadi", "00442074542000", 300, 1, 1);

            //LstChanels[i].sip.phone.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(phone_OnInitialized);
            //LstChanels[i].sip.phone.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(phone_OnLineSwiched);
            //LstChanels[i].sip.phone.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(phone_OnEstablishedCall);
            //LstChanels[i].sip.phone.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(phone_OnClearedCall);
            //LstChanels[i].sip.phone.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(phone_OnRegistered);
            //LstChanels[i].sip.phone.OnUnRegistered += new _IAbtoPhoneEvents_OnUnRegisteredEventHandler(phone_OnUnRegistered);
            //LstChanels[i].sip.phone.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(phone_OnPlayFinished);
            //LstChanels[i].sip.phone.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(phone_OnEstablishedConnection);
            //LstChanels[i].sip.phone.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(phone_OnClearedConnection);
            //LstChanels[i].sip.phone.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(phone_OnPhoneNotify);

            //LstChanels[i].sip.phone.OnRemoteAlerting2 += new _IAbtoPhoneEvents_OnRemoteAlerting2EventHandler(phone_OnRemoteAlerting2);
            //LstChanels[i].sip.phone.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
            //LstChanels[i].sip.phone.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);
            //WriteToEventLog("Reconfigure 3");
            //LstChanels[i].sip.phone.ApplyConfig();
            //WriteToEventLog("Reconfigure 4");
            //LstChanels[i].sip.phone.Initialize();
            //WriteToEventLog("Reconfigure 5");
            //System.Threading.Thread.Sleep(1000);
            ///////////////////////////////////////////////////////////////////////////////////////
        }

        public void AddnewSIP(User user)
        {
            SipAccount spAccount = SipAccountRepository.Load(1);
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
            config.CallerId = user.CallerId;
            config.RegDomain = user.CallerId;
            config.RegUser = user.CallerId;
            config.RegPass = spAccount.Password;
            config.RegAuthId = user.CallerId;
            config.RegExpire = 300;

            //config.ExSipAccount_Add(sp.Server, sp.Login, sp.Password, sp.Username, sp.DisplayName, 300, 1, 0);
            //System.Threading.Thread.Sleep(1000);

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
               // List<User> LstUsers = UserRepository.GetSipUsers();
                SipAccount sp = SipAccountRepository.Load(1);
               
                //List<SipAccount> LstSipAccounts = SipAccountRepository.GetSipAccounts();
                int i = 0;
                //foreach (SipAccount sp in LstSipAccounts)
                {
                    SIP s = new SIP();

                    s.phone = Generalphone;
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

                    //config.CallerId = "00442074646665";
                    config.CallerId = sp.User.CallerId;
                    config.RegDomain = "91.236.236.53";
                    config.RegUser = sp.User.CallerId;
                    config.RegPass = sp.User.CallerId;
                    config.RegAuthId = sp.User.CallerId;
                    config.RegExpire = 3000;

                    //config.ExSipAccount_Add(sp.Server, sp.Login, sp.Password, sp.Username, sp.DisplayName, 300, 1, 0);
                    //System.Threading.Thread.Sleep(1000);

                    //s.SipId = sp.Id;
                    s.SipId = 1;
                    s.ConfigId = i;
                    
                    //config.ExSipAccount_Add("sip.telbo.com", "myworld80", "hello2013", "myworld80", "00442074542000", 300, 1, 1);
                    //config.ExSipAccount_Add("149.7.44.141", "hadi", "had1", "hadi", "00442074542000", 300, 1, 1);

                    //config.ExSipAccount_Add("91.236.236.53", "00442074646665", "00442074646665", "00442074646665", "00442074646665", 300, 1, 0);
                    //config.ExSipAccount_Add("91.236.236.53", "00442074646666", "00442074646666", "00442074646666", "00442074646666", 300, 1, 1);
                    //config.ExSipAccount_Add("91.236.236.53", "00442074646667", "00442074646667", "00442074646667", "00442074646667", 300, 1, 1);

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
                //int lineCount = phone.Config.ExSipAccount_Count();
                for (int i = 0; i < 64; i++)
                {
                    ChannelAllocation Chanel = new ChannelAllocation();
                    Chanel.Id = i;
                    Chanel.Idle = true;
                    Chanel.DestinationNumber = "";
                    Chanel.StartLastCall = DateTime.MinValue;

                    //Addedddd
                    Chanel.sip = new SIP();
                    Chanel.sip.ConfigId = 1;
                    Chanel.sip.SipId = 1;
                    Chanel.sip.phone = LstSip[0].phone;
                    ////////////////////////////////////////////

                    LstChanels.Add(Chanel);
                }
                thGetCalls.Start();
                c.Start();
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
                        ChannelAllocation c2 = ChannelAllocation.GetCallServiceConnection(ConnectionId);

                        //ChannelAllocation c = ChannelAllocation.GetCallService(LineId);
                        GeneratedCall GenCall = GeneratedCallRepository.Load(c2.GeneratedCallid);

                        if (GenCall != null)
                        {
                            //WriteToEventLog(" \r\n" + "GenCall : " + GenCall.Id);
                            GenCall.Status = "4";
                            //GenCall.EndDate = DateTime.Now;
                            GenCall.ResponseCode = responseCode.ToString();
                            GeneratedCallRepository.Save(GenCall);
                        }
                        //else
                        //WriteToEventLog(" \r\n" + "GenCall NULL: " + GenCall);

                        //System.Threading.Thread.Sleep(5000);
                        //WriteToEventLog("LineId: " + LineId + " ConnectionId: " + ConnectionId);
                        //NewCallGenCLI.LstChanels[LineId].sip.phone.HangUp(ConnectionId);

                        //LstChanels[c2.Id].Idle = true;
                        //LstChanels[c2.Id].StartDate = DateTime.MinValue;
                        //LstChanels[c2.Id].StartLastCall = DateTime.MinValue;
                        //LstChanels[c2.Id].GeneratedCallid = 0;
                        //LstChanels[c2.Id].ConnectionId = 0;
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.LogException(ex);
                    WriteToEventLog(" \r\n" + (ex.ToString()));

                    ChannelAllocation c2 = ChannelAllocation.GetCallServiceConnection(ConnectionId);

                    //ChannelAllocation c = ChannelAllocation.GetCallService(LineId);
                    GeneratedCall GenCall = GeneratedCallRepository.Load(c2.GeneratedCallid);

                    if (GenCall != null)
                    {
                        //WriteToEventLog(" \r\n" + "GenCall : " + GenCall.Id);
                        //GenCall.Status = "3";
                        GenCall.EndDate = DateTime.Now;
                        GenCall.ResponseCode = responseCode.ToString();
                        GeneratedCallRepository.Save(GenCall);
                    }

                    LstChanels[c2.Id].Idle = true;
                    LstChanels[c2.Id].StartDate = DateTime.MinValue;
                    LstChanels[c2.Id].StartLastCall = DateTime.MinValue;
                    LstChanels[c2.Id].GeneratedCallid = 0;
                    LstChanels[c2.Id].ConnectionId = 0;
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

                ChannelAllocation c = ChannelAllocation.GetCallServiceConnection(ConnectionId);
                if (c == null)
                    WriteToEventLog(" \r\n" + "c is null : ");
                else
                {
                    WriteToEventLog(" \r\n" + "c GeneratedCallid : " + c.GeneratedCallid + " ID: " + c.Id);
                    GeneratedCall GenCall = GeneratedCallRepository.Load(c.GeneratedCallid);

                    if (GenCall != null && c.GeneratedCallid != 0)
                    {
                        //LstChanels[c.Id].Idle = true;
                        //LstChanels[c.Id].StartDate = DateTime.MinValue;
                        //LstChanels[c.Id].StartLastCall = DateTime.MinValue;
                        //LstChanels[c.Id].GeneratedCallid = 0;
                        //LstChanels[c.Id].ConnectionId = 0;

                        //WriteToEventLog( " \r\n" + "GenCall : " + GenCall.Id);
                        //GenCall.Status = "6";
                        GenCall.EndDate = DateTime.Now;
                        //GenCall.ResponseCode = Status.ToString();
                        GeneratedCallRepository.Save(GenCall);
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
                
                ChannelAllocation c = ChannelAllocation.GetCallServiceConnection(ConnectionId);
                if (c == null)
                    WriteToEventLog(" \r\n" + "c is null : ");
                else
                {
                    WriteToEventLog(" \r\n" + "c GeneratedCallid : " + c.GeneratedCallid + " ID: " + c.Id);
                    GeneratedCall GenCall = GeneratedCallRepository.Load(c.GeneratedCallid);

                    if (GenCall != null && c.GeneratedCallid != 0)
                    {
                        //LstChanels[c.Id].Idle = true;
                        //LstChanels[c.Id].StartDate = DateTime.MinValue;
                        //LstChanels[c.Id].StartLastCall = DateTime.MinValue;
                        //LstChanels[c.Id].GeneratedCallid = 0;
                        //LstChanels[c.Id].ConnectionId = 0;

                        //WriteToEventLog( " \r\n" + "GenCall : " + GenCall.Id);
                        GenCall.Status = "6";
                        GenCall.EndDate = DateTime.Now;
                        //GenCall.ResponseCode = Status.ToString();
                        GeneratedCallRepository.Save(GenCall);
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
                //WriteToEventLog( " \r\n" + ("phone_OnRegistered: Msg" + Msg.ToString()));
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
                    ChannelAllocation c = ChannelAllocation.GetCallService(LineId);
                    //if (c == null)
                    //WriteToEventLog( " \r\n" + "c is null : ");
                    //else
                    //WriteToEventLog( " \r\n" + "c GeneratedCallid : " + c.GeneratedCallid + " ID: " + c.Id);
                    GeneratedCall GenCall = GeneratedCallRepository.Load(c.GeneratedCallid);

                    if (GenCall != null && c.GeneratedCallid != 0)
                    {
                        LstChanels[c.Id].Idle = true;
                        LstChanels[c.Id].StartDate = DateTime.MinValue;
                        LstChanels[c.Id].StartLastCall = DateTime.MinValue;
                        LstChanels[c.Id].GeneratedCallid = 0;
                        LstChanels[c.Id].ConnectionId = 0;

                        //WriteToEventLog( " \r\n" + "GenCall : " + GenCall.Id);
                        //GenCall.Status = "3";
                        //GenCall.EndDate = DateTime.Now;
                        GenCall.ResponseCode = Status.ToString();
                        GeneratedCallRepository.Save(GenCall);
                    }
                    //else
                    //WriteToEventLog( " \r\n" + "GenCall NULL: " + GenCall);


                    // displayList(this, "Ophone_OnClearedCall on Line " + LineId);
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
