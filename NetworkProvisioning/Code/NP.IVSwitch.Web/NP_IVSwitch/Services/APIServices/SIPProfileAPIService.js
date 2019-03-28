
(function (appControllers) {

	"use strict";
	SIPProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig'];

	function SIPProfileAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig) {

		var controllerName = "SIPProfile";

		function GetSIPProfilesInfo(filter) {
			return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetSIPProfilesInfo'), {
				filter: filter
			});
		}
		return ({
			GetSIPProfilesInfo: GetSIPProfilesInfo,
		});
	}

	appControllers.service('NP_IVSwitch_SIPProfileAPIService', SIPProfileAPIService);

})(appControllers);