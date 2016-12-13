(function (appControllers) {

    "use strict";
    MasterLogAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function MasterLogAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'MasterLog';

        function GetMasterLogDirectives(viewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetMasterLogDirectives"), { viewId: viewId });
        }
        return ({
            GetMasterLogDirectives: GetMasterLogDirectives
        });
    }

    appControllers.service('VRCommon_MasterLogAPIService', MasterLogAPIService);

})(appControllers);