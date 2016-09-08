using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Data.SQL;
using Vanrise.Security.Business;
using QM.BusinessEntity.Business;
using QM.CLITester.Entities;

namespace QM.CLITester.Data.SQL
{
    public class TestCallDataManager : BaseSQLDataManager, ITestCallDataManager
    {
        public TestCallDataManager() :
            base(GetConnectionStringName("QM_CLITester_DBConnStringKey", "QM_CLITester_DBConnString"))
        {

        }

        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        static TestCallDataManager()
        {
            _columnMapper.Add("SupplierName", "SupplierId");
            _columnMapper.Add("UserName", "UserId");
            _columnMapper.Add("CountryName", "CountryId");
            _columnMapper.Add("ZoneName", "ZoneId");
            _columnMapper.Add("CallTestStatusDescription", "CallTestStatus");
            _columnMapper.Add("CallTestResultDescription", "CallTestResult");
            _columnMapper.Add("Entity.ID", "ID");
            _columnMapper.Add("Entity.CreationDate", "CreationDate");
        }

        public bool Insert(int supplierId, int countryId, long zoneId, int callTestStatus, int callTestResult, int initiationRetryCount, int getProgressRetryCount,
            int userId, int profileId, long? batchNumber, int? scheduleId, int quantity)
        {
            object testCallId;

            int recordsEffected = ExecuteNonQuerySP("QM_CLITester.sp_TestCall_Insert", out testCallId, supplierId, countryId, zoneId,
                    callTestStatus, callTestResult, initiationRetryCount, getProgressRetryCount, userId, profileId, batchNumber, scheduleId, quantity);
            return (recordsEffected > 0);
        }

        public List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus)
        {
            string callTestStatusids = null;
            if (listCallTestStatus != null && listCallTestStatus.Any())
                callTestStatusids = string.Join<int>(",", Array.ConvertAll(listCallTestStatus.ToArray(), value => (int)value));
            return GetItemsSP("QM_CLITester.sp_TestCall_GetRequestedTestCall", TestCallMapper, callTestStatusids);
        }

        public List<TestCall> GetAllbyBatchNumber(long batchNumber)
        {
            return GetItemsSP("QM_CLITester.sp_TestCall_GetAllbyBatchNumber", TestCallMapper, batchNumber);
        }

