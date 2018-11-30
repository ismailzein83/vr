(function (appControllers) {

	"use strict";
	supplierRateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_WhS_ModuleConfig'];

	function supplierRateAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

		var controllerName = "SupplierRate";

		function GetSupplierRateQueryHandlerInfo(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierRateQueryHandlerInfo"), input);
		}

		return ({
			GetSupplierRateQueryHandlerInfo: GetSupplierRateQueryHandlerInfo
		});
	}

	appControllers.service("CP_WhS_SupplierRateAPIService", supplierRateAPIService);
})(appControllers);