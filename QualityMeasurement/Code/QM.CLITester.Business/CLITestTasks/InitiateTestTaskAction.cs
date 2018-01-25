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
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument,
            Dictionary<string, object> evaluatedExpressions)
        {
            InitiateTestTaskActionArgument initiateTestTaskActionArgument = taskActionArgument as InitiateTestTaskActionArgument;

            if (initiateTestTaskActionArgument == null)
                throw new Exception(String.Format("taskActionArgument '{0}' is not of type InitiateTestTaskActionArgument", initiateTestTaskActionArgument));

            if (initiateTestTaskActionArgument.CLITestConnector == null)
                throw new ArgumentNullException("initiateTestTaskActionArgument.CLITestConnector");

            TestCallManager manager = new TestCallManager();
            SupplierManager supplierManager = new SupplierManager();
            ProfileManager profileManager = new ProfileManager();
            var countryManager = new Vanrise.Common.Business.CountryManager();
            ZoneManager zoneManager = new ZoneManager();

            List<CallTestStatus> listCallTestStatus = new List<CallTestStatus>()
            {
                CallTestStatus.New,
                CallTestStatus.InitiationFailedWithRetry
            };

            foreach (TestCall testCall in manager.GetTestCalls(listCallTestStatus))
            {
                var initiateTestContext = new InitiateTestContext()
                {
                    Supplier = supplierManager.GetSupplier(testCall.SupplierID),
                    Profile = profileManager.GetProfile(testCall.ProfileID),
                    Country = countryManager.GetCountry(testCall.CountryID),
                    Zone = zoneManager.GetZone(testCall.ZoneID),
                    Quantity = testCall.Quantity
                };

                InitiateTestOutput initiateTestOutput = new InitiateTestOutput();
                try
                {
                    initiateTestOutput = initiateTestTaskActionArgument.CLITestConnector.InitiateTest(initiateTestContext);
                }
                catch (Exception ex)
                {
                    initiateTestOutput.Result = InitiateTestResult.FailedWithRetry;
                    initiateTestOutput.FailureMessage = ex.Message;
                }

                CallTestStatus callTestStatus;

                switch (initiateTestOutput.Result)
                {
                    case InitiateTestResult.Created:
                        callTestStatus = CallTestStatus.Initiated;
                        break;

                    case InitiateTestResult.FailedWithRetry:
                            callTestStatus = CallTestStatus.InitiationFailedWithRetry;
                            if (testCall.InitiationRetryCount < initiateTestTaskActionArgument.MaximumRetryCount)
                                testCall.InitiationRetryCount = testCall.InitiationRetryCount + 1;
                            else
                                callTestStatus = CallTestStatus.InitiationFailedWithNoRetry;
                            break;

                    case InitiateTestResult.FailedWithNoRetry:
                        callTestStatus = CallTestStatus.InitiationFailedWithNoRetry;
                        break;

                    default:
                        callTestStatus = CallTestStatus.InitiationFailedWithRetry;
                        break;
                }

                manager.UpdateInitiateTest(testCall.ID, initiateTestOutput.InitiateTestInformation, callTestStatus, testCall.InitiationRetryCount, initiateTestOutput.FailureMessage);
            }

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };

            return output;
        }
    }
}
