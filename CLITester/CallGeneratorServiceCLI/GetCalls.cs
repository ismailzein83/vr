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
using System.Configuration;

namespace CallGeneratorServiceCLI
{
    public class GetCalls
    {
        public static bool locked = false;

        public void Start()
        {
            locked = false;
            //GetCalls.f = f;
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
                    List<User> LstUsers = UserRepository.GetUsers();
                    foreach (User user in LstUsers)
                    {
                        if (user.IsChangedCallerId == true)
                        {
                            WriteToEventLogEx("IsChangedCallerId: ");
                            bool finish = false;
                            bool ExistCall = false;
                            //Search for idle client
                            while (finish == false)
                            {
                                for (int i = 0; i < 64; i++)
                                {
                                    ChannelAllocation c = NewCallGenCLI.LstChanels[i];
                                    if (c.idle == false)
                                    {
                                        ExistCall = true;
                                        break;
                                    }
                                }

                                if (ExistCall == false)
                                {
                                    // apply configuration, no call exists
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.CallerId = user.CallerId;
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.RegDomain = "91.236.236.53";
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.RegUser = user.CallerId;
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.RegPass = user.CallerId;
                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.RegAuthId = user.CallerId;

                                    CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.ApplyConfig();
                                    System.Threading.Thread.Sleep(1000);
                                    finish = true;
                                    WriteToEventLogEx("finish: ");
                                }
                            }


                            //CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.Config.CallerId = user.CallerId;
                            //CallGeneratorServiceCLI.NewCallGenCLI.LstSip[0].phone.ApplyConfig();
                            //CallGeneratorServiceCLI.NewCallGenCLI c = new CallGeneratorServiceCLI.NewCallGenCLI();
                            //c.AddnewSIP(user);
                            user.IsChangedCallerId = false;
                            UserRepository.Save(user);
                        }
                    }
                    string SipAccId = ConfigurationManager.AppSettings["SipAccId"];
                    int SipAccountId = 0;
                    int.TryParse(SipAccId, out SipAccountId);
                    GeneratedCall GenCall = GeneratedCallRepository.GetTopGeneratedCall(SipAccountId);
                    if (GenCall != null)
                    {
                        WriteToEventLogEx("GenIDD: " + GenCall.Id);
                        bool ClientFound = false;
                        int ClientId = -1;
                        //Search for idle client
                        for (int i = 0; i < 64; i++)
                        {
                            ChannelAllocation c = NewCallGenCLI.LstChanels[i];
                            if (c.idle == true)
                            {
                                ClientFound = true;
                                ClientId = i;
                                String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                                //f.AppendTextBox("threadId: " + threadId + " Using Idle Channel: " + ClientId.ToString());
                                break;
                            }
                        }

                        if (ClientFound == true)
                        {
                            //foreach (SIP sip in NewCallGenCLI.LstSip)
                            //{
                            //    if (GenCall.SipAccount.Id == sip.SipId)
                            //    {
                            //        NewCallGenCLI.LstChanels[ClientId].sip = new SIP();
                            //        NewCallGenCLI.LstChanels[ClientId].sip.ConfigId = sip.ConfigId;
                            //        NewCallGenCLI.LstChanels[ClientId].sip.SipId = sip.SipId;
                            //        NewCallGenCLI.LstChanels[ClientId].sip.phone = sip.phone;
                            //        WriteToEventLogEx("SIP :: " + sip.SipId.ToString() + " CallerId :: " + sip.phone.Config.CallerId + " ClientId :: " + ClientId);
                            //    }
                            //}

                            NewCallGenCLI.LstChanels[ClientId].GeneratedCallid = GenCall.Id;
                            NewCallGenCLI.LstChanels[ClientId].generatedCallid = GenCall.Id;
                            NewCallGenCLI.LstChanels[ClientId].destinationNumber = GenCall.Number;
                            NewCallGenCLI.LstChanels[ClientId].idle = false;
                            String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();

                            //NewCallGenCLI.displayList(f, "threadId: " + threadId + " ClientFound and added to channel " + ClientId);
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
