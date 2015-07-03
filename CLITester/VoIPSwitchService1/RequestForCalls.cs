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
using VoIPSwitchService.PhoneNumberWebReference;

namespace VoIPSwitchService
{
    public class RequestForCalls
    {
        public bool locked = false;
        public int OperatorId = 0;
        private readonly object _syncRoot = new object();

        public void Start()
        {
            //locked = false;
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
                        CallGeneratorLibrary.Utilities.BalanceDetails.FillBalanceDetails();

                        CallGeneratorLibrary.Utilities.ScheduleManager.CLISchedule();
                        //PhoneNumberRepository.FreeAllLockedPhone();


                        List<TestOperator> lstTestOperators = TestOperatorRepository.GetMontyTestOperators();
                        foreach (TestOperator testOperator in lstTestOperators)
                        {
                            //if (op.Operator.ServiceAndroid == true)
                            {
                                OperatorId = testOperator.Id;
                                //Check if the number is free or busy

                                PhoneNumberService pp = new PhoneNumberService();
                                PhoneNumberReturn ret = pp.RequestForCall(ConfigurationManager.AppSettings["ClientName"], ConfigurationManager.AppSettings["Password"], testOperator.Operator.mcc, testOperator.Operator.mnc);
                                System.Threading.Thread.Sleep(1000);
                                //PhoneNumber phoneNumber = PhoneNumberRepository.GetFreePhoneNumber(testOperator.OperatorId.Value);

                                if (ret.ErrorStatus == "-1")
                                //if (phoneNumber == null)
                                {
                                    TestOperator tOperator = TestOperatorRepository.Load(testOperator.Id);
                                    tOperator.ErrorMessage = "Busy";
                                    tOperator.EndDate = DateTime.Now;
                                    TestOperatorRepository.Save(tOperator);
                                }
                                else
                                {
                                    User userParent = UserRepository.Load(testOperator.UserId.Value);
                                    SipAccount sipAccount = SipAccountRepository.GetTop();

                                    GeneratedCall generatedCall = new GeneratedCall()
                                    {
                                        //Number = String.Format("{0}{1}@{2}", testOperator.CarrierPrefix, phoneNumber.Number, testOperator.User.IpSwitch),
                                        Number = String.Format("{0}{1}@{2}", testOperator.CarrierPrefix, ret.Number, sipAccount.Server),
                                        SipAccountId = sipAccount.Id
                                    };
                                    GeneratedCallRepository.Save(generatedCall);

                                    GeneratedCall newGeneratedCall = GeneratedCallRepository.Load(generatedCall.Id);

                                    //Save The MSISDN and the Request ID into MontyCall table
                                    MontyCall montyCall = new MontyCall()
                                    {
                                        TestOperatorId = testOperator.Id,
                                        MSISDN = testOperator.CarrierPrefix + ret.Number,
                                        CreationDate = DateTime.Now,
                                        RequestId = ret.RequestCallId.ToString(),
                                        GeneratedCallId = newGeneratedCall.Id,
                                        ServiceId = (int)CallGeneratorLibrary.Utilities.Enums.Service.AndroidDevice
                                    };
                                    MontyCallRepository.Save(montyCall);

                                    TestOperator tOperator = TestOperatorRepository.Load(testOperator.Id);
                                    tOperator.PhonePrefix = ret.Prefix;
                                    TestOperatorRepository.Save(tOperator);
                                }
                            }
                        }
                        lstTestOperators.Clear();
                    }
                    catch (System.Exception ex)
                    {
                        WriteToEventLog(ex.ToString());
                        Logger.LogException(ex);
                        TestOperator tstOperator = TestOperatorRepository.Load(OperatorId);
                        if (tstOperator.EndDate == null)
                        {
                            tstOperator.ErrorMessage = "Failed while Processing";
                            tstOperator.EndDate = DateTime.Now;
                            TestOperatorRepository.Save(tstOperator);
                        }
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    locked = false;
                    System.Threading.Thread.Sleep(5000);
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
