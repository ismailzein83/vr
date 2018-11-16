(function (appControllers) {
    "use strict";
    vrDynamicAPIAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];
    function vrDynamicAPIAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controller = "VRDynamicAPI";

        function GetFilteredVRDynamicAPIs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetFilteredVRDynamicAPIs"), input);
        }

        function GetVRDynamicAPIById(vrDynamicAPIId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetVRDynamicAPIById"),
                {
                    vrDynamicAPIId: vrDynamicAPIId
                });
        }

        function UpdateVRDynamicAPI(vrDynamicAPI) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "UpdateVRDynamicAPI"), vrDynamicAPI);
        }

        function AddVRDynamicAPI(vrDynamicAPI) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "AddVRDynamicAPI"), vrDynamicAPI);
        }
        
        function GetVRDynamicAPIMethodSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetVRDynamicAPIMethodSettingsConfigs"));
        }

        return {
            GetFilteredVRDynamicAPIs: GetFilteredVRDynamicAPIs,
            GetVRDynamicAPIById: GetVRDynamicAPIById,
            UpdateVRDynamicAPI: UpdateVRDynamicAPI,
            AddVRDynamicAPI: AddVRDynamicAPI,
            GetVRDynamicAPIMethodSettingsConfigs:GetVRDynamicAPIMethodSettingsConfigs
        };
    }
    appControllers.service("VRCommon_VRDynamicAPIAPIService", vrDynamicAPIAPIService);

})(appControllers);