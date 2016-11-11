using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
	public class CalculateOutboundRateInput
	{
		public SwapDealAnalysisOutboundItemRateCalcMethod OutboundItemRateCalcMethod { get; set; }

		public int SupplierId { get; set; }

		public int CountryId { get; set; }

		public List<long> SupplierZoneIds { get; set; }
	}
}
