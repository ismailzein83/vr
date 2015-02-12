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

namespace AndroidApplicationService
{
    public class RequestForCalls
    {
        public static bool locked = false;
        public static int OperatorId = 0;
        private static readonly object _syncRoot = new object();

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
                            TestOperator t = TestOperatorRepository.Load(op.Id);

                            DateTime dt = t.CreationDate.Value;
                            TimeSpan span = new TimeSpan();
                            if (dt != null)
                                span = DateTime.Now - dt;
                            double totalSeconds = span.TotalSeconds;

                            int exptime = 0;
                            int.TryParse(ConfigurationManager.AppSettings["LineAvailable"], out exptime);

                            if (totalSeconds >= exptime)
                            {
                                t.ErrorMessage = exptime + " sec - No Line Availables";
                                t.EndDate = DateTime.Now;
                                TestOperatorRepository.Save(t);
                            }
                            else
                            {
                                t.Requested = null;
                                TestOperatorRepository.Save(t);
                            }
                        }
                        else
                        {
                            if (resp.mobileNumber != "")
                            {
                                GeneratedCall GenCall = new GeneratedCall();
                                GenCall.Number = op.CarrierPrefix + resp.mobileNumber + "@" + op.User.IpSwitch;
                                //GenCall.Number = resp.mobileNumber;
                                //SipAccount sp = SipAccountRepository.LoadbyUser(op.UserId.Value);
                                SipAccount sp = SipAccountRepository.LoadbyUser(op.UserId.Value);
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
    }
}
