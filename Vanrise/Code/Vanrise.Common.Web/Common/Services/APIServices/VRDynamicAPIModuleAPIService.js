(function (appControllers) {
    "use strict";
    vrDynamicAPIModuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];
    function vrDynamicAPIModuleAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controller = "VRDynamicAPIModule";

        function GetFilteredVRDynamicAPIModules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetFilteredVRDynamicAPIModules"), input);
        }

        function GetVRDynamicAPIModuleById(vrDynamicAPIModuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetVRDynamicAPIModuleById"),
                {
                    vrDynamicAPIModuleId: vrDynamicAPIModuleId
                });
        }

        function UpdateVRDynamicAPIModule(vrDynamicAPIModule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "UpdateVRDynamicAPIModule"), vrDynamicAPIModule);
        }

        function AddVRDynamicAPIModule(vrDynamicAPIModule) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "AddVRDynamicAPIModule"), vrDynamicAPIModule);
        }

        function BuildAllDynamicAPIControllers() {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "BuildAllDynamicAPIControllers"),);
        }
        function HasAddVRDynamicAPIModulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controller, ['AddVRDynamicAPIModule']));
        }

        function HasEditVRDynamicAPIModulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controller, ['UpdateVRDynamicAPIModule']));
        }

        return {
            GetFilteredVRDynamicAPIModules: GetFilteredVRDynamicAPIModules,
            GetVRDynamicAPIModuleById: GetVRDynamicAPIModuleById,
            UpdateVRDynamicAPIModule: UpdateVRDynamicAPIModule,
            AddVRDynamicAPIModule: AddVRDynamicAPIModule,
            HasAddVRDynamicAPIModulePermission: HasAddVRDynamicAPIModulePermission,
            HasEditVRDynamicAPIModulePermission: HasEditVRDynamicAPIModulePermission
        };
    }
    appControllers.service("VRCommon_VRDynamicAPIModuleAPIService", vrDynamicAPIModuleAPIService);

})(appControllers);