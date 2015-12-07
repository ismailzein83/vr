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

        List<TestCall> GetTestCalls(int callTestStatus);
        //List<TestCall> GetRequestedTestCallResults();
        bool UpdateInitiateTest(string initiateTestOutput, CallTestStatus callTestStatus, long testCallID);
        bool UpdateTestProgress(string initiateTestOutput, CallTestResult callTestStatus, long testCallID);
    }
}
