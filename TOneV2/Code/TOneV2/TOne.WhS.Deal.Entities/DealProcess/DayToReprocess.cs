using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
	public class DayToReprocess
	{
		public long DayToReprocessId { get; set; }
		public DateTime Date { get; set; }
		public bool IsSale { get; set; }
		public int CarrierAccountId { get; set; }
	}

	public class DayToReprocessSummary
	{
		public DateTime Date { get; set; }
		public List<int> CustomerIds { get; set; }
		public List<int> SupplierIds { get; set; }
	}
}