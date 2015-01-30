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
            while (locked != true)
            {
                locked = true;
                GeneratedCall GenCall = GeneratedCallRepository.GetTopGeneratedCall();
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
                        foreach (SIP sip in Form2.LstSip)
                        {
                            if (GenCall.SipAccount.Id == sip.SipId)
                            {
                                Form2.LstChanels[ClientId].sip = new SIP();
                                Form2.LstChanels[ClientId].sip.ConfigId = sip.ConfigId;
                                Form2.LstChanels[ClientId].sip.SipId = sip.SipId;
                                Form2.LstChanels[ClientId].sip.phone = sip.phone;
                            }
                        }

                        Form2.LstChanels[ClientId].generatedCallid = GenCall.Id;
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
            }
        }
    }
}
