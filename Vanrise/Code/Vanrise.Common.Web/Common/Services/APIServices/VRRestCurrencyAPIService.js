(function (appControllers) {

    "use strict";

    currencyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function currencyAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'VRRestCurrency';

        function GetRemoteAllCurrencies(connectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRemoteAllCurrencies"), {
                connectionId: connectionId

            });
        }
        function GetRemoteSystemCurrency(connectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRemoteSystemCurrency"),
                {
                    connectionId: connectionId
                });
        }

        return ({
            GetRemoteAllCurrencies: GetRemoteAllCurrencies,
            GetRemoteSystemCurrency: GetRemoteSystemCurrency,
        });
    }

    appControllers.service('VRCommon_VRRestCurrencyAPIService', currencyAPIService);

})(appControllers);