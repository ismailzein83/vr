(function (appControllers) {

    "use strict";

    mobileNetworkAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_MobileNetwork_ModuleConfig'];

    function mobileNetworkAPIService(BaseAPIService, UtilsService, VR_MobileNetwork_ModuleConfig) {
        var controllerName = 'MobileCountry';

        function GetMobileCountriesInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_MobileNetwork_ModuleConfig.moduleName, controllerName, "GetMobileCountriesInfo"), { serializedFilter: serializedFilter });
        }

        return {
            GetMobileCountriesInfo: GetMobileCountriesInfo
        };
    }

    appControllers.service('VR_MobileNetwork_MobileCountryAPIService', mobileNetworkAPIService);
})(appControllers);