using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Entities.Settings;
using Vanrise.Web.Base;

namespace TOne.WhS.Deal.Web.Controllers
{
	[JSONWithTypeAttribute]
	[RoutePrefix(Constants.ROUTE_PREFIX + "SwapDealAnalysis")]
	public class SwapDealAnalysisController : Vanrise.Web.Base.BaseAPIController
	{
		[HttpPost]
		[Route("AnalyzeDeal")]
		public SwapDealAnalysisResult AnalyzeDeal(SwapDealAnalysisSettings analysisSettings)
		{
			var manager = new SwapDealAnalysisManager();
			return manager.AnalyzeDeal(analysisSettings);
		}

		[HttpGet]
		[Route("GetInboundRateCalcMethodExtensionConfigs")]
		public IEnumerable<SwapDealAnalysisInboundRateCalcMethodConfig> GetInboundRateCalcMethodExtensionConfigs()
		{
			var manager = new SwapDealAnalysisManager();
			return manager.GetInboundRateCalcMethodExtensionConfigs();
		}

		[HttpGet]
		[Route("GetOutboundRateCalcMethodExtensionConfigs")]
		public IEnumerable<SwapDealAnalysisOutboundRateCalcMethodConfig> GetOutboundRateCalcMethodExtensionConfigs()
		{
			var manager = new SwapDealAnalysisManager();
			return manager.GetOutboundRateCalcMethodExtensionConfigs();
		}

		[HttpPost]
		[Route("CalculateInboundRate")]
		public decimal? CalculateInboundRate(CalculateInboundRateInput input)
		{
			var manager = new SwapDealAnalysisManager();
			return manager.CalculateInboundRate(input);
		}

		[HttpPost]
		[Route("CalculateOutboundRate")]
		public decimal? CalculateOutboundRate(CalculateOutboundRateInput input)
		{
			var manager = new SwapDealAnalysisManager();
			return manager.CalculateOutboundRate(input);
		}
	}
}