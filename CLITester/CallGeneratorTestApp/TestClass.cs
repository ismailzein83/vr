using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIPVoipSDK;
using System.Configuration;

namespace CallGeneratorTestApp
{
    public class TestClass
    {
        private CAbtoPhone phone1;
        private CConfig config1;

        private CAbtoPhone phone2;
        private CConfig config2;

        public static Form2 f;

        public void Start(Form2 f)
        {
            TestClass.f = f;

            Configure();
            Configure2();

            phone1.Initialize();
            System.Threading.Thread.Sleep(1000);
            phone2.Initialize();
            System.Threading.Thread.Sleep(1000);

            //phone1.SetCurrentLine(1);
            phone1.StartCall2("0096170713228");
            
          //  phone2.SetCurrentLine(2);
            phone2.StartCall2("009613551833");

            //phone1.SetCurrentLine(3);
            phone1.StartCall2("0096171584024");


            //phone2.SetCurrentLine(4);
            phone2.StartCall2("009613955125");


            //phone1.SetCurrentLine(5);
            phone1.StartCall2("00971509031804");


            //phone1.Config.ExSipAccount_SetDefaultIdx(0);
            //phone1.ApplyConfig();
            //phone1.Initialize();
            //System.Threading.Thread.Sleep(1000);
            //phone1.SetCurrentLine(5);
            //int Connection1 = phone1.StartCall2("009613224722");


            //phone1.Config.ExSipAccount_SetDefaultIdx(1);
            //phone1.ApplyConfig();
            //phone1.Initialize();
            //System.Threading.Thread.Sleep(1000);
            //phone1.SetCurrentLine(7);
            //int Connection2 = phone1.StartCall2("0096170713228");


            //phone1.Config.ExSipAccount_SetDefaultIdx(0);
            //phone1.ApplyConfig();
            //phone1.Initialize();
            //System.Threading.Thread.Sleep(1000);
            //phone1.SetCurrentLine(9);
            //int Connection3 = phone1.StartCall2("009613224722");


            //phone1.Config.ExSipAccount_SetDefaultIdx(1);
            //phone1.ApplyConfig();
            //phone1.Initialize();
            //System.Threading.Thread.Sleep(1000);
            //phone1.SetCurrentLine(15);
            //int Connection8 = phone1.StartCall2("0096170713228");
        }

        public void Configure()
        {
            try
            {
                phone1 = new CAbtoPhone();
                //for (int i = 0; i < 2; i++)
                //{
                //    CAbtoPhone c = new CAbtoPhone();
                //    phone.Add(c);
                //}

                // for (int i = 0; i <= 1; i++)
                {
                    config1 = phone1.Config;
                  
                    config1.ActivePlaybackDevice = "";
                    config1.ActiveNetworkInterface = "Ethernet-IPV4-192.168.22.12";

                    if (config1.RecordDeviceCount > 0)
                        config1.ActiveRecordDevice = config1.get_RecordDevice(0);

                    config1.LicenseUserId = ConfigurationManager.AppSettings["LicenseUserId"];
                    config1.LicenseKey = ConfigurationManager.AppSettings["LicenseKey"];

                    config1.StunServer = "";
                    config1.ListenPort = 5060;
                    config1.EchoCancelationEnabled = 1;
                    config1.NoiseReductionEnabled = 1;
                    config1.VolumeUpdateSubscribed = 1;
                    config1.DialToneEnabled = 1;
                    config1.MP3RecordingEnabled = 0;
                    config1.EncryptedCallEnabled = 0;
                    config1.AutoAnswerEnabled = 0;
                    config1.RingToneEnabled = 0;
                    config1.LocalAudioEnabled = 0;
                    config1.LocalTonesEnabled = 0;
                    config1.MixerFilePlayerEnabled = 1;
                    config1.SamplesPerSecond = 32000;
                    config1.AutoGainControlEnabled = 1;
                    config1.LogLevel = LogLevelType.eLogInfo;
                    config1.CallInviteTimeout = 60;
                    config1.UserAgent = "ABTO Video SIP SDK";
                    config1.CallerId = "00442074542000";
                    config1.RegDomain = "sip.telbo.com";
                    config1.RegUser = "myworld80";
                    config1.RegPass = "hello2013";
                    config1.RegAuthId = "myworld80";

                    //config1.RegDomain = "149.7.44.141";
                    //config1.RegUser = "hadi";
                    //config1.RegPass = "had1";
                    //config1.RegAuthId = "hadi";

                     config1.RegExpire = 300;
                    //if( i == 0)
                   // config1.ExSipAccount_Add("sip.telbo.com", "myworld80", "hello2013", "myworld80", "00442074542000", 300, 1, 1);
                    //config1.ExSipAccount_Add("149.7.44.141", "hadi", "had1", "hadi", "00442074542000", 300, 1, 1);


                    phone1.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(phone_OnInitialized);
                    phone1.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(phone_OnLineSwiched);
                    phone1.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(phone_OnEstablishedCall);
                    phone1.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(phone_OnClearedCall);
                    phone1.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(phone_OnRegistered);
                    phone1.OnUnRegistered += new _IAbtoPhoneEvents_OnUnRegisteredEventHandler(phone_OnUnRegistered);
                    phone1.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(phone_OnPlayFinished);
                    phone1.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(phone_OnEstablishedConnection);
                    phone1.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(phone_OnClearedConnection);
                    phone1.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(phone_OnPhoneNotify);
                    phone1.OnRemoteAlerting += new _IAbtoPhoneEvents_OnRemoteAlertingEventHandler(phone_OnRemoteAlerting);
                    phone1.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
                    phone1.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);

                    phone1.ApplyConfig();
                }


            }
            catch (System.Exception ex)
            {

            }
        }


