(function (appControllers) {

	"use strict";
	supplierCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "CP_WhS_ModuleConfig"];

	function supplierCodeAPIService(BaseAPIService, UtilsService, CP_WhS_ModuleConfig) {

		var controllerName = "SupplierCode";

		function GetFilteredSupplierCodes(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierCodes"), input);
		}

		return ({
			GetFilteredSupplierCodes: GetFilteredSupplierCodes
		});
	}

	appControllers.service("CP_WhS_SupplierCodeAPIService", supplierCodeAPIService);
})(appControllers);