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
        public bool Insert(TestCallResult testCallResult, out int insertedId)
        {
            object testCallId;

            int recordsEffected = ExecuteNonQuerySP("QM_CLITester.sp_TestCall_Insert", out testCallId, testCallResult.SupplierID, testCallResult.CountryID, testCallResult.ZoneID, testCallResult.Test_ID, testCallResult.Name, testCallResult.Calls_Total,
                testCallResult.Calls_Complete, testCallResult.CLI_Success, testCallResult.CLI_No_Result, testCallResult.CLI_Fail, testCallResult.PDD, testCallResult.Status);
            insertedId = (int)testCallId;
            return (recordsEffected > 0);
        }

        public List<TestCallResult> GetTestCalls()
        {
            return GetItemsSP("QM_CLITester.sp_TestCall_GetAll", TestCallMapper);
        }
        TestCallResult TestCallMapper(IDataReader reader)
        {
            TestCallResult testCallResult = new TestCallResult
            {
                Id = (int)reader["ID"],
                SupplierID = (int)reader["SupplierID"],
                CountryID = (int)reader["CountryID"],
                ZoneID = (int)reader["ZoneID"],
                CreationDate = GetReaderValue<DateTime>(reader, "CreationDate"),
                Test_ID = reader["Test_ID"] as string,
                Name = reader["Name"] as string,
                Calls_Total = (int)reader["Calls_Total"],
                Calls_Complete = (int)reader["Calls_Complete"],
                CLI_Success = (int)reader["CLI_Success"],
                CLI_No_Result = (int)reader["CLI_No_Result"],
                CLI_Fail = (int)reader["CLI_Fail"],
                PDD = (int)reader["PDD"],
                Share_URL = reader["Share_URL"] as string,
                Status = (int)reader["Status"]
            };
            return testCallResult;
        }
    }
}
