(function (appControllers) {

    'use strict';

    CataleyaAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

    function CataleyaAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

        var controllerName = 'Cataleya';

        function GetCataleyaDataManagerConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetCataleyaDataManagerConfigs"));
        }

        return {
            GetCataleyaDataManagerConfigs: GetCataleyaDataManagerConfigs
        };
    }

    appControllers.service('WhS_RouteSync_CataleyaAPIService', CataleyaAPIService);
})(appControllers);