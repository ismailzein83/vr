using System;

namespace CP.SupplierPricelist.Entities
{
    public class PriceList
    {
        public long PriceListId { get; set; }
        public int UserId { get; set; }
        public long FileId { get; set; }
        public PriceListType PriceListType { get; set; }
        public PriceListStatus Status { get; set; }
        public PriceListResult Result { get; set; }
        public object UploadedInformation { get; set; }
        public object PriceListProgress { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime EffectiveOnDate { get; set; }
        public int ResultMaxRetryCount { get; set; }
        public int UploadMaxRetryCount { get; set; }
        public string AlertMessage { get; set; }
        public long AlertFileId { get; set; }
        public int CustomerId { get; set; }
        public string CarrierAccountId { get; set; }
        public string CarrierAccountName { get; set; }
    }
}
