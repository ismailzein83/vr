using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Timers;
using System.Threading;
using System.Configuration;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;
using System.Windows;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace WCFServiceMonty
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
     
    public class CallDistributor
    {
        #region Definitions
        public static List<MontyOperator> lstOperators = new List<MontyOperator>();

        public bool finishCall = false;

        System.Timers.Timer timer = new System.Timers.Timer();

        // Create a timer with a two second interval.
        System.Timers.Timer aTimer = new System.Timers.Timer(5000);

        string filename = "D:\\" + DateTime.Now.ToString("ddMMyyyyhhmm") + ".txt";
        #endregion



        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (finishCall == true)
            {
                WriteToEventLog("OnTimedEvent + finishCall: " + finishCall);
                finishCall = false;
           //     Start();
            }
        }

  
        //public void Start()
        //{
        //    timer.Start();
        //    finishCall = false;
        //    WriteToEventLog("start Monty  service + finishcall: " + finishCall);

        //    timer.Enabled = true;
        //    timer.Interval = 5000;

        //    // Hook up the Elapsed event for the timer. 
        //    aTimer.Elapsed += OnTimedEvent;
        //    aTimer.Enabled = true;

        //    int OpId = 0;
        //    try
        //    {
        //        List<TestOperator> testoperators = TestOperatorRepository.GetMontyTestOperators();
        //        WriteToEventLog("testoperators.Count: " + testoperators.Count());
        //        //messages.Add("testoperators Found: " + testoperators.Count);
        //        if (testoperators.Count > 0)
        //        {
        //            string url = "http://93.89.95.9/mymonty/restApi/api.php?apiKey=b6c551b5a0673a3ca79dfb196827c739&method=getOperatorListCLI";
        //            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@url);
        //            webRequest.Headers.Add(@"SOAP:Action");
        //            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        //            webRequest.Accept = "text/xml";
        //            webRequest.Method = "POST";

        //            // Get the list of Operators from Monty service => lstOperators /////////////////////////////////////
        //            using (WebResponse response = webRequest.GetResponse())
        //            {
        //                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        //                {
        //                    string soapResult = rd.ReadToEnd();

        //                    string result = soapResult.Substring(13, 4);

        //                    if (result == "TRUE")
        //                    {
        //                        string message = soapResult.Substring(30);

        //                        char[] delimiterChars = { '{', '}' };
        //                        string[] words = message.Split(delimiterChars);
        //                        foreach (string s in words)
        //                        {
        //                            if (s != "," && s != "")
        //                            {
        //                                MontyOperator op = new MontyOperator();
        //                                char[] delimiterChars2 = { ',' };
        //                                string[] words2 = s.Split(delimiterChars2);

        //                                foreach (string s2 in words2)
        //                                {
        //                                    char[] delimiterChars3 = { ':' };
        //                                    string[] words3 = s2.Split(delimiterChars3);
        //                                    for (int i = 0; i < words3.Length; i++)
        //                                    {
        //                                        if (words3[i] == "\"MSISDN\"")
        //                                            op.MSISDN = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                        if (words3[i] == "\"mcc\"")
        //                                            op.mcc = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                        if (words3[i] == "\"mnc\"")
        //                                            op.mnc = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                        if (words3[i] == "\"operator_name\"")
        //                                            op.operator_name = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                        if (words3[i] == "\"country_name\"")
        //                                            op.country_name = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                        i++;
        //                                    }
        //                                }
        //                                if (op.country_name != null)
        //                                {
        //                                    OpId++;
        //                                    op.OperatorId = OpId;
        //                                    op.Name = op.operator_name + " - " + op.country_name;
        //                                    lstOperators.Add(op);
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        int len = soapResult.Length;
        //                        string msg = soapResult.Substring(31);
        //                        msg = msg.Substring(0, msg.Length - 3);
        //                    }
        //                }
        //            }
        //            /////////////////////////////////////////////////////////////////////////////
        //        }

        //        foreach (TestOperator op in testoperators)
        //        {
        //            //messages.Add("OperatorId Found: " + op.OperatorId);
        //            string MSISDN = "";
        //            string RequestID = "";
        //            List<CallEntry> LstEntries = new List<CallEntry>();

        //            foreach (MontyOperator MonOperator in lstOperators)
        //            {
        //                int mcc = 0;
        //                int.TryParse(MonOperator.mcc, out mcc);

        //                int mnc = 0;
        //                int.TryParse(MonOperator.mnc, out mnc);

        //                if ((op.Operator.mcc == MonOperator.mcc) && (op.Operator.mnc == MonOperator.mnc))
        //                {
        //                    MSISDN = MonOperator.MSISDN;
        //                    break;
        //                }
        //            }
        //            //Save The MSISDN into MontyCall table
        //            MontyCall montycall = new MontyCall();
        //            montycall.TestOperatorId = op.Id;
        //            //montycall.MSISDN = op.CarrierPrefix + MSISDN;
        //            montycall.MSISDN = "00" + MSISDN;
        //            montycall.CreationDate = DateTime.Now;
        //            MontyCallRepository.Save(montycall);

        //            string msg = "";

        //            //Get the request ID from Monty
        //            string url2 = "http://93.89.95.9/mymonty/restApi/api.php?apiKey=b6c551b5a0673a3ca79dfb196827c739&method=testCLIRequest&mcc=" + op.Operator.mcc + "&mnc=" + op.Operator.mnc + "&testCLI=" + op.CallerId;
        //            WriteToEventLog(url2);
        //            HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create(@url2);
        //            webRequest2.Headers.Add(@"SOAP:Action");
        //            webRequest2.ContentType = "text/xml;charset=\"utf-8\"";
        //            webRequest2.Accept = "text/xml";
        //            webRequest2.Method = "POST";
        //            CallEntry entry = new CallEntry();
        //            using (WebResponse response = webRequest2.GetResponse())
        //            {
        //                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        //                {
        //                    string soapResult = rd.ReadToEnd();

        //                    string result = soapResult.Substring(13, 4);
        //                    if (result == "TRUE")
        //                    {
        //                        string message = soapResult.Substring(30);

        //                        char[] delimiterChars = { '{', '}' };
        //                        string[] words = message.Split(delimiterChars);
        //                        foreach (string s in words)
        //                        {
        //                            if (s != "," && s != "")
        //                            {
        //                                char[] delimiterChars2 = { ',' };
        //                                string[] words2 = s.Split(delimiterChars2);

        //                                foreach (string s2 in words2)
        //                                {
        //                                    char[] delimiterChars3 = { ':' };
        //                                    string[] words3 = s2.Split(delimiterChars3);
        //                                    for (int i = 0; i < words3.Length; i++)
        //                                    {
        //                                        if (words3[i] == "\"RequestID\"")
        //                                            RequestID = words3[i + 1];
        //                                        i++;
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        //Save the entry call into CallGenerator
        //                        //entry.Number = op.CarrierPrefix + MSISDN;
        //                        entry.Number = "00" + MSISDN;
        //                        //entry.RequestId = RequestID;
        //                        entry.StatusId = 1;
        //                        LstEntries.Add(entry);
        //                    }
        //                    else
        //                    {
        //                        int len = soapResult.Length;
        //                        msg = soapResult.Substring(31);
        //                        msg = msg.Substring(0, msg.Length - 3);
        //                        //((Master)this.Master).WriteError(msg);
        //                        //break;
        //                    }

        //                    //Update the MontyCall record by the RequestId or the error message "msg" returned from Monty Service
        //                    MontyCall m = MontyCallRepository.Load(montycall.Id);
        //                    m.RequestId = RequestID;
        //                    m.ReturnMessage = msg;
        //                    MontyCallRepository.Save(m);
        //                }
        //            }

        //            if (msg == "")
        //            {
        //                string sessionId = CallSessionRepository.AddNumbers(LstEntries, op.CallerId);

        //                bool h = false;

        //                //Wait the call generator service to do the call
        //                while (h == false)
        //                {
        //                    CallSession cs = CallSessionRepository.GetCallSession(sessionId);
        //                    if ((DateTime.Now.Minute - cs.StartDate.Value.Minute) > 3)
        //                    {
        //                        cs.EndDate = DateTime.Now;
        //                    }

        //                    //Check the call entries
        //                    if (cs.EndDate != null)
        //                    {
        //                        MontyCall m = MontyCallRepository.Load(montycall.Id);
        //                        m.CallDate = DateTime.Now;
        //                        m.CallEntryId = CallEntryRepository.GetCallEntryBySession(sessionId).FirstOrDefault().Id;
        //                        MontyCallRepository.Save(m);

        //                        TestOperator t = TestOperatorRepository.Load(m.TestOperatorId.Value);
        //                        t.EndDate = DateTime.Now;

        //                        string TestCli = null;
        //                        string ReceivedCli = null;
        //                        string Status = null;
        //                        string ErrorMessage = null;
        //                        GetResult(RequestID, out TestCli, out ReceivedCli, out Status, out ErrorMessage);
                               
        //                        //using (System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\" + DateTime.Now.ToString("ddMMyyyyhhmm") + ".txt", true))
        //                        //    file.WriteLine(Status);

        //                        if (ErrorMessage == null)
        //                        {
        //                            t.TestCli = TestCli;
        //                            t.ReceivedCli = ReceivedCli;
        //                            if (Status == "CLI-NOTVALID")
        //                                t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
        //                            else if (Status == "CLI-VALID")
        //                                t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
        //                            else if (Status =="Expired")
        //                                t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Expired;
        //                            else
        //                                t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Waiting;
        //                        }
        //                        else
        //                        {
        //                            t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.ErrorMessage;
        //                            t.ErrorMessage = ErrorMessage;
        //                        }
        //                        TestOperatorRepository.Save(t);
        //                        h = true;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                MontyCall m = MontyCallRepository.Load(montycall.Id);
        //                m.CallDate = null;
        //                m.CallEntryId = null;
        //                MontyCallRepository.Save(m);

        //                TestOperator t = TestOperatorRepository.Load(m.TestOperatorId.Value);
        //                t.EndDate = DateTime.Now;
        //                TestOperatorRepository.Save(t);
        //            }
        //        }
        //        finishCall = true;
        //        WriteToEventLog("Finish Monty  service + finishcall: " + finishCall);
               
        //    }
        //    catch (System.Exception ex)
        //    {
        //        WriteToEventLog(ex.ToString());
        //    }
        //    timer.Close();
        //}

        //private void GetResult(string RequestId, out string Test_CLI, out string RecievedCLI, out string Test_Status, out string ErrorMessage)
        //{
        //    Test_CLI = null;
        //    RecievedCLI = null;
        //    Test_Status = null;
        //    ErrorMessage = null;

        //    //Get the request ID from Monty
        //    string url2 = "http://93.89.95.9/mymonty/restApi/api.php?apiKey=b6c551b5a0673a3ca79dfb196827c739&method=testCLIResult&RequestID=" + RequestId;
        //    HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create(@url2);
        //    webRequest2.Headers.Add(@"SOAP:Action");
        //    webRequest2.ContentType = "text/xml;charset=\"utf-8\"";
        //    webRequest2.Accept = "text/xml";
        //    webRequest2.Method = "POST";
        //    CallEntry entry = new CallEntry();
        //    using (WebResponse response = webRequest2.GetResponse())
        //    {
        //        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
        //        {
        //            string soapResult = rd.ReadToEnd();

        //            string result = soapResult.Substring(13, 4);
        //            if (result == "TRUE")
        //            {
        //                string message = soapResult.Substring(30);

        //                char[] delimiterChars = { '{', '}' };
        //                string[] words = message.Split(delimiterChars);
        //                foreach (string s in words)
        //                {
        //                    if (s != "," && s != "")
        //                    {
        //                        char[] delimiterChars2 = { ',' };
        //                        string[] words2 = s.Split(delimiterChars2);

        //                        foreach (string s2 in words2)
        //                        {
        //                            char[] delimiterChars3 = { ':' };
        //                            string[] words3 = s2.Split(delimiterChars3);
        //                            for (int i = 0; i < words3.Length; i++)
        //                            {
        //                                if (words3[i] == "\"Test_CLI\"")
        //                                {
        //                                    Test_CLI = words3[i + 1];
        //                                    Test_CLI = Test_CLI.Replace("\"","");
        //                                }

        //                                if (words3[i] == "\"RecievedCLI\"")
        //                                {
        //                                    RecievedCLI = words3[i + 1];
        //                                    RecievedCLI = RecievedCLI.Replace("\"", "");
        //                                }

        //                                if (words3[i] == "\"Test_Status\"")
        //                                {
        //                                    Test_Status = words3[i + 1];
        //                                    Test_Status = Test_Status.Replace("\"", "");
        //                                }

        //                                i++;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                int len = soapResult.Length;
        //                ErrorMessage = soapResult.Substring(31);
        //                ErrorMessage = ErrorMessage.Substring(0, ErrorMessage.Length - 3);
        //            }
        //        }
        //    }
        //}

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
    }
}