        public void Configure2()
        {
            try
            {
                phone2 = new CAbtoPhone();
                //for (int i = 0; i < 2; i++)
                //{
                //    CAbtoPhone c = new CAbtoPhone();
                //    phone.Add(c);
                //}

                // for (int i = 0; i <= 1; i++)
                {
                    config2 = phone2.Config;

                    config2.ActivePlaybackDevice = "";
                    config2.ActiveNetworkInterface = "Ethernet-IPV4-192.168.22.12";

                    if (config2.RecordDeviceCount > 0)
                        config2.ActiveRecordDevice = config2.get_RecordDevice(0);

                    config2.LicenseUserId = ConfigurationManager.AppSettings["LicenseUserId"];
                    config2.LicenseKey = ConfigurationManager.AppSettings["LicenseKey"];

                    config2.StunServer = "";
                    config2.ListenPort = 5060;
                    config2.EchoCancelationEnabled = 1;
                    config2.NoiseReductionEnabled = 1;
                    config2.VolumeUpdateSubscribed = 1;
                    config2.DialToneEnabled = 1;
                    config2.MP3RecordingEnabled = 0;
                    config2.EncryptedCallEnabled = 0;
                    config2.AutoAnswerEnabled = 0;
                    config2.RingToneEnabled = 0;
                    config2.LocalAudioEnabled = 0;
                    config2.LocalTonesEnabled = 0;
                    config2.MixerFilePlayerEnabled = 1;
                    config2.SamplesPerSecond = 32000;
                    config2.AutoGainControlEnabled = 1;
                    config2.LogLevel = LogLevelType.eLogInfo;
                    config2.CallInviteTimeout = 60;
                    config2.UserAgent = "ABTO Video SIP SDK";
                    config2.CallerId = "00442074542000";
                    //config2.RegDomain = "sip.telbo.com";
                    //config2.RegUser = "myworld80";
                    //config2.RegPass = "hello2013";
                    //config2.RegAuthId = "myworld80";

                    config2.RegDomain = "149.7.44.141";
                    config2.RegUser = "hadi";
                    config2.RegPass = "had1";
                    config2.RegAuthId = "hadi";

                    config2.RegExpire = 300;
                    //if( i == 0)
                    // config1.ExSipAccount_Add("sip.telbo.com", "myworld80", "hello2013", "myworld80", "00442074542000", 300, 1, 1);
                    //config1.ExSipAccount_Add("149.7.44.141", "hadi", "had1", "hadi", "00442074542000", 300, 1, 1);


                    phone2.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(phone_OnInitialized);
                    phone2.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(phone_OnLineSwiched);
                    phone2.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(phone_OnEstablishedCall);
                    phone2.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(phone_OnClearedCall);
                    phone2.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(phone_OnRegistered);
                    phone2.OnUnRegistered += new _IAbtoPhoneEvents_OnUnRegisteredEventHandler(phone_OnUnRegistered);
                    phone2.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(phone_OnPlayFinished);
                    phone2.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(phone_OnEstablishedConnection);
                    phone2.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(phone_OnClearedConnection);
                    phone2.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(phone_OnPhoneNotify);
                    phone2.OnRemoteAlerting += new _IAbtoPhoneEvents_OnRemoteAlertingEventHandler(phone_OnRemoteAlerting);
                    phone2.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
                    phone2.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);

                    phone2.ApplyConfig();
                }


            }
            catch (System.Exception ex)
            {

            }
        }

        //public void Configure2()
        //{
        //    try
        //    {
        //        phone2 = new CAbtoPhone();
        //        //for (int i = 0; i < 2; i++)
        //        //{
        //        //    CAbtoPhone c = new CAbtoPhone();
        //        //    phone.Add(c);
        //        //}

        //        // for (int i = 0; i <= 1; i++)
        //        {
        //            config2 = phone2.Config;

        //            config2.ActivePlaybackDevice = "";
        //            config2.ActiveNetworkInterface = "Ethernet-IPV4-192.168.22.12";

        //            if (config2.RecordDeviceCount > 0)
        //                config2.ActiveRecordDevice = config2.get_RecordDevice(0);

        //            config2.LicenseUserId = ConfigurationManager.AppSettings["LicenseUserId"];
        //            config2.LicenseKey = ConfigurationManager.AppSettings["LicenseKey"];

        //            config2.StunServer = "";
        //            config2.ListenPort = 5060;
        //            config2.EchoCancelationEnabled = 1;
        //            config2.NoiseReductionEnabled = 1;
        //            config2.VolumeUpdateSubscribed = 1;
        //            config2.DialToneEnabled = 1;
        //            config2.MP3RecordingEnabled = 0;
        //            config2.EncryptedCallEnabled = 0;
        //            config2.AutoAnswerEnabled = 0;
        //            config2.RingToneEnabled = 0;
        //            config2.LocalAudioEnabled = 0;
        //            config2.LocalTonesEnabled = 0;
        //            config2.MixerFilePlayerEnabled = 1;
        //            config2.SamplesPerSecond = 32000;
        //            config2.AutoGainControlEnabled = 1;
        //            config2.LogLevel = LogLevelType.eLogInfo;
        //            config2.CallInviteTimeout = 60;
        //            config2.UserAgent = "ABTO Video SIP SDK";
        //            config2.CallerId = "00442074542000";
        //            config2.RegDomain = "sip.telbo.com";
        //            config2.RegUser = "myworld80";
        //            config2.RegPass = "hello2013";
        //            config2.RegAuthId = "myworld80";

        //            //config.RegDomain = "91.236.236.236";
        //            //config.RegUser = "00442074542000";
        //            //config.RegPass = "6u6i7uy5";
        //            //config.RegAuthId = "vr001";

        //            config2.RegDomain = "149.7.44.141";
        //            config2.RegUser = "hadi";
        //            config2.RegPass = "had1";
        //            config2.RegAuthId = "hadi";

        //            config2.RegExpire = 300;
        //            //if( i == 0)
        //            //    config2.ExSipAccount_Add("sip.telbo.com", "myworld80", "hello2013", "myworld80", "00442074542000", 300, 1, 1);

        //            //if (i == 1)
        //            //config.ExSipAccount_Add("149.7.44.141", "hadi", "had1", "hadi", "00442074542000", 300, 1, 1);


        //            phone2.OnInitialized += new _IAbtoPhoneEvents_OnInitializedEventHandler(phone_OnInitialized);
        //            phone2.OnLineSwiched += new _IAbtoPhoneEvents_OnLineSwichedEventHandler(phone_OnLineSwiched);
        //            phone2.OnEstablishedCall += new _IAbtoPhoneEvents_OnEstablishedCallEventHandler(phone_OnEstablishedCall);
        //            phone2.OnClearedCall += new _IAbtoPhoneEvents_OnClearedCallEventHandler(phone_OnClearedCall);
        //            phone2.OnRegistered += new _IAbtoPhoneEvents_OnRegisteredEventHandler(phone_OnRegistered);
        //            phone2.OnUnRegistered += new _IAbtoPhoneEvents_OnUnRegisteredEventHandler(phone_OnUnRegistered);
        //            phone2.OnPlayFinished += new _IAbtoPhoneEvents_OnPlayFinishedEventHandler(phone_OnPlayFinished);
        //            phone2.OnEstablishedConnection += new _IAbtoPhoneEvents_OnEstablishedConnectionEventHandler(phone_OnEstablishedConnection);
        //            phone2.OnClearedConnection += new _IAbtoPhoneEvents_OnClearedConnectionEventHandler(phone_OnClearedConnection);
        //            phone2.OnPhoneNotify += new _IAbtoPhoneEvents_OnPhoneNotifyEventHandler(phone_OnPhoneNotify);
        //            phone2.OnRemoteAlerting += new _IAbtoPhoneEvents_OnRemoteAlertingEventHandler(phone_OnRemoteAlerting);
        //            phone2.OnTextMessageSentStatus += new _IAbtoPhoneEvents_OnTextMessageSentStatusEventHandler(phone_OnTextMessageSentStatus);
        //            phone2.OnTextMessageReceived += new _IAbtoPhoneEvents_OnTextMessageReceivedEventHandler(phone_OnTextMessageReceived);
        //        }


        //    }
        //    catch (System.Exception ex)
        //    {

        //    }
        //}

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
               f.AppendTextBox ("phone_OnRemoteAlerting: ConnectionId" + ConnectionId.ToString() + " responseCode: " + responseCode.ToString() + " reasonMsg: " + reasonMsg.ToString());
               f.AppendTextBox("phone_OnRemoteAlerting: ConnectionId" + ConnectionId.ToString() + " responseCode: " + responseCode.ToString() + " reasonMsg: " + reasonMsg.ToString());
            }
            catch (System.Exception ex)
            {
               f.AppendTextBox (ex.ToString());
            }
        }

        void phone_OnPhoneNotify(string Msg)
        {
        }

        void phone_OnClearedConnection(int ConnectionId, int LineId)
        {
            try
            {
             f.AppendTextBox ("phone_OnClearedConnection: ConnectionId" + ConnectionId.ToString() + " LineId: " + LineId.ToString());
            }
            catch (System.Exception ex)
            {
               f.AppendTextBox (ex.ToString());
            }
        }

        void phone_OnEstablishedConnection(string AddrFrom, string AddrTo, int ConnectionId, int LineId)
        {
            try
            {
               f.AppendTextBox ("phone_OnEstablishedConnection: ConnectionId" + ConnectionId.ToString() + " LineId: " + LineId.ToString() + " AddrFrom: " + AddrFrom.ToString() + " AddrTo: " + AddrTo);
            }
            catch (System.Exception ex)
            {
                f.AppendTextBox (ex.ToString());
            }
        }

        void phone_OnPlayFinished(string Msg)
        {
        }

        void phone_OnUnRegistered(string Msg)
        {
            try
            {
                f.AppendTextBox ("phone_OnUnRegistered: Msg" + Msg.ToString());
            }
            catch (System.Exception ex)
            {
                f.AppendTextBox (ex.ToString());
            }
        }

        void phone_OnRegistered(string Msg)
        {
            try
            {
                f.AppendTextBox ("phone_OnRegistered: Msg" + Msg.ToString());
            }
            catch (System.Exception ex)
            {
                f.AppendTextBox (ex.ToString());
            }

        }

        void phone_OnClearedCall(string Msg, int Status, int LineId)
        {
            f.AppendTextBox("phone_OnClearedCall: Msg" + Msg.ToString() + " Status: " + Status.ToString() + " LineId: " + LineId.ToString());

            //try
            //{
            //    ChannelAllocation c = ChannelAllocation.GetCallService(LineId);
            //    if (c == null)
            //        f.AppendTextBox "c is null : ";
            //    else
            //        f.AppendTextBox "c GeneratedCallid : " + c.GeneratedCallid + " ID: " + c.Id;
            //    GeneratedCall GenCall = GeneratedCallRepository.Load(c.GeneratedCallid);

            //    if (GenCall != null)
            //    {
            //        f.AppendTextBox "GenCall : " + GenCall.Id;
            //        GenCall.Status = "3";
            //        GenCall.EndDate = DateTime.Now;
            //        GenCall.ResponseCode = Status.ToString();
            //        GeneratedCallRepository.Save(GenCall);
            //    }
            //    else
            //        f.AppendTextBox "GenCall NULL: " + GenCall;

            //    LstChanels[c.Id].Idle = true;
            //    LstChanels[c.Id].StartDate = DateTime.MinValue;
            //    LstChanels[c.Id].StartLastCall = DateTime.MinValue;
            //    LstChanels[c.Id].GeneratedCallid = 0;
            //    displayList(this, "Ophone_OnClearedCall on Line " + LineId);
            //    f.AppendTextBox ("phone_OnClearedCall: Msg" + Msg.ToString() + " LineId: " + LineId.ToString() + " Status: " + Status.ToString());
            //}
            //catch (System.Exception ex)
            //{
            //    f.AppendTextBox (ex.ToString());
            //}
        }

        void phone_OnEstablishedCall(string Msg, int LineId)
        {
            try
            {
                f.AppendTextBox ("phone_OnEstablishedCall: Msg" + Msg.ToString() + " LineId: " + LineId.ToString());
            }
            catch (System.Exception ex)
            {
                f.AppendTextBox (ex.ToString());
            }
        }

        void phone_OnLineSwiched(int LineId)
        {
            try
            {
                f.AppendTextBox ("phone_OnLineSwiched: LineId" + LineId.ToString());
            }
            catch (System.Exception ex)
            {
                f.AppendTextBox (ex.ToString());
            }
        }

        void phone_OnInitialized(string Msg)
        {
            try
            {
                f.AppendTextBox ("phone_OnInitialized: Msg" + Msg.ToString());
            }
            catch (System.Exception ex)
            {
                f.AppendTextBox (ex.ToString());
            }
        }

        #endregion
       
    }
}
