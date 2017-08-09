(function (appControllers) {


    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {
        var controllerName = "CountryWithDefaultRate";

        function GetFilteredCountries(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, controllerName, "GetFilteredCountries"), input);
        }


        return ({
            GetFilteredCountries: GetFilteredCountries
        });
    }

    appControllers.service('WhS_Sales_CountryWithDefaultRateAPIService', countryAPIService);

})(appControllers);