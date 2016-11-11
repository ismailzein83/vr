using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
	public class CalculateInboundRateInput
	{
		public SwapDealAnalysisInboundItemRateCalcMethod InboundItemRateCalcMethod { get; set; }

		public int CustomerId { get; set; }

		public int CountryId { get; set; }

		public IEnumerable<long> SaleZoneIds { get; set; }
	}
}
