﻿(function (appControllers) {

    "use strict";
    packageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function packageAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "PackageDefinition";
    
        function GetPackageDefinitionExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackageDefinitionExtendedSettingsConfigs"));
        }
        return ({
            GetPackageDefinitionExtendedSettingsConfigs: GetPackageDefinitionExtendedSettingsConfigs,
        });
    }

    appControllers.service('Retail_BE_PackageDefinitionAPIService', packageAPIService);

})(appControllers);