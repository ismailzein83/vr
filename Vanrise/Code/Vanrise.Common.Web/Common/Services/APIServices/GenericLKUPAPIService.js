
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

        function HasAddGenericLKUPItemPermission() {

            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddGenericLKUPItem']));
        }

        function UpdateGenericLKUPItem(genericLKUPItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateGenericLKUPItem'), genericLKUPItem);
        }

        function HasEditGenericLKUPItemPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateGenericLKUPItem']));
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
        function GetGenericLKUPItemsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetGenericLKUPItemsInfo'), {
                filter: filter
            });
        }
        function GetGenericLKUPItemHistoryDetailbyHistoryId(genericLKUPItemHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetGenericLKUPItemHistoryDetailbyHistoryId'), {
                genericLKUPItemHistoryId: genericLKUPItemHistoryId
            });
        }
        return ({
            GetFilteredGenericLKUPItems: GetFilteredGenericLKUPItems,
            AddGenericLKUPItem: AddGenericLKUPItem,
            HasAddGenericLKUPItemPermission:HasAddGenericLKUPItemPermission,
            UpdateGenericLKUPItem: UpdateGenericLKUPItem,
            HasEditGenericLKUPItemPermission:HasEditGenericLKUPItemPermission,
            GetGenericLKUPItem: GetGenericLKUPItem,
            GetGenericLKUPDefinitionExtendedSetings: GetGenericLKUPDefinitionExtendedSetings,
            GetGenericLKUPItemsInfo: GetGenericLKUPItemsInfo,
            GetGenericLKUPItemHistoryDetailbyHistoryId: GetGenericLKUPItemHistoryDetailbyHistoryId
        });
    }

    appControllers.service('VR_Common_GnericLKUPAPIService', GenericLKUPAPIService);

})(appControllers);