"use strict";
(function (appControllers) {

	cityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

	function cityAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

		var controller = "Demo_City";

		function GetFilteredCities(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredCities"), input);
		}

		function GetCityById(cityId) {
			return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCityById"), {
				cityId: cityId
			});
		}

		function GetCityTypesConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCityTypesConfigs"));
		}

		function GetDistrictSettingsConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDistrictSettingsConfigs"));
		}

		function GetFactoryTypesConfigs() {
			return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFactoryTypesConfigs"));
		}

		function AddCity(city) {
			return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddCity"), city);
		}

		function UpdateCity(city) {
			return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateCity"), city);
		}

		return {
			GetFilteredCities: GetFilteredCities,
			GetCityById: GetCityById,
			GetCityTypesConfigs: GetCityTypesConfigs,
			GetDistrictSettingsConfigs: GetDistrictSettingsConfigs,
			GetFactoryTypesConfigs: GetFactoryTypesConfigs,
			AddCity: AddCity,
			UpdateCity: UpdateCity
		};
	}

	appControllers.service("Demo_Module_CityAPIService", cityAPIService);
})(appControllers);