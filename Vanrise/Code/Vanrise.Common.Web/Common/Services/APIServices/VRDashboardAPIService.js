(function (appControllers) {

    "use strict";
    VRDashboardAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRDashboardAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRDashboardDefinition';

        return {
            GetDashboardDefinitionInfo: GetDashboardDefinitionInfo,
            GetDashboardDefinitionEntity: GetDashboardDefinitionEntity
        };

        function GetDashboardDefinitionInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetDashboardDefinitionInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetDashboardDefinitionEntity(dashboardDefinitonId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetDashboardDefinitionEntity"), {
                dashboardDefinitonId: dashboardDefinitonId
            });
        }
    }

    appControllers.service('VRCommon_VRDashboardAPIService', VRDashboardAPIService);

})(appControllers);