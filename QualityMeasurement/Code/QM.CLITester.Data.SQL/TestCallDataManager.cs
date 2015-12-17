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
        public bool Insert(TestCallQueryInsert testCall, out int insertedId)
        {
            object testCallId;
            bool rec = false;
            foreach (int supplierId in testCall.SupplierID)
            {
                int recordsEffected = ExecuteNonQuerySP("QM_CLITester.sp_TestCall_Insert", out testCallId, supplierId, testCall.CountryID, testCall.ZoneID,
                testCall.CallTestStatus, testCall.CallTestResult, testCall.InitiationRetryCount,
                testCall.GetProgressRetryCount, testCall.UserID, testCall.ProfileID);
                insertedId = (int)testCallId;
                if (recordsEffected > 0)
                    rec = true;
            }
            insertedId = 0;
            return rec;
        }

        public List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus)
        {
            string callTestStatusids = null;
            if (listCallTestStatus != null && listCallTestStatus.Any())
                callTestStatusids = string.Join<int>(",", Array.ConvertAll(listCallTestStatus.ToArray(), value => (int)value));
            return GetItemsSP("QM_CLITester.sp_TestCall_GetRequestedTestCall", TestCallMapper, callTestStatusids);
        }

        public List<TestCallDetail> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows)
        {
            List<TestCallDetail> listTestCalls = new List<TestCallDetail>();
            byte[] timestamp = null;

            ExecuteReaderSP("QM_CLITester.sp_TestCall_GetUpdated", (reader) =>
            {
                while (reader.Read())
                    listTestCalls.Add(TestCallDetailMapper(reader));
                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");
            },
               maxTimeStamp, nbOfRows);
            maxTimeStamp = timestamp;
            return listTestCalls;
        }
        public List<TestCallDetail> GetBeforeId(LastCallUpdateInput input)
        {
            List<TestCallDetail> listTestCalls = new List<TestCallDetail>();

            ExecuteReaderSP("QM_CLITester.sp_TestCall_GetBeforeID", (reader) =>
            {
                while (reader.Read())
                    listTestCalls.Add(TestCallDetailMapper(reader));
            },
               input.LessThanID, input.NbOfRows);
            return listTestCalls;
        }

        public bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus, int initiationRetryCount, string failureMessage)
        {
            int recordsEffected = ExecuteNonQuerySP("[QM_CLITester].[sp_TestCall_UpdateInitiateTest]", testCallId,
                initiateTestInformation != null ? Serializer.Serialize(initiateTestInformation) : null, callTestStatus, initiationRetryCount, failureMessage);
            return (recordsEffected > 0);
        }

        public bool UpdateTestProgress(long testCallId, Object testProgress, CallTestStatus callTestStatus, CallTestResult? callTestResult, int getProgressRetryCount, string failureMessage)
        {
            int recordsEffected = ExecuteNonQuerySP("QM_CLITester.sp_TestCall_UpdateTestProgress", testCallId,
                testProgress != null ? Serializer.Serialize(testProgress) : null, callTestStatus, callTestResult, getProgressRetryCount, failureMessage);
            return (recordsEffected > 0);
        }

        public Vanrise.Entities.BigResult<Entities.TestCallDetail> GetTestCallFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.TestCallQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("Entity.ID", "ID");

            string callTestStatusids = null;
            if (input.Query.CallTestStatus != null && input.Query.CallTestStatus.Any())
                callTestStatusids = string.Join<int>(",", Array.ConvertAll(input.Query.CallTestStatus.ToArray(), value => (int)value));

            string callTestResultsids = null;
            if (input.Query.CallTestResult != null && input.Query.CallTestResult.Any())
                callTestResultsids = string.Join<int>(",", Array.ConvertAll(input.Query.CallTestResult.ToArray(), value => (int)value));

            string userids = null;
            if (input.Query.UserIds != null && input.Query.UserIds.Any())
                userids = string.Join<int>(",", Array.ConvertAll(input.Query.UserIds.ToArray(), value => (int)value));

            string supplierids = null;
            if (input.Query.SupplierIds != null && input.Query.SupplierIds.Any())
                supplierids = string.Join<int>(",", Array.ConvertAll(input.Query.SupplierIds.ToArray(), value => (int)value));


            string profileids = null;
            if (input.Query.ProfileIds != null && input.Query.ProfileIds.Any())
                profileids = string.Join<int>(",", Array.ConvertAll(input.Query.ProfileIds.ToArray(), value => (int)value));

            string countryids = null;
            if (input.Query.CountryIds != null && input.Query.CountryIds.Any())
                countryids = string.Join<int>(",", Array.ConvertAll(input.Query.CountryIds.ToArray(), value => (int)value));

            string zoneids = null;
            if (input.Query.ZoneIds != null && input.Query.ZoneIds.Any())
                zoneids = string.Join<int>(",", Array.ConvertAll(input.Query.ZoneIds.ToArray(), value => (int)value));



            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("QM_CLITester.sp_TestCall_CreateTempByFiltered", tempTableName, callTestStatusids, callTestResultsids, userids, supplierids, profileids, countryids, zoneids,
                    input.Query.FromTime, input.Query.ToTime == DateTime.MinValue ? DateTime.Now : input.Query.ToTime);
            };

            return RetrieveData(input, createTempTableAction, TestCallDetailMapper, mapper);
        }


        TestCall TestCallMapper(IDataReader reader)
        {
            TestCall testCall = new TestCall
            {
                ID = (long)reader["ID"],
                SupplierID = (int)reader["SupplierID"],
                CountryID = (int)reader["CountryID"],
                ZoneID = (int)reader["ZoneID"],
                UserID = (int)reader["UserID"],
                ProfileID =  GetReaderValue<int> (reader,"ProfileID"),
                CreationDate = GetReaderValue<DateTime>(reader, "CreationDate"),
                CallTestStatus = GetReaderValue<CallTestStatus>(reader, "CallTestStatus"),
                CallTestResult = GetReaderValue<CallTestResult>(reader, "CallTestResult"),
                InitiationRetryCount = GetReaderValue<int>(reader, "InitiationRetryCount"),
                GetProgressRetryCount = GetReaderValue<int>(reader, "GetProgressRetryCount"),
                FailureMessage = reader["FailureMessage"] as string
            };

            string initiateTestInformationSerialized = reader["InitiateTestInformation"] as string;
            if (initiateTestInformationSerialized != null)
                testCall.InitiateTestInformation = Serializer.Deserialize(initiateTestInformationSerialized);

            string testProgressSerialized = reader["TestProgress"] as string;
            if (testProgressSerialized != null)
                testCall.TestProgress = Serializer.Deserialize(testProgressSerialized);

            return testCall;
        }

        TestCallDetail TestCallDetailMapper(IDataReader reader)
        {
            SupplierManager supplierManager = new SupplierManager();
            ZoneManager zoneManager = new ZoneManager();
            CountryManager countryManager = new CountryManager();
            UserManager userManager = new UserManager();

            return new TestCallDetail()
            {
                Entity = TestCallMapper(reader),
                CallTestStatusDescription = Utilities.GetEnumAttribute<CallTestStatus, DescriptionAttribute>((CallTestStatus)TestCallMapper(reader).CallTestStatus).Description,
                CallTestResultDescription = Utilities.GetEnumAttribute<CallTestResult, DescriptionAttribute>((CallTestResult)TestCallMapper(reader).CallTestResult).Description,
                SupplierName = supplierManager.GetSupplier(TestCallMapper(reader).SupplierID) == null ? "" : supplierManager.GetSupplier(TestCallMapper(reader).SupplierID).Name,
                UserName = userManager.GetUserbyId(TestCallMapper(reader).UserID) == null ? "" : userManager.GetUserbyId(TestCallMapper(reader).UserID).Name,
                CountryName = countryManager.GetCountry(TestCallMapper(reader).CountryID) == null ? "" : countryManager.GetCountry(TestCallMapper(reader).CountryID).Name,
                ZoneName = zoneManager.GetZone(TestCallMapper(reader).ZoneID) == null ? "" : zoneManager.GetZone(TestCallMapper(reader).ZoneID).Name,
            };
        }
    }
}
