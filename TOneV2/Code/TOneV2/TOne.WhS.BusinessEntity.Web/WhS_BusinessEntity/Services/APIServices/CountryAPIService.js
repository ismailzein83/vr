﻿(function (appControllers) {

    "use strict";
    countryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function countryAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredCountries(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "GetFilteredCountries"), input);
        }
        function GetAllCountries() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "GetAllCountries"));
        }
        function GetCountry(countryId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "GetCountry"), {
                countryId: countryId
            });

        }
        function UpdateCountry(countryObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "UpdateCountry"), countryObject);
        }
        function AddCountry(countryObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "Country", "AddCountry"), countryObject);
        }
        return ({
            GetFilteredCountries: GetFilteredCountries,
            GetAllCountries: GetAllCountries,
            GetCountry: GetCountry,
            UpdateCountry: UpdateCountry,
            AddCountry: AddCountry
        });
    }

    appControllers.service('WhS_BE_CountryAPIService', countryAPIService);

})(appControllers);