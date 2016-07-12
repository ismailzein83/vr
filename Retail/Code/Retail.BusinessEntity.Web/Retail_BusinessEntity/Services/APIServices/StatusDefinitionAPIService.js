
(function (appControllers) {

    "use strict";
    StatusDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function StatusDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "StatusDefinition";

        function GetStatusDefinitionSettingsTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetStatusDefinitionSettingsTemplateConfigs"));
        }

        function GetFilteredStatusDefinition(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredStatusDefinition'), input);
        }


        return ({
            GetStatusDefinitionSettingsTemplateConfigs: GetStatusDefinitionSettingsTemplateConfigs,
            GetFilteredStatusDefinition: GetFilteredStatusDefinition,
        });
    }

    appControllers.service('Retail_BE_StatusDefinitionAPIService', StatusDefinitionAPIService);

})(appControllers);