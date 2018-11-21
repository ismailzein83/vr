(function (appControllers) {

    "use strict";
    VRBulkActionDraftAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRBulkActionDraftAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRBulkActionDraftController';

        function GetVRBulkActionDrafts(finaleBulkActionDraftObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRBulkActionDrafts"), finaleBulkActionDraftObject);
        }
        return ({
            GetVRBulkActionDrafts: GetVRBulkActionDrafts,
        });
    }

    appControllers.service('VRCommon_VRBulkActionDraftAPIService', VRBulkActionDraftAPIService);

})(appControllers);