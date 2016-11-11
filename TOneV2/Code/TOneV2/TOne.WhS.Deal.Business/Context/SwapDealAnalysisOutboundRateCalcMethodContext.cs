using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
	public class SwapDealAnalysisOutboundRateCalcMethodContext : ISwapDealAnalysisOutboundRateCalcMethodContext
	{
		public int SupplierId { get; set; }

		public int CountryId { get; set; }

		public List<long> SupplierZoneIds { get; set; }
	}
}
