using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using QM.BusinessEntity.Entities;
using QM.CLITester.Business;
using QM.CLITester.Entities;

namespace QualityMeasurement.DevRuntime
{
    public class GetCalls
    {
        private static bool _locked;
        private static Form1 _form1;
        private static readonly object _syncRoot = new object();

        public void Start(Form1 f)
        {

            _locked = false;
            GetCalls._form1 = f;
            Thread thread = new Thread(new ThreadStart(GetCall));
            thread.IsBackground = true;
            thread.Start();
        }

        ICLITesterConnector cliTestConnector = new QM.CLITester.iTestIntegration.CLITesterConnector();

        private void GetCall()
        {
            try
            {
                while (_locked != true)
                {
                    TestCallManager manager = new TestCallManager();

                    List<CallTestStatus> listCallTestStatus = new List<CallTestStatus>()
                        {
                            CallTestStatus.New,
                            CallTestStatus.InitiationFailedWithRetry
                        };

                    _locked = true;
                    lock (_syncRoot)
                    {
                        foreach (TestCall testCall in manager.GetTestCalls(listCallTestStatus))
                        {
                            var initiateTestContext = new InitiateTestContext()
                            {
                                Supplier = new Supplier
                                {
                                    SupplierId = testCall.SupplierID
                                },

                                Zone = new Zone
                                {
                                    ZoneId = testCall.ZoneID
                                },
                                Country = new Vanrise.Entities.Country
                                {
                                    CountryId = testCall.CountryID
                                }
                            };

                            InitiateTestOutput initiateTestOutput = new InitiateTestOutput();
                            try
                            {
                                initiateTestOutput = cliTestConnector.InitiateTest(initiateTestContext);
                            }
                            catch (Exception ex)
                            {
                                initiateTestOutput.Result = InitiateTestResult.FailedWithRetry;
                                testCall.FailureMessage = ex.Message;
                            }
                            
                            CallTestStatus callTestStatus;

                            switch (initiateTestOutput.Result)
                            {
                                case InitiateTestResult.Created:
                                    callTestStatus =  CallTestStatus.Initiated; break;
                                case InitiateTestResult.FailedWithRetry:
                                {
                                    callTestStatus = CallTestStatus.InitiationFailedWithRetry;
                                    if(testCall.InitiationRetryCount < 5)
                                        testCall.InitiationRetryCount = testCall.InitiationRetryCount + 1;
                                    else
                                        callTestStatus = CallTestStatus.InitiationFailedWithNoRetry;
                                    break;
                                }
                                    
                                case InitiateTestResult.FailedWithNoRetry:
                                    callTestStatus =  CallTestStatus.InitiationFailedWithNoRetry; break;
                                default:
                                    callTestStatus =  CallTestStatus.InitiationFailedWithRetry; break;
                            }

                            manager.UpdateInitiateTest(testCall.ID, initiateTestOutput.InitiateTestInformation, callTestStatus, testCall.InitiationRetryCount, testCall.FailureMessage); break;
                        }

                        _locked = false;
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
