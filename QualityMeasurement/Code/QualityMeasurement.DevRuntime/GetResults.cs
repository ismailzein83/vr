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
        
        CLITesterConnectorBase cliTestConnector = new QM.CLITester.iTestIntegration.CLITesterConnector();

        private void GetResult()
        {
            try
            {
                while (_locked != true)
                {
                    _locked = true;
                    lock (_syncRoot)
                    {
                        TestCallManager manager = new TestCallManager();

                        List<CallTestStatus> listCallTestStatus = new List<CallTestStatus>()
                        {
                            CallTestStatus.Initiated,
                            CallTestStatus.PartiallyCompleted,
                            CallTestStatus.GetProgressFailedWithRetry
                        };

                        foreach (TestCall testCall in manager.GetTestCalls(listCallTestStatus))
                        {
                            var getTestProgressContext = new GetTestProgressContext()
                            {
                                InitiateTestInformation = testCall.InitiateTestInformation,
                                RecentTestProgress = testCall.TestProgress
                            };

                            GetTestProgressOutput testProgressOutput = new GetTestProgressOutput();
                            try
                            {
                                testProgressOutput = cliTestConnector.GetTestProgress(getTestProgressContext);
                            }
                            catch (Exception ex)
                            {
                                testProgressOutput.Result = GetTestProgressResult.FailedWithRetry;
                                testCall.FailureMessage = ex.Message;
                            }
                            
                            CallTestStatus callTestStatus;
                            switch (testProgressOutput.Result)
                            {
                                case GetTestProgressResult.TestCompleted:
                                    callTestStatus = CallTestStatus.Completed;
                                    break;
                                case GetTestProgressResult.FailedWithRetry:
                                {
                                    callTestStatus = CallTestStatus.GetProgressFailedWithRetry;
                                    if (testCall.GetProgressRetryCount < 5)
                                        testCall.GetProgressRetryCount = testCall.GetProgressRetryCount + 1;
                                    else
                                        callTestStatus = CallTestStatus.GetProgressFailedWithNoRetry;
                                    break;   
                                }
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
                            if (testProgressOutput.Result != GetTestProgressResult.ProgressNotChanged)
                                manager.UpdateTestProgress(testCall.ID, testProgressOutput.TestProgress, callTestStatus, testProgressOutput.CallTestResult, testCall.GetProgressRetryCount, testCall.FailureMessage);
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
