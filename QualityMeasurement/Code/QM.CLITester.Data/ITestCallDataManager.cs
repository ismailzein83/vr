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
        bool Insert(TestCallQueryInsert carrierAccount, out int carrierAccountId);
        List<TestCallDetail> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows);
        List<TestCallDetail> GetBeforeId(LastCallUpdateInput input);
        List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus);
        bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus, int initiationRetryCount, string failureMessage);
        bool UpdateTestProgress(long testCallId, Object testProgress, CallTestStatus callTestStatus, CallTestResult? callTestResult, int getProgressRetryCount, string failureMessage);
        Vanrise.Entities.BigResult<Entities.TestCallDetail> GetTestCallFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.TestCallQuery> input);
    }
}
