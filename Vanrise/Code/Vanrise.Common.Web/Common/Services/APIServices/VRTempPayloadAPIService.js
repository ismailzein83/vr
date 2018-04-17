(function (appControllers) {

    "use strict";
    VRTempPayload.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRTempPayload(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'VRTempPayload';

        function AddVRTempPayload(vrTempPayload) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddVRTempPayload"), vrTempPayload);
        }
        function GetVRTempPayload(vrTempPayloadId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRTempPayload"), {
                vrTempPayloadId: vrTempPayloadId
            });
        }

        return ({
            AddVRTempPayload: AddVRTempPayload,
            GetVRTempPayload: GetVRTempPayload
        });
    }

    appControllers.service('VRCommon_VRTempPayloadAPIService', VRTempPayload);

})(appControllers);