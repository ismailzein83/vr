using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
	public abstract class SwapDealAnalysisOutboundItemRateCalcMethod
	{
		public Guid CalculationMethodId { get; set; }

		public abstract decimal? Execute(ISwapDealAnalysisOutboundRateCalcMethodContext context);
	}

	public interface ISwapDealAnalysisOutboundRateCalcMethodContext
	{
		int SupplierId { get; }

		int CountryId { get; set; }

		List<long> SupplierZoneIds { get; }
	}
}
