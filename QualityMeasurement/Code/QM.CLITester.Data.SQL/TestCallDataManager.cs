using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.CLITester.Entities;
using Vanrise.Data.SQL;
using System.Data;

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

        public List<TestCall> GetTestCalls(int callTestStatus)
        {
            return GetItemsSP("QM_CLITester.sp_TestCall_GetRequestedTestCall", TestCallMapper, callTestStatus);
        }

        //public List<TestCallResult> GetRequestedTestCallResults()
        //{
        //    return GetItemsSP("QM_CLITester.sp_TestCall_GetRequestedTestCallResult", TestCallMapper);
        //}

        public bool UpdateInitiateTest(string initiateTestOutput, CallTestStatus callTestStatus, long testCallID)
        {
            int recordsEffected = ExecuteNonQuerySP("[QM_CLITester].[sp_TestCall_UpdateInitiateTest]", testCallID,
                initiateTestOutput, callTestStatus);
            return (recordsEffected > 0);
        }

        public bool UpdateTestProgress(string initiateTestOutput, CallTestResult callTestResult, long testCallID)
        {
            int recordsEffected = ExecuteNonQuerySP("[QM_CLITester].[sp_TestCall_UpdateTestProgress]", testCallID,
                initiateTestOutput, callTestResult);
            return (recordsEffected > 0);
        }

        TestCall TestCallMapper(IDataReader reader)
        {
            TestCall testCallResult = new TestCall
            {
                ID = (long)reader["ID"],
                SupplierID = (int)reader["SupplierID"],
                CountryID = (int)reader["CountryID"],
                ZoneID = (int)reader["ZoneID"],
                CreationDate = GetReaderValue<DateTime>(reader, "CreationDate"),
                InitiateTestInformation = reader["InitiateTestInformation"] as string,
                TestProgress = reader["TestProgress"] as string,
                CallTestStatus = GetReaderValue<int?>(reader, "CallTestStatus"),
                CallTestResult = GetReaderValue<int?>(reader, "CallTestResult")
            };
            return testCallResult;
        }
    }
}
