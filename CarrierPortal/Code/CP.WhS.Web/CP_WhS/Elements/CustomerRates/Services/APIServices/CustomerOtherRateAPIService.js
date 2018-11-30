(function (appControllers) {

	'use strict';

	OtherCustomerRateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_WhS_ModuleConfig'];

	function OtherCustomerRateAPIService(BaseAPIService, UtilsService, CP_WhS_ModuleConfig) {

		var controllerName = 'CustomerOtherRate';

		function GetCustomerOtherRates(query) {
			return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, 'GetCustomerOtherRates'), query);
		}

		return {
			GetCustomerOtherRates: GetCustomerOtherRates
		};
	}

	appControllers.service('CP_WhS_CustomerOtherRateAPIService', OtherCustomerRateAPIService);

})(appControllers);