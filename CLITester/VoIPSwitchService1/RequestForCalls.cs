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
                        CallGeneratorLibrary.Utilities.ScheduleManager.CLISchedule();
                        PhoneNumberRepository.FreeAllLockedPhone();


                        List<TestOperator> lstTestOperators = TestOperatorRepository.GetMontyTestOperators();
                        foreach (TestOperator testOperator in lstTestOperators)
                        {
                            //if (op.Operator.ServiceAndroid == true)
                            {
                                OperatorId = testOperator.Id;
                                //Check if the number is free or busy

                                PhoneNumber phoneNumber = PhoneNumberRepository.GetFreePhoneNumber(testOperator.OperatorId.Value);

                                if (phoneNumber == null)
                                {
                                    TestOperator tOperator = TestOperatorRepository.Load(testOperator.Id);
                                    tOperator.ErrorMessage = "No Line Availables";
                                    tOperator.EndDate = DateTime.Now;
                                    TestOperatorRepository.Save(tOperator);
                                }
                                else
                                {
                                    User userParent = UserRepository.Load(testOperator.UserId.Value);
                                    SipAccount sipAccount = new SipAccount();
                                    if (userParent.ParentId == null)
                                        sipAccount = SipAccountRepository.LoadbyUser(testOperator.UserId.Value);
                                    else
                                        sipAccount = SipAccountRepository.LoadbyUser(userParent.ParentId.Value);

                                    GeneratedCall generatedCall = new GeneratedCall()
                                    {
                                        Number = String.Format("{0}{1}@{2}", testOperator.CarrierPrefix, phoneNumber.Number, testOperator.User.IpSwitch),
                                        SipAccountId = sipAccount.Id
                                    };
                                    GeneratedCallRepository.Save(generatedCall);

                                    GeneratedCall newGeneratedCall = GeneratedCallRepository.Load(generatedCall.Id);

                                    //Save The MSISDN and the Request ID into MontyCall table
                                    MontyCall montyCall = new MontyCall()
                                    {
                                        TestOperatorId = testOperator.Id,
                                        MSISDN = testOperator.CarrierPrefix + phoneNumber.Number,
                                        CreationDate = DateTime.Now,
                                        ReturnMessage = String.Empty,
                                        CallEntryId = newGeneratedCall.Id,
                                        ServiceId = (int)CallGeneratorLibrary.Utilities.Enums.Service.AndroidDevice
                                    };
                                    MontyCallRepository.Save(montyCall);

                                    TestOperator tOperator = TestOperatorRepository.Load(testOperator.Id);
                                    tOperator.PhonePrefix = phoneNumber.Prefix;
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
