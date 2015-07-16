using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using SIPVoipSDK;
using System.Configuration;
using System.Timers;
using System.Windows.Forms;

namespace CallGeneratorTestApp
{
    public class ChannelAllocation
    {
        // Generated Call - Status
        // Status = 1 just on select the record from the database
        // Status = 2 directly before starting the call
        // Status = 3 on phone_OnClearedCall function
        // Status = 0 when the call is started without ending the call by the event in a time out = 2 minutes
        public override string ToString()
        {
            int SIPIDD = -1;
            int SIPIDCONF = -1;
            if (sip != null)
            {
                SIPIDD = sip.SipId;
                SIPIDCONF = sip.ConfigId;
            }


            return String.Format("{0}   , {1}             , {2}              , {3}               , {4}        , {5}   ,   {6}       , {7} ", id, generatedCallid, SIPIDD, SIPIDCONF, idle, startDate, startLastCall, ConnectionId);
        }
 
        System.Timers.Timer timer = new System.Timers.Timer(2000);

        public static Form2 f;

        #region Initialisation of Channels
        private int id;
        public int generatedCallid;
        public SIP sip;
        public bool idle = false;
        public string destinationNumber;
        private DateTime startDate = DateTime.MinValue;
        private DateTime startLastCall;
        private DateTime endDate = DateTime.MinValue;
        private int connectionId = 0;

        public delegate void EndingCallDelegate(ChannelAllocation service);
        public event EndingCallDelegate EndingCall;

        public delegate void BecomeIdleDelegate(ChannelAllocation center);
        public event BecomeIdleDelegate BecomeIdle;

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public int GeneratedCallid
        {
            get
            {
                return generatedCallid;
            }
            set
            {
                generatedCallid = value;
            }
        }

        public bool Idle
        {
            get
            {
                return idle;
            }
            set
            {
                idle = true;
            }
        }

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

        public string DestinationNumber
        {
            get
            {
                return destinationNumber;
            }
            set
            {
                destinationNumber = "";
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

        public DateTime StartLastCall
        {
            get
            {
                return startLastCall;
            }
            set
            {
                startLastCall = value;
            }
        }

        public SIP Account
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

        #endregion

        public void Start(Form2 f)
        {
            ChannelAllocation.f = f;
            timer.Enabled = true;
            timer.Elapsed += OnTimedEvent;
        }

        private static readonly object _syncRoot = new object();

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //lock (_syncRoot)
            //{
                for (int i = 0; i < 64; i++)
                {
                    ChannelAllocation c = Form2.LstChanels[i];
                    if ((c.idle == false) && (c.startDate == DateTime.MinValue))
                    {
                        Form2.LstChanels[i].startDate = DateTime.Now;
                        Form2.LstChanels[i].startLastCall = DateTime.Now;
                        int ii = Form2.LstChanels[i].sip.ConfigId;
                      
                        //p.Config.ExSipAccount_SetDefaultIdx(ii);
                        //p.ApplyConfig();
                        //p.Initialize();
                        //System.Threading.Thread.Sleep(1000);
                        //p.SetCurrentLine(Form2.LstChanels[i].id + 1);

                        //int ConnectionId = p.StartCall2(Form2.LstChanels[i].destinationNumber);

                        lock (_syncRoot)
                        {
                            Form2.LstChanels[i].sip.phone.SetCurrentLine(Form2.LstChanels[i].id + 1);
                           
                            int ConnectionId = Form2.LstChanels[i].sip.phone.StartCall2(Form2.LstChanels[i].destinationNumber);
                            Form2.LstChanels[i].ConnectionId = ConnectionId;
                            String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                            Form2.displayList(f, "threadId: " + threadId + " StartCall " + ConnectionId + " Line :" + Form2.LstChanels[i].id + 1 + " SIPCONFIG: " + Form2.LstChanels[i].sip.ConfigId + " ii " + ii);
                        }
                    }

                    //Clear Time out channels
                    ChannelAllocation chanel = Form2.LstChanels[i];

                    if (chanel.startLastCall != DateTime.MinValue)
                    {
                        DateTime dt = chanel.startLastCall;
                        TimeSpan span = new TimeSpan();
                        if (dt != null)
                            span = DateTime.Now - dt;
                        double totalSeconds = span.TotalSeconds;

                        if (chanel.Idle == false && totalSeconds > 60)
                        {
                            //Reset the phone with this Id;
                            if (Form2.LstChanels[i].destinationNumber == "")
                            {
                                Form2.LstChanels[i].EndDate = DateTime.Now;
                                Form2.LstChanels[i].EndCall();
                            }
                            else
                            {
                                GeneratedCall CallGenEnd = GeneratedCallRepository.Load(Form2.LstChanels[i].generatedCallid);
                                if (CallGenEnd != null)
                                {
                                    //CallGenEnd.Status = "0";
                                    CallGenEnd.EndDate = DateTime.Now;
                                    bool tr = GeneratedCallRepository.Save(CallGenEnd);
                                }
                                //else
                                //WriteToEventLog("CallCenter=> the entry is null");

                                Form2.LstChanels[i].EndDate = DateTime.Now;
                                Form2.LstChanels[i].EndCall();
                            }

                            //Reset the phone with this Id and the Object
                            Form2.displayList(f, "Clear TimeOut: " + Form2.LstChanels[i].ConnectionId);
                            Form2.LstChanels[i].Idle = true;
                            Form2.LstChanels[i].DestinationNumber = "";
                            Form2.LstChanels[i].sip = null;
                            Form2.LstChanels[i].startLastCall = DateTime.MinValue;
                            Form2.LstChanels[i].generatedCallid = 0;
                            Form2.LstChanels[i].GeneratedCallid = 0;
                            Form2.LstChanels[i].ConnectionId = 0;
                            Form2.displayList(f, "Clear TimeOut");
                        }
                    }
                }
            //}
        }

        public void EndCall()
        {
            try
            {
                startDate = DateTime.MinValue;
                endDate = DateTime.MinValue;
                startLastCall = DateTime.MinValue;
                idle = true;
                DestinationNumber = "";
                if (BecomeIdle != null)
                    BecomeIdle(this);

                if (EndingCall != null)
                    EndingCall(this);
            }
            catch (System.Exception ex)
            {
                startDate = DateTime.MinValue;
                endDate = DateTime.MinValue;
                startLastCall = DateTime.MinValue;
                idle = true;
                if (BecomeIdle != null)
                    BecomeIdle(this);
                DestinationNumber = "";
            }
        }

        public static ChannelAllocation GetCallService(int lineId)
        {
            try
            {
                ChannelAllocation service = Form2.LstChanels[lineId - 1];
                return service;
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
        }

        public static ChannelAllocation GetCallServiceConnection(int ConnectionId)
        {
            try
            {
                for (int i = 0; i < 64; i++)
                {
                    if (Form2.LstChanels[i].ConnectionId == ConnectionId)
                    {
                        ChannelAllocation service = Form2.LstChanels[i];
                        return service;
                    }
                }
                return null;
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
        }
    }
}
