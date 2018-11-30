(function (appControllers) {

	"use strict";
	CustomerRateAPIService.$inject = ["BaseAPIService", "UtilsService", "CP_WhS_ModuleConfig"];

	function CustomerRateAPIService(BaseAPIService, UtilsService, CP_WhS_ModuleConfig) {

		var controllerName = "CustomerRate";

		function GetFilteredCustomerRates(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, "GetFilteredCustomerRates"), input);
		}

		function GetPrimaryCustomerEntity() {
			return BaseAPIService.get(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, "GetPrimaryCustomerEntity"));
		}
		return ({
			GetFilteredCustomerRates: GetFilteredCustomerRates,
			GetPrimaryCustomerEntity: GetPrimaryCustomerEntity
		});
	}

	appControllers.service("CP_WhS_CustomerRateAPIService", CustomerRateAPIService);

})(appControllers);