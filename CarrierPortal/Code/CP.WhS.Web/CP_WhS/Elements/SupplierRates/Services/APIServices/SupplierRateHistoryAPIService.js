(function (appControllers) {

	"use strict";
	supplierRateHistoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_WhS_ModuleConfig'];

	function supplierRateHistoryAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

		var controllerName = "SupplierRateHistory";

		function GetSupplierRateHistoryQueryHandlerInfo(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSupplierRateHistoryQueryHandlerInfo"), input);
		}


		return ({
			GetSupplierRateHistoryQueryHandlerInfo: GetSupplierRateHistoryQueryHandlerInfo
		});
	}

	appControllers.service("CP_WhS_SupplierRateHistoryAPIService", supplierRateHistoryAPIService);
})(appControllers);