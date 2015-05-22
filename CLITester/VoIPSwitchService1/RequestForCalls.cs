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

namespace VoIPSwitchService
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

                    try
                    {
                        CallGeneratorLibrary.Utilities.ScheduleManager.CLISchedule();

                        List<TestOperator> testoperators = TestOperatorRepository.GetMontyTestOperators();
                        foreach (TestOperator op in testoperators)
                        {
                            //if (op.Operator.ServiceAndroid == true)
                            {
                                OperatorId = op.Id;
                                //Check if the number is free or busy

                                PhoneNumber ph = PhoneNumberRepository.GetFreePhoneNumber(op.Id);

                                if (ph == null)
                                {
                                    TestOperator t = TestOperatorRepository.Load(op.Id);
                                    t.ErrorMessage = "No Line Availables";
                                    t.EndDate = DateTime.Now;
                                    TestOperatorRepository.Save(t);
                                }
                                else
                                {
                                    GeneratedCall GenCall = new GeneratedCall();
                                    GenCall.Number = op.CarrierPrefix + ph.Number + "@" + op.User.IpSwitch;
                                    //GenCall.Number = resp.mobileNumber;
                                    //SipAccount sp = SipAccountRepository.LoadbyUser(op.UserId.Value);
                                    User usParent = UserRepository.Load(op.UserId.Value);
                                    SipAccount sp = new SipAccount();
                                    if (usParent.ParentId == null)
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
                                    montycall.MSISDN = op.CarrierPrefix + ph.Number;
                                    montycall.CreationDate = DateTime.Now;
                                    //montycall.RequestId = resp.Callid;
                                    //RequestId = resp.Callid;
                                    montycall.ReturnMessage = "";
                                    montycall.CallEntryId = NewGen.Id;
                                    montycall.ServiceId = (int)CallGeneratorLibrary.Utilities.Enums.Service.AndroidDevice;
                                    //WriteToEventLog("Monty Table: Id: " + op.Id + " CarrierPrefix: " + op.CarrierPrefix + " mobileNumber: " + resp.mobileNumber + " RequestId " + resp.Callid);
                                    MontyCallRepository.Save(montycall);
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
            string cs = "VoIPSwitchService";
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
