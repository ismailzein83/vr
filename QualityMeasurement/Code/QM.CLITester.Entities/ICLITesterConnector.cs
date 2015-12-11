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

        public string FailureMessage { get; set; }
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

        public CallTestResult CallTestResult { get; set; }
    }

    public class InitiateTestInformation
    {
        public string Test_ID { get; set; }
    }

    public class TestProgress
    {
        public string Name { get; set; }
        public int TotalCalls { get; set; }
        public int CompletedCalls { get; set; }
        public int CliSuccess { get; set; }
        public int CliNoResult { get; set; }
        public int CliFail { get; set; }
        public int Pdd { get; set; }
        public string ShareUrl { get; set; }
    }
}
