(function (appControllers) {

    'use strict';

    ModuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig','VR_Sec_ModuleService'];

    function ModuleAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, VR_Sec_ModuleService) {
        var controllerName = 'Module';

        return ({
            GetModule: GetModule,
            AddModule: AddModule,
            UpdateModule: UpdateModule,
            GetModules: GetModules,
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

        function UpdateModule(module) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateModule'), module);
        }
 
    }

    appControllers.service('VR_Sec_ModuleAPIService', ModuleAPIService);

})(appControllers);