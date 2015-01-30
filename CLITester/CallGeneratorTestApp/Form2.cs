using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using SIPVoipSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CallGeneratorTestApp
{
    public partial class Form2 : Form
    {
        public ChannelAllocation c = new ChannelAllocation();
        
        public GetCalls thGetCalls = new GetCalls();

        public TestClass t = new TestClass();

        public static List<ChannelAllocation> LstChanels = new List<ChannelAllocation>();

        public static void displayList(Form2 f, string s) {
            f.AppendTextBox2( s + " :");
            int count = 5;
            f.AppendTextBox2("Id  GCallId      SIPID     SIPCONFIG     IDLE          STARTDATE                      startLastCall                       CONID     ");
            //Id: {0}, GCallId:{1}, SIPID:{2}, SIPCONFIG:{3} , IDLE:{4} , STARTDATE:{5} , ENDDATE:{6} , CONID:{7}
            foreach (ChannelAllocation c in LstChanels)
            {
                if (count == 0)
                    break;
                count--;
                f.AppendTextBox2(c.ToString());
            }

            f.AppendTextBox2("------------------");
        }

        public static List<SIP> LstSip = new List<SIP>();

        //public static CAbtoPhone phone;
        private CConfig config;

        public Form2()
        {
            InitializeComponent();
        }

        //public  CAbtoPhone copyPhone(CAbtoPhone MainPhone)
        //{
        //    CAbtoPhone phone = new CAbtoPhone();

        //    phone.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(phone_OnInitialized);
        //    phone.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(phone_OnLineSwiched);
        //    phone.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(phone_OnEstablishedCall);
        //    phone.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(phone_OnClearedCall);
        //    phone.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(phone_OnRegistered);
        //    phone.OnUnRegistered += new _IAbtoPhoneEvents_OnUnRegisteredEventHandler(phone_OnUnRegistered);
        //    phone.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(phone_OnPlayFinished);
        //    phone.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(phone_OnEstablishedConnection);
        //    phone.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(phone_OnClearedConnection);
        //    phone.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(phone_OnPhoneNotify);
        //    phone.OnRemoteAlerting += new _IAbtoPhoneEvents_OnRemoteAlertingEventHandler(phone_OnRemoteAlerting);
        //    phone.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
        //    phone.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);

        //    CConfig configNew = phone.Config;
        //    CConfig configOld = MainPhone.Config;


        //    configNew.ActivePlaybackDevice = "";
        //    configNew.ActiveNetworkInterface = "Ethernet-IPV4-192.168.22.12";

        //    if (configNew.RecordDeviceCount > 0)
        //        configNew.ActiveRecordDevice = configNew.get_RecordDevice(0);

        //    configNew.LicenseUserId = configOld.LicenseUserId;
        //    configNew.LicenseKey = configOld.LicenseKey;

        //    configNew.StunServer = "";
        //    configNew.ListenPort = 5060;
        //    configNew.EchoCancelationEnabled = 1;
        //    configNew.NoiseReductionEnabled = 1;
        //    configNew.VolumeUpdateSubscribed = 1;
        //    configNew.DialToneEnabled = 1;
        //    configNew.MP3RecordingEnabled = 0;
        //    configNew.EncryptedCallEnabled = 0;
        //    configNew.AutoAnswerEnabled = 0;
        //    configNew.RingToneEnabled = 0;
        //    configNew.LocalAudioEnabled = 0;
        //    configNew.LocalTonesEnabled = 0;
        //    configNew.MixerFilePlayerEnabled = 1;
        //    configNew.SamplesPerSecond = 32000;
        //    configNew.AutoGainControlEnabled = 1;
        //    configNew.LogLevel = LogLevelType.eLogInfo;
        //    configNew.CallInviteTimeout = 60;
        //    configNew.UserAgent = "ABTO Video SIP SDK";
        //    configNew.CallerId = "00442074542000";

        //    for(int i = 0 ; i< configOld.ExSipAccount_Count() ; i ++)
        //    {
        //        string domain ,user , pass , username, displayName ;
        //        int exp = 300;
        //        int en = 1;
        //        configOld.ExSipAccount_Get(i, out domain, out user, out pass, out username, out displayName, out exp, out en);
        //        configNew.ExSipAccount_Add(domain, user, pass, username , displayName, 300, 1, 0);
        //    }

        //    phone.ApplyConfig();

        //    return phone;
        //}

        public void Configure()
        {
            try
            {
                List<SipAccount> LstSipAccounts = SipAccountRepository.GetSipAccounts();
                int i = 0;
                foreach (SipAccount sp in LstSipAccounts)
                {
                    SIP s = new SIP();

                    s.phone  = new CAbtoPhone();
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
                    config.CallerId = sp.User.CallerId;
                    config.RegDomain = sp.Server;
                    config.RegUser = sp.Login;
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
                    s.phone.OnRemoteAlerting += new _IAbtoPhoneEvents_OnRemoteAlertingEventHandler(phone_OnRemoteAlerting);
                    s.phone.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
                    s.phone.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);

                    s.phone.ApplyConfig();
                    s.phone.Initialize();
                    System.Threading.Thread.Sleep(1000);

                    LstSip.Add(s);
                    i++;
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        private void Form2_Load(object sender, EventArgs e)
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
                    LstChanels.Add(Chanel);
                }
                thGetCalls.Start(this);
                c.Start(this);
                //t.Start(this);
            }
            catch (System.Exception ex)
            {
                // WriteToEventLogEx("EXCEPTION S: " + ex.ToString());
            }
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBox1.Text += " \r\n" + value;
        }

        public void AppendTextBox2(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox2), new object[] { value });
                return;
            }
            textBox2.Text += " \r\n" + value;

        }

        #region phone functions

        void phone_OnTextMessageReceived(string address, string message)
        {
        }

        void phone_OnTextMessageSentStatus(string address, string reason, int bSuccess)
        {
        }

        void phone_OnRemoteAlerting(int ConnectionId, int responseCode, string reasonMsg)
        {
            try
            {
                textBox1.Text += " \r\n" + ("phone_OnRemoteAlerting: ConnectionId" + ConnectionId.ToString() + " responseCode: " + responseCode.ToString() + " reasonMsg: " + reasonMsg.ToString());
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }
        }

        void phone_OnPhoneNotify(string Msg)
        {
        }

        void phone_OnClearedConnection(int ConnectionId, int LineId)
        {
            try
            {
                textBox1.Text += " \r\n" + ("phone_OnClearedConnection: ConnectionId" + ConnectionId.ToString() + " LineId: " + LineId.ToString());
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }
        }

        void phone_OnEstablishedConnection(string AddrFrom, string AddrTo, int ConnectionId, int LineId)
        {
            try
            {
                textBox1.Text += " \r\n" + ("phone_OnEstablishedConnection: ConnectionId" + ConnectionId.ToString() + " LineId: " + LineId.ToString() + " AddrFrom: " + AddrFrom.ToString() + " AddrTo: " + AddrTo);
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }
        }

        void phone_OnPlayFinished(string Msg)
        {
        }

        void phone_OnUnRegistered(string Msg)
        {
            try
            {
                textBox1.Text += " \r\n" + ("phone_OnUnRegistered: Msg" + Msg.ToString());
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }
        }

        void phone_OnRegistered(string Msg)
        {
            try
            {
                textBox1.Text += " \r\n" + ("phone_OnRegistered: Msg" + Msg.ToString());
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }

        }

        void phone_OnClearedCall(string Msg, int Status, int LineId)
        {
            try
            {
                ChannelAllocation c = ChannelAllocation.GetCallService(LineId);
                if (c == null)
                    textBox1.Text += " \r\n" + "c is null : ";
                else
                    textBox1.Text += " \r\n" + "c GeneratedCallid : " + c.GeneratedCallid + " ID: " + c.Id;
                GeneratedCall GenCall = GeneratedCallRepository.Load(c.GeneratedCallid);

                if (GenCall != null)
                {
                    textBox1.Text += " \r\n" + "GenCall : " + GenCall.Id;
                    GenCall.Status = "3";
                    GenCall.EndDate = DateTime.Now;
                    GenCall.ResponseCode = Status.ToString();
                    GeneratedCallRepository.Save(GenCall);
                }
                else
                    textBox1.Text += " \r\n" + "GenCall NULL: " + GenCall;

                LstChanels[c.Id].Idle = true;
                LstChanels[c.Id].StartDate = DateTime.MinValue;
                LstChanels[c.Id].StartLastCall = DateTime.MinValue;
                LstChanels[c.Id].GeneratedCallid = 0;
                displayList(this, "Ophone_OnClearedCall on Line " + LineId);
                textBox1.Text += " \r\n" + ("phone_OnClearedCall: Msg" + Msg.ToString() + " LineId: " + LineId.ToString() + " Status: " + Status.ToString());
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }
        }

        void phone_OnEstablishedCall(string Msg, int LineId)
        {
            try
            {
                textBox1.Text += " \r\n" + ("phone_OnEstablishedCall: Msg" + Msg.ToString() + " LineId: " + LineId.ToString());
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }
        }

        void phone_OnLineSwiched(int LineId)
        {
            try
            {
                textBox1.Text += " \r\n" + ("phone_OnLineSwiched: LineId" + LineId.ToString());
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }
        }

        void phone_OnInitialized(string Msg)
        {
            try
            {
                textBox1.Text += " \r\n" + ("phone_OnInitialized: Msg" + Msg.ToString());
            }
            catch (System.Exception ex)
            {
                textBox1.Text += " \r\n" + (ex.ToString());
            }
        }

        #endregion

    }
}
