(function (appControllers) {

	"use strict";
	SupplierOtherRateAPIService.$inject = ["BaseAPIService", "UtilsService", "CP_WhS_ModuleConfig"];

	function SupplierOtherRateAPIService(BaseAPIService, UtilsService, CP_WhS_ModuleConfig) {

		var controllerName = "SupplierOtherRate";

		function GetFilteredSupplierOtherRates(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierOtherRates"), input);
		}


		return ({
			GetFilteredSupplierOtherRates: GetFilteredSupplierOtherRates
		});
	}

	appControllers.service("CP_WhS_SupplierOtherRateAPIService", SupplierOtherRateAPIService);
})(appControllers);