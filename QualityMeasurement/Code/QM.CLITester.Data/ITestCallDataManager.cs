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
        bool Insert(int supplierId, int countryId, int zoneId, int callTestStatus, int callTestResult,
            int initiationRetryCount, int getProgressRetryCount, int userId, int profileId);
        List<TestCallDetail> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int userId);
        List<TestCallDetail> GetBeforeId(GetBeforeIdInput input);
        List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus);
        bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus, int initiationRetryCount, string failureMessage);
        bool UpdateTestProgress(long testCallId, Object testProgress, CallTestStatus callTestStatus, CallTestResult? callTestResult, int getProgressRetryCount, string failureMessage);
        Vanrise.Entities.BigResult<Entities.TestCallDetail> GetTestCallFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.TestCallQuery> input);
    }
}
