using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace QM.CLITester.Business
{
    public class TestCallTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument,
            Dictionary<string, object> evaluatedExpressions)
        {
            TestCallTaskActionArgument testCallTaskActionArgument =
                taskActionArgument as TestCallTaskActionArgument;

            if (testCallTaskActionArgument == null)
                throw new Exception(
                    String.Format("taskActionArgument '{0}' is not of type TestCallTaskActionArgument",
                        testCallTaskActionArgument));
            if (testCallTaskActionArgument.AddTestCallInput == null)
                throw new ArgumentNullException("testCallTaskActionArgument.AddTestCallInput");

            TestCallManager manager = new TestCallManager();
            AddTestCallOutput exuctionInfo = manager.AddNewTestCall(testCallTaskActionArgument.AddTestCallInput, task.OwnerId);

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.WaitingEvent,
                ExecutionInfo = exuctionInfo
            };

            return output;
        }

        public override SchedulerTaskCheckProgressOutput CheckProgress(ISchedulerTaskCheckProgressContext context)
        {
            if (context.ExecutionInfo == null)
                throw new ArgumentNullException("context.ExecutionInfo");

            SchedulerTaskCheckProgressOutput output = new SchedulerTaskCheckProgressOutput()
            {
                Result = ExecuteOutputResult.Completed
            };

            TestCallManager manager = new TestCallManager();
            List<TestCall> listTestCalls = manager.GetAllbyBatchNumber(((TaskCheckProgressContext)context.ExecutionInfo).BatchNumber);
            foreach (TestCall testCall in listTestCalls)
            {
                if (testCall.CallTestResult == CallTestResult.NotCompleted ||
                    testCall.CallTestResult == CallTestResult.PartiallySucceeded)
                    output.Result = ExecuteOutputResult.WaitingEvent;
            }

            return output;
        }
    }
}
