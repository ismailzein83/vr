using System;

namespace TOne.WhS.SupplierPriceList.Entities
{
	public class ReceivedPricelistDetail
	{
        public ReceivedPricelist ReceivedPricelist { get; set; }
        public int Id { get; set; }
        public string SupplierName{ get; set; }
        public string StatusDescription { get; set; }
        public string PriceListTypeDescription { get; set; }
        public bool SentToSupplier { get; set; }
	}
}
