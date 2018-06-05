using System;

namespace TOne.WhS.SupplierPriceList.Entities
{
	public class ReceivedPricelistDetails
	{
		public int SupplierId { get; set; }
		public long FileId { get; set; }
		public DateTime ReceivedDate { get; set; }
		public SupplierPriceListType PricelistType { get; set; }
		public ReceivedPricelistStatus Status { get; set; }
		public int? PricelistId { get; set; }
		public long? ProcessInstanceId { get; set; }
	}
}
