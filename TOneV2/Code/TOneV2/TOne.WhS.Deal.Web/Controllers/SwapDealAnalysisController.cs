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
		[HttpGet]
		[Route("GetOutboundRateCalcMethodExtensionConfigs")]
		public IEnumerable<SwapDealAnalysisOutboundRateCalcMethodConfig> GetOutboundRateCalcMethodExtensionConfigs()
		{
			var manager = new SwapDealAnalysisManager();
			return manager.GetOutboundRateCalcMethodExtensionConfigs();
		}

		[HttpGet]
		[Route("GetSwapDealAnalysisSettingData")]
		public SwapDealAnalysisSettingData GetSwapDealAnalysisSettingData()
		{
			var manager = new SwapDealAnalysisManager();
			return manager.GetSwapDealAnalysisSettingData();
		}
	}
}