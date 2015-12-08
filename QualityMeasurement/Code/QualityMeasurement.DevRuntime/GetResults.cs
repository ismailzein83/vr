using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QM.CLITester.Business;
using QM.CLITester.Entities;

namespace QualityMeasurement.DevRuntime
{
    public class GetResults
    {
        private static Form1 f;
        private static readonly object _syncRoot = new object();
        private static bool _locked;

        public void Start(Form1 f)
        {
            GetResults.f = f;
            Thread thread = new Thread(GetResult);
            thread.IsBackground = true;
            thread.Start();
        }
        
        ICLITesterConnector cliTestConnector = new QM.CLITester.iTestIntegration.CLITesterConnector();

        private void GetResult()
        {
            try
            {
                while (_locked != true)
                {
                    _locked = true;
                    lock (_syncRoot)
                    {
                        //Get Result
                        TestCallManager manager = new TestCallManager();
                        List<int> listCallTestStatusInts = new List<int>();
                        listCallTestStatusInts.Add((int)CallTestStatus.Initiated);
                        listCallTestStatusInts.Add((int)CallTestStatus.PartiallyCompleted);

                        List<TestCall> listTestCall = manager.GetTestCalls(listCallTestStatusInts);

                        foreach (TestCall testCall in listTestCall)
                        {
                            var getTestProgressContext = new GetTestProgressContext()
                            {
                                InitiateTestInformation = testCall.InitiateTestInformation,
                                RecentTestProgress = testCall.TestProgress
                            };
                            var testProgressOutput = cliTestConnector.GetTestProgress(getTestProgressContext);

                            CallTestStatus callTestStatus;
                            switch (testProgressOutput.Result)
                            {
                                case GetTestProgressResult.TestCompleted:
                                    callTestStatus = CallTestStatus.Completed;
                                    break;
                                case GetTestProgressResult.FailedWithRetry:
                                    callTestStatus = CallTestStatus.GetProgressFailedWithRetry;
                                    break;
                                case GetTestProgressResult.FailedWithNoRetry:
                                    callTestStatus = CallTestStatus.GetProgressFailedWithNoRetry;
                                    break;
                                case GetTestProgressResult.ProgressChanged:
                                    callTestStatus = CallTestStatus.PartiallyCompleted;
                                    break;
                                default:
                                    callTestStatus = testCall.CallTestStatus;
                                    break;
                            }
                            manager.UpdateTestProgress(testCall.ID, testProgressOutput.TestProgress, callTestStatus, testProgressOutput.CallTestResult);
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
