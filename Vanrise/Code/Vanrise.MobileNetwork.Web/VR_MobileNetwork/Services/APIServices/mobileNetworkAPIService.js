(function (appControllers) {

    "use strict";

    mobileNetworkAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_MobileNetwork_ModuleConfig'];

    function mobileNetworkAPIService(BaseAPIService, UtilsService, VR_MobileNetwork_ModuleConfig) {
        var controllerName = 'MobileNetwork';

        function GetMobileNetworksInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_MobileNetwork_ModuleConfig.moduleName, controllerName, "GetMobileNetworksInfo"), { serializedFilter: serializedFilter });
        }

        return {
            GetMobileNetworksInfo: GetMobileNetworksInfo
        };
    }

    appControllers.service('VR_MobileNetwork_MobileNetworkAPIService', mobileNetworkAPIService);
})(appControllers);