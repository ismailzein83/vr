(function (appControllers) {

    "use strict";

    BusinessEntityConfigurationAPIService.$inject = ['BaseAPIService', 'WhS_BE_ModuleConfig', 'UtilsService', 'SecurityService'];

    function BusinessEntityConfigurationAPIService(BaseAPIService, WhS_BE_ModuleConfig, UtilsService, SecurityService) {

        var controllerName = 'BusinessEntityConfiguration';

        function GetCodeListResolverSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCodeListResolverSettingsTemplates"));
        }

        function GetExcludedDestinationsTemplates() {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetExcludedDestinationsTemplates"));
        }

        return ({
            GetCodeListResolverSettingsTemplates: GetCodeListResolverSettingsTemplates,
            GetExcludedDestinationsTemplates: GetExcludedDestinationsTemplates
        });
    }

    appControllers.service('whs_BE_BusinessEntityConfigurationAPIService', BusinessEntityConfigurationAPIService);

})(appControllers);