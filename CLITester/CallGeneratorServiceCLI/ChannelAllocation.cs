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

namespace CallGeneratorServiceCLI
{
    public class ChannelAllocation
    {
        // Generated Call - Status
        // Status = 1 just on select the record from the database
        // Status = 2 directly before starting the call
        // Status = 4 on phone_OnRemoteAlerting2 function 180 ringing
        // Status = 0 when the call is started without ending the call by the event in a time out = 1 minutes
        
        System.Timers.Timer timer = new System.Timers.Timer(2000);

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

        public void Start()
        {
            //ChannelAllocation.f = f;
            timer.Enabled = true;
            timer.Elapsed += OnTimedEvent;
        }

        private static readonly object _syncRoot2 = new object();

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //lock (_syncRoot)
            try
            {
                for (int i = 0; i < 64; i++)
                {
                    ChannelAllocation c = NewCallGenCLI.LstChanels[i];
                    if ((c.idle == false) && (c.startDate == DateTime.MinValue))
                    {
                        GeneratedCall GenCall = GeneratedCallRepository.Load(NewCallGenCLI.LstChanels[i].GeneratedCallid);
                        //WriteToEventLogEx("GenCall: " + GenCall.Id.ToString());
                        //GenCall.Status = "2";
                        //GenCall.StartCall = DateTime.Now;
                        //GeneratedCallRepository.Save(GenCall);

                        NewCallGenCLI.LstChanels[i].startDate = DateTime.Now;
                        NewCallGenCLI.LstChanels[i].startLastCall = DateTime.Now;

                        lock (_syncRoot2)
                        {
                            WriteToEventLogEx("SetCurrentLine: " + (NewCallGenCLI.LstChanels[i].id + 1).ToString());

                            try
                            {
                                NewCallGenCLI.LstChanels[i].sip.phone.SetCurrentLine(NewCallGenCLI.LstChanels[i].id + 1);
                            }
                            catch(System.Exception exx)
                            {
                                WriteToEventLogEx("SetCurrentLine: " + exx.ToString());
                                Logger.LogException(exx);
                                SipAccount sp = SipAccountRepository.Load(1);
                                WriteToEventLogEx("CallerId: " + sp.User.CallerId);

                                NewCallGenCLI.Reconfigure(i, sp.User.CallerId);

                                NewCallGenCLI.LstChanels[i].sip.phone.SetCurrentLine(NewCallGenCLI.LstChanels[i].id + 1);
                            }
                            

                            WriteToEventLogEx("CallerId: " + NewCallGenCLI.LstChanels[i].sip.phone.Config.CallerId);
                            //NewCallGenCLI.LstChanels[i].sip.phone.ApplyConfig();
                            //System.Threading.Thread.Sleep(1000);
                            int ConnectionId = NewCallGenCLI.LstChanels[i].sip.phone.StartCall2(NewCallGenCLI.LstChanels[i].destinationNumber);
                            NewCallGenCLI.LstChanels[i].ConnectionId = ConnectionId;

                            //WriteToEventLogEx("ConnectionId: " + ConnectionId);
                            //String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                            //NewCallGenCLI.displayList(f, "threadId: " + threadId + " StartCall " + ConnectionId + " Line :" + NewCallGenCLI.LstChanels[i].id + 1 + " SIPCONFIG: " + NewCallGenCLI.LstChanels[i].sip.ConfigId + " ii " + ii);
                        }
                    }

                    //Clear Time out channels
                    ChannelAllocation chanel = NewCallGenCLI.LstChanels[i];

                    if (chanel.startLastCall != DateTime.MinValue)
                    {
                        DateTime dt = chanel.startLastCall;
                        TimeSpan span = new TimeSpan();
                        if (dt != null)
                            span = DateTime.Now - dt;
                        double totalSeconds = span.TotalSeconds;

                        if (chanel.Idle == false && totalSeconds > 120)
                        {
                            //Reset the phone with this Id;
                            if (NewCallGenCLI.LstChanels[i].destinationNumber == "")
                            {
                                NewCallGenCLI.LstChanels[i].EndDate = DateTime.Now;
                                NewCallGenCLI.LstChanels[i].EndCall();
                            }
                            else
                            {
                                GeneratedCall CallGenEnd = GeneratedCallRepository.Load(NewCallGenCLI.LstChanels[i].generatedCallid);
                                if (CallGenEnd != null)
                                {
                                    CallGenEnd.Status = "-1";
                                    CallGenEnd.EndDate = DateTime.Now;
                                    bool tr = GeneratedCallRepository.Save(CallGenEnd);
                                }
                                //else
                                //WriteToEventLog("CallCenter=> the entry is null");

                                NewCallGenCLI.LstChanels[i].EndDate = DateTime.Now;
                                NewCallGenCLI.LstChanels[i].EndCall();
                            }

                            //Reset the phone with this Id and the Object
                            WriteToEventLogEx("Clear TimeOut: " + NewCallGenCLI.LstChanels[i].ConnectionId);
                            NewCallGenCLI.LstChanels[i].Idle = true;
                            NewCallGenCLI.LstChanels[i].DestinationNumber = "";
                            NewCallGenCLI.LstChanels[i].sip = null;
                            NewCallGenCLI.LstChanels[i].startLastCall = DateTime.MinValue;
                            NewCallGenCLI.LstChanels[i].generatedCallid = 0;
                            NewCallGenCLI.LstChanels[i].GeneratedCallid = 0;
                            NewCallGenCLI.LstChanels[i].ConnectionId = 0;
                            //NewCallGenCLI.displayList(f, "Clear TimeOut");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("ChannelAllocation: " + ex.ToString());
                Logger.LogException(ex);
            }
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
                Logger.LogException(ex);

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
                ChannelAllocation service = NewCallGenCLI.LstChanels[lineId - 1];
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
                    if (NewCallGenCLI.LstChanels[i].ConnectionId == ConnectionId)
                    {
                        ChannelAllocation service = NewCallGenCLI.LstChanels[i];
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

        private static void WriteToEventLogEx(string message)
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