        public List<TestCall> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int userId, int numberOfMinutes)
        {
            List<TestCall> listTestCalls = new List<TestCall>();
            byte[] timestamp = null;

            ExecuteReaderSP("QM_CLITester.sp_TestCall_GetUpdated", (reader) =>
            {
                while (reader.Read())
                    listTestCalls.Add(TestCallMapper(reader));
                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");
            },
               maxTimeStamp, nbOfRows, userId, numberOfMinutes);
            if (timestamp != null)
                maxTimeStamp = timestamp;
            return listTestCalls;
        }
        public List<TestCall> GetBeforeId(GetBeforeIdInput input)
        {
            return GetItemsSP("[QM_CLITester].[sp_TestCall_GetBeforeID]", TestCallMapper, input.LessThanID, input.NbOfRows, input.UserId);
        }

        public List<TotalCallsChart> GetTotalCallsByUserId(int userId)
        {
            return GetItemsSP("[QM_CLITester].[sp_TestCall_GetTotalCallsByUserId]", TotalCallsByUserIdMapper, userId);
        }

        public bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus, int initiationRetryCount, string failureMessage)
        {
            int recordsEffected = ExecuteNonQuerySP("QM_CLITester.sp_TestCall_UpdateInitiateTest", testCallId,
                initiateTestInformation != null ? Serializer.Serialize(initiateTestInformation) : null, callTestStatus, initiationRetryCount, failureMessage);
            return (recordsEffected > 0);
        }

        public bool UpdateTestProgress(long testCallId, Object testProgress, Measure measure, CallTestStatus callTestStatus, CallTestResult? callTestResult, int getProgressRetryCount, string failureMessage)
        {
            int recordsEffected = ExecuteNonQuerySP("QM_CLITester.sp_TestCall_UpdateTestProgress", testCallId, testProgress != null ? Serializer.Serialize(testProgress) : null,
                callTestStatus, callTestResult, getProgressRetryCount, failureMessage, measure.Pdd, measure.Mos, measure.Duration, measure.RingDuration);
            return (recordsEffected > 0);
        }

        public Vanrise.Entities.BigResult<TestCall> GetTestCallFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<TestCallQuery> input)
        {
            string callTestStatusids = null;
            if (input.Query.CallTestStatus != null && input.Query.CallTestStatus.Any())
                callTestStatusids = string.Join(",", input.Query.CallTestStatus.Select(itm => (int)itm));

            string callTestResultsids = null;
            if (input.Query.CallTestResult != null && input.Query.CallTestResult.Any())
                callTestResultsids = string.Join(",", input.Query.CallTestResult.Select(itm => (int)itm));

            string userids = null;
            if (input.Query.UserIds != null && input.Query.UserIds.Any())
                userids = string.Join(",", input.Query.UserIds);

            string supplierids = null;
            if (input.Query.SupplierIds != null && input.Query.SupplierIds.Any())
                supplierids = string.Join(",", input.Query.SupplierIds);


            string profileids = null;
            if (input.Query.ProfileIds != null && input.Query.ProfileIds.Any())
                profileids = string.Join(",", input.Query.ProfileIds);

            string scheduleids = null;
            if (input.Query.ScheduleIds != null && input.Query.ScheduleIds.Any())
                scheduleids = string.Join(",", input.Query.ScheduleIds);

            string countryids = null;
            if (input.Query.CountryIds != null && input.Query.CountryIds.Any())
                countryids = string.Join(",", input.Query.CountryIds);

            string zoneids = null;
            if (input.Query.ZoneIds != null && input.Query.ZoneIds.Any())
                zoneids = string.Join(",", input.Query.ZoneIds);

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("QM_CLITester.sp_TestCall_CreateTempByFiltered", tempTableName, callTestStatusids, callTestResultsids, userids, supplierids, profileids, scheduleids, countryids, zoneids,
                    input.Query.FromTime, input.Query.ToTime == DateTime.MinValue ? DateTime.Now : input.Query.ToTime);
            };

            return RetrieveData(input, createTempTableAction, TestCallMapper, _columnMapper);
        }

        TestCall TestCallMapper(IDataReader reader)
        {
            Measure measure = new Measure
            {
                Pdd = GetReaderValue<decimal>(reader, "PDD"),
                Mos = GetReaderValue<decimal>(reader, "MOS"),
                Duration =  GetReaderValue<DateTime?>(reader, "Duration"),
                //ReleaseCode = reader["ReleaseCode"] as string,
                //ReceivedCli = reader["ReceivedCLI"] as string,
                RingDuration = reader["RingDuration"] as string,
            };

            TestCall testCall = new TestCall
            {
                ID = (long)reader["ID"],
                SupplierID = (int)reader["SupplierID"],
                CountryID = (int)reader["CountryID"],
                ZoneID = (long)reader["ZoneID"],
                UserID = (int)reader["UserID"],
                ProfileID =  GetReaderValue<int> (reader,"ProfileID"),
                CreationDate = GetReaderValue<DateTime>(reader, "CreationDate"),
                CallTestStatus = GetReaderValue<CallTestStatus>(reader, "CallTestStatus"),
                CallTestResult = GetReaderValue<CallTestResult>(reader, "CallTestResult"),
                InitiationRetryCount = GetReaderValue<int>(reader, "InitiationRetryCount"),
                GetProgressRetryCount = GetReaderValue<int>(reader, "GetProgressRetryCount"),
                Measure = measure,
                FailureMessage = reader["FailureMessage"] as string,
                BatchNumber = GetReaderValue<long>(reader, "BatchNumber"),
                ScheduleId = GetReaderValue<int>(reader, "ScheduleID"),
                Quantity = GetReaderValue<int>(reader, "Quantity")
            };

            string initiateTestInformationSerialized = reader["InitiateTestInformation"] as string;
            if (initiateTestInformationSerialized != null)
                testCall.InitiateTestInformation = Serializer.Deserialize(initiateTestInformationSerialized);

            string testProgressSerialized = reader["TestProgress"] as string;
            if (testProgressSerialized != null)
                testCall.TestProgress = Serializer.Deserialize(testProgressSerialized);

            return testCall;
        }

        TotalCallsChart TotalCallsByUserIdMapper(IDataReader reader)
        {
            return new TotalCallsChart()
            {
                CreationDate = reader["CreationDate"] as string,
                TotalCalls = (int)reader["TotalCalls"]
            };
        }
    }
}
