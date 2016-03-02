using CP.SupplierPricelist.Entities;
using System;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Business
{
    public class PriceListUploadContext : IPriceListUploadContext
    {
        public int UserId { get; set; }
        public string PriceListType { get; set; }
        public VRFile File { get; set; }
        public DateTime EffectiveOnDateTime { get; set; }
        public string CarrierAccountId { get; set; }
        public string UserMail { get; set; }
    }
}
