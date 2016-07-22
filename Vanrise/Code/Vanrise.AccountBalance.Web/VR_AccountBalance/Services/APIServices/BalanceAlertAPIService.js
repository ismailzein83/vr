
(function (appControllers) {

    "use strict";
    BalanceAlertAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig'];

    function BalanceAlertAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig) {

        var controllerName = "BalanceAlert";

        function GetBalanceAlertThresholdConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetBalanceAlertThresholdConfigs"));
        }

        return ({
            GetBalanceAlertThresholdConfigs: GetBalanceAlertThresholdConfigs,
        });
    }

    appControllers.service('VR_AccountBalance_BalanceAlertAPIService', BalanceAlertAPIService);

})(appControllers);