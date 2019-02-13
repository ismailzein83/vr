(function (appControllers) {

    "use strict";
    VRDashboardAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRDashboardAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRDashboardDefinition';

        return {
            GetDashboardInfo: GetDashboardInfo,
            GetDashboardEntity: GetDashboardEntity
        };

        function GetDashboardInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetDashboardInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetDashboardEntity(dashboardId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetDashboardEntity"), {
                dashboardId: dashboardId
            });
        }
    }

    appControllers.service('VRCommon_VRDashboardAPIService', VRDashboardAPIService);

})(appControllers);