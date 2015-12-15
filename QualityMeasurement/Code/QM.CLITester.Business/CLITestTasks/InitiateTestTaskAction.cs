using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using QM.CLITester.Entities;

namespace QM.CLITester.Business
{
    public class InitiateTestTaskAction : SchedulerTaskAction
    {
        public override void Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument,
            Dictionary<string, object> evaluatedExpressions)
        {
            InitiateTestTaskActionArgument initiateTestTaskActionArgument =
                taskActionArgument as InitiateTestTaskActionArgument;

            if (initiateTestTaskActionArgument == null)
                throw new Exception(
                    String.Format("taskActionArgument '{0}' is not of type InitiateTestTaskActionArgument",
                        initiateTestTaskActionArgument));
            if (initiateTestTaskActionArgument.CLITestConnector == null)
                throw new ArgumentNullException("initiateTestTaskActionArgument.CLITestConnector");
            TestCallManager manager = new TestCallManager();

            List<CallTestStatus> listCallTestStatus = new List<CallTestStatus>()
            {
                CallTestStatus.New,
                CallTestStatus.InitiationFailedWithRetry
            };
            SupplierManager supplierManager = new SupplierManager();
            foreach (TestCall testCall in manager.GetTestCalls(listCallTestStatus))
            {
                var initiateTestContext = new InitiateTestContext()
                {
                    Supplier = supplierManager.GetSupplier(testCall.SupplierID),

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
                    initiateTestOutput =
                        initiateTestTaskActionArgument.CLITestConnector.InitiateTest(initiateTestContext);
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
                        callTestStatus = CallTestStatus.Initiated;
                        break;
                    case InitiateTestResult.FailedWithRetry:
                    {
                        callTestStatus = CallTestStatus.InitiationFailedWithRetry;
                        if (testCall.InitiationRetryCount < 5)
                            testCall.InitiationRetryCount = testCall.InitiationRetryCount + 1;
                        else
                            callTestStatus = CallTestStatus.InitiationFailedWithNoRetry;
                        break;
                    }

                    case InitiateTestResult.FailedWithNoRetry:
                        callTestStatus = CallTestStatus.InitiationFailedWithNoRetry;
                        break;
                    default:
                        callTestStatus = CallTestStatus.InitiationFailedWithRetry;
                        break;
                }

                manager.UpdateInitiateTest(testCall.ID, initiateTestOutput.InitiateTestInformation, callTestStatus,
                    testCall.InitiationRetryCount, testCall.FailureMessage);
            }
        }
    }
}
