using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Timers;

using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;

namespace WcfServiceTestMonty
{
     public class MontyOperator
    {
        public int OperatorId { get; set; }
        public string MSISDN { get; set; }
        public string mcc { get; set; }
        public string mnc { get; set; }
        public string operator_name { get; set; }
        public string country_name { get; set; }
        public string Name { get; set; }
    }

         

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public static List<MontyOperator> lstOperators = new List<MontyOperator>();


        System.Timers.Timer timer = new System.Timers.Timer();

        // Create a timer with a two second interval.
        System.Timers.Timer aTimer = new System.Timers.Timer(180000);

        string filename = "D:\\" + DateTime.Now.ToString("ddMMyyyyhhmm") + ".txt";

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //string s = "The Elapsed event was raised at" + e.SignalTime;
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\" + DateTime.Now.ToString("ddMMyyyyhhmm") + ".txt", true))
            //    file.WriteLine(s);
        }
        private void WriteToEventLog(string message)
        {
            string cs = "Monty Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        public void GetData()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\CallDistributor\\CallDistributorMonty" + DateTime.Now.ToString("ddMMyyyyhhmm") + ".txt", true))
                file.WriteLine("Start the Monty service");
            WriteToEventLog("start distributor Monty");

            timer.Enabled = true;
            timer.Interval = 180000;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;

            int OpId = 0;

            string url = "http://93.89.95.9/mymonty/restApi/api.php?apiKey=b6c551b5a0673a3ca79dfb196827c739&method=getOperatorListCLI";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@url);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            // Get the list of Operators from Monty service => lstOperators /////////////////////////////////////
            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();

                    string result = soapResult.Substring(13, 4);

                    if (result == "TRUE")
                    {
                        string message = soapResult.Substring(30);

                        char[] delimiterChars = { '{', '}' };
                        string[] words = message.Split(delimiterChars);
                        foreach (string s in words)
                        {
                            if (s != "," && s != "")
                            {
                                MontyOperator op = new MontyOperator();
                                char[] delimiterChars2 = { ',' };
                                string[] words2 = s.Split(delimiterChars2);

                                foreach (string s2 in words2)
                                {
                                    char[] delimiterChars3 = { ':' };
                                    string[] words3 = s2.Split(delimiterChars3);
                                    for (int i = 0; i < words3.Length; i++)
                                    {
                                        if (words3[i] == "\"MSISDN\"")
                                            op.MSISDN = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

                                        if (words3[i] == "\"mcc\"")
                                            op.mcc = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

                                        if (words3[i] == "\"mnc\"")
                                            op.mnc = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

                                        if (words3[i] == "\"operator_name\"")
                                            op.operator_name = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

                                        if (words3[i] == "\"country_name\"")
                                            op.country_name = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

                                        i++;
                                    }
                                }
                                if (op.country_name != null)
                                {
                                    OpId++;
                                    op.OperatorId = OpId;
                                    op.Name = op.operator_name + " - " + op.country_name;
                                    lstOperators.Add(op);
                                }
                            }
                        }
                    }
                    else
                    {
                        int len = soapResult.Length;
                        string msg = soapResult.Substring(31);
                        msg = msg.Substring(0, msg.Length - 3);
                    }
                }
            }
            /////////////////////////////////////////////////////////////////////////////


            List<TestOperator> testoperators = TestOperatorRepository.GetMontyTestOperators();
            //messages.Add("testoperators Found: " + testoperators.Count);

            foreach (TestOperator op in testoperators)
            {
                //messages.Add("OperatorId Found: " + op.OperatorId);
                string MSISDN = "";
                string RequestID = "";
                List<CallEntry> LstEntries = new List<CallEntry>();

                foreach (MontyOperator MonOperator in lstOperators)
                {
                    int mcc = 0;
                    int.TryParse(MonOperator.mcc, out mcc);

                    int mnc = 0;
                    int.TryParse(MonOperator.mnc, out mnc);

                    if ((op.Operator.mcc == MonOperator.mcc) && (op.Operator.mnc == MonOperator.mnc))
                    {
                        MSISDN = MonOperator.MSISDN;
                        break;
                    }
                }
                MontyCall montycall = new MontyCall();
                montycall.TestOperatorId = op.Id;
                montycall.MSISDN = MSISDN;
                montycall.CreationDate = DateTime.Now;
                MontyCallRepository.Save(montycall);

                WriteToEventLog("montycall1: " + montycall.Id);
                string msg = "";
                //Get the request ID from Monty
                string url2 = "http://93.89.95.9/mymonty/restApi/api.php?apiKey=b6c551b5a0673a3ca79dfb196827c739&method=testCLIRequest&mcc=" + op.Operator.mcc + "&mnc=" + op.Operator.mnc + "&testCLI=0096170713298";
                HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create(@url2);
                webRequest2.Headers.Add(@"SOAP:Action");
                webRequest2.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest2.Accept = "text/xml";
                webRequest2.Method = "POST";
                CallEntry entry = new CallEntry();
                using (WebResponse response = webRequest2.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();

                        string result = soapResult.Substring(13, 4);
                        
                        
                        if (result == "TRUE")
                        {
                            string message = soapResult.Substring(30);

                            char[] delimiterChars = { '{', '}' };
                            string[] words = message.Split(delimiterChars);
                            foreach (string s in words)
                            {
                                if (s != "," && s != "")
                                {
                                    char[] delimiterChars2 = { ',' };
                                    string[] words2 = s.Split(delimiterChars2);

                                    foreach (string s2 in words2)
                                    {
                                        char[] delimiterChars3 = { ':' };
                                        string[] words3 = s2.Split(delimiterChars3);
                                        for (int i = 0; i < words3.Length; i++)
                                        {
                                            if (words3[i] == "\"RequestID\"")
                                                RequestID = words3[i + 1];
                                            i++;
                                        }
                                    }
                                }
                            }

                            
                            entry.Number = MSISDN;
                            //entry.RequestId = RequestID;
                            entry.StatusId = 1;
                            LstEntries.Add(entry);
                        }
                        else
                        {
                            int len = soapResult.Length;
                            msg = soapResult.Substring(31);
                            msg = msg.Substring(0, msg.Length - 3);
                            //((Master)this.Master).WriteError(msg);
                            //break;
                        }
                        WriteToEventLog("montycall2: " + montycall.Id);
                        MontyCall m = MontyCallRepository.Load(montycall.Id);
                        m.RequestId = RequestID;
                        m.ReturnMessage = msg;
                        WriteToEventLog("m: " + m.Id);
                        WriteToEventLog("RequestID: " + RequestID);
                        WriteToEventLog("msg: " + msg);
                        bool tr = MontyCallRepository.Save(m);
                        WriteToEventLog("Monty save: " + tr.ToString());
                    }
                }
                WriteToEventLog("Call session start");
                CallEntry en = new CallEntry();
                en.Number = "101";
                //entry.RequestId = RequestID;
                en.StatusId = 1;
                LstEntries.Add(entry);

                string sessionId = CallSessionRepository.AddNumbers(LstEntries, "CallerIDD0");
                WriteToEventLog("sessionId " + sessionId);
                bool h = false;

                //Wait the call generator service to do the call
                while (h == false && msg != "")
                {
                    //Check the call entries
                    if(CallSessionRepository.GetCallSession(sessionId).EndDate != null)
                    {
                        h = true;
                    }
                }
            }
        }
    }
      
    
}
