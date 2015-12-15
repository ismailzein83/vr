(function (appControllers) {

    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        function GetFilteredCountries(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Country", "GetFilteredCountries"), input);
        }
        function GetCountriesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Country", "GetCountriesInfo"));
        }
        function GetCountry(countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Country", "GetCountry"), {
                countryId: countryId
            });

        }
        function UpdateCountry(countryObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Country", "UpdateCountry"), countryObject);
        }
        function AddCountry(countryObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Country", "AddCountry"), countryObject);
        }
        function GetCountrySourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, "Country", "GetCountrySourceTemplates"));
        }
        return ({
            GetFilteredCountries: GetFilteredCountries,
            GetCountriesInfo: GetCountriesInfo,
            GetCountry: GetCountry,
            UpdateCountry: UpdateCountry,
            AddCountry: AddCountry,
            GetCountrySourceTemplates: GetCountrySourceTemplates 
        });
    }

    appControllers.service('VRCommon_CountryAPIService', countryAPIService);

})(appControllers);