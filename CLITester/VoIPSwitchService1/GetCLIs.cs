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
using MySql.Data.MySqlClient;

namespace VoIPSwitchService
{
    public class GetCLIs
    {
        public bool locked = false;
        public int operatorId = 0;
        private readonly object _syncRoot = new object();

        public void Start()
        {
            try
            {
                locked = false;
                Thread thread = new Thread(new ThreadStart(GetCLI));
                thread.IsBackground = true;
                thread.Start();
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(ex.ToString());
                Logger.LogException(ex);
            }
        }

        private void GetCLI()
        {
            try
            {
                string Err = null;

                lock (_syncRoot)
                    while (locked != true)
                    {
                        locked = true;
                        //Expiry minutes
                        int expTime = 0;
                        int.TryParse(ConfigurationManager.AppSettings["ExpiryTime"], out expTime);

                        try
                        {
                            List<TestOperator> lstTestOperators = TestOperatorRepository.GetRequestedTestOperators();

                            for (int i = 0; i < lstTestOperators.Count(); i++)
                            {
                                operatorId = lstTestOperators[i].Id;
                                MontyCall montyCall = MontyCallRepository.LoadbyTestOperatorId(lstTestOperators[i].Id);
                                if (montyCall != null)
                                {
                                    GeneratedCall generatedCall = GeneratedCallRepository.Load(montyCall.CallEntryId.Value);
                                    if (generatedCall != null)
                                    {
                                        if (generatedCall.StartDate.HasValue)
                                        {
                                            if (expTime != 0 && (DateTime.Now.Second - generatedCall.StartDate.Value.Second) > expTime)
                                            {
                                                if (generatedCall.AlertDate != null && generatedCall.StartCall != null)
                                                {
                                                    lstTestOperators[i].PDD = ((generatedCall.AlertDate.Value - generatedCall.StartCall.Value).TotalSeconds).ToString();
                                                }

                                                if (generatedCall.ConnectDate != null && generatedCall.DisconnectDate != null)
                                                {
                                                    lstTestOperators[i].Duration = ((generatedCall.DisconnectDate.Value - generatedCall.ConnectDate.Value).TotalSeconds).ToString();
                                                }

                                                Err = "Expired - No Line Availables";
                                                generatedCall.EndDate = DateTime.Now;

                                                lstTestOperators[i].ErrorMessage = Err;
                                                lstTestOperators[i].EndDate = DateTime.Now;
                                                TestOperatorRepository.Save(lstTestOperators[i]);
                                                PhoneNumberRepository.FreeThisPhoneNumber(lstTestOperators[i]);
                                            }
                                        }

                                        //Check the call entries
                                        if (generatedCall.EndDate != null)
                                        {
                                            // Check if the Call is failed // don't check the result from Android Service //480 - 487 removed
                                            if ((generatedCall.ResponseCode == "408" && generatedCall.Status != "4" && generatedCall.Status != "6") ||
                                                (generatedCall.ResponseCode == "404" && generatedCall.Status != "4" && generatedCall.Status != "6") ||
                                                (generatedCall.ResponseCode == "503" && generatedCall.Status != "4" && generatedCall.Status != "6"))
                                            //(generatedCall.ResponseCode == "487" && generatedCall.Status != "4" && generatedCall.Status != "6"))
                                            {
                                                if (generatedCall.AlertDate != null && generatedCall.StartCall != null)
                                                {
                                                    lstTestOperators[i].PDD = ((generatedCall.AlertDate.Value - generatedCall.StartCall.Value).TotalSeconds).ToString();
                                                }

                                                if (generatedCall.ConnectDate != null && generatedCall.DisconnectDate != null)
                                                {
                                                    lstTestOperators[i].Duration = ((generatedCall.DisconnectDate.Value - generatedCall.ConnectDate.Value).TotalSeconds).ToString();
                                                }

                                                lstTestOperators[i].TestCli = generatedCall.SipAccount.User.CallerId;
                                                if (generatedCall.ResponseCode == "408")
                                                    lstTestOperators[i].ErrorMessage = "408 - Request Timeout";
                                                if (generatedCall.ResponseCode == "404")
                                                    lstTestOperators[i].ErrorMessage = "404 - Not Found";
                                                if (generatedCall.ResponseCode == "503")
                                                    lstTestOperators[i].ErrorMessage = "503 - Service Unavailable";
                                                //if (generatedCall.ResponseCode == "487")
                                                //    lstTestOperators[i].ErrorMessage = "487 - Request Terminated";
                                                lstTestOperators[i].EndDate = DateTime.Now;
                                                lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Failed;
                                                TestOperatorRepository.Save(lstTestOperators[i]);
                                                PhoneNumberRepository.FreeThisPhoneNumber(lstTestOperators[i]);
                                                //h = true;
                                            }
                                            else
                                            {
                                                new DataManager().GetData(lstTestOperators[i].CarrierPrefix.Length, generatedCall, (reader) =>
                                                {
                                                    if (reader.HasRows)
                                                    {
                                                        /// Result done (Delivered or Not delivered) => decrease the balance
                                                        ///
                                                        UserRepository.DecreaseBalance(lstTestOperators[i].User.Id);
                                                        ///
                                                        if (reader.Read())
                                                        {
                                                            lstTestOperators[i].EndDate = DateTime.Now;
                                                            string testcli = generatedCall.SipAccount.User.CallerId;

                                                            ///Remove the character '+' from the Cli received, if exist
                                                            if (testcli.Substring(0, 1) == "+")
                                                            {
                                                                int len2 = generatedCall.SipAccount.User.CallerId.Length;
                                                                len2 = len2 - 1;
                                                                lstTestOperators[i].TestCli = generatedCall.SipAccount.User.CallerId.Substring(1, len2);
                                                            }
                                                            else
                                                                lstTestOperators[i].TestCli = generatedCall.SipAccount.User.CallerId;
                                                            string RecCLi = reader[3].ToString();

                                                            ///Remove the 4 zeroes from the Cli received, if exist
                                                            if (reader[3].ToString().Length > 4)
                                                            {
                                                                if (RecCLi.Substring(0, 4) == "0000")
                                                                {
                                                                    int lenn = reader[3].ToString().Length;
                                                                    lenn = lenn - 2;
                                                                    lstTestOperators[i].ReceivedCli = reader[3].ToString().Substring(2, lenn);
                                                                }
                                                                else
                                                                    lstTestOperators[i].ReceivedCli = reader[3].ToString();
                                                            }
                                                            else
                                                            {
                                                                lstTestOperators[i].ReceivedCli = reader[3].ToString();
                                                            }

                                                            //Check without zeroes
                                                            string TestCli1 = lstTestOperators[i].TestCli.Substring(0, 2);
                                                            string RecCli1 = "";

                                                            if (lstTestOperators[i].ReceivedCli.Length > 2)
                                                            {
                                                                RecCli1 = lstTestOperators[i].ReceivedCli.Substring(0, 2);
                                                            }

                                                            if ((TestCli1 == "00" && RecCli1 == "00") || (TestCli1 != "00" && RecCli1 != "00"))
                                                            {
                                                                if (lstTestOperators[i].TestCli == lstTestOperators[i].ReceivedCli)
                                                                    lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                                                                else
                                                                    lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                                                            }
                                                            else
                                                            {
                                                                if (TestCli1 == "00")
                                                                {
                                                                    string testCli2 = lstTestOperators[i].TestCli.Substring(2);
                                                                    if (testCli2 == lstTestOperators[i].ReceivedCli)
                                                                        lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                                                                    else
                                                                        lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                                                                }
                                                                else
                                                                {
                                                                    if (RecCli1 == "00")
                                                                    {
                                                                        string RecCli2 = "";
                                                                        if (lstTestOperators[i].ReceivedCli.Length > 2)
                                                                        {
                                                                            RecCli2 = lstTestOperators[i].ReceivedCli.Substring(2);
                                                                        }

                                                                        if (lstTestOperators[i].TestCli == RecCli2)
                                                                            lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                                                                        else
                                                                            lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (lstTestOperators[i].TestCli == lstTestOperators[i].ReceivedCli)
                                                                            lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLIValid;
                                                                        else
                                                                            lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.CLINotValid;
                                                                    }
                                                                }
                                                            }


                                                            if (generatedCall.AlertDate != null && generatedCall.StartCall != null)
                                                            {
                                                                lstTestOperators[i].PDD = ((generatedCall.AlertDate.Value - generatedCall.StartCall.Value).TotalSeconds).ToString();
                                                            }

                                                            if (generatedCall.ConnectDate != null && generatedCall.DisconnectDate != null)
                                                            {
                                                                lstTestOperators[i].Duration = ((generatedCall.DisconnectDate.Value - generatedCall.ConnectDate.Value).TotalSeconds).ToString();
                                                            }


                                                            TestOperatorRepository.Save(lstTestOperators[i]);
                                                            PhoneNumberRepository.FreeThisPhoneNumber(lstTestOperators[i]);
                                                            //h = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (generatedCall.Status == "6")
                                                        {
                                                            lstTestOperators[i].ErrorMessage = "FAS";
                                                            lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Phase;
                                                        }
                                                        else
                                                        {
                                                            if (
                                                                //(generatedCall.ResponseCode == "408") ||
                                                                (generatedCall.ResponseCode == "486") ||
                                                                // (generatedCall.ResponseCode == "503") ||
                                                                (generatedCall.ResponseCode == "487"))
                                                            {
                                                                //lstTestOperators[i].TestCli = generatedCall.SipAccount.User.CallerId;
                                                                //if (generatedCall.ResponseCode == "408")
                                                                //    lstTestOperators[i].ErrorMessage = "408 - Request Timeout";
                                                                if (generatedCall.ResponseCode == "486")
                                                                    lstTestOperators[i].ErrorMessage = "486 - Busy Here";
                                                                //if (generatedCall.ResponseCode == "503")
                                                                //    lstTestOperators[i].ErrorMessage = "503 - Service Unavailable";
                                                                if (generatedCall.ResponseCode == "487")
                                                                    lstTestOperators[i].ErrorMessage = "487 - Request Terminated";
                                                                //lstTestOperators[i].EndDate = DateTime.Now;
                                                                lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Failed;
                                                                //TestOperatorRepository.Save(lstTestOperators[i]);
                                                            }
                                                            else
                                                            {
                                                                lstTestOperators[i].ErrorMessage = "Expired";
                                                                lstTestOperators[i].Status = (int)CallGeneratorLibrary.Utilities.Enums.CallStatus.Expired;
                                                            }
                                                        }
                                                        lstTestOperators[i].TestCli = generatedCall.SipAccount.User.CallerId;
                                                        lstTestOperators[i].EndDate = DateTime.Now;


                                                        if (generatedCall.AlertDate != null && generatedCall.StartCall != null)
                                                        {
                                                            lstTestOperators[i].PDD = ((generatedCall.AlertDate.Value - generatedCall.StartCall.Value).TotalSeconds).ToString();
                                                        }

                                                        if (generatedCall.ConnectDate != null && generatedCall.DisconnectDate != null)
                                                        {
                                                            lstTestOperators[i].Duration = ((generatedCall.DisconnectDate.Value - generatedCall.ConnectDate.Value).TotalSeconds).ToString();
                                                        }

                                                        TestOperatorRepository.Save(lstTestOperators[i]);
                                                        PhoneNumberRepository.FreeThisPhoneNumber(lstTestOperators[i]);
                                                        //h = true;
                                                    }
                                                });
                                            }  
                                        }
                                    }
                                }

                                if (lstTestOperators[i].CreationDate.HasValue)
                                {
                                    if (expTime != 0 && (DateTime.Now.Second - lstTestOperators[i].CreationDate.Value.Second) > expTime)
                                    {
                                        MontyCall montyCall2 = MontyCallRepository.LoadbyTestOperatorId(lstTestOperators[i].Id);
                                        if (montyCall2 != null)
                                        {
                                            GeneratedCall generatedCall = GeneratedCallRepository.Load(montyCall2.CallEntryId.Value);
                                            if (generatedCall != null)
                                            {
                                                if (generatedCall.AlertDate != null && generatedCall.StartCall != null)
                                                {
                                                    lstTestOperators[i].PDD = ((generatedCall.AlertDate.Value - generatedCall.StartCall.Value).TotalSeconds).ToString();
                                                }

                                                if (generatedCall.ConnectDate != null && generatedCall.DisconnectDate != null)
                                                {
                                                    lstTestOperators[i].Duration = ((generatedCall.DisconnectDate.Value - generatedCall.ConnectDate.Value).TotalSeconds).ToString();
                                                }
                                            }
                                        }

                                        
                                        Err = "Expired Call";
                                        lstTestOperators[i].EndDate = DateTime.Now;

                                        lstTestOperators[i].ErrorMessage = Err;
                                        lstTestOperators[i].EndDate = DateTime.Now;
                                        TestOperatorRepository.Save(lstTestOperators[i]);
                                        PhoneNumberRepository.FreeThisPhoneNumber(lstTestOperators[i]);
                                    }
                                }
                            }

                            lstTestOperators.TrimExcess();
                            lstTestOperators.Clear();
                        }
                        catch (System.Exception ex)
                        {
                            WriteToEventLog(ex.ToString());
                            Logger.LogException(ex);
                            TestOperator testOperator = TestOperatorRepository.Load(operatorId);
                            if (testOperator.EndDate == null)
                            {
                                MontyCall montyCall3 = MontyCallRepository.LoadbyTestOperatorId(testOperator.Id);
                                if (montyCall3 != null)
                                {
                                    GeneratedCall generatedCall = GeneratedCallRepository.Load(montyCall3.CallEntryId.Value);
                                    if (generatedCall != null)
                                    {
                                        if (generatedCall.AlertDate != null && generatedCall.StartCall != null)
                                        {
                                            testOperator.PDD = ((generatedCall.AlertDate.Value - generatedCall.StartCall.Value).TotalSeconds).ToString();
                                        }

                                        if (generatedCall.ConnectDate != null && generatedCall.DisconnectDate != null)
                                        {
                                            testOperator.Duration = ((generatedCall.DisconnectDate.Value - generatedCall.ConnectDate.Value).TotalSeconds).ToString();
                                        }
                                    }
                                }

                                testOperator.ErrorMessage = "Failed while Processing";
                                testOperator.EndDate = DateTime.Now;
                                TestOperatorRepository.Save(testOperator);
                                PhoneNumberRepository.FreeThisPhoneNumber(testOperator);
                            }
                        }
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////
                        locked = false;
                        System.Threading.Thread.Sleep(5000);
                    }
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(ex.ToString());
                Logger.LogException(ex);
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
