using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Entities
{
    public abstract class SupplierPriceListConnectorBase
    {
        public abstract Guid ConfigId { get;}
        public abstract PriceListUploadOutput UploadPriceList(IPriceListUploadContext context);

        public abstract PriceListProgressOutput GetPriceListResult(IPriceListProgressContext context);

        public virtual List<SupplierInfo> GetSuppliers(GetSuppliersContext context)
        {
            return null;
        }
    }

    

    public interface GetSuppliersContext
    {

    }

    public enum PriceListSupplierUploadResult { Uploaded, Failed, FailedWithRetry }
    public enum PriceListProgressResult { Completed, ProgressChanged, ProgressNotChanged, FailedWithRetry, FailedWithNoRetry }

    public class PriceListUploadOutput
    {
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
        public PriceListProgressResult PriceListProgress { get; set; }

        public Object UploadProgress { get; set; }

        public PriceListResult PriceListResult { get; set; }
        public PriceListStatus PriceListStatus { get; set; }

        public string AlertMessage { get; set; }

        public byte[] AlertFile { get; set; }
        public string AlerFileName { get; set; }
    }
    public interface IPriceListProgressContext
    {
        Object UploadInformation { get; }

        Object UploadProgress { get; }
    }

}
