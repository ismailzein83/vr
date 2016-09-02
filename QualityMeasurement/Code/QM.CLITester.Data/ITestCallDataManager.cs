using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.CLITester.Entities;

namespace QM.CLITester.Data
{
    public interface ITestCallDataManager : IDataManager
    {
        bool Insert(int supplierId, int countryId, long zoneId, int callTestStatus, int callTestResult,
            int initiationRetryCount, int getProgressRetryCount, int userId, int profileId, long? batchNumber, int? scheduleId);
        List<TestCall> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int userId, int numberOfMinutes);
        List<TestCall> GetBeforeId(GetBeforeIdInput input);
        List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus);
        List<TotalCallsChart> GetTotalCallsByUserId(int userId);
        List<TestCall> GetAllbyBatchNumber(long batchNumber);
        bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus, int initiationRetryCount, string failureMessage);
        bool UpdateTestProgress(long testCallId, Object testProgress, Measure measure, CallTestStatus callTestStatus, CallTestResult? callTestResult, int getProgressRetryCount, string failureMessage);
        Vanrise.Entities.BigResult<Entities.TestCall> GetTestCallFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.TestCallQuery> input);
    }
}
