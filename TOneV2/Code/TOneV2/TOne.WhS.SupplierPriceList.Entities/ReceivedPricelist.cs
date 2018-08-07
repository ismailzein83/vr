using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities
{
	public class ReceivedPricelist
	{
		public int Id { get; set; }
		public int SupplierId { get; set; }
		public long? FileId { get; set; }
		public DateTime ReceivedDateTime { get; set; }
		public SupplierPricelistType? PricelistType { get; set; }
		public ReceivedPricelistStatus Status { get; set; }
		public int? PricelistId { get; set; }
		public long? ProcessInstanceId { get; set; }
		public DateTime? StartProcessingDateTime { get; set; }
		public List<SPLImportErrorDetail> MessageDetails { get; set; }
        public bool SentToSupplier { get; set; }
	}

    //TODO: This should be changed from Error Message to Message (also change it in Database)
	public class SPLImportErrorDetail
	{
		public string Message { get; set; }
	}
}
