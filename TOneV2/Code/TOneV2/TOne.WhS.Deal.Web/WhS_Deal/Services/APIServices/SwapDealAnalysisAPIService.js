(function (appControllers) {

	'use strict';

	SwapDealAnalysisAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Deal_ModuleConfig', 'SecurityService'];

	function SwapDealAnalysisAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig, SecurityService) {

		var controllerName = 'SwapDealAnalysis';

		function GetOutboundRateCalcMethodExtensionConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetOutboundRateCalcMethodExtensionConfigs'));
		}

		return {
			GetOutboundRateCalcMethodExtensionConfigs: GetOutboundRateCalcMethodExtensionConfigs
		};
	}

	appControllers.service('WhS_Deal_SwapDealAnalysisAPIService', SwapDealAnalysisAPIService);

})(appControllers);