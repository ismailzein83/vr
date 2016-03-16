(function (appControllers) {

    'use strict';

    ExtensibleBEItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function ExtensibleBEItemAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
        return {
            GetExtensibleBEItem: GetExtensibleBEItem,
            AddExtensibleBEItem: AddExtensibleBEItem,
            HasAddExtensibleBEItem: HasAddExtensibleBEItem,
            GetFilteredExtensibleBEItems: GetFilteredExtensibleBEItems,
            UpdateExtensibleBEItem: UpdateExtensibleBEItem,
            HasUpdateExtensibleBEItem: HasUpdateExtensibleBEItem
        };

        function GetExtensibleBEItem(extensibleBEItemId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'ExtensibleBEItem', 'GetExtensibleBEItem'), { extensibleBEItemId: extensibleBEItemId });
        }
        function AddExtensibleBEItem(extensibleBEItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'ExtensibleBEItem', 'AddExtensibleBEItem'), extensibleBEItem);
        }
        function HasAddExtensibleBEItem() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "ExtensibleBEItem", ['AddExtensibleBEItem']));
        }
        function UpdateExtensibleBEItem(extensibleBEItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'ExtensibleBEItem', 'UpdateExtensibleBEItem'), extensibleBEItem);
        }
        function HasUpdateExtensibleBEItem() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "ExtensibleBEItem", ['UpdateExtensibleBEItem']));
        }
        function GetFilteredExtensibleBEItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'ExtensibleBEItem', 'GetFilteredExtensibleBEItems'), input);
        }

    }

    appControllers.service('VR_GenericData_ExtensibleBEItemAPIService', ExtensibleBEItemAPIService);

})(appControllers);