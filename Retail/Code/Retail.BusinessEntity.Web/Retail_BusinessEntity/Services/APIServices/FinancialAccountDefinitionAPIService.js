﻿(function (appControllers) {

    'use strict';

    FinancialAccountDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function FinancialAccountDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'FinancialAccountDefinition';

        function GetFinancialAccountDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFinancialAccountDefinitionsInfo"), {
                filter: filter
            });
        }

        return {
            GetFinancialAccountDefinitionsInfo: GetFinancialAccountDefinitionsInfo
        };
    }

    appControllers.service('Retail_BE_FinancialAccountDefinitionAPIService', FinancialAccountDefinitionAPIService);

})(appControllers);