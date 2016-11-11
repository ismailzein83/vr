using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
	public abstract class SwapDealAnalysisInboundItemRateCalcMethod
	{
		public Guid CalculationMethodId { get; set; }

		public abstract decimal? Execute(ISwapDealAnalysisInboundRateCalcMethodContext context);
	}

	public interface ISwapDealAnalysisInboundRateCalcMethodContext
	{
		int CustomerId { get; set; }

		int CountryId { get; set; }

		IEnumerable<long> SaleZoneIds { get; set; }
	}
}
