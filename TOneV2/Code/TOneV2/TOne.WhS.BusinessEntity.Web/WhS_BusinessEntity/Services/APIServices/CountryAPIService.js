(function (appControllers) {

    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredCountries(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "GetFilteredCountries"), input);
        }
        
        
        return ({
            GetFilteredCountries: GetFilteredCountries
        });
    }

    appControllers.service('WhS_BE_CountryAPIService', countryAPIService);

})(appControllers);