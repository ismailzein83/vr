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

		public abstract Decimal Execute(ISwapDealAnalysisInboundRateCalcMethodContext context);
	}

	public interface ISwapDealAnalysisInboundRateCalcMethodContext
	{
		SwapDealAnalysisInboundRateCalcMethod RateCalculationMethod { get; set; }
	}
}
