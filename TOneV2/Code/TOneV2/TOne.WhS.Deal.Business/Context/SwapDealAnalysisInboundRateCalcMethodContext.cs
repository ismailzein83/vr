using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
	public class SwapDealAnalysisInboundRateCalcMethodContext : ISwapDealAnalysisInboundRateCalcMethodContext
	{
		public int CustomerId { get; set; }

		public int CountryId { get; set; }

		public IEnumerable<long> SaleZoneIds { get; set; }
	}
}
