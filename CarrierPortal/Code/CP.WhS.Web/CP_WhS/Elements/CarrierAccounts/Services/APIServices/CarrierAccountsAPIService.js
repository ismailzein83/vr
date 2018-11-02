(function (appControllers) {
    "use strict";
    CarrierAccountsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_WhS_ModuleConfig'];

    function CarrierAccountsAPIService(BaseAPIService, UtilsService,  CP_WhS_ModuleConfig) {
        var controllerName = "WhSCarrierAccountBE";

        function GetRemoteCarrierAccountsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, 'GetRemoteCarrierAccountsInfo'), {
                serializedFilter : filter
            });
        }
        return ({
            GetRemoteCarrierAccountsInfo: GetRemoteCarrierAccountsInfo
        });
    }
    appControllers.service('CP_WhS_CarrierAccountsAPIService', CarrierAccountsAPIService);

})(appControllers);