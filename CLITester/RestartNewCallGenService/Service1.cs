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
using SIPVoipSDK;
namespace RestartNewCallGenService
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer GlobalTimer = new System.Timers.Timer(1800000);
        //public CallDistributor c = new CallDistributor();
        public CallDistributor c = new CallDistributor();
        internal static ServiceHost myServiceHost = null;
        CAbtoPhone phone = new CAbtoPhone();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //base.RequestAdditionalTime(30000); // 1 minutes timeout for startup
                WriteToEventLog("new start Thread111:: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());


                GlobalTimer.Elapsed += OnTimedEvent;
                GlobalTimer.Enabled = true;

                c.Start();
            }
            catch (System.Exception ex)
            {
                WriteToEventLog("EXCEPTION S: " + ex.ToString());
            }
        }

        //protected override void OnStart2(string[] args)
        //{
        //    try
        //    {
        //        phone = new CAbtoPhone();

        //        CConfig config = phone.Config;

        //        config.ActivePlaybackDevice = "";
        //        config.ActiveNetworkInterface = "Real-IPV4-84.45.10.180";

        //        if (config.RecordDeviceCount > 0)
        //            config.ActiveRecordDevice = config.get_RecordDevice(0);

        //        config.LicenseUserId = "{Licensed_for_Mohamad_Yahya_Company_Vanrise_Solutions-7DF6-49FA-0D794438-0CED-B96D-2122-7570DF50BFBC}";
        //        config.LicenseKey = "{J+cvweJ22K+vpbvnVAtqZNV7Dd6723rMnwJLYMZrEB1oclZtewl66Wt00ntispXFCxNV0+OgNF4rXKkNiQGw6Q==}";

        //        config.StunServer = "";
        //        config.ListenPort = 5060;

        //        config.EchoCancelationEnabled = 1;
        //        config.NoiseReductionEnabled = 1;
        //        config.VolumeUpdateSubscribed = 1;
        //        config.DialToneEnabled = 1;
        //        config.MP3RecordingEnabled = 0;
        //        config.EncryptedCallEnabled = 0;
        //        config.AutoAnswerEnabled = 0;
        //        config.RingToneEnabled = 0;
        //        config.LocalAudioEnabled = 0;
        //        config.LocalTonesEnabled = 0;
        //        config.MixerFilePlayerEnabled = 1;
        //        config.SamplesPerSecond = 32000;
        //        config.AutoGainControlEnabled = 1;

        //        config.LogLevel = LogLevelType.eLogInfo;
        //        config.CallInviteTimeout = 60;

        //        config.UserAgent = "ABTO Video SIP SDK";



        //        config.CallerId = "120";
        //        //config.RegDomain = sip.Server;
        //        //config.RegUser = sip.Username;
        //        //config.RegPass = sip.Password;
        //        //config.RegAuthId = sip.Login;
        //        config.RegExpire = 300;

        //        //if (sip.UseProxy.Value)
        //        //{
        //        //    config.ProxyDomain = sip.ProxyServer;
        //        //    config.ProxyUser = sip.ProxyUser;
        //        //    config.ProxyPass = sip.ProxyPass;
        //        //}
        //        //config.SetCodecOrder(sip.Codecs, 0);
               



        //        phone.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(phone_OnInitialized);
        //        phone.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(phone_OnLineSwiched);
        //        phone.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(phone_OnEstablishedCall);
        //        phone.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(phone_OnClearedCall);
        //        phone.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(phone_OnRegistered);
        //        phone.OnUnRegistered += new _IAbtoPhoneEvents_OnUnRegisteredEventHandler(phone_OnUnRegistered);
        //        phone.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(phone_OnPlayFinished);
        //        phone.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(phone_OnEstablishedConnection);
        //        phone.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(phone_OnClearedConnection);
        //        phone.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(phone_OnPhoneNotify);
        //        phone.OnRemoteAlerting += new _IAbtoPhoneEvents_OnRemoteAlertingEventHandler(phone_OnRemoteAlerting);
        //        phone.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
        //        phone.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);

        //        phone.ApplyConfig();
        //        System.Threading.Thread.Sleep(5000);

        //        phone.Initialize();
        //        System.Threading.Thread.Sleep(5000);
        //        phone.StartCall("5250929613586132@192.168.31.103");
        //        phone.StartCall("5250929613586132@91.236.236.236:5060");
        //    }
        //    catch (Exception ex)
        //    {
        //       // WriteToEventLog("EXCEPTION S: " + ex.ToString());
        //    }
        //}

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
            
        }

        void phone_OnEstablishedConnection(string AddrFrom, string AddrTo, int ConnectionId, int LineId)
        {
           
        }

        void phone_OnPlayFinished(string Msg)
        {
           
        }

        void phone_OnClearedCall(string Msg, int Status, int LineId)
        {
            
        }

        void phone_OnEstablishedCall(string Msg, int LineId)
        {

        }

        void phone_OnLineSwiched(int LineId)
        {
            
        }

        void phone_OnInitialized(string Msg)
        {
           
        }

        private void btnHangUp_Click(object sender, EventArgs e)
        {
            phone.HangUpLastCall();
        }
        #endregion

        protected override void OnStop()
        {
            if (myServiceHost != null)
            {
                myServiceHost.Close();
                myServiceHost = null;
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                WriteToEventLog("new start OnTimedEvent:: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());


              //  c.Start();
            }
            catch (System.Exception ex)
            {
                WriteToEventLog("OnTimedEvent EXCEPTION  S: " + ex.ToString());
            }
        }

        private void WriteToEventLog(string message)
        {
            string cs = "RestartNewCallGenSer";
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
