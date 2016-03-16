(function (appControllers) {

    'use strict';

    BusinessEntityModuleAPIService.$inject = ['BaseAPIService', 'VR_Sec_ModuleConfig', 'UtilsService', 'SecurityService'];

    function BusinessEntityModuleAPIService(BaseAPIService, VR_Sec_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'BusinessEntityModule';
        return {
            GetBusinessEntityModuleById: GetBusinessEntityModuleById,
            UpdateBusinessEntityModule: UpdateBusinessEntityModule,
            AddBusinessEntityModule: AddBusinessEntityModule,
            HasUpdateBusinessEntityModulePermission: HasUpdateBusinessEntityModulePermission,
            HasAddBusinessEntityModulePermission: HasAddBusinessEntityModulePermission
        };

        function GetBusinessEntityModuleById(moduleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetBusinessEntityModuleById'), {
                moduleId: moduleId
            });
        }
        function UpdateBusinessEntityModule(moduleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateBusinessEntityModule'), moduleObject);
        }
        function HasUpdateBusinessEntityModulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateBusinessEntityModule']));
        }
        function AddBusinessEntityModule(moduleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'AddBusinessEntityModule'), moduleObject);
        }
        function HasAddBusinessEntityModulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddBusinessEntityModule']));
        }
    }

    appControllers.service('VR_Sec_BusinessEntityModuleAPIService', BusinessEntityModuleAPIService);

})(appControllers);