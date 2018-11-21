(function (appControllers) {

	"use strict";

	countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig'];

	function countryAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig) {

		var controller = "Demo_Country";

		function GetFilteredCountries(input) {
			return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredCountries"), input);
		}

		function GetCountryById(countryId) {
			return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCountryById"), {
				countryId: countryId
			});
		}

		function GetCountriesInfo(filter) {
			return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCountriesInfo"), {
				filter: filter
			});
		}

		function AddCountry(country) {
			return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddCountry"), country);
		}

		function UpdateCountry(country) {
			return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdateCountry"), country);
		}

		return {
			GetFilteredCountries: GetFilteredCountries,
			GetCountryById: GetCountryById,
			GetCountriesInfo: GetCountriesInfo,
			AddCountry: AddCountry,
			UpdateCountry: UpdateCountry
		};
	}

	appControllers.service("Demo_Module_CountryAPIService", countryAPIService);
})(appControllers);