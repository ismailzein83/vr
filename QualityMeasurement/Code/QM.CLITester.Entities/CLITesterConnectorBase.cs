using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace QM.CLITester.Entities
{    
    public abstract class CLITesterConnectorBase
    {
        public abstract Guid ConfigId { get;  }

        public abstract InitiateTestOutput InitiateTest(IInitiateTestContext context);

        public abstract GetTestProgressOutput GetTestProgress(IGetTestProgressContext context);

        public abstract void ConvertResultToExcelData(IConvertResultToExcelDataContext<TestCallDetail> context);
    }

    public enum InitiateTestResult { Created, FailedWithRetry, FailedWithNoRetry }

    public interface IInitiateTestContext
    {
        CLITester.Entities.Profile Profile { get; }
        BusinessEntity.Entities.Supplier Supplier { get; }

        BusinessEntity.Entities.Zone Zone { get; }

        Vanrise.Entities.Country Country { get; }

        int Quantity { get; }
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

        [Description("CLI Delivered")]
        Succeeded = 1,

        [Description("Partially Succeeded")]
        PartiallySucceeded = 2,

        [Description("CLI Failure")]
        Failed = 3,

        [Description("Call Failure")]
        NotAnswered = 4,

        [Description("FAS")]
        Fas = 5
    }

    public interface IGetTestProgressContext
    {
        Object InitiateTestInformation { get; }

        Object RecentTestProgress { get; }
        Measure RecentMeasure { get; }
    }

    public class GetTestProgressOutput
    {
        public GetTestProgressResult Result { get; set; }

        public Object TestProgress { get; set; }
        public Measure Measure { get; set; }

        public CallTestResult CallTestResult { get; set; }
    }

    public class Measure
    {
        public decimal Pdd { get; set; }
        public decimal Mos { get; set; }
        public DateTime? Duration { get; set; }
        //public string ReleaseCode { get; set; }
        //public string ReceivedCli { get; set; }
        public string RingDuration { get; set; }
    }
}
