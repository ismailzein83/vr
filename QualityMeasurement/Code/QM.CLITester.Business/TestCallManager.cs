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
        public Vanrise.Entities.InsertOperationOutput<TestCallQueryInput> AddNewTestCall(TestCallQueryInput testCallResult)
        {
            Vanrise.Entities.InsertOperationOutput<TestCallQueryInput> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<TestCallQueryInput>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            bool insertActionSucc = true;

            foreach (int supplierId in testCallResult.SupplierID)
            {
                if (!dataManager.Insert(supplierId, testCallResult.CountryID, testCallResult.ZoneID, (int)CallTestStatus.New, (int)CallTestResult.NotCompleted, 0,0,
                    Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId(),testCallResult.ProfileID))
                    insertActionSucc = false;
            }
            
            if (insertActionSucc)
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;

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

        public LastCallUpdateOutput GetUpdated(ref byte[] maxTimeStamp, int nbOfRows)
        {
            LastCallUpdateOutput lastCallUpdateOutputs = new LastCallUpdateOutput();

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            lastCallUpdateOutputs.ListTestCallDetails = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows);
            lastCallUpdateOutputs.MaxTimeStamp = maxTimeStamp;
            return lastCallUpdateOutputs;
        }

        public List<TestCallDetail> GetBeforeId(GetBeforeIdInput input)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return  dataManager.GetBeforeId(input);
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
