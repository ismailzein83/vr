(function (appControllers) {

    "use strict";

    VRLocalizationModuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRLocalizationModuleAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = 'VRLocalizationModule';

        function GetFilteredVRLocalizationModules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRLocalizationModules'), input);
        }

        function GetVRLocalizationModule(vrLocalizationModuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRLocalizationModule'), {
                vrLocalizationModule: vrLocalizationModuleId
            });
        }

        function GetVRLocalizationModuleInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRLocalizationModuleInfo"), {
                filter: filter
            });
        }

        function AddVRLocalizationModule(vrLocalizationModuleItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddVRLocalizationModule"), vrLocalizationModuleItem);
        }

        function UpdateVRLocalizationModule(vrLocalizationModuleItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateVRLocalizationModule"), vrLocalizationModuleItem);
        }

        return ({
            GetFilteredVRLocalizationModules: GetFilteredVRLocalizationModules,
            GetVRLocalizationModule: GetVRLocalizationModule,
            GetVRLocalizationModuleInfo: GetVRLocalizationModuleInfo,
            AddVRLocalizationModule: AddVRLocalizationModule,
            UpdateVRLocalizationModule: UpdateVRLocalizationModule
        });

    }
    appControllers.service('VRCommon_VRLocalizationModuleAPIService', VRLocalizationModuleAPIService);

})(appControllers);