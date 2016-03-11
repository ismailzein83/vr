(function (appControllers) {

    'use strict';

    ModuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig','VR_Sec_ModuleService','SecurityService'];

    function ModuleAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, VR_Sec_ModuleService, SecurityService) {
        var controllerName = 'Module';

        return ({
            GetModule: GetModule,
            AddModule: AddModule,
            UpdateModule: UpdateModule,
            GetModules: GetModules,
            HasAddModulePermission: HasAddModulePermission,
            HasUpdateModulePermission:HasUpdateModulePermission
        });

        function GetModule(moduleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetModule'), {
                moduleId: moduleId
            });
        }

        function GetModules() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetModules'));
        }
        function AddModule(module) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'AddModule'), module);
        }
        function HasAddModulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddModule']));
        }
        function UpdateModule(module) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateModule'), module);
        }
        function HasUpdateModulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateModule']));
        }


 
    }

    appControllers.service('VR_Sec_ModuleAPIService', ModuleAPIService);

})(appControllers);