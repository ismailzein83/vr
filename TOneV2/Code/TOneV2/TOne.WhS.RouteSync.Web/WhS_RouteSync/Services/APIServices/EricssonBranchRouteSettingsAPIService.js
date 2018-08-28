(function (appControllers) {

	"use strict";

	EricssonBranchRouteSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

	function EricssonBranchRouteSettingsAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

		var controllerName = 'EricssonBranchRouteSettings';

		function GetEricssonBranchRouteSettingsConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetEricssonBranchRouteSettingsConfigs"));
		}

		return ({
			GetEricssonBranchRouteSettingsConfigs: GetEricssonBranchRouteSettingsConfigs
		});
	}

	appControllers.service('WhS_RouteSync_EricssonBranchRouteSettingsAPIService', EricssonBranchRouteSettingsAPIService);

})(appControllers);