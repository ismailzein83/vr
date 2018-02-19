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
using System.Diagnostics;
using System.Globalization;

namespace CallGeneratorService
{
    public class ChannelAllocation
    {
        // Generated Call - Status
        // Status = 1 just on select the record from the database
        // Status = 2 directly before starting the call
        // Status = 3 on phone_OnClearedCall function
        // Status = 0 when the call is started without ending the call by the event in a time out = 2 minutes

        System.Timers.Timer timer = new System.Timers.Timer(2000);
        static Random r = new Random();
        #region Initialisation of Channels
        private int id;
        public int generatedCallid;
        public SIP sip;
        public bool idle = false;
        public string destinationNumber;
        public DateTime startDate = DateTime.MinValue;
        private DateTime startLastCall;
        private DateTime endDate = DateTime.MinValue;
        private DateTime attemptDate = DateTime.MinValue;
        private DateTime connectDateTime = DateTime.MinValue;
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
                idle = value;
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

        public DateTime AttemptDate
        {
            get
            {
                return attemptDate;
            }
            set
            {
                attemptDate = value;
            }
        }
        public DateTime ConnectDateTime
        {
            get
            {
                return connectDateTime;
            }
            set
            {
                connectDateTime = value;
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

        public void Start()
        {
            
            //ChannelAllocation.f = f;
            timer.Enabled = true;
            timer.Elapsed += OnTimedEvent;
        }

        private static readonly object _syncRoot = new object();

        static Object s_lockObj = new object();
        static bool s_isRunning;

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            lock(s_lockObj)
            {
                if (s_isRunning)
                    return;
                s_isRunning = true;
            }
            try
            {
               
                for (int i = 0; i < 64; i++)
                {
                    ChannelAllocation c = Service1.LstChanels[i];
                    
                    if ((c.Idle == false) && (c.StartDate == DateTime.MinValue))
                    {
                        ///////GeneratedCall GenCall = GeneratedCallRepository.Load(Service1.LstChanels[i].GeneratedCallid);
                        //WriteToEventLog("iiii " + Service1.LstChanels[i].GeneratedCallid + " " + Service1.LstChanels[i].DestinationNumber);
                        
                        //////GenCall.Status = "2";
                        ////////GenCall.StartCall = DateTime.Now;
                        ///////////GeneratedCallRepository.Save(GenCall);

                        Service1.LstChanels[i].StartDate = DateTime.Now;
                        Service1.LstChanels[i].startLastCall = DateTime.Now;
                        Service1.LstChanels[i].connectDateTime = DateTime.MinValue;
                        //int ii = Service1.LstChanels[i].sip.ConfigId;

                        //p.Config.ExSipAccount_SetDefaultIdx(ii);
                        //p.ApplyConfig();
                        //p.Initialize();
                        //System.Threading.Thread.Sleep(1000);
                        //p.SetCurrentLine(Service1.LstChanels[i].id + 1);

                        //int ConnectionId = p.StartCall2(Service1.LstChanels[i].destinationNumber);

                        lock (_syncRoot)
                        {

                            //Service1.LstChanels[i].sip.phone.Config.CallerId = i.ToString() +  "999";
                            //Service1.LstChanels[i].sip.phone.ApplyConfig();
                            //Service1.LstChanels[i].sip.phone.Initialize();

                            Service1.LstChanels[i].sip.phone.SetCurrentLine(Service1.LstChanels[i].id + 1);

                            var leftPart = r.Next(1, 9);
                            var rightPart = Math.Round(r.NextDouble() * 1e+11, 0);
                            string newCli = string.Format("{0}{1}", leftPart.ToString(CultureInfo.InvariantCulture), rightPart.ToString(CultureInfo.InvariantCulture).PadLeft(11, '0'));

                            int ConnectionId = Service1.LstChanels[i].sip.phone.StartCall3(Service1.LstChanels[i].DestinationNumber, newCli);
                            Service1.LstChanels[i].AttemptDate = DateTime.Now;

                            String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                            //Service1.displayList(f, "threadId: " + threadId + " StartCall " + ConnectionId + " Line :" + Service1.LstChanels[i].id + 1 + " SIPCONFIG: " + Service1.LstChanels[i].sip.ConfigId + " ii " + ii);
                        }
                    }

                    //Clear Time out channels
                    ChannelAllocation chanel = Service1.LstChanels[i];

                    if (chanel.startLastCall != DateTime.MinValue)
                    {
                        DateTime dt = chanel.startLastCall;
                        TimeSpan span = new TimeSpan();
                        if (dt != null)
                            span = DateTime.Now - dt;
                        double totalSeconds = span.TotalSeconds;

                        if (chanel.Idle == false && totalSeconds > 180)
                        {
                            GeneratedCall CallGenEnd = GeneratedCallRepository.Load(Service1.LstChanels[i].GeneratedCallid);

                            //Reset the phone with this Id;
                            if (Service1.LstChanels[i].DestinationNumber == "")
                            {
                                Service1.LstChanels[i].EndDate = DateTime.Now;
                                Service1.LstChanels[i].connectDateTime = DateTime.MinValue;
                                Service1.LstChanels[i].EndCall();
                            }
                            else
                            {
                                if (CallGenEnd != null)
                                {
                                    CallGenEnd.Status = "0";
                                    CallGenEnd.EndDate = DateTime.Now;
                                    bool tr = GeneratedCallRepository.Save(CallGenEnd);
                                }
                                //else
                                //WriteToEventLog("CallCenter=> the entry is null");

                                Service1.LstChanels[i].EndDate = DateTime.Now;
                                Service1.LstChanels[i].connectDateTime = DateTime.MinValue;
                                Service1.LstChanels[i].EndCall();
                            }


                           
                            //Reset the phone with this Id and the Object
                            Service1.LstChanels[i].Idle = true;
                            Service1.LstChanels[i].DestinationNumber = "";
                            Service1.LstChanels[i].sip = null;
                            Service1.LstChanels[i].startLastCall = DateTime.MinValue;
                            Service1.LstChanels[i].connectDateTime = DateTime.MinValue;
                            Service1.LstChanels[i].generatedCallid = 0;
                            //Service1.displayList(f, "Clear TimeOut");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLog("EXCEPTION ChannelAll: " + ex.ToString());
            }
            finally
            {
                lock (s_lockObj)
                {
                    s_isRunning = false;
                }
            }
        }

        public void EndCall()
        {
            
            try
            {
                
                startDate = DateTime.MinValue;
                endDate = DateTime.MinValue;
                startLastCall = DateTime.MinValue;
                connectDateTime = DateTime.MinValue;
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
                connectDateTime = DateTime.MinValue;
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
                ChannelAllocation service = Service1.LstChanels[lineId - 1];
                return service;
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
        }

        private static void WriteToEventLog(string message)
        {
            string cs = "VanCallGen";
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
