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


            AddTestCallOutput testCallOutput = new AddTestCallOutput();

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            
            ZoneManager zoneManager = new ZoneManager();
            SupplierManager supplierManager = new SupplierManager();

            
            Zone zone = new Zone();
            List<Zone> zones = new List<Zone>();

            if (testCallInput.ZoneIds == null)
            {
                zone = zoneManager.GetZonebySourceId(testCallInput.ZoneSourceId);
                if (zone != null)
                {
                    zones.Add(zone);  

                }
            }
            else
            {
                foreach (long? zoneId in testCallInput.ZoneIds)
                {
                    Zone z = zoneManager.GetZone(zoneId.Value);
                    zones.Add(z);
                }
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


            long batchNumber;
            IDManager.Instance.ReserveIDRange(this.GetType(), listSuppliersIds.Count, out batchNumber);

            foreach (int supplierId in listSuppliersIds)
                foreach (Zone z in zones)
                    dataManager.Insert(supplierId, z.CountryId, z.ZoneId, (int)CallTestStatus.New, (int)CallTestResult.NotCompleted, 0, 0,
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

            SettingManager settingManager = new SettingManager();
            int numberOfMinutes = settingManager.GetSetting<LastTestCallsSettingsData>(Constants.LastTestCallSettings).LastTestCall;

            List<TestCall> listTestCalls = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId(), numberOfMinutes);
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

            TestCallExcelExportHandler testCallExcel = new TestCallExcelExportHandler(input.Query);

            ResultProcessingHandler<TestCallDetail> handler = new ResultProcessingHandler<TestCallDetail>()
            {
                ExportExcelHandler = testCallExcel
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, testBigResult, handler);
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

         #region Private Members

        private class TestCallRequestHandler : BigDataRequestHandler<TestCallQuery, TestCall, TestCallDetail>
        {
            public override IEnumerable<TestCall> RetrieveAllData(DataRetrievalInput<TestCallQuery> input)
            {
                throw new ArgumentException("query.ReportType is invalid");
            }
            protected override ResultProcessingHandler<TestCallDetail> GetResultProcessingHandler(DataRetrievalInput<TestCallQuery> input, BigResult<TestCallDetail> bigResult)
            {
                return new ResultProcessingHandler<TestCallDetail>
                {
                    ExportExcelHandler = new TestCallExcelExportHandler(input.Query)
                };
            }

            public override TestCallDetail EntityDetailMapper(TestCall entity)
            {
                return new TestCallDetail()
                {
                    Entity = entity
                };
            }
        }

        private class TestCallExcelExportHandler : ExcelExportHandler<TestCallDetail>
        {
            private TestCallQuery _query;
            public TestCallExcelExportHandler(TestCallQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<TestCallDetail> context)
            {
                if (context.BigResult == null)
                    throw new ArgumentNullException("context.BigResult");
                if (context.BigResult.Data == null)
                    throw new ArgumentNullException("context.BigResult.Data");
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "User Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Creation Date" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "PDD" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "MOS" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Batch Number" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Schedule Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Call Test Status" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Call Test Result" });

                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.Rows.Add(row);
                    if (record.Entity != null)
                        row.Cells.Add(new ExportExcelCell { Value = record.Entity.ID });
                    row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
                    row.Cells.Add(new ExportExcelCell { Value = record.UserName });
                    row.Cells.Add(new ExportExcelCell { Value = record.CountryName });
                    row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });
                    if (record.Entity != null)
                    {
                        row.Cells.Add(new ExportExcelCell { Value = record.Entity.CreationDate });
                        row.Cells.Add(new ExportExcelCell { Value = record.Entity.Measure.Pdd });
                        row.Cells.Add(new ExportExcelCell { Value = record.Entity.Measure.Mos });
                        row.Cells.Add(new ExportExcelCell { Value = record.Entity.BatchNumber });    
                    }
                    row.Cells.Add(new ExportExcelCell { Value = record.ScheduleName });
                    row.Cells.Add(new ExportExcelCell { Value = record.CallTestStatusDescription });
                    row.Cells.Add(new ExportExcelCell { Value = record.CallTestResultDescription });
                }
                context.MainSheet = sheet;
            }
        }


         #endregion
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
