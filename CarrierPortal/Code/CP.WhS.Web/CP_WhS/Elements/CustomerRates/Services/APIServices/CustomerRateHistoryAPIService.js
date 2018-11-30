(function (appControllers) {

	'use strict';

	CustomerRateHistoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_WhS_ModuleConfig'];

	function CustomerRateHistoryAPIService(BaseAPIService, UtilsService, CP_WhS_ModuleConfig) {
		var controllerName = 'CustomerRateHistory';

		function GetFilteredCustomerRateHistoryRecords(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, 'GetFilteredCustomerRateHistoryRecords'), input);
		}

		return {
			GetFilteredCustomerRateHistoryRecords: GetFilteredCustomerRateHistoryRecords
		};
	}

	appControllers.service('CP_WhS_CustomerRateHistoryAPIService', CustomerRateHistoryAPIService);

})(appControllers);