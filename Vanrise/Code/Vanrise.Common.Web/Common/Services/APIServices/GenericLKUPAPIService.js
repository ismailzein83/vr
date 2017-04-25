
(function (appControllers) {

    "use strict";
    GenericLKUPAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function GenericLKUPAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "GenericLKUP";


        function GetFilteredGenericLKUPItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredGenericLKUPItems'), input);
        }

        function AddGenericLKUPItem(genericLKUPItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddGenericLKUPItem'), genericLKUPItem);
        }

        function UpdateGenericLKUPItem(genericLKUPItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateGenericLKUPItem'), genericLKUPItem);
        }


        function GetGenericLKUPItem(genericLKUPItemId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetGenericLKUPItem'), {
                genericLKUPItemId: genericLKUPItemId
            });
        }
        function GetGenericLKUPDefinitionExtendedSetings(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetGenericLKUPDefinitionExtendedSetings'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }
        return ({
            GetFilteredGenericLKUPItems: GetFilteredGenericLKUPItems,
            AddGenericLKUPItem: AddGenericLKUPItem,
            UpdateGenericLKUPItem: UpdateGenericLKUPItem,
            GetGenericLKUPItem: GetGenericLKUPItem,
            GetGenericLKUPDefinitionExtendedSetings: GetGenericLKUPDefinitionExtendedSetings,
        });
    }

    appControllers.service('VR_Common_GnericLKUPAPIService', GenericLKUPAPIService);

})(appControllers);