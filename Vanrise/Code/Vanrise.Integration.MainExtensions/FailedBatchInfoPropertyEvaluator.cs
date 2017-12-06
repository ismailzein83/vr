using System;
using Vanrise.Entities;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions
{
    public enum FailedBatchInfoField { Message = 0, IsEmpty = 1, BatchDescription = 2, DataSourceId = 3, DataSourceName = 4 }
    public class FailedBatchInfoPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public FailedBatchInfoField FailedBatchInfoField { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("708F5F03-F14E-4487-B916-5617013E8B3E"); }
        }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            FailedBatchInfo faildBatchInfo = context.Object as FailedBatchInfo;

            switch (FailedBatchInfoField)
            {
                case FailedBatchInfoField.Message:
                    return faildBatchInfo.Message;
                case FailedBatchInfoField.IsEmpty:
                    return faildBatchInfo.IsEmpty;
                case FailedBatchInfoField.DataSourceId:
                    return faildBatchInfo.DataSourceId;
                case FailedBatchInfoField.BatchDescription:
                    return faildBatchInfo.BatchDescription;
                case MainExtensions.FailedBatchInfoField.DataSourceName:
                    return faildBatchInfo.DataSourceName;
                default:
                    return null;
            }
        }
    }
}
