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
                        List<TestCall> listTestCall = manager.GetTestCalls((int)CallTestStatus.Initiated);

                        foreach (TestCall testCall in listTestCall)
                        {
                            var getTestProgressContext = new GetTestProgressContext()
                            {
                                InitiateTestInformation = testCall.InitiateTestInformation,
                                RecentTestProgress = testCall.TestProgress
                            };
                            var testProgressOutput = cliTestConnector.GetTestProgress(getTestProgressContext);


                            switch (testProgressOutput.Result)
                            {
                                case GetTestProgressResult.TestCompleted:
                                    manager.UpdateTestProgress(
                                        testProgressOutput.TestProgress.ToString(), CallTestResult.Succeeded,
                                        testCall.ID); break;
                                case GetTestProgressResult.ProgressChanged:
                                    manager.UpdateTestProgress(testProgressOutput.TestProgress.ToString(), CallTestResult.PartiallySucceeded, testCall.ID); break;
                                default:
                                    manager.UpdateTestProgress(testProgressOutput.TestProgress.ToString(), CallTestResult.PartiallySucceeded, testCall.ID); break;
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
