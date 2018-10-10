(function (appControllers) {

    "use strict";
    cacheRefreshhandleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function cacheRefreshhandleAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'CacheRefreshHandle';

        function GetFilteredCacheRefreshHandles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredCacheRefreshHandles"), input);
        }

        function SetCacheExpired(typeName) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "SetCacheExpired"), { typeName: typeName });
        }
       
        function HasSetCacheExpiredPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['SetCacheExpired']));
        }

        return ({
            GetFilteredCacheRefreshHandles: GetFilteredCacheRefreshHandles,
            SetCacheExpired: SetCacheExpired,
            HasSetCacheExpiredPermission: HasSetCacheExpiredPermission
        });
    }

    appControllers.service('VRCommon_CacheRefreshhandleAPIService', cacheRefreshhandleAPIService);

})(appControllers);