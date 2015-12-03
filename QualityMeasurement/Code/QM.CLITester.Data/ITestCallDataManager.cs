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
        bool Insert(TestCallResult carrierAccount, out int carrierAccountId);
        List<TestCallResult> GetTestCalls();

        List<TestCallResult> GetRequestedTestCalls();
        List<TestCallResult> GetRequestedTestCallResults();
        bool Update(TestCallResult testCallResult);
    }
}
