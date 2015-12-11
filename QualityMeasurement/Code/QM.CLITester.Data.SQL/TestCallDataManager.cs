using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.CLITester.Entities;
using Vanrise.Data.SQL;
using System.Data;
using System.Reflection;
using Vanrise.Common;
namespace QM.CLITester.Data.SQL
{
    public class TestCallDataManager : BaseSQLDataManager, ITestCallDataManager
    {
        public bool Insert(TestCall testCall, out int insertedId)
        {
            object testCallId;

            int recordsEffected = ExecuteNonQuerySP("QM_CLITester.sp_TestCall_Insert", out testCallId, testCall.SupplierID, testCall.CountryID, testCall.ZoneID, 
                testCall.InitiateTestInformation, testCall.TestProgress, testCall.CallTestStatus, testCall.CallTestResult, testCall.InitiationRetryCount, 
                testCall.GetProgressRetryCount, testCall.UserID);
            insertedId = (int)testCallId;
            return (recordsEffected > 0);
        }

        public List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus)
        {
            string callTestStatusids = null;
            if (listCallTestStatus.Any())
                callTestStatusids = string.Join<int>(",", Array.ConvertAll(listCallTestStatus.ToArray(), value => (int)value));
            return GetItemsSP("QM_CLITester.sp_TestCall_GetRequestedTestCall", TestCallMapper, callTestStatusids);
        }

        public List<TestCallDetail> GetUpdatedTestCalls(ref byte[] maxTimeStamp, List<CallTestStatus> listPendingCallTestStatus)
        {
            List<TestCallDetail> listTestCalls = new List<TestCallDetail>();
            byte[] timestamp = null;

            string callTestStatusids = null;
            if (listPendingCallTestStatus.Any())
                callTestStatusids = string.Join<int>(",", Array.ConvertAll(listPendingCallTestStatus.ToArray(), value => (int)value));

            ExecuteReaderSP("QM_CLITester.sp_TestCall_GetRecent", (reader) =>
            {
                while (reader.Read())
                    listTestCalls.Add(TestCallDetailMapper(reader));

                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");
            },
               maxTimeStamp, callTestStatusids);
            maxTimeStamp = timestamp;
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
            Dictionary<string,string> mapper=new Dictionary<string, string>();
            mapper.Add("Entity.ID","ID");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("QM_CLITester.sp_TestCall_CreateTempByFiltered", tempTableName, input.Query.SupplierID, input.Query.CountryID, input.Query.ZoneID, input.Query.FromTime, input.Query.ToTime);
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
                CreationDate = GetReaderValue<DateTime>(reader, "CreationDate"),
                CallTestStatus = GetReaderValue<CallTestStatus>(reader, "CallTestStatus"),
                CallTestResult = GetReaderValue<CallTestResult>(reader, "CallTestResult"),
                InitiationRetryCount = reader["InitiationRetryCount"] == "" ? 0 : (int)reader["InitiationRetryCount"],
                GetProgressRetryCount = reader["GetProgressRetryCount"] == "" ? 0 : (int)reader["GetProgressRetryCount"],
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
            return new TestCallDetail()
            {
                Entity = TestCallMapper(reader),
                CallTestResultDescription = TestCallMapper(reader).CallTestResult.ToString(),
                CallTestStatusDescription = TestCallMapper(reader).CallTestStatus.ToString()
            };
        }
    }
}
