using System;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Entities
{
    public abstract class SupplierPriceListConnectorBase
    {
        public int ConfigId { get; set; }
        public abstract PriceListUploadOutput PriceListUploadOutput(IPriceListUploadContext context);
        public abstract PriceListProgressOutput GetPriceListProgressOutput(IPriceListProgressContext context);
    }
    public enum PriceListSupplierUploadResult { Uploaded, Failed, FailedWithRetry }
    public enum PriceListProgressResult { NotCompleted, Approved, PartiallyApproved, Rejected }

    public class PriceListUploadOutput
    {
        public int QueueId { get; set; }
        public PriceListSupplierUploadResult Result { get; set; }

        public Object UploadPriceListInformation { get; set; }

        public string FailureMessage { get; set; }

    }

    public interface IPriceListUploadContext
    {
        int UserId { get; }
        string PriceListType { get; }
        VRFile File { get; }
        DateTime EffectiveOnDateTime { get; }
    }
    public class PriceListProgressOutput
    {
        public PriceListProgressResult Result { get; set; }

        public Object UploadProgress { get; set; }

        public PriceListResult PriceListResult { get; set; }

        public string AlertMessage { get; set; }

        public byte[] AlertFile { get; set; }
    }
    public interface IPriceListProgressContext
    {
        Object UploadInformation { get; }

        Object UploadProgress { get; }
    }

}
