using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Net;
using System.IO;

namespace AndroidApplicationService
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

    public class RequestForCalls
    {
        public static bool locked = false;
        public static int OperatorId = 0;
        private static readonly object _syncRoot = new object();
        public static List<MontyOperator> lstOperators = new List<MontyOperator>();

        public void Start()
        {
            locked = false;
            Thread thread = new Thread(new ThreadStart(RequestForCall));
            thread.IsBackground = true;
            thread.Start();
        }

        private void RequestForCall()
        {
            lock (_syncRoot)
            while (locked != true)
            {
                locked = true;

                ///
                string RequestId = "";
                try
                {
                    CallGeneratorLibrary.Utilities.ScheduleManager.CLISchedule();

                    List<TestOperator> testoperators = TestOperatorRepository.GetMontyTestOperators();
                    foreach (TestOperator op in testoperators)
                    {
                        if (op.Operator.ServiceAndroid == true)
                        {
                            OperatorId = op.Id;
                            //Call the Android service to get the Caller ID
                            AndroidLiveService.ServiceAuthHeader auth = new AndroidLiveService.ServiceAuthHeader();
                            AndroidLiveService.clireporterSoapClient cc = new AndroidLiveService.clireporterSoapClient();
                            AndroidLiveService.NumberID resp = new AndroidLiveService.NumberID();
                            auth.Username = "user";
                            auth.Password = "idsP@ssw0rdids";
                            string dd = DateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month + "-" + DateTime.Now.Date.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                            resp = cc.RequestForCall(auth, "user", "idsP@ssw0rdids", op.Operator.mcc, op.Operator.mnc, dd);
                            //System.Threading.Thread.Sleep(5000);
                            //WriteToEventLog("Response: Callid " + resp.Callid.ToString() + " MobileNumber: " + resp.mobileNumber.ToString() + " Error: " + resp.ErrorStatus.ToString());

                            if (resp.ErrorStatus == "1")
                            {
                                //Check Monty service
                                //if (op.Operator.ServiceMonty == true)
                                //{
                                //    GetNumber(op);
                                //}

                                TestOperator t = TestOperatorRepository.Load(op.Id);

                                //DateTime dt = t.CreationDate.Value;
                                //TimeSpan span = new TimeSpan();
                                //if (dt != null)
                                //    span = DateTime.Now - dt;
                                //double totalSeconds = span.TotalSeconds;

                                //int exptime = 0;
                                //int.TryParse(ConfigurationManager.AppSettings["LineAvailable"], out exptime);

                                //if (totalSeconds >= exptime)
                                //{
                                    //t.ErrorMessage = exptime + " sec - No Line Availables";
                                    t.ErrorMessage = "No Line Availables";
                                    t.EndDate = DateTime.Now;
                                    TestOperatorRepository.Save(t);
                                //}
                                //else
                                //{
                                //    t.Requested = null;
                                //    TestOperatorRepository.Save(t);
                                //}
                            }
                            else
                            {
                                if (resp.mobileNumber != "")
                                {
                                    GeneratedCall GenCall = new GeneratedCall();
                                    GenCall.Number = op.CarrierPrefix + resp.mobileNumber + "@" + op.User.IpSwitch;
                                    //GenCall.Number = resp.mobileNumber;
                                    //SipAccount sp = SipAccountRepository.LoadbyUser(op.UserId.Value);
                                    User usParent = UserRepository.Load(op.UserId.Value);
                                    SipAccount sp = new SipAccount();
                                    if(usParent.ParentId == null)
                                        sp = SipAccountRepository.LoadbyUser(op.UserId.Value);
                                    else
                                        sp = SipAccountRepository.LoadbyUser(usParent.ParentId.Value);
                                    GenCall.SipAccountId = sp.Id;
                                    GeneratedCallRepository.Save(GenCall);
                                    //WriteToEventLog("GeneratedCall: " + GenCall.Id);

                                    GeneratedCall NewGen = GeneratedCallRepository.Load(GenCall.Id);

                                    //Save The MSISDN and the Request ID into MontyCall table
                                    MontyCall montycall = new MontyCall();
                                    montycall.TestOperatorId = op.Id;
                                    montycall.MSISDN = op.CarrierPrefix + resp.mobileNumber;
                                    montycall.CreationDate = DateTime.Now;
                                    montycall.RequestId = resp.Callid;
                                    RequestId = resp.Callid;
                                    montycall.ReturnMessage = "";
                                    montycall.CallEntryId = NewGen.Id;
                                    montycall.ServiceId = (int)CallGeneratorLibrary.Utilities.Enums.Service.AndroidDevice;
                                    //WriteToEventLog("Monty Table: Id: " + op.Id + " CarrierPrefix: " + op.CarrierPrefix + " mobileNumber: " + resp.mobileNumber + " RequestId " + resp.Callid);
                                    MontyCallRepository.Save(montycall);
                                }
                                else
                                {
                                    TestOperator t = TestOperatorRepository.Load(OperatorId);
                                    if (t.EndDate == null)
                                    {
                                        t.ErrorMessage = "Failed while Processing - No number";
                                        t.EndDate = DateTime.Now;
                                        TestOperatorRepository.Save(t);
                                    }
                                }
                            }
                        }
                        //else if (op.Operator.ServiceMonty == true)
                        //{
                        //    GetNumber(op);
                        //}
                    }
                }
                catch (System.Exception ex)
                {
                    WriteToEventLog(ex.ToString());
                    Logger.LogException(ex);
                    TestOperator t = TestOperatorRepository.Load(OperatorId);
                    if (t.EndDate == null)
                    {
                        t.ErrorMessage = "Failed while Processing";
                        t.EndDate = DateTime.Now;
                        TestOperatorRepository.Save(t);
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                locked = false;

                System.Threading.Thread.Sleep(1000);
            }
        }

        private void WriteToEventLog(string message)
        {
            string cs = "Android Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        //private void GetNumber(TestOperator tstOp)
        //{
        //    string url = "http://93.89.95.9/mymonty/restApi/api.php?apiKey=b6c551b5a0673a3ca79dfb196827c739&method=getOperatorListCLI";
        //    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@url);
        //    webRequest.Headers.Add(@"SOAP:Action");
        //    webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        //    webRequest.Accept = "text/xml";
        //    webRequest.Method = "POST";
        //    int OpId = 0;
            
        //    // Get the list of Operators from Monty service => lstOperators /////////////////////////////////////
        //    using (WebResponse response = webRequest.GetResponse())
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
        //                        MontyOperator op = new MontyOperator();
        //                        char[] delimiterChars2 = { ',' };
        //                        string[] words2 = s.Split(delimiterChars2);

        //                        foreach (string s2 in words2)
        //                        {
        //                            char[] delimiterChars3 = { ':' };
        //                            string[] words3 = s2.Split(delimiterChars3);
        //                            for (int i = 0; i < words3.Length; i++)
        //                            {
        //                                if (words3[i] == "\"MSISDN\"")
        //                                    op.MSISDN = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                if (words3[i] == "\"mcc\"")
        //                                    op.mcc = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                if (words3[i] == "\"mnc\"")
        //                                    op.mnc = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                if (words3[i] == "\"operator_name\"")
        //                                    op.operator_name = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                if (words3[i] == "\"country_name\"")
        //                                    op.country_name = words3[i + 1].Substring(1, words3[i + 1].Length - 2);

        //                                i++;
        //                            }
        //                        }
        //                        if (op.country_name != null)
        //                        {
        //                            OpId++;
        //                            op.OperatorId = OpId;
        //                            op.Name = op.operator_name + " - " + op.country_name;
        //                            lstOperators.Add(op);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                int len = soapResult.Length;
        //                string msg = soapResult.Substring(31);
        //                msg = msg.Substring(0, msg.Length - 3);
        //            }
        //        }
        //    }
        //    /////////////////////////////////////////////////////////////////////////////

        //    string MSISDN = "";
        //    string RequestID = "";
            
        //    foreach (MontyOperator MonOperator in lstOperators)
        //    {
        //        int mcc = 0;
        //        int.TryParse(MonOperator.mcc, out mcc);

        //        int mnc = 0;
        //        int.TryParse(MonOperator.mnc, out mnc);

        //        if ((tstOp.Operator.mcc == MonOperator.mcc) && (tstOp.Operator.mnc == MonOperator.mnc))
        //        {
        //            MSISDN = MonOperator.MSISDN;
        //            break;
        //        }
        //    }

        //    string msg2 = "";
        //    //Get the request ID from Monty
        //    string url2 = "http://93.89.95.9/mymonty/restApi/api.php?apiKey=b6c551b5a0673a3ca79dfb196827c739&method=testCLIRequest&mcc=" + tstOp.Operator.mcc + "&mnc=" + tstOp.Operator.mnc + "&testCLI=0096170713298";
        //    HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create(@url2);
        //    webRequest2.Headers.Add(@"SOAP:Action");
        //    webRequest2.ContentType = "text/xml;charset=\"utf-8\"";
        //    webRequest2.Accept = "text/xml";
        //    webRequest2.Method = "POST";
            
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
        //                                if (words3[i] == "\"RequestID\"")
        //                                    RequestID = words3[i + 1];
        //                                i++;
        //                            }
        //                        }
        //                    }
        //                }

        //                GeneratedCall GenCall = new GeneratedCall();
        //                GenCall.Number = tstOp.CarrierPrefix + MSISDN + "@" + tstOp.User.IpSwitch;
        //                //GenCall.Number = resp.mobileNumber;
        //                //SipAccount sp = SipAccountRepository.LoadbyUser(op.UserId.Value);
        //                SipAccount sp = SipAccountRepository.LoadbyUser(tstOp.UserId.Value);
        //                GenCall.SipAccountId = sp.Id;
        //                GeneratedCallRepository.Save(GenCall);
        //                //WriteToEventLog("GeneratedCall: " + GenCall.Id);

        //                GeneratedCall NewGen = GeneratedCallRepository.Load(GenCall.Id);

        //                //Save The MSISDN and the Request ID into MontyCall table
        //                MontyCall montycall = new MontyCall();
        //                montycall.TestOperatorId = tstOp.Id;
        //                montycall.MSISDN = tstOp.CarrierPrefix + MSISDN;
        //                montycall.CreationDate = DateTime.Now;
        //                montycall.RequestId = RequestID;
        //                montycall.ReturnMessage = "";
        //                montycall.CallEntryId = NewGen.Id;
        //                montycall.ServiceId = (int)CallGeneratorLibrary.Utilities.Enums.Service.Monty;
        //                //WriteToEventLog("Monty Table: Id: " + op.Id + " CarrierPrefix: " + op.CarrierPrefix + " mobileNumber: " + resp.mobileNumber + " RequestId " + resp.Callid);
        //                MontyCallRepository.Save(montycall);
        //            }
        //            else
        //            {
        //                int len = soapResult.Length;
        //                msg2 = soapResult.Substring(31);
        //                msg2 = msg2.Substring(0, msg2.Length - 3);


        //                TestOperator t = TestOperatorRepository.Load(OperatorId);
        //                if (t.EndDate == null)
        //                {
        //                    t.ErrorMessage = "Failed while Processing - No number";
        //                    t.EndDate = DateTime.Now;
        //                    TestOperatorRepository.Save(t);
        //                }

        //            }
        //        }
        //    }



        //}
    }
}
