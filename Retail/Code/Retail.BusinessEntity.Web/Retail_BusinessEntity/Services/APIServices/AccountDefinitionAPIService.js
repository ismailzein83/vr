(function (appControllers) {

    'use strict';

    AccountDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'AccountDefinition';

        function GetAccountViewDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountViewDefinitionSettingsConfigs"));
        }

        return {
            GetAccountViewDefinitionSettingsConfigs: GetAccountViewDefinitionSettingsConfigs
        };
    }

    appControllers.service('Retail_BE_AccountDefinitionAPIService', AccountDefinitionAPIService);

})(appControllers);