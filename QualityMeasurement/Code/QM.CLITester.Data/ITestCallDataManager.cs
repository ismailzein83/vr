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
        bool Insert(TestCall carrierAccount, out int carrierAccountId);
        List<TestCall> GetTestCalls();

        List<TestCall> GetTestCalls(List<int> callTestStatus);
        //List<TestCall> GetRequestedTestCallResults();
        bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus);
        bool UpdateTestProgress(long testCallId, Object testProgress, CallTestStatus callTestStatus, CallTestResult? callTestResult);
    }
}
