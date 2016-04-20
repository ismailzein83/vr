(function (appControllers) {

    'use strict';

    ViewTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig', 'SecurityService'];

    function ViewTypeAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, SecurityService) {
        var controllerName = 'ViewType';

        return ({
            GetViewTypeIdByName: GetViewTypeIdByName,
            GetViewTypes: GetViewTypes,
        });

        function GetViewTypeIdByName(name) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetViewTypeIdByName'), {
                name: name
            });
        }
        function GetViewTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetViewTypes'));
        }
    }

    appControllers.service('VR_Sec_ViewTypeAPIService', ViewTypeAPIService);

})(appControllers);