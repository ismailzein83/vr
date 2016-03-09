using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using QM.CLITester.Business.CLITestTasks;

namespace QM.CLITester.Business
{
    public class TestCallManager
    {
        public AddTestCallOutput AddNewTestCall(AddTestCallInput testCallInput)
        {
            long batchNumber;
            IDManager.Instance.ReserveIDRange(this.GetType(), testCallInput.SuppliersIds.Count, out batchNumber);

            AddTestCallOutput testCallOutput = new AddTestCallOutput();

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            
            ZoneManager zoneManager = new ZoneManager();
            SupplierManager supplierManager = new SupplierManager();
            
            
            Zone zone = new Zone();
            List<long?> zoneIds = new List<long?>();
            int countryId = 0;
            if(testCallInput.CountryID != null)
                countryId = testCallInput.CountryID.Value;

            if (testCallInput.ZoneIds == null)
            {
                zone = zoneManager.GetZonebySourceId(testCallInput.ZoneSourceId);
                if (zone != null)
                {
                    zoneIds.Add(zone.ZoneId);
                    countryId = zone.CountryId;   
                }
            }
            else
            {
                zoneIds = testCallInput.ZoneIds;
            }

            List<int?> listSuppliersIds = new List<int?>();

            if (testCallInput.SuppliersIds != null && testCallInput.SuppliersIds.Count > 0)
            {
                listSuppliersIds = testCallInput.SuppliersIds;
            }
            else
            {
                foreach (string suppliersSourceId in testCallInput.SuppliersSourceIds)
                {
                    Supplier supplier = supplierManager.GetSupplierbySourceId(suppliersSourceId);
                    listSuppliersIds.Add(supplier.SupplierId);
                }
            }

            foreach (int supplierId in listSuppliersIds)
                foreach (long zoneId in zoneIds)
                    dataManager.Insert(supplierId, countryId, zoneId, (int)CallTestStatus.New, (int)CallTestResult.NotCompleted, 0, 0,
                                        testCallInput.UserId, testCallInput.ProfileID, batchNumber, testCallInput.ScheduleId);

            testCallOutput.BatchNumber = batchNumber;
            return testCallOutput;
        }

        public bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus, int initiationRetryCount, string failureMessage)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateInitiateTest(testCallId, initiateTestInformation, callTestStatus, initiationRetryCount, failureMessage);
        }

        public bool UpdateTestProgress(long testCallId, Object testProgress, Measure measure, CallTestStatus callTestStatus, CallTestResult? callTestResult, int getProgressRetryCount, string failureMessage)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateTestProgress(testCallId, testProgress, measure, callTestStatus, callTestResult, getProgressRetryCount, failureMessage);
        }

        public LastCallUpdateOutput GetUpdated(ref byte[] maxTimeStamp, int nbOfRows)
        {
            LastCallUpdateOutput lastCallUpdateOutputs = new LastCallUpdateOutput();

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();

            List<TestCall> listTestCalls = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId());
            List<TestCallDetail> listTestCallDetails = new List<TestCallDetail>();
            foreach (TestCall testCall in listTestCalls)
            {
                listTestCallDetails.Add(TestCallDetailMapper(testCall));
            }

            lastCallUpdateOutputs.ListTestCallDetails = listTestCallDetails;
            lastCallUpdateOutputs.MaxTimeStamp = maxTimeStamp;
            return lastCallUpdateOutputs;
        }

        public List<TestCallDetail> GetBeforeId(GetBeforeIdInput input)
        {
            input.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();

            List<TestCall> listTestCalls = dataManager.GetBeforeId(input);
            List<TestCallDetail> listTestCallDetails = new List<TestCallDetail>();
            foreach (TestCall testCall in listTestCalls)
            {
                listTestCallDetails.Add(TestCallDetailMapper(testCall));
            }
            return listTestCallDetails;
        }


        public List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.GetTestCalls(listCallTestStatus);
        }
        public List<TotalCallsChart> GetTotalCallsByUserId()
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.GetTotalCallsByUserId(Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId());
        }

        public List<TestCallDetail> GetAllbyBatchNumber(long batchNumber)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();

            List<TestCall> listTestCalls = dataManager.GetAllbyBatchNumber(batchNumber);
            List<TestCallDetail> listTestCallDetails = new List<TestCallDetail>();
            foreach (TestCall testCall in listTestCalls)
            {
                listTestCallDetails.Add(TestCallDetailMapper(testCall));
            }
            return listTestCallDetails;
        }

        public IDataRetrievalResult<TestCallDetail> GetFilteredTestCalls(DataRetrievalInput<TestCallQuery> input)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            
            Vanrise.Entities.BigResult<TestCall> testCalls= dataManager.GetTestCallFilteredFromTemp(input);
             Vanrise.Entities.BigResult<TestCallDetail> testBigResult = new BigResult<TestCallDetail>();

            List<TestCallDetail> listTestCallDetails = new List<TestCallDetail>();

            foreach (TestCall testCall in testCalls.Data)
            {
                listTestCallDetails.Add(TestCallDetailMapper(testCall));
            }
            testBigResult.Data = listTestCallDetails;
            testBigResult.ResultKey = testCalls.ResultKey;
            testBigResult.TotalCount = testCalls.TotalCount;
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, testBigResult);
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


        TestCallDetail TestCallDetailMapper(TestCall testCall)
        {
            SupplierManager supplierManager = new SupplierManager();
            Supplier supplier = supplierManager.GetSupplier(testCall.SupplierID);
            
            ZoneManager zoneManager = new ZoneManager();
            Zone zone = zoneManager.GetZone(testCall.ZoneID);
            
            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(testCall.CountryID);

            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(testCall.UserID);

            SchedulerTaskManager schedulerTaskManager = new SchedulerTaskManager();
            SchedulerTask schedulerTask = schedulerTaskManager.GetTask(testCall.ScheduleId); 

            return new TestCallDetail()
            {
                Entity = testCall,
                CallTestStatusDescription = Utilities.GetEnumAttribute<CallTestStatus, DescriptionAttribute>(testCall.CallTestStatus).Description,
                CallTestResultDescription = Utilities.GetEnumAttribute<CallTestResult, DescriptionAttribute>(testCall.CallTestResult).Description,
                SupplierName = supplier == null ? "" : supplier.Name,
                UserName = user == null ? "" : user.Name,
                CountryName = country == null ? "" : country.Name,
                ZoneName = zone == null ? "" : zone.Name,
                ScheduleName = schedulerTask == null ? "" : schedulerTask.Name
            };
        }

    }
}
