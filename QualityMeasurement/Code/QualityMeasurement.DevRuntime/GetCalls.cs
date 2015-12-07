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
                    _locked = true;
                    lock (_syncRoot)
                    {
                        //Get Calls
                        TestCallManager manager = new TestCallManager();
                        List<TestCall> listTestCall = manager.GetTestCalls((int)CallTestStatus.New);
                        
                        foreach (TestCall testCall in listTestCall)
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
                            var initiateTestOutput = cliTestConnector.InitiateTest(initiateTestContext);

                            switch (initiateTestOutput.Result)
                            {
                                case InitiateTestResult.Created:
                                    manager.UpdateInitiateTest(initiateTestOutput.InitiateTestInformation.ToString(), CallTestStatus.Initiated, testCall.ID); break;
                                case InitiateTestResult.FailedWithRetry:
                                    manager.UpdateInitiateTest(initiateTestOutput.InitiateTestInformation.ToString(), CallTestStatus.InitiationFailedWithRetry, testCall.ID); break;
                                case InitiateTestResult.FailedWithNoRetry:
                                    manager.UpdateInitiateTest(initiateTestOutput.InitiateTestInformation.ToString(), CallTestStatus.InitiationFailedWithNoRetry, testCall.ID); break;
                                default:
                                    manager.UpdateInitiateTest(initiateTestOutput.InitiateTestInformation.ToString(), CallTestStatus.InitiationFailedWithNoRetry, testCall.ID); break;
                            }
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
