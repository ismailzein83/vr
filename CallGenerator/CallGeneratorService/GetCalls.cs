using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using SIPVoipSDK;
using System.Diagnostics;
    
namespace CallGeneratorService
{
    public class GetCalls
    {
        public static bool locked = false;

        public void Start()
        {
            locked = false;

            Thread thread = new Thread(new ThreadStart(GetCall));
            thread.IsBackground = true;
            thread.Start();
        }

        private void GetCall()
        {
            try
            {
                while (locked != true)
                {
                    locked = true;
                    CallGeneratorLibrary.Utilities.ScheduleManager.CLISchedule();

                    GeneratedCall GenCall = GeneratedCallRepository.GetTopGeneratedCall();

                    if (GenCall != null)
                    {
                        bool ClientFound = false;
                        int ClientId = -1;
                        //Search for idle client
                        for (int i = 0; i < 64; i++)
                        {
                            ChannelAllocation c = Service1.LstChanels[i];
                            if (c.idle == true)
                            {
                                ClientFound = true;
                                ClientId = i;
                                String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                                break;
                            }
                        }

                        if (ClientFound == true)
                        {
                            foreach (SIP sip in Service1.LstSip)
                            {
                                if (GenCall.SipAccount.Id == sip.SipId)
                                {
                                    Service1.LstChanels[ClientId].sip = new SIP();
                                    Service1.LstChanels[ClientId].sip.ConfigId = sip.ConfigId;
                                    Service1.LstChanels[ClientId].sip.SipId = sip.SipId;
                                    Service1.LstChanels[ClientId].sip.phone = sip.phone;
                                }
                            }

                            Service1.LstChanels[ClientId].GeneratedCallid = GenCall.Id;
                            Service1.LstChanels[ClientId].generatedCallid = GenCall.Id;
                            Service1.LstChanels[ClientId].DestinationNumber = GenCall.Number;
                            Service1.LstChanels[ClientId].destinationNumber = GenCall.Number;

                            Service1.LstChanels[ClientId].startDate = DateTime.MinValue;
                            Service1.LstChanels[ClientId].StartDate = DateTime.MinValue;
                            Service1.LstChanels[ClientId].ConnectDateTime = DateTime.MinValue;
                            Service1.LstChanels[ClientId].idle = false;
                            Service1.LstChanels[ClientId].Idle = false;

                            //WriteToEventLog("ClientId " + Service1.LstChanels[ClientId].GeneratedCallid + " " + 
                            //    Service1.LstChanels[ClientId].DestinationNumber + " " + Service1.LstChanels[ClientId].Idle + " " + Service1.LstChanels[ClientId].StartDate);

                            String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                        }
                        else
                        {
                            //reset the call to status null in database; because we don't find any Channel free,
                            //so we reset the database to re get the call
                            GenCall.StartDate = null;
                            GenCall.EndDate = null;
                            GenCall.StartCall = null;
                            GenCall.Status = null;
                            GenCall.ResponseCode = null;
                            GeneratedCallRepository.Save(GenCall);
                        }
                    }
                    locked = false;

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLog("EXCEPTION GETCALL: " + ex.ToString());
            }
        }

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
