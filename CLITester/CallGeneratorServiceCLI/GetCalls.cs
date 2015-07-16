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

namespace CallGeneratorServiceCLI
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
                    //End any generated call having duration more then 150 seconds
                    GeneratedCallRepository.EndExpiredGeneratedCall();

                    SipAccount sipAccount = SipAccountRepository.GetTop();
                    
                    //List<User> lstUsers = UserRepository.GetUsers();
                    //foreach (User user in lstUsers)
                    //{
                        if (sipAccount.IsChangedCallerId == true)
                        {
                            WriteToEventLogEx("IsChangedCallerId: ");
                            bool finish = false;
                            bool existCall = false;
                            //Search for idle client
                            while (finish == false)
                            {
                                for (int i = 0; i < 64; i++)
                                {
                                    ChannelAllocation channel = NewCallGenCLI.LstChanels[i];
                                    if (channel.idle == false)
                                    {
                                        existCall = true;
                                        break;
                                    }
                                }

                                if (existCall == false)
                                {
                                    // apply configuration, no call exists
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.CallerId = sipAccount.DisplayName;
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.RegDomain = "91.236.236.53";
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.RegUser = sipAccount.DisplayName;
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.RegPass = sipAccount.DisplayName;
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.RegAuthId = sipAccount.DisplayName;

                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.ApplyConfig();
                                    System.Threading.Thread.Sleep(1000);
                                    finish = true;
                                    WriteToEventLogEx("finish: ");
                                }
                            }
                            sipAccount.IsChangedCallerId = false;
                            SipAccountRepository.Save(sipAccount);
                        }
                    //} 

                    GeneratedCall generatedCall = GeneratedCallRepository.GetTopGeneratedCall(sipAccount.Id);
                    if (generatedCall != null)
                    {
                        WriteToEventLogEx("generatedCallID: " + generatedCall.Id);
                        bool clientFound = false;
                        int clientId = -1;
                        //Search for idle client
                        for (int i = 0; i < 64; i++)
                        {
                            ChannelAllocation c = NewCallGenCLI.LstChanels[i];
                            if (c.idle == true)
                            {
                                clientFound = true;
                                clientId = i;
                                String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                                break;
                            }
                        }

                        if (clientFound == true)
                        {
                            NewCallGenCLI.LstChanels[clientId].GeneratedCallid = generatedCall.Id;
                            NewCallGenCLI.LstChanels[clientId].generatedCallid = generatedCall.Id;
                            NewCallGenCLI.LstChanels[clientId].destinationNumber = generatedCall.Number;
                            NewCallGenCLI.LstChanels[clientId].idle = false;
                            String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                        }
                        else
                        {
                            //reset the call to status null in database; because we don't find any Channel free,
                            //so we reset the database to re get the call
                            generatedCall.StartDate = null;
                            generatedCall.EndDate = null;
                            generatedCall.StartCall = null;
                            generatedCall.Status = null;
                            generatedCall.ResponseCode = null;
                            GeneratedCallRepository.Save(generatedCall);
                        }
                    }
                    locked = false;

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("GetCall: " + ex.ToString());
                Logger.LogException(ex);
            }
        }

        private void WriteToEventLogEx(string message)
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
