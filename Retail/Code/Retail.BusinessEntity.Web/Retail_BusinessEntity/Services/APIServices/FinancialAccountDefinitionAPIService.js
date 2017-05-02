(function (appControllers) {

    'use strict';

    FinancialAccountDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function FinancialAccountDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'FinancialAccountDefinition';

        function GetFinancialAccountDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountDefinitionsInfo"), {
                filter: filter
            });
        }
        function GetFinancialAccountDefinitionSettings(financialAccountDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountDefinitionSettings"), {
                financialAccountDefinitionId: financialAccountDefinitionId
            });
        }
        function GetFinancialAccountDefinitionsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountDefinitionsConfigs"));
        }
        
        return {
            GetFinancialAccountDefinitionsInfo: GetFinancialAccountDefinitionsInfo,
            GetFinancialAccountDefinitionsConfigs: GetFinancialAccountDefinitionsConfigs,
            GetFinancialAccountDefinitionSettings: GetFinancialAccountDefinitionSettings
        };
    }

    appControllers.service('Retail_BE_FinancialAccountDefinitionAPIService', FinancialAccountDefinitionAPIService);

})(appControllers);