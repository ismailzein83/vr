(function (appControllers) {

    'use strict';

    AccountDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'AccountDefinition';

        function GetAccountViewDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountViewDefinitionSettingsConfigs"));
        }

        function GetAccountGridColumnAttributes() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountGridColumnAttributes"));
        }

        function GetAccountViewDefinitions() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountViewDefinitions"));
        }


        return {
            GetAccountViewDefinitionSettingsConfigs: GetAccountViewDefinitionSettingsConfigs,
            GetAccountGridColumnAttributes: GetAccountGridColumnAttributes,
            GetAccountViewDefinitions: GetAccountViewDefinitions
        };
    }

    appControllers.service('Retail_BE_AccountDefinitionAPIService', AccountDefinitionAPIService);

})(appControllers);