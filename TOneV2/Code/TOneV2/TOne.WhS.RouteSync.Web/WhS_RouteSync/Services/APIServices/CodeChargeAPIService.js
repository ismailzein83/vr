(function (appControllers) {

    'use strict';

    CodeChargeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

    function CodeChargeAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

        var controllerName = 'CodeCharge';

        function GetCodeChargeEvaluatorExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetCodeChargeEvaluatorExtensionConfigs"));
        }

        return {
            GetCodeChargeEvaluatorExtensionConfigs: GetCodeChargeEvaluatorExtensionConfigs
        };

    }

    appControllers.service('WhS_RouteSync_CodeChargeAPIService', CodeChargeAPIService);

})(appControllers);