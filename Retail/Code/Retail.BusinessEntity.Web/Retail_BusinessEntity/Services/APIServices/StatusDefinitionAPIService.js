﻿
(function (appControllers) {

    "use strict";
    StatusDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function StatusDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "StatusDefinition";

        //function GetStatusDefinitionSettingsTemplateConfigs() {
        //    return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetStatusDefinitionSettingsTemplateConfigs"));
        //}

        function GetFilteredStatusDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredStatusDefinitions'), input);
        }

        function AddStatusDefinition(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddStatusDefinition'), statusDefinitionItem);
        }

        function UpdateStatusDefinition(statusDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateStatusDefinition'), statusDefinitionItem);
        }

        function GetStatusDefinition(statusDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetStatusDefinition'), {
                statusDefinitionId: statusDefinitionId
            });
        }


        return ({
            //GetStatusDefinitionSettingsTemplateConfigs: GetStatusDefinitionSettingsTemplateConfigs,
            GetFilteredStatusDefinitions: GetFilteredStatusDefinitions,
            AddStatusDefinition: AddStatusDefinition,
            UpdateStatusDefinition: UpdateStatusDefinition,
            GetStatusDefinition: GetStatusDefinition
        });
    }

    appControllers.service('Retail_BE_StatusDefinitionAPIService', StatusDefinitionAPIService);

})(appControllers);