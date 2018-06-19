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
		public List<SPLImportErrorDetail> ErrorDetails { get; set; }
	}

	public class SPLImportErrorDetail
	{
		public string ErrorMessage { get; set; }
	}
}
