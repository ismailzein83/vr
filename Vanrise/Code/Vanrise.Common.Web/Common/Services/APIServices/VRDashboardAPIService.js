//(function (appControllers) {

//    "use strict";
//    VRDashboardAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

//    function VRDashboardAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

//        var controllerName = 'VRDashboard';

//        function GetTileExtendedSettingsConfigs() {
//            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetTileExtendedSettingsConfigs"));
//        }

//        return ({
//            GetTileExtendedSettingsConfigs: GetTileExtendedSettingsConfigs

//        });
//    }

//    appControllers.service('VRCommon_VRDashboardAPIService', VRDashboardAPIService);

//})(appControllers);