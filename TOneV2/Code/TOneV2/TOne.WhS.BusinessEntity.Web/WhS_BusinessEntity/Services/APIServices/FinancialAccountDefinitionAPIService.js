(function (appControllers) {

    'use strict';

    FinancialAccountDefinitionAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];

    function FinancialAccountDefinitionAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = 'FinancialAccountDefinition';

        function GetFinancialAccountDefinitionsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountDefinitionsConfigs"));
        }
        function GetFinancialAccountDefinitionSettings(financialAccountDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountDefinitionSettings"), {
                financialAccountDefinitionId: financialAccountDefinitionId
            });
        }
        function GetFinancialAccountDefinitionInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountDefinitionInfo"), {
                filter: filter
            });
        }
        return ({
            GetFinancialAccountDefinitionsConfigs: GetFinancialAccountDefinitionsConfigs,
            GetFinancialAccountDefinitionSettings: GetFinancialAccountDefinitionSettings,
            GetFinancialAccountDefinitionInfo: GetFinancialAccountDefinitionInfo
        });
    }

    appControllers.service("WhS_BE_FinancialAccountDefinitionAPIService", FinancialAccountDefinitionAPIService);

})(appControllers);
