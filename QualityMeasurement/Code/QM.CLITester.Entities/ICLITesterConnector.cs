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
}
