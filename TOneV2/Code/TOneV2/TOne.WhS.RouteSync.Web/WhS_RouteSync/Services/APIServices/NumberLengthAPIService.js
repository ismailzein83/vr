(function (appControllers) {

    'use strict';

    NumberLengthAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

    function NumberLengthAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

        var controllerName = 'NumberLength';

        function GetNumberLengthEvaluatorExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetNumberLengthEvaluatorExtensionConfigs"));
        }

        return {
            GetNumberLengthEvaluatorExtensionConfigs: GetNumberLengthEvaluatorExtensionConfigs
        };

    }

    appControllers.service('WhS_RouteSync_NumberLengthAPIService', NumberLengthAPIService);
})(appControllers);