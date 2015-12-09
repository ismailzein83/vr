using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.CLITester.Entities;
using Vanrise.Data.SQL;
using System.Data;
using Vanrise.Common;

namespace QM.CLITester.Data.SQL
{
    public class TestCallDataManager : BaseSQLDataManager, ITestCallDataManager
    {
        public bool Insert(TestCall testCall, out int insertedId)
        {
            object testCallId;

            int recordsEffected = ExecuteNonQuerySP("QM_CLITester.sp_TestCall_Insert", out testCallId, testCall.SupplierID, testCall.CountryID, testCall.ZoneID, 
                testCall.InitiateTestInformation, testCall.TestProgress, testCall.CallTestStatus, testCall.CallTestResult);
            insertedId = (int)testCallId;
            return (recordsEffected > 0);
        }

        public List<TestCall> GetTestCalls()
        {
            return GetItemsSP("QM_CLITester.sp_TestCall_GetAll", TestCallMapper);
        }

        public List<TestCall> GetTestCalls(List<int> listCallTestStatus)
        {
            string callTestStatusids = null;
            if (listCallTestStatus.Any())
                callTestStatusids = string.Join<int>(",", listCallTestStatus);
            return GetItemsSP("QM_CLITester.sp_TestCall_GetRequestedTestCall", TestCallMapper, callTestStatusids);
        }

        public List<TestCall> GetUpdatedTestCalls(ref object maxTimeStamp)
        {
            return GetItemsSP("QM_CLITester.sp_TestCall_GetAll", TestCallMapper, maxTimeStamp);
        }

        public bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus)
        {
            int recordsEffected = ExecuteNonQuerySP("[QM_CLITester].[sp_TestCall_UpdateInitiateTest]", testCallId,
                initiateTestInformation != null ? Serializer.Serialize(initiateTestInformation) : null, callTestStatus);
            return (recordsEffected > 0);
        }

        public bool UpdateTestProgress(long testCallId, Object testProgress, CallTestStatus callTestStatus, CallTestResult? callTestResult)
        {
            int recordsEffected = ExecuteNonQuerySP("[QM_CLITester].[sp_TestCall_UpdateTestProgress]", testCallId,
                testProgress != null ? Serializer.Serialize(testProgress) : null, callTestStatus, callTestResult);
            return (recordsEffected > 0);
        }

        TestCall TestCallMapper(IDataReader reader)
        {
            TestCall testCall = new TestCall
            {
                ID = (long)reader["ID"],
                SupplierID = (int)reader["SupplierID"],
                CountryID = (int)reader["CountryID"],
                ZoneID = (int)reader["ZoneID"],
                CreationDate = GetReaderValue<DateTime>(reader, "CreationDate"),
                CallTestStatus = GetReaderValue<CallTestStatus>(reader, "CallTestStatus"),
                CallTestResult = GetReaderValue<CallTestResult>(reader, "CallTestResult")
            };

            string initiateTestInformationSerialized = reader["InitiateTestInformation"] as string;
            if (initiateTestInformationSerialized != null)
                testCall.InitiateTestInformation = Serializer.Deserialize(initiateTestInformationSerialized);
            
            string testProgressSerialized = reader["TestProgress"] as string;
            if (testProgressSerialized != null)
                testCall.TestProgress = Serializer.Deserialize(testProgressSerialized);
            
            return testCall;
        }
    }
}
