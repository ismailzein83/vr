(function (appControllers) {

    'use strict';

    MenuAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function MenuAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        return ({
            GetMenuItems: GetMenuItems,
            GetAllMenuItems:GetAllMenuItems
        });

        function GetMenuItems() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Menu', 'GetMenuItems'));
        }

        function GetAllMenuItems(getOnlyAllowDynamic) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Menu', 'GetAllMenuItems'),
                {
                    getOnlyAllowDynamic: getOnlyAllowDynamic
                });
        }
    }

    appControllers.service('VR_Sec_MenuAPIService', MenuAPIService);

})(appControllers);
