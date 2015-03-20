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
    public class GetCLIs
    {
        public static bool locked = false;
        public static int OperatorId = 0;
        private static readonly object _syncRoot = new object();

        public void Start()
        {
            locked = false;
            Thread thread = new Thread(new ThreadStart(GetCLI));
            thread.IsBackground = true;
            thread.Start();
        }

        private void GetCLI()
        {
            AndroidLiveService.ServiceAuthHeader auth = new AndroidLiveService.ServiceAuthHeader();
            auth.Username = "user";
            auth.Password = "idsP@ssw0rdids";
            string Err = null;

            lock (_syncRoot)
            while (locked != true)
            {
                locked = true;

                //Expiry minutes
                int exptime = 0;
                int.TryParse(ConfigurationManager.AppSettings["ExpiryTime"], out exptime);

                try
                {
                    List<TestOperator> testoperators = TestOperatorRepository.GetRequestedTestOperators();
                    for (int i = 0; i < testoperators.Count(); i++)
                    {
                        OperatorId = testoperators[i].Id;
                        MontyCall m = MontyCallRepository.LoadbyTestOperatorId(testoperators[i].Id);
                        if (m != null)
                        {
                            GeneratedCall GenEnd = GeneratedCallRepository.Load(m.CallEntryId.Value);
                            if (GenEnd != null)
                            {
                                if (GenEnd.StartDate.HasValue)
                                {
                                    if (exptime != 0 && (DateTime.Now.Second - GenEnd.StartDate.Value.Second) > exptime)
                                    {
                                        Err = "Expired - No Line Availables";
                                        GenEnd.EndDate = DateTime.Now;
                                        
                                        testoperators[i].ErrorMessage = Err;
                                        testoperators[i].EndDate = DateTime.Now;
                                        TestOperatorRepository.Save(testoperators[i]);
                                    }
                                }

                                //Check the call entries
                                if (GenEnd.EndDate != null)
                                {
                                    // Check if the Call is failed // don't check the result from Android Service //480 - 487 removed
                                    if ((GenEnd.ResponseCode == "408" && GenEnd.Status != "4" && GenEnd.Status != "6") ||
                                        (GenEnd.ResponseCode == "404" && GenEnd.Status != "4" && GenEnd.Status != "6") ||
                                        (GenEnd.ResponseCode == "503" && GenEnd.Status != "4" && GenEnd.Status != "6"))
                                        //(GenEnd.ResponseCode == "487" && GenEnd.Status != "4" && GenEnd.Status != "6"))
                                    {
                                        testoperators[i].TestCli = GenEnd.SipAccount.User.CallerId;
                                        if (GenEnd.ResponseCode == "408")
                                            testoperators[i].ErrorMessage = "408 - Request Timeout";
                                        if (GenEnd.ResponseCode == "404")
                                            testoperators[i].ErrorMessage = "404 - Not Found";
                                        if (GenEnd.ResponseCode == "503")
                                            testoperators[i].ErrorMessage = "503 - Service Unavailable";
                                        //if (GenEnd.ResponseCode == "487")
                                        //    testoperators[i].ErrorMessage = "487 - Request Terminated";
                                        testoperators[i].EndDate = DateTime.Now;
                                        testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Failed;
                                        TestOperatorRepository.Save(testoperators[i]);
                                        //h = true;
                                    }
                                    else
                                    {
                                        ///Get Result
                                        ///
                                        AndroidLiveService.ServiceAuthHeader auth2 = new AndroidLiveService.ServiceAuthHeader();
                                        AndroidLiveService.clireporterSoapClient Req = new AndroidLiveService.clireporterSoapClient();
                                        AndroidLiveService.Call resp2 = new AndroidLiveService.Call();
                                        resp2 = Req.GetCLI(auth, "user", "idsP@ssw0rdids", m.RequestId);

                                        //WriteToEventLog("Response RequestId " +  m.RequestId +"  CLI : " + resp2.CLI + " status " + resp2.status);
                                        if (resp2.status == "1")//Success
                                        {
                                            /// Result done (Delivered or Not delivered) => decrease the balance
                                            ///
                                            UserRepository.DecreaseBalance(testoperators[i].User.Id);
                                            ///

                                            testoperators[i].EndDate = DateTime.Now;
                                            testoperators[i].TestCli = GenEnd.SipAccount.User.CallerId;
                                            string RecCLi = resp2.CLI;

                                            if (resp2.CLI.Length > 4)
                                            {
                                                if (RecCLi.Substring(0, 4) == "0000")
                                                {
                                                    int lenn = resp2.CLI.Length;
                                                    lenn = lenn - 2;
                                                    testoperators[i].ReceivedCli = resp2.CLI.Substring(2, lenn);
                                                }
                                                else
                                                    testoperators[i].ReceivedCli = resp2.CLI;
                                            }
                                            else
                                            {
                                                testoperators[i].ReceivedCli = resp2.CLI;
                                            }

                                            //Check without zeroes
                                            string TestCli1 = testoperators[i].TestCli.Substring(0, 2);
                                            string RecCli1 = "";

                                            if (testoperators[i].ReceivedCli.Length > 2)
                                            {
                                                RecCli1 = testoperators[i].ReceivedCli.Substring(0, 2);
                                            }

                                            if ((TestCli1 == "00" && RecCli1 == "00") || (TestCli1 != "00" && RecCli1 != "00"))
                                            {
                                                if (testoperators[i].TestCli == testoperators[i].ReceivedCli)
                                                    testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                                                else
                                                    testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                                            }
                                            else
                                            {
                                                if (TestCli1 == "00")
                                                {
                                                    string testCli2 = testoperators[i].TestCli.Substring(2);
                                                    if (testCli2 == testoperators[i].ReceivedCli)
                                                        testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                                                    else
                                                        testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                                                }
                                                else
                                                {
                                                    if (RecCli1 == "00")
                                                    {
                                                        string RecCli2 = "";
                                                        if (testoperators[i].ReceivedCli.Length > 2)
                                                        {
                                                            RecCli2 = testoperators[i].ReceivedCli.Substring(2);
                                                        }

                                                        if (testoperators[i].TestCli == RecCli2)
                                                            testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                                                        else
                                                            testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                                                    }
                                                    else
                                                    {
                                                        if (testoperators[i].TestCli == testoperators[i].ReceivedCli)
                                                            testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                                                        else
                                                            testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                                                    }
                                                }
                                            }


                                            TestOperatorRepository.Save(testoperators[i]);
                                            //h = true;
                                        }
                                        else if (resp2.status == "-1")//Expired
                                        {
                                            if (GenEnd.Status == "6")
                                            {
                                                testoperators[i].ErrorMessage = "FAS";
                                                testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Phase;
                                            }
                                            else
                                            {
                                                if (
                                                    //(GenEnd.ResponseCode == "408") ||
                                                    (GenEnd.ResponseCode == "486") ||
                                                   // (GenEnd.ResponseCode == "503") ||
                                                    (GenEnd.ResponseCode == "487"))
                                                {
                                                    //testoperators[i].TestCli = GenEnd.SipAccount.User.CallerId;
                                                    //if (GenEnd.ResponseCode == "408")
                                                    //    testoperators[i].ErrorMessage = "408 - Request Timeout";
                                                    if (GenEnd.ResponseCode == "486")
                                                        testoperators[i].ErrorMessage = "486 - Busy Here";
                                                    //if (GenEnd.ResponseCode == "503")
                                                    //    testoperators[i].ErrorMessage = "503 - Service Unavailable";
                                                    if (GenEnd.ResponseCode == "487")
                                                        testoperators[i].ErrorMessage = "487 - Request Terminated";
                                                    //testoperators[i].EndDate = DateTime.Now;
                                                    testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Failed;
                                                    //TestOperatorRepository.Save(testoperators[i]);
                                                }
                                                else
                                                {
                                                    testoperators[i].ErrorMessage = "Expired";
                                                    testoperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Expired;
                                                }
                                            }
                                            testoperators[i].TestCli = GenEnd.SipAccount.User.CallerId;
                                            testoperators[i].EndDate = DateTime.Now;

                                            TestOperatorRepository.Save(testoperators[i]);
                                            //h = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (testoperators[i].CreationDate.HasValue)
                        {
                            if (exptime != 0 && (DateTime.Now.Second - testoperators[i].CreationDate.Value.Second) > exptime)
                            {
                                Err = "Expired Call";
                                testoperators[i].EndDate = DateTime.Now;

                                testoperators[i].ErrorMessage = Err;
                                testoperators[i].EndDate = DateTime.Now;
                                TestOperatorRepository.Save(testoperators[i]);
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
