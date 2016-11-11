(function (appControllers) {

	'use strict';

	SwapDealAnalysisAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

	function SwapDealAnalysisAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {

		var controllerName = 'SwapDealAnalysis';

		function AnalyzeDeal(analysisSettings) {
			return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'AnalyzeDeal'), analysisSettings);
		}

		function GetInboundRateCalcMethodExtensionConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetInboundRateCalcMethodExtensionConfigs'));
		}

		function GetOutboundRateCalcMethodExtensionConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetOutboundRateCalcMethodExtensionConfigs'));
		}

		function CalculateInboundRate(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'CalculateInboundRate'), input);
		}

		function CalculateOutboundRate(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'CalculateOutboundRate'), input);
		}

		return {
			AnalyzeDeal: AnalyzeDeal,
			GetOutboundRateCalcMethodExtensionConfigs: GetOutboundRateCalcMethodExtensionConfigs,
			GetInboundRateCalcMethodExtensionConfigs: GetInboundRateCalcMethodExtensionConfigs,
			CalculateInboundRate: CalculateInboundRate,
			CalculateOutboundRate: CalculateOutboundRate
		};
	}

	appControllers.service('WhS_Deal_SwapDealAnalysisAPIService', SwapDealAnalysisAPIService);

})(appControllers);