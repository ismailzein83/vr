using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using QM.CLITester.Entities;

namespace QM.CLITester.Business
{
    public class TestProgressTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument,
            Dictionary<string, object> evaluatedExpressions)
        {
            TestProgressTaskActionArgument testProgressTaskActionArgument =
                taskActionArgument as TestProgressTaskActionArgument;

            if (testProgressTaskActionArgument == null)
                throw new Exception(
                    String.Format("taskActionArgument '{0}' is not of type TestProgressTaskActionArgument",
                        testProgressTaskActionArgument));

            if (testProgressTaskActionArgument.CLITestConnector == null)
                throw new ArgumentNullException("testProgressTaskActionArgument.CLITestConnector");

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
                    testProgressOutput = testProgressTaskActionArgument.CLITestConnector.GetTestProgress(getTestProgressContext);
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
                            if (testCall.GetProgressRetryCount < testProgressTaskActionArgument.MaximumRetryCount)
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

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
