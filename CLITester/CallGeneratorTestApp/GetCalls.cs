using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CallGeneratorTestApp
{
    public class GetCalls
    {
        public static bool locked = false;
        public static Form2 f = null;

        public void Start(Form2 f)
        {
            locked = false;
            GetCalls.f = f;
            Thread thread = new Thread(new ThreadStart(GetCall));
            thread.IsBackground = true;
            thread.Start();    
        }

        private  void GetCall( )
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
                            bool finish = false;
                            bool ExistCall = false;
                            //Search for idle client
                            while (finish == false)
                            {
                                for (int i = 0; i < 64; i++)
                                {
                                    ChannelAllocation c = Form2.LstChanels[i];
                                    if (c.idle == false)
                                    {
                                        ExistCall = true;
                                        break;
                                    }
                                }

                                if (ExistCall == false)
                                {
                                    // apply configuration, no call exists

                                    Form2.LstSip[0].phone.Config.CallerId = user.CallerId;
                                    Form2.LstSip[0].phone.Config.RegDomain = "91.236.236.53";
                                    Form2.LstSip[0].phone.Config.RegUser = user.CallerId;
                                    Form2.LstSip[0].phone.Config.RegPass = user.CallerId;
                                    Form2.LstSip[0].phone.Config.RegAuthId = user.CallerId;

                                    Form2.LstSip[0].phone.ApplyConfig();
                                    System.Threading.Thread.Sleep(1000);
                                    finish = true;
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
                    string SipAccId = "1";
                    int SipAccountId = 0;
                    int.TryParse(SipAccId, out SipAccountId);




                    GeneratedCall GenCall = GeneratedCallRepository.GetTopGeneratedCall(SipAccountId);
                    if (GenCall != null)
                    {
                        bool ClientFound = false;
                        int ClientId = -1;
                        //Search for idle client
                        for (int i = 0; i < 64; i++)
                        {
                            ChannelAllocation c = Form2.LstChanels[i];
                            if (c.idle == true)
                            {
                                ClientFound = true;
                                ClientId = i;
                                String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                                f.AppendTextBox("threadId: " + threadId + " Using Idle Channel: " + ClientId.ToString());
                                break;
                            }
                        }

                        if (ClientFound == true)
                        {
                            //foreach (SIP sip in Form2.LstSip)
                            //{
                            //    if (GenCall.SipAccount.Id == sip.SipId)
                            //    {
                            //        Form2.LstChanels[ClientId].sip = new SIP();
                            //        Form2.LstChanels[ClientId].sip.ConfigId = sip.ConfigId;
                            //        Form2.LstChanels[ClientId].sip.SipId = sip.SipId;
                            //        Form2.LstChanels[ClientId].sip.phone = sip.phone;
                            //    }
                            //}

                            Form2.LstChanels[ClientId].generatedCallid = GenCall.Id;
                            Form2.LstChanels[ClientId].GeneratedCallid = GenCall.Id;

                            Form2.LstChanels[ClientId].destinationNumber = GenCall.Number;
                            Form2.LstChanels[ClientId].idle = false;
                            String threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();

                            Form2.displayList(f, "threadId: " + threadId + " ClientFound and added to channel " + ClientId);
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
                Logger.LogException(ex);
            }
        }
    }
}
