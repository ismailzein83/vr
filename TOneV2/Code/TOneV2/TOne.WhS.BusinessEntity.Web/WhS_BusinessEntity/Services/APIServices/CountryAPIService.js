(function (appControllers) {

    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredCountries(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "GetFilteredCountries"), input);
        }
        function GetCountry(countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "GetCountry"), {
                countryId: countryId
            });

        }
        function UpdateCountry(countryObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "UpdateCountry"), countryObject);
        }
        return ({
            GetFilteredCountries: GetFilteredCountries,
            GetCountry: GetCountry,
            UpdateCountry: UpdateCountry
        });
    }

    appControllers.service('WhS_BE_CountryAPIService', countryAPIService);

})(appControllers);