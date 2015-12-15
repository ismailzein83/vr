using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{    
    public abstract class CLITesterConnectorBase
    {
        public int ConfigId { get; set; }

        public abstract InitiateTestOutput InitiateTest(IInitiateTestContext context);

        public abstract GetTestProgressOutput GetTestProgress(IGetTestProgressContext context);
    }

    public enum InitiateTestResult { Created, FailedWithRetry, FailedWithNoRetry }

    public interface IInitiateTestContext
    {
        CLITester.Entities.Profile Profile { get; }
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

    public enum CallTestResult
    {
        [Description("Not Completed")]
        NotCompleted = 0,

        [Description("Succeeded")]
        Succeeded = 1,

        [Description("Partially Succeeded")]
        PartiallySucceeded = 2,

        [Description("Failed")]
        Failed = 3,

        [Description("Not Answered")]
        NotAnswered = 4
    }

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
