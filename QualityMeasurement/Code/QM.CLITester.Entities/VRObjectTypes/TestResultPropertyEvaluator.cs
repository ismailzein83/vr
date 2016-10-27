using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace QM.CLITester.Entities.VRObjectTypes
{
    public enum TestCallDetailField { SupplierName = 0, UserName = 1, CountryName = 2, ZoneName = 3, CallTestStatusDescription = 4,
    CallTestResultDescription = 5, ScheduleName = 6, Pdd = 7, Mos = 8, CreationDate = 9, Source = 10, Destination = 11, ReceivedCli = 12,
    RingDuration = 13, CallDuration = 14, ReleaseCode = 15, ToMail = 16, Start = 17 }
    public class TestResultPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("3c2d781d-5089-4d1e-9061-fa5a895ae9a2"); } }
        public TestCallDetailField TestCallDetailField { get; set; }
        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            TestCallInfo testCallDetailInfo = context.Object as TestCallInfo;

            if (testCallDetailInfo == null)
                throw new NullReferenceException("testCallDetailInfo");

            switch (this.TestCallDetailField)
            {
                case TestCallDetailField.SupplierName: return testCallDetailInfo.SupplierName;
                case TestCallDetailField.UserName: return testCallDetailInfo.UserName;
                case TestCallDetailField.CountryName: return testCallDetailInfo.CountryName;
                case TestCallDetailField.ZoneName: return testCallDetailInfo.ZoneName;
                case TestCallDetailField.CallTestStatusDescription: return testCallDetailInfo.CallTestStatusDescription;
                case TestCallDetailField.CallTestResultDescription: return testCallDetailInfo.CallTestResultDescription;
                case TestCallDetailField.ScheduleName: return testCallDetailInfo.ScheduleName;
                case TestCallDetailField.Pdd: return testCallDetailInfo.Pdd;
                case TestCallDetailField.Mos: return testCallDetailInfo.Mos;
                case TestCallDetailField.CreationDate: return testCallDetailInfo.CreationDate;
                case TestCallDetailField.Source: return testCallDetailInfo.Source;
                case TestCallDetailField.Destination: return testCallDetailInfo.Destination;
                case TestCallDetailField.ReceivedCli: return testCallDetailInfo.ReceivedCli;
                case TestCallDetailField.RingDuration: return testCallDetailInfo.RingDuration;
                case TestCallDetailField.CallDuration: return testCallDetailInfo.CallDuration;
                case TestCallDetailField.ReleaseCode: return testCallDetailInfo.ReleaseCode;
                case TestCallDetailField.ToMail: return testCallDetailInfo.ToMail;
                case TestCallDetailField.Start: return testCallDetailInfo.Start;
            }

            return null;
        }
    }
}
