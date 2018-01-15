(function (appControllers) {

    "use strict";

    qualityConfigurationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function qualityConfigurationAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "QualityConfiguration";

        function GetQualityConfigurationFields() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetQualityConfigurationFields"));
        }

        function GetQualityConfigurationInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetQualityConfigurationInfo"), { filter: serializedFilter });
        }

        function TryCompileQualityConfigurationExpression(expression) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "TryCompileQualityConfigurationExpression"), {
                qualityConfigurationExpression: expression
            });
        }


        return ({
            GetQualityConfigurationFields: GetQualityConfigurationFields,
            GetQualityConfigurationInfo: GetQualityConfigurationInfo,
            TryCompileQualityConfigurationExpression: TryCompileQualityConfigurationExpression
        });
    }

    appControllers.service('WhS_Routing_QualityConfigurationAPIService', qualityConfigurationAPIService);
})(appControllers);