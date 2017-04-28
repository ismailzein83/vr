(function (appControllers) {

    'use strict';

    FinancialAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function FinancialAccountAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'FinancialAccount';

        function GetFilteredFinancialAccounts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFilteredFinancialAccounts"), input);
        }

        return {
            GetFilteredFinancialAccounts: GetFilteredFinancialAccounts
        };
    }

    appControllers.service('Retail_BE_FinancialAccountAPIService', FinancialAccountAPIService);

})(appControllers);