(function (appControllers) {

    "use strict";

    vrRestCountryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

	function vrRestCountryAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
		var controllerName = 'VRRestCountry';

		function GetRemoteCountriesInfo(connectionId, filter) {
			return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRemoteCountriesInfo"), {
				connectionId: connectionId,
				filter: filter
			});
		}
        return ({
			GetRemoteCountriesInfo: GetRemoteCountriesInfo
        });
    }

	appControllers.service('VRCommon_VRRestCountryAPIService', vrRestCountryAPIService);

})(appControllers);
