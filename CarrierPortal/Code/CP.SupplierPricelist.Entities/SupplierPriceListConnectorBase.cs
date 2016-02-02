using System;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Entities
{
    public abstract class SupplierPriceListConnectorBase
    {
        public int ConfigId { get; set; }
        public abstract InitiatePriceListOutput InitiatePriceList(IInitiateUploadContext context);
        public abstract PriceListProgressOutput GetPriceListProgressOutput(IPriceListProgressContext context);
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public enum InitiateSupplierResult { Uploaded, Failed }
    public enum PriceListProgressResult { NotCompleted, Approved, PartiallyApproved, Rejected }

    public class InitiatePriceListOutput
    {
        public int QueueId { get; set; }
        public InitiateSupplierResult Result { get; set; }

        public Object InitiateUploadInformation { get; set; }

        public string FailureMessage { get; set; }

    }

    public interface IInitiateUploadContext
    {
        int UserId { get; }
        string PriceListType { get; }
        VRFile File { get; }
        DateTime EffectiveOnDateTime { get; }
    }
    public class PriceListProgressOutput
    {
        public PriceListProgressResult Result { get; set; }

        public Object TestProgress { get; set; }

        public PriceListResult PriceListResult { get; set; }
    }
    public interface IPriceListProgressContext
    {
        int QueueId { get; set; }
        Object InitiateTestInformation { get; }

        Object RecentTestProgress { get; }
    }

}
