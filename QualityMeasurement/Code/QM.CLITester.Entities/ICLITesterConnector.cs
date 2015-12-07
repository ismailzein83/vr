using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{    
    public interface ICLITesterConnector
    {
        InitiateTestOutput InitiateTest(IInitiateTestContext context);

        GetTestProgressOutput GetTestProgress(IGetTestProgressContext context);
    }

    public enum InitiateTestResult { Created, FailedWithRetry, FailedWithNoRetry }

    public interface IInitiateTestContext
    {
        BusinessEntity.Entities.Supplier Supplier { get; }

        BusinessEntity.Entities.Zone Zone { get; }

        Vanrise.Entities.Country Country { get; }
    }

    public class InitiateTestOutput
    {
        public InitiateTestResult Result { get; set; }

        public Object InitiateTestInformation { get; set; }
    }

    public enum GetTestProgressResult { TestCompleted, ProgressChanged, ProgressNotChanged, FailedWithRetry, FailedWithNoRetry }

    public enum CallTestResult { NotCompleted = 0, Succeeded = 1, PartiallySucceeded = 2, Failed = 3, NotAnswered = 4}

    public interface IGetTestProgressContext
    {
        Object InitiateTestInformation { get; }

        Object RecentTestProgress { get; }
    }

    public class GetTestProgressOutput
    {
        public GetTestProgressResult Result { get; set; }

        public Object TestProgress { get; set; }

        public CallTestResult TestResult { get; set; }
    }

    public class InitiateTestInformation
    {
        public string Test_ID { get; set; }
    }

    public class TestProgress
    {
        public string Name { get; set; }
        public int Calls_Total { get; set; }
        public int Calls_Complete { get; set; }
        public int CLI_Success { get; set; }
        public int CLI_No_Result { get; set; }
        public int CLI_Fail { get; set; }
        public int PDD { get; set; }
        public string Share_URL { get; set; }
    }
}
