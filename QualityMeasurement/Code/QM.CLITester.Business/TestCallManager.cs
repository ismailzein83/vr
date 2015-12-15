using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using QM.CLITester.Data;
using QM.CLITester.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace QM.CLITester.Business
{
    public class TestCallManager
    {
        public Vanrise.Entities.InsertOperationOutput<TestCallQueryInsert> AddNewTestCall(TestCallQueryInsert testCallResult)
        {
            testCallResult.CallTestStatus = CallTestStatus.New;
            testCallResult.CallTestResult = CallTestResult.NotCompleted;
            testCallResult.InitiationRetryCount = 0;
            testCallResult.GetProgressRetryCount = 0;
            testCallResult.UserID = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            Vanrise.Entities.InsertOperationOutput<TestCallQueryInsert> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<TestCallQueryInsert>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int testCallId = -1;

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            bool insertActionSucc = dataManager.Insert(testCallResult, out testCallId);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = testCallResult;
            }

            return insertOperationOutput;
        }

        public bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus, int initiationRetryCount, string failureMessage)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateInitiateTest(testCallId, initiateTestInformation, callTestStatus, initiationRetryCount, failureMessage);
        }

        public bool UpdateTestProgress(long testCallId, Object testProgress, CallTestStatus callTestStatus, CallTestResult? callTestResult, int getProgressRetryCount, string failureMessage)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateTestProgress(testCallId, testProgress, callTestStatus, callTestResult, getProgressRetryCount, failureMessage);
        }

        public LastCallUpdateOutput GetUpdatedTestCalls(ref byte[] maxTimeStamp)
        {
            LastCallUpdateOutput lastCallUpdateOutputs = new LastCallUpdateOutput();
            List<CallTestStatus> listPendingCallTestStatus = new List<CallTestStatus>();
            listPendingCallTestStatus.Add(CallTestStatus.Initiated);
            listPendingCallTestStatus.Add(CallTestStatus.InitiationFailedWithRetry);
            listPendingCallTestStatus.Add(CallTestStatus.New);
            listPendingCallTestStatus.Add(CallTestStatus.PartiallyCompleted);
            listPendingCallTestStatus.Add(CallTestStatus.GetProgressFailedWithRetry);

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            lastCallUpdateOutputs.ListTestCallDetails = dataManager.GetUpdatedTestCalls(ref maxTimeStamp, listPendingCallTestStatus);
            lastCallUpdateOutputs.MaxTimeStamp = maxTimeStamp;
            return lastCallUpdateOutputs;
        }

        public List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.GetTestCalls(listCallTestStatus);
        }

        public IDataRetrievalResult<TestCallDetail> GetFilteredTestCalls(DataRetrievalInput<TestCallQuery> input)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            Vanrise.Entities.BigResult<TestCallDetail> testCallDetails = dataManager.GetTestCallFilteredFromTemp(input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, testCallDetails);
        }

        public List<Vanrise.Entities.TemplateConfig> GetInitiateTestTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CliTesterConnectorInitiateTest);
        }

        public List<Vanrise.Entities.TemplateConfig> GetTestProgressTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CliTesterConnectorTestProgress);
        }
    }

}
