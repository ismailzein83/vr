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
        // public string Url { get; set; }
        // public string UserName { get; set; }
        //  public string Password { get; set; }
    }
}
