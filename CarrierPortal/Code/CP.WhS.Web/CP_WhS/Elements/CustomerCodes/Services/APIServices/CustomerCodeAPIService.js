(function (appControllers) {

	"use strict";
	customerCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "CP_WhS_ModuleConfig"];
	function customerCodeAPIService(BaseAPIService, UtilsService, CP_WhS_ModuleConfig) {

		var controllerName = "CustomerCode";

		function GetRemoteFilteredCustomerCodes(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, "GetRemoteFilteredCustomerCodes"), input);
		}
		return ({
			GetRemoteFilteredCustomerCodes: GetRemoteFilteredCustomerCodes,
		});
	}

	appControllers.service("CP_WhS_SaleCodeAPIService", customerCodeAPIService);

})(appControllers);