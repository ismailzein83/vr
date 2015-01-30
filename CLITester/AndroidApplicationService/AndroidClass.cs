using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using System.Threading.Tasks;
using System.Configuration;

namespace AndroidApplicationService
{
    public class AndroidClass
    {
        #region Definitions
        
        private static readonly object _syncRoot = new object();
   
        //public bool finishCall = false;
        public static int OperatorId = 0;
        // Create a timer with a five seconds interval.
        System.Timers.Timer aTimer = new System.Timers.Timer(1000);

        public RequestForCalls thRequestForCalls = new RequestForCalls();
        public GetCLIs thGetCLIs = new GetCLIs();

        #endregion

        public void Start()
        {
            WriteToEventLog("Start Android Service: " +DateTime.Now);

            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
            aTimer.Interval = 1000;
            aTimer.Start();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //string RequestId = "";
            try
            {
                thRequestForCalls.Start();
                thGetCLIs.Start();

                //lock (_syncRoot)
                //{
                //    CallGeneratorLibrary.Utilities.ScheduleManager.CLISchedule();

                //    List<TestOperator> testoperators = TestOperatorRepository.GetMontyTestOperators();
                //    string Err = null;
                //    foreach (TestOperator op in testoperators)
                //    {
                //        OperatorId = op.Id;
                //        //Call the Android service to get the Caller ID
                //        AndroidLiveService.ServiceAuthHeader auth = new AndroidLiveService.ServiceAuthHeader();
                //        AndroidLiveService.clireporterSoapClient cc = new AndroidLiveService.clireporterSoapClient();
                //        AndroidLiveService.NumberID resp = new AndroidLiveService.NumberID();
                //        auth.Username = "user";
                //        auth.Password = "idsP@ssw0rdids";
                //        string dd = DateTime.Now.Date.Year + "-" + DateTime.Now.Date.Month + "-" + DateTime.Now.Date.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                //        resp = cc.RequestForCall(auth, "user", "idsP@ssw0rdids", op.Operator.mcc, op.Operator.mnc, dd);
                //        //System.Threading.Thread.Sleep(5000);
                //        WriteToEventLog("Response: Callid " + resp.Callid.ToString() + " MobileNumber: " + resp.mobileNumber.ToString() + " Error: " + resp.ErrorStatus.ToString());
                        
                //        if (resp.ErrorStatus == "1")
                //        {
                //            TestOperator t = TestOperatorRepository.Load(op.Id);
                //            t.ErrorMessage = "No Line Availables";
                //            t.EndDate = DateTime.Now;
                //            TestOperatorRepository.Save(t);
                //        }
                //        else
                //        {
                //            if (resp.mobileNumber != "")
                //            {

                //                GeneratedCall GenCall = new GeneratedCall();
                //                GenCall.Number = op.CarrierPrefix + resp.mobileNumber + "@" + op.User.IpSwitch;
                //                //GenCall.Number = resp.mobileNumber;
                //                SipAccount sp = SipAccountRepository.LoadbyUser(op.UserId.Value);
                //                GenCall.SipAccountId = sp.Id;
                //                GeneratedCallRepository.Save(GenCall);
                //                WriteToEventLog("GeneratedCall: " + GenCall.Id);

                //                GeneratedCall NewGen = GeneratedCallRepository.Load(GenCall.Id);

                //                //Save The MSISDN and the Request ID into MontyCall table
                //                MontyCall montycall = new MontyCall();
                //                montycall.TestOperatorId = op.Id;
                //                montycall.MSISDN = op.CarrierPrefix + resp.mobileNumber;
                //                montycall.CreationDate = DateTime.Now;
                //                montycall.RequestId = resp.Callid;
                //                RequestId = resp.Callid;
                //                montycall.ReturnMessage = "";
                //                montycall.CallEntryId = NewGen.Id;
                //                WriteToEventLog("Monty Table: Id: " + op.Id + " CarrierPrefix: " + op.CarrierPrefix + " mobileNumber: " + resp.mobileNumber + " RequestId " + resp.Callid);
                //                MontyCallRepository.Save(montycall);

                //                bool h = false;
                //                ///
                //                ///Wait the call generator service to do the call
                //                ///
                //                while (h == false)
                //                {
                //                    GeneratedCall GenEnd = GeneratedCallRepository.Load(GenCall.Id);
                //                    MontyCall m = MontyCallRepository.Load(montycall.Id);
                //                    TestOperator t = TestOperatorRepository.Load(m.TestOperatorId.Value);

                //                    //Expiry minutes
                //                    int exptime = 0;
                //                    int.TryParse(ConfigurationManager.AppSettings["ExpiryTime"], out exptime);

                //                    if (GenEnd.StartDate.HasValue)
                //                    {
                //                        if (exptime != 0 && (DateTime.Now.Second - GenEnd.StartDate.Value.Second) > exptime)
                //                        {
                //                            Err = "Expired - more then " + exptime.ToString() + " seconds";
                //                            GenEnd.EndDate = DateTime.Now;
                //                            h = true;

                //                            t.ErrorMessage = Err;
                //                            t.EndDate = DateTime.Now;
                //                            TestOperatorRepository.Save(t);
                //                        }
                //                    }

                //                    //Check the call entries
                //                    if (GenEnd.EndDate != null && h == false)
                //                    {
                //                        // Check if the Call is failed // don't check the result from Android Service
                //                        if ((GenEnd.ResponseCode == "408") || (GenEnd.ResponseCode == "480") || (GenEnd.ResponseCode == "503") || (GenEnd.ResponseCode == "487"))
                //                        {
                //                            t.TestCli = GenEnd.SipAccount.User.CallerId;
                //                            if (GenEnd.ResponseCode == "408")
                //                                t.ErrorMessage = "408 - Request Timeout";
                //                            if (GenEnd.ResponseCode == "480")
                //                                t.ErrorMessage = "480 - Temporarily Unavailable";
                //                            //if (GenEnd.ResponseCode == "503")
                //                            //    t.ErrorMessage = "503 - Service Unavailable";
                //                            if (GenEnd.ResponseCode == "487")
                //                                t.ErrorMessage = "487 - Request Terminated";
                //                            t.EndDate = DateTime.Now;
                //                            t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Failed;
                //                            TestOperatorRepository.Save(t);
                //                            h = true;
                //                        }
                //                        else
                //                        {
                //                            ///Get Result
                //                            ///
                //                            AndroidLiveService.ServiceAuthHeader auth2 = new AndroidLiveService.ServiceAuthHeader();
                //                            AndroidLiveService.clireporterSoapClient Req = new AndroidLiveService.clireporterSoapClient();
                //                            AndroidLiveService.Call resp2 = new AndroidLiveService.Call();
                //                            resp2 = Req.GetCLI(auth, "user", "idsP@ssw0rdids", RequestId);

                //                            WriteToEventLog("Response status " + resp2.status);

                //                            if (resp2.status == "1")//Success
                //                            {
                //                                t.EndDate = DateTime.Now;
                //                                t.TestCli = GenEnd.SipAccount.User.CallerId;
                //                                t.ReceivedCli = resp2.CLI;
                //                                if (t.TestCli == t.ReceivedCli)
                //                                    t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                //                                else
                //                                    t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                //                                TestOperatorRepository.Save(t);
                //                                h = true;
                //                            }
                //                            else if (resp2.status == "-1")//Expired
                //                            {
                //                                t.TestCli = GenEnd.SipAccount.User.CallerId;
                //                                t.ErrorMessage = "Expired";
                //                                t.EndDate = DateTime.Now;
                //                                t.Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Expired;
                //                                TestOperatorRepository.Save(t);
                //                                h = true;
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
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
    }
}